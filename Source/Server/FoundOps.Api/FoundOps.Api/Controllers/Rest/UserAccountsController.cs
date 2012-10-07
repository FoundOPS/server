using FoundOps.Api.Tools;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Tools;
using System.Collections.Generic;
using System.Data.Entity;
using System.Net;
using System.Net.Http;
using System;
using System.Linq;
using System.Web.Http;
using UserAccount = FoundOps.Api.Models.UserAccount;

namespace FoundOps.Api.Controllers.Rest
{
    public class UserAccountsController : BaseApiController
    {
        //Helpers
        private Core.Models.CoreEntities.UserAccount GetUser(string email)
        {
            return CoreEntitiesContainer.Parties.OfType<Core.Models.CoreEntities.UserAccount>()
                .FirstOrDefault(ua => ua.EmailAddress.Trim() == email.Trim());
        }

        /// <summary>
        /// If a role id is passed:
        /// REQUIRES admin access to the role
        /// Return the role's UserAccounts their EmployeeIds 
        /// 
        /// If the role id is null:
        /// Return the current user account with it's image url (no EmployeeId)
        /// </summary>
        /// <param name="roleId">The role</param>
        /// <returns>UserAccounts with access to the role role</returns>
        public IQueryable<UserAccount> Get(Guid? roleId)
        {
            var onlyCurrentUser = !roleId.HasValue;

            IEnumerable<Core.Models.CoreEntities.UserAccount> userAccounts;

            //for use below to find the employees
            BusinessAccount businessAccount = null;
            if (!onlyCurrentUser)
            {
                businessAccount = CoreEntitiesContainer.Owner(roleId.Value, new[] { RoleType.Administrator }).Include(ba => ba.OwnedRoles).Include("OwnedRoles.MemberParties")
                    //need to include employee's for the Id
                    .Include(ba => ba.Employees).FirstOrDefault();

                if (businessAccount == null)
                    throw Request.NotAuthorized();

                userAccounts = businessAccount.OwnedRoles.SelectMany(r => r.MemberParties).OfType<Core.Models.CoreEntities.UserAccount>();

                //if the user is not a foundops user, do not return FoundOPS accounts
                if (!CoreEntitiesContainer.CanAdministerFoundOPS())
                    userAccounts = userAccounts.Where(ua => !ua.EmailAddress.Contains("foundops.com"));
            }
            else
            {
                userAccounts = CoreEntitiesContainer.CurrentUserAccount();
            }

            var enumerated = userAccounts.OrderBy(ua => ua.LastName + " " + ua.FirstName).ToList();

            if (onlyCurrentUser)
            {
                //load the party image so it gets included in the convert below
                enumerated.First().PartyImageReference.Load();
            }

            var apiUserAccounts = enumerated.Select(UserAccount.Convert).ToList();

            //find and set the employee id for each user account
            if (!onlyCurrentUser)
            {
                foreach (var userAccount in apiUserAccounts)
                {
                    var employee = businessAccount.Employees.FirstOrDefault(e => e.LinkedUserAccountId == userAccount.Id);
                    if (employee != null)
                        userAccount.EmployeeId = employee.Id;
                }
            }

            return apiUserAccounts.AsQueryable();
        }

        /// <summary>
        /// Inserts a new User Account or adds the user to the business's role
        /// </summary>
        /// <param name="userAccount">The new user account to create</param>
        /// <param name="roleId">the role</param>
        public void Post(Guid roleId, UserAccount userAccount)
        {
            //check for admin abilities
            var businessAccount = CoreEntitiesContainer.Owner(roleId, new[] { RoleType.Administrator }).Include(ba => ba.OwnedRoles).FirstOrDefault();
            if (businessAccount == null)
                throw Request.NotAuthorized();

            //find the role in the BusinessAccount that matches the name of the one passed in
            var role = CoreEntitiesContainer.Roles.FirstOrDefault(r => r.OwnerBusinessAccountId == businessAccount.Id && r.Name == userAccount.Role);
            if (role == null)
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest, "This role does not exist on the business account. Please contact customer support"));

            var user = GetUser(userAccount.EmailAddress);

            //if the user does not exist, create a new one
            if (user == null)
            {
                user = new Core.Models.CoreEntities.UserAccount
                {
                    Id = Guid.NewGuid(),
                    EmailAddress = userAccount.EmailAddress.Trim(),
                    FirstName = userAccount.FirstName,
                    LastName = userAccount.LastName,
                    TimeZone = "Eastern Standard Time"
                };

                //Send new user email
                var sender = CoreEntitiesContainer.CurrentUserAccount().First().DisplayName;
                var recipient = user.FirstName;

                //Construct the email
                var subject = "Your FoundOPS invite from " + sender;
                var body = "Hi " + recipient + ", \r\n\r\n" +
                            sender + " has created a user account for you in FoundOPS. \r\n\r\n" +
                            "FoundOPS is an easy to use tool that helps field services teams communicate and provide the best possible service to their clients. \r\n\r\n" +
                            "Click here to accept the invite: \r\n " + @"{0}" + "\r\n\r\n" +
                            "If you have any difficulty accepting the invitation, email us at support@foundops.com. This invitation expires in 7 days. \r\n\r\n\r\n" +
                            "The FoundOPS Team";

                CoreEntitiesMembershipProvider.ResetAccount(user.EmailAddress, subject, body, TimeSpan.FromDays(7));
            }

            //add the user to the role
            user.RoleMembership.Add(role);

            //TODO add employee
            //if (userAccount.LastName != null)
            //{
            //    var employee = CoreEntitiesContainer.Employees.FirstOrDefault(e => e.FirstName == userAccount.FirstName && e.LastName == userAccount.LastName && e.EmployerId == businessAccount.Id);

            //    user.LinkedEmployees.Add(employee);
            //}
            //else if (userAccount.FirstName == "Create")
            //{
            //    var newEmployee = new Employee
            //    {
            //        Id = Guid.NewGuid(),
            //        FirstName = userAccount.FirstName,
            //        LastName = userAccount.LastName,
            //        Employer = businessAccount
            //    };
            //    user.LinkedEmployees.Add(newEmployee);
            //}

            SaveWithRetry();
        }

        //TODO
        //Update user image
        //if (updateUserImage)
        //{
        //    user.PartyImageReference.Load();
        //    if (user.PartyImage == null)
        //    {
        //        var partyImage = new PartyImage { OwnerParty = user };
        //        user.PartyImage = partyImage;
        //    }

        //    value = PartyTools.UpdatePartyImageHelper(CoreEntitiesContainer, user, Request);
        //}

        /// <summary>
        /// If roleId is set, changing properties on behalf of a business account. Can only change role and employee link.
        /// If roleId is null, changing properties on behalf of the current user. Can only change user properties: FirstName, LastName, Email Address, TimeZone, Password
        /// </summary>
        /// <param name="userAccount">The UserAccount</param>
        /// <param name="roleId">(Optional) The role of the business to change</param>
        /// <param name="newPass">(Optional) For resetting the password: the users new password</param>
        /// <param name="oldPass">(Optional) For resetting the password: the users old password</param>
        public void Put(UserAccount userAccount, Guid? roleId = null, string newPass = null, string oldPass = null)
        {
            var currentUserAccount = CoreEntitiesContainer.CurrentUserAccount().FirstOrDefault();

            //changing properties on behalf of a business account
            if (roleId.HasValue)
            {
                //used for changing roles
                var businessAccount = CoreEntitiesContainer.Owner(roleId.Value, new[] { RoleType.Administrator }).FirstOrDefault();
                if (businessAccount == null)
                    throw Request.NotAuthorized();

                var user = CoreEntitiesContainer.Parties.OfType<Core.Models.CoreEntities.UserAccount>()
                    .Include(u => u.RoleMembership).Include(u => u.LinkedEmployees).First(ua => ua.Id == userAccount.Id);

                //Get the users current role for this business account
                var userRole = CoreEntitiesContainer.Roles.FirstOrDefault(r => r.OwnerBusinessAccountId == businessAccount.Id
                    && r.MemberParties.Any(p => p.Id == user.Id));

                //If a new role has been selected for the user, remove all old ones and assign the new one
                if (userRole != null && userRole.Name != userAccount.Role)
                {
                    user.RoleMembership.Remove(userRole);

                    //Find the new role to be added for the user
                    var newRole = CoreEntitiesContainer.Roles.FirstOrDefault(r => r.OwnerBusinessAccountId == businessAccount.Id && r.Name == userAccount.Role);

                    //Add the new role for the user
                    if (newRole != null)
                        user.RoleMembership.Add(newRole);
                }

                var userLinkedEmployee = user.LinkedEmployees.FirstOrDefault(e => e.EmployerId == businessAccount.Id);

                var employee = userLinkedEmployee != null ? CoreEntitiesContainer.Employees.FirstOrDefault(e => userLinkedEmployee.Id == e.Id) : null;

                //Changing a user from one employee to another 
                //ignore the none option
                if (employee != null && employee.Id != userAccount.EmployeeId && userAccount.EmployeeId != Guid.Empty)
                {
                    user.LinkedEmployees.Remove(employee);

                    LinkEmployeeToUser(userAccount, user, businessAccount);
                }
                //Changing a user from an employee to no employee
                //If the employee Id == Guid.Empty, the none option was chosen
                else if (employee != null && userAccount.EmployeeId == Guid.Empty)
                {
                    user.LinkedEmployees.Remove(employee);
                }
                //Changing a user from no employee to an employee
                else if (employee == null && userAccount.EmployeeId != Guid.Empty)
                {
                    LinkEmployeeToUser(userAccount, user, businessAccount);
                }
            }
            //changing properties on behalf of the current user
            else
            {
                //if the user is not editing itself, and if it does not have admin access to the business account
                //throw not authorized
                if (currentUserAccount == null || currentUserAccount.Id != userAccount.Id)
                    throw Request.NotAuthorized();

                //if the email address changed, check it does not conflict
                if (userAccount.EmailAddress != currentUserAccount.EmailAddress &&
                    GetUser(userAccount.EmailAddress) != null)
                    throw ApiExceptions.Create(Request.CreateResponse(HttpStatusCode.Conflict,
                                                                      "This email address already exists"));

                //try to change the password
                if (newPass != null && oldPass != null)
                {
                    if (!CoreEntitiesMembershipProvider.ChangePassword(userAccount.EmailAddress, oldPass, newPass))
                        throw ApiExceptions.Create(Request.CreateResponse(HttpStatusCode.NotAcceptable));
                }

                currentUserAccount.FirstName = userAccount.FirstName;
                currentUserAccount.LastName = userAccount.LastName;
                currentUserAccount.EmailAddress = userAccount.EmailAddress;
                currentUserAccount.TimeZone = userAccount.TimeZone.Id;

                SaveWithRetry();
            }
        }

        /// <summary>
        /// Deletes a User account
        /// </summary>
        /// <param name="roleId">The role</param>
        /// <param name="accountId">The Id of the account to be deleted</param>
        public HttpResponseMessage Delete(Guid roleId, Guid accountId)
        {
            //TODO

            //If they are not an admin, they do not have the ability to insert new Users
            var businessAccount = CoreEntitiesContainer.Owner(roleId, new[] { RoleType.Administrator }).FirstOrDefault();
            if (businessAccount == null)
                throw Request.NotAuthorized();

            var user = CoreEntitiesContainer.Parties.OfType<Core.Models.CoreEntities.UserAccount>().First(ua => ua.Id == accountId);

            //Remove employees from the user account
            foreach (var employee in user.LinkedEmployees)
                user.LinkedEmployees.Remove(employee);

            CoreEntitiesContainer.DeleteUserAccountBasedOnId(accountId);

            return Request.CreateResponse(HttpStatusCode.Accepted);
        }

        #region Helper Methods

        //This will link the employee to the user account. If there is no employee, it will create a new one
        private void LinkEmployeeToUser(UserAccount account, Core.Models.CoreEntities.UserAccount user, BusinessAccount businessAccount)
        {
            var employee = CoreEntitiesContainer.Employees.FirstOrDefault(e => e.Id == account.EmployeeId);

            if (employee != null)
                user.LinkedEmployees.Add(employee);
            else
            {
                employee = new Employee
                {
                    Id = Guid.NewGuid(),
                    FirstName = account.EmailAddress,
                    LastName = account.LastName,
                    Employer = businessAccount
                };
                user.LinkedEmployees.Add(employee);
            }
        }

        #endregion
    }
}
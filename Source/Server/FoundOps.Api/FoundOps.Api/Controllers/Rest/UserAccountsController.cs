using FoundOps.Api.Tools;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Tools;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
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
        /// Return the role's (business's) UserAccounts with EmployeeId. TODO image url when it is used somewhere
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

            //find and set the employee id and role for each user account
            if (!onlyCurrentUser)
            {
                foreach (var userAccount in apiUserAccounts)
                {
                    var employee = businessAccount.Employees.FirstOrDefault(e => e.LinkedUserAccountId == userAccount.Id);
                    userAccount.EmployeeId = employee != null ?
                        employee.Id :
                        //set an empty value to signify there is no employee
                        //if it is left null, that property will not be serialized (signifying we did not send the link)
                        new Guid();

                    var role = businessAccount.OwnedRoles.First(r => r.MemberParties.Any(mp => mp.Id == userAccount.Id));
                    userAccount.Role = role.Name;
                }
            }

            return apiUserAccounts.AsQueryable();
        }

        /// <summary>
        /// Insert a new User Account, or adds the user with this business account to the business's role
        /// </summary>
        /// <param name="userAccount">The new user account to create</param>
        /// <param name="roleId">The role</param>
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
            }

            //add the user to the role
            user.RoleMembership.Add(role);

            //setup the employee link
            SetupEmployee(userAccount.EmployeeId, user, businessAccount);

            SaveWithRetry();

            //Send new user email
            var sender = CoreEntitiesContainer.CurrentUserAccount().First().DisplayName;
            var recipient = user.FirstName;
            var subject = "Your FoundOPS invite from " + sender;
            var body = "Hi " + recipient + ", \r\n\r\n" +
                        sender + " has created a user account for you in FoundOPS. \r\n\r\n" +
                        "FoundOPS is an easy to use tool that helps field services teams communicate and provide the best possible service to their clients. \r\n\r\n" +
                        "Click here to accept the invite: \r\n " + @"{0}" + "\r\n\r\n" +
                        "If you have any difficulty accepting the invitation, email us at support@foundops.com. This invitation expires in 7 days. \r\n\r\n\r\n" +
                        "The FoundOPS Team";

            CoreEntitiesMembershipProvider.ResetAccount(user.EmailAddress, subject, body, TimeSpan.FromDays(7));
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

                SetupEmployee(userAccount.EmployeeId, user, businessAccount);

                SaveWithRetry();
            }
            //changing properties on behalf of the current user
            else
            {
                var currentUserAccount = CoreEntitiesContainer.CurrentUserAccount().FirstOrDefault();
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
        /// Remove a user account from a role. It will delete the user account if it is not part of any other roles
        /// REQUIRES: Admin access to role
        /// </summary>
        /// <param name="roleId">The role</param>
        /// <param name="id">The userAccount's Id</param>
        public void Delete(Guid roleId, Guid id)
        {
            var businessAccount = CoreEntitiesContainer.Owner(roleId, new[] { RoleType.Administrator }).FirstOrDefault();
            if (businessAccount == null)
                throw Request.NotAuthorized();

            var userAccount = CoreEntitiesContainer.Parties.OfType<Core.Models.CoreEntities.UserAccount>()
                .Include(ua => ua.LinkedEmployees).Include(ua => ua.RoleMembership)
                .First(ua => ua.Id == id);

            //clear the user from the business's roles and employees
            foreach (var role in userAccount.RoleMembership.Where(r => r.OwnerBusinessAccountId == businessAccount.Id).ToArray())
                role.MemberParties.Remove(userAccount);
            foreach (var employee in userAccount.LinkedEmployees.Where(e => e.EmployerId == businessAccount.Id).ToArray())
                employee.LinkedUserAccountId = null;

            SaveWithRetry();

            //delete the account if the user is not part of other roles
            if (!userAccount.RoleMembership.Any())
                CoreEntitiesContainer.DeleteUserAccountBasedOnId(id);
        }

        /// <summary>
        /// Setup the employee link for a user account.
        /// If the employeeId is null or Empty, it will create a new Employee.
        /// Otherwise it will link the existing employee 
        /// </summary>
        /// <param name="employeeId">The employee Id to link</param>
        /// <param name="userAccount">The user account</param>
        /// <param name="businessAccount">The business account</param>
        private void SetupEmployee(Guid? employeeId, Core.Models.CoreEntities.UserAccount userAccount, BusinessAccount businessAccount)
        {
            //clear any linked employees for this business account
            foreach (var oldEmployee in userAccount.LinkedEmployees.Where(e => e.EmployerId == businessAccount.Id).ToArray())
                oldEmployee.LinkedUserAccountId = null;

            Employee employee;
            //find the existing employee
            if (employeeId.HasValue && employeeId != Guid.Empty)
            {
                employee = CoreEntitiesContainer.Employees.First(e => e.Id == employeeId.Value);
            }
            //add a new employee
            else
            {
                employee = new Employee
                {
                    FirstName = userAccount.FirstName,
                    LastName = userAccount.LastName,
                    EmployerId = businessAccount.Id
                };
            }

            employee.LinkedUserAccount = userAccount;
        }
    }
}
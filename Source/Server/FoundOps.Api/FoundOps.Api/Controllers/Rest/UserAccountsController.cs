using FoundOps.Api.Tools;
using FoundOps.Common.NET;
using FoundOps.Core.Models.Azure;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Tools;
using System.Collections.Generic;
using System.Data.Entity;
using System.Net;
using System.Net.Http;
using System;
using System.Linq;
using UserAccount = FoundOps.Api.Models.UserAccount;

namespace FoundOps.Api.Controllers.Rest
{
    public class UserAccountsController : BaseApiController
    {
        #region Conflicts and Responses

        /// <summary>
        /// Will return true if the email already exists.
        /// </summary>
        /// <param name="newEmail">The new email address</param>
        /// <param name="oldEmail">If the oldEmail and newEmail match this will return no conflict.</param>
        private bool UserExistsConflict(string newEmail, string oldEmail)
        {
            return oldEmail != newEmail && UserExistsConflict(newEmail);
        }
        
        private bool UserExistsConflict(string newEmail)
        {
            return CoreEntitiesContainer.Parties.OfType<Core.Models.CoreEntities.UserAccount>().Any(ua => ua.EmailAddress.Trim() == newEmail.Trim());
        }

        /// <summary>
        /// The user exists response.
        /// </summary>
        private HttpResponseMessage UserExistsResponse()
        {
            return Request.CreateResponse(HttpStatusCode.Conflict, "This email address already exists");
        }

        #endregion

        /// <summary>
        /// REQUIRES: Admin access to the role.
        /// </summary>
        /// <param name="roleId">The role</param>
        /// <param name="currentUserOnly">Set to true if you want the current users settings instead of all users</param>
        /// <returns>UserAccounts with access to the role's business account</returns>
        public IQueryable<UserAccount> Get(Guid roleId, bool currentUserOnly = false)
        {
            //Contents of If statment are used for Personal Settings
            if(currentUserOnly)
            {
                var user = CoreEntitiesContainer.CurrentUserAccount().First();
                user.PartyImageReference.Load();

                //var timezone = GetTimeZones().FirstOrDefault(tz => tz.TimeZoneId == user.TimeZone);

                var account = new UserAccount
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    EmailAddress = user.EmailAddress.Trim()
                    //TimeZoneInfo = timezone
                };

                //Load image url
                if (user.PartyImage != null)
                {
                    var imageUrl = user.PartyImage.RawUrl + AzureServerHelpers.GetBlobUrlHelper(user.Id, user.PartyImage.Id);
                    account.ImageUrl = imageUrl;
                }
                else
                {
                    account.ImageUrl = "img/emptyPerson.png";
                }

                var currentUserAccounts = new List<UserAccount> {account};
                return currentUserAccounts.AsQueryable();
            }

            //Begin code to get all users for a business account
            //Used in User Settings
            var business = GetBusinessAccount(roleId, new[] { RoleType.Administrator }, new[] { "Employees", "OwnedRoles", "MemberParties" });

            var userAccounts = business.OwnedRoles.SelectMany(r => r.MemberParties).OfType<Core.Models.CoreEntities.UserAccount>();

            //if the user is not a foundops user, do not return FoundOPS accounts
            if (!CoreEntitiesContainer.CanAdministerFoundOPS())
                userAccounts = userAccounts.Where(ua => !ua.EmailAddress.Contains("foundops.com"));

            var apiUserAccounts = userAccounts.OrderBy(ua => ua.LastName + " " + ua.FirstName).Select(UserAccount.Convert).ToList();

            //find the employee id for each user account
            foreach (var ua in apiUserAccounts)
            {
                var employee = business.Employees.FirstOrDefault(e => e.LinkedUserAccountId == ua.Id);
                if (employee != null)
                    ua.EmployeeId = employee.Id;
            }

            return apiUserAccounts.AsQueryable();
        }

        /// <summary>
        /// Inserts a new User Account
        /// </summary>
        /// <param name="account">new user account to create</param>
        /// <param name="roleId">the role</param>
        public HttpResponseMessage Post(Guid roleId, UserAccount account)
        {
            //Check for admin abilities
            var businessAccount = CoreEntitiesContainer.Owner(roleId, new[] { RoleType.Administrator }).Include(ba => ba.OwnedRoles).FirstOrDefault();
            if (businessAccount == null)
                throw Request.NotAuthorized();

            #region Create new UserAccount

            //check the email address is not in use yet for this business account
            if (UserExistsConflict(account.EmailAddress))
                return UserExistsResponse();

            var temporaryPassword = EmailPasswordTools.GeneratePassword();
            var salt = EncryptionTools.GenerateSalt();

            var user = new Core.Models.CoreEntities.UserAccount
            {
                EmailAddress = account.EmailAddress.Trim(),
                Id = Guid.NewGuid(),
                FirstName = account.FirstName,
                LastName = account.LastName,
                PasswordHash = EncryptionTools.Hash(temporaryPassword, salt),
                PasswordSalt = salt,
                TimeZone = "Eastern Standard Time"
            };

            //Find the role in the BusinessAccount that matches the name of the one passed in
            var role = CoreEntitiesContainer.Roles.FirstOrDefault(r => r.OwnerBusinessAccountId == businessAccount.Id && r.Name == account.Role);
            user.RoleMembership.Add(role);

            #endregion

            //Add the newly created UserAccount to the database
            CoreEntitiesContainer.Parties.AddObject(user);

            if (account.LastName != null)
            {
                var employee = CoreEntitiesContainer.Employees.FirstOrDefault(e => e.FirstName == account.FirstName && e.LastName == account.LastName && e.EmployerId == businessAccount.Id);

                user.LinkedEmployees.Add(employee);
            }
            else if (account.FirstName == "Create")
            {
                var newEmployee = new FoundOps.Core.Models.CoreEntities.Employee
                {
                    Id = Guid.NewGuid(),
                    FirstName = account.FirstName,
                    LastName = account.LastName,
                    Employer = businessAccount
                };
                user.LinkedEmployees.Add(newEmployee);
            }

            SaveWithRetry();

            #region Send New User Email

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

            #endregion

            return Request.CreateResponse(HttpStatusCode.Created);
        }

        /// <summary>
        /// Updates a User account
        /// </summary>
        /// <param name="roleId">The role</param>
        /// <param name="account">The User account</param>
        /// <param name="updateUserImage">Set to true if you would like to update the Users image</param>
        /// <param name="newPass">Users new password</param>
        /// <param name="oldPass">Users old password</param>
        public HttpResponseMessage Put(Guid roleId, UserAccount account, bool updateUserImage = false, string newPass = null, string oldPass = null)
        {
            var businessAccount = CoreEntitiesContainer.Owner(roleId, new[] { RoleType.Administrator }).FirstOrDefault();

            if (businessAccount == null)
                return Request.CreateResponse(HttpStatusCode.Unauthorized);

            var user = CoreEntitiesContainer.Parties.OfType<FoundOps.Core.Models.CoreEntities.UserAccount>()
                .Include("RoleMembership").Include("LinkedEmployees").First(ua => ua.Id == account.Id);

            if (UserExistsConflict(account.EmailAddress, user.EmailAddress))
                return UserExistsResponse();

            user.FirstName = account.FirstName;
            user.LastName = account.LastName;
            user.EmailAddress = account.EmailAddress;
            //user.TimeZone = account.TimeZoneInfo.TimeZoneId;

            //Get the users current role for this business account
            var userRole = CoreEntitiesContainer.Roles.FirstOrDefault(r => r.OwnerBusinessAccountId == businessAccount.Id 
                && r.MemberParties.Any(p => p.Id == user.Id));

            //If a new role has been selected for the user, remove all old ones and assign the new one
            if (userRole != null && userRole.Name != account.Role)
            {
                user.RoleMembership.Remove(userRole);

                //Find the new role to be added for the user
                var newRole = CoreEntitiesContainer.Roles.FirstOrDefault(r => r.OwnerBusinessAccountId == businessAccount.Id && r.Name == account.Role);

                //Add the new role for the user
                if (newRole != null)
                    user.RoleMembership.Add(newRole);
            }

            var userLinkedEmployee = user.LinkedEmployees.FirstOrDefault(e => e.EmployerId == businessAccount.Id);

            var employee = userLinkedEmployee != null ? CoreEntitiesContainer.Employees.FirstOrDefault(e => userLinkedEmployee.Id == e.Id) : null;

            //Changing a user from one employee to another 
            //ignore the none option
            if (employee != null && employee.Id != account.EmployeeId && account.EmployeeId != Guid.Empty)
            {
                user.LinkedEmployees.Remove(employee);

                LinkEmployeeToUser(account, user, businessAccount);
            }
            //Changing a user from an employee to no employee
            //If the employee Id == Guid.Empty, the none option was chosen
            else if (employee != null && account.EmployeeId == Guid.Empty)
            {
                user.LinkedEmployees.Remove(employee);
            }
            //Changing a user from no employee to an employee
            else if (employee == null &&  account.EmployeeId != Guid.Empty)
            {
                LinkEmployeeToUser(account, user, businessAccount);
            }

            SaveWithRetry();

            var value = account.FirstName + ' ' + account.LastName;

            //Update user image
            if (updateUserImage)
            {
                user.PartyImageReference.Load();
                if (user.PartyImage == null)
                {
                    var partyImage = new PartyImage {OwnerParty = user};
                    user.PartyImage = partyImage;
                }

                value = PartyTools.UpdatePartyImageHelper(user, Request);
            }
            //Will only be set to false if there is a problem changing the users password
            var changeSuccessful = true;

            if (newPass != null && oldPass != null)
                changeSuccessful = CoreEntitiesMembershipProvider.ChangePassword(user.EmailAddress, oldPass, newPass);

            //if there was a problem changing the password, return a Not acceptable status code
            //else, return Accepted status code
            return Request.CreateResponse(changeSuccessful ? HttpStatusCode.Accepted : HttpStatusCode.NotAcceptable, value);
        }

        /// <summary>
        /// Deletes a User account
        /// </summary>
        /// <param name="roleId">The role</param>
        /// <param name="accountId">The Id of the account to be deleted</param>
        public HttpResponseMessage Delete(Guid roleId, Guid accountId)
        {
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

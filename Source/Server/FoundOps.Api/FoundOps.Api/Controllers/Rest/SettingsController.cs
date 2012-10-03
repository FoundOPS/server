using FoundOps.Api.Models;
using FoundOps.Api.Tools;
using FoundOps.Core.Models.Authentication;
using FoundOps.Core.Models.Azure;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Tools;
using Microsoft.WindowsAzure.StorageClient;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Employee = FoundOps.Api.Models.Employee;

namespace FoundOps.Api.ApiControllers
{
    [Core.Tools.Authorize]
    public class SettingsController : ApiController
    {
        private readonly CoreEntitiesContainer _coreEntitiesContainer;
        private readonly CoreEntitiesMembershipProvider _coreEntitiesMembershipProvider;

        public SettingsController()
        {
            _coreEntitiesContainer = new CoreEntitiesContainer();
            _coreEntitiesContainer.ContextOptions.LazyLoadingEnabled = false;
            _coreEntitiesMembershipProvider = new CoreEntitiesMembershipProvider(_coreEntitiesContainer);
        }

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
            return _coreEntitiesContainer.Parties.OfType<FoundOps.Core.Models.CoreEntities.UserAccount>().Any(ua => ua.EmailAddress.Trim() == newEmail.Trim());
        }

        /// <summary>
        /// The user exists response.
        /// </summary>
        private HttpResponseMessage UserExistsResponse()
        {
            return Request.CreateResponse(HttpStatusCode.Conflict, "This email address already exists");
        }

        #endregion

        #region Personal Settings

        //[System.Web.Http.AcceptVerbs("GET", "POST")]
        //public UserSettings GetPersonalSettings()
        //{
        //    var user = _coreEntitiesContainer.CurrentUserAccount().First();

        //    user.PartyImageReference.Load();

        //    var timezone = GetTimeZones().FirstOrDefault(tz => tz.TimeZoneId == user.TimeZone);

        //    var userSettings = new UserSettings
        //    {
        //        Id = user.Id,
        //        FirstName = user.FirstName,
        //        LastName = user.LastName,
        //        EmailAddress = user.EmailAddress.Trim(),
        //        TimeZoneInfo = timezone
        //    };

        //    //Load image url
        //    if (user.PartyImage != null)
        //    {
        //        var imageUrl = user.PartyImage.RawUrl + AzureServerHelpers.GetBlobUrlHelper(user.Id, user.PartyImage.Id);
        //        userSettings.ImageUrl = imageUrl;
        //    }
        //    else
        //    {
        //        userSettings.ImageUrl = "img/emptyPerson.png";
        //    }

        //    return userSettings;
        //}

        //[System.Web.Http.AcceptVerbs("POST")]
        //public HttpResponseMessage UpdatePersonalSettings(UserSettings settings, Guid roleId)
        //{
        //    var user = _coreEntitiesContainer.CurrentUserAccount().First();

        //    //The settings passed are not for the current User
        //    if (user.Id != settings.Id)
        //        return Request.CreateResponse(HttpStatusCode.BadRequest);

        //    if (UserExistsConflict(settings.EmailAddress, user.EmailAddress))
        //        return UserExistsResponse();

        //    //Update Properties
        //    user.FirstName = settings.FirstName;
        //    user.LastName = settings.LastName;
        //    user.EmailAddress = settings.EmailAddress.Trim();
        //    user.TimeZone = settings.TimeZoneInfo.TimeZoneId;

        //    //Save changes
        //    _coreEntitiesContainer.SaveChanges();

        //    return Request.CreateResponse(HttpStatusCode.Accepted);
        //}

        [AcceptVerbs("POST")]
        public HttpResponseMessage UpdatePassword(string newPass, string oldPass)
        {
            var user = _coreEntitiesContainer.CurrentUserAccount().First();

            return Request.CreateResponse(_coreEntitiesMembershipProvider.ChangePassword(user.EmailAddress, oldPass, newPass)
                                       ? HttpStatusCode.Accepted
                                       : HttpStatusCode.NotAcceptable);
        }

        /// <summary>
        /// Updates a user's image.
        /// The form should have 5 inputs:
        /// imageData: image file
        /// x, y, w, h: for cropping
        /// </summary>
        /// <returns>The image url, expiring in 3 hours</returns>
        public string UpdateUserImage()
        {
            var user = _coreEntitiesContainer.CurrentUserAccount().First();

            user.PartyImageReference.Load();

            if (user.PartyImage == null)
            {
                var partyImage = new PartyImage { OwnerParty = user };
                user.PartyImage = partyImage;
            }

            return UpdatePartyImageHelper(user);
        }

        #endregion

        //#region User Settings

        //[AcceptVerbs("GET", "POST")]
        //public IQueryable<UserSettings> GetAllUserSettings(Guid? roleId)
        //{
        //    //If there is no RoleId passed, we can assume that the user is not authorized to see all the User Settings
        //    if (!roleId.HasValue)
        //        ExceptionHelper.ThrowNotAuthorizedBusinessAccount();

        //    var businessAccount = _coreEntitiesContainer.Owner(roleId.Value, new[] { RoleType.Administrator }).FirstOrDefault();

        //    //If they are not an admin, they do not have the ability to view Users
        //    if (businessAccount == null)
        //        ExceptionHelper.ThrowNotAuthorizedBusinessAccount();

        //    UserAccount[] userAccounts;
        //    IQueryable<UserAccount> accesibleUserAccounts;

        //    //If the current account is a foundops account, return all user accounts
        //    if (businessAccount.Id == BusinessAccountsDesignData.FoundOps.Id)
        //    {
        //        accesibleUserAccounts = _coreEntitiesContainer.Parties.OfType<UserAccount>();
        //    }
        //    else
        //    {
        //        //If not a FoundOPS account, return the current business's owned roles memberparties
        //        accesibleUserAccounts =
        //            (from role in _coreEntitiesContainer.Roles.Where(r => r.OwnerBusinessAccountId == businessAccount.Id)
        //             from userAccount in _coreEntitiesContainer.Parties.OfType<UserAccount>()
        //             where role.MemberParties.Any(p => p.Id == userAccount.Id)
        //             select userAccount);
        //    }

        //    //If the serviceProviderId context is not empty
        //    //Filter only user accounts that are member parties of one of it's roles
        //    if (businessAccount.Id != Guid.Empty)
        //    {
        //        userAccounts = (from role in _coreEntitiesContainer.Roles.Where(r => r.OwnerBusinessAccountId == businessAccount.Id)
        //                        from userAccount in accesibleUserAccounts
        //                        where role.MemberParties.Any(p => p.Id == userAccount.Id)
        //                        orderby userAccount.LastName + " " + userAccount.FirstName
        //                        select userAccount).ToArray();
        //    }
        //    else
        //        userAccounts = accesibleUserAccounts.OrderBy(ua => ua.LastName + " " + ua.FirstName).ToArray();

        //    var userAccountsQueryable = (from user in userAccounts
        //                                 let role = _coreEntitiesContainer.Roles.FirstOrDefault(r => r.OwnerBusinessAccountId == businessAccount.Id && r.MemberParties.Any(p => p.Id == user.Id))
        //                                 let account = _coreEntitiesContainer.Parties.OfType<UserAccount>().Where(ua => ua.Id == user.Id).Include(ua => ua.LinkedEmployees).First()
        //                                 select UserSettings.ConvertModel(account, role)).ToList().AsQueryable();

        //    //if the user is not a foundops user, do not return FoundOPS accounts
        //    if (!_coreEntitiesContainer.CanAdministerFoundOPS())
        //        userAccountsQueryable = userAccountsQueryable.Where(ua => !ua.EmailAddress.Contains("foundops.com"));

        //    return userAccountsQueryable;
        //}

        //[AcceptVerbs("POST")]
        //public HttpResponseMessage InsertUserSettings(UserSettings settings, Guid? roleId)
        //{
        //    //Check for admin abilities
        //    var businessAccount = _coreEntitiesContainer.Owner(roleId.Value, new[] { RoleType.Administrator }).Include(ba => ba.OwnedRoles)
        //        .FirstOrDefault();

        //    //User must be in an admin role to Create new Users
        //    if (businessAccount == null)
        //        return Request.CreateResponse(HttpStatusCode.Unauthorized);

        //    #region Create new UserAccount

        //    //check the email address is not in use yet for this business account
        //    if (UserExistsConflict(settings.EmailAddress))
        //        return UserExistsResponse();

        //    var temporaryPassword = EmailPasswordTools.GeneratePassword();
        //    var salt = EncryptionTools.GenerateSalt();

        //    var user = new UserAccount
        //    {
        //        EmailAddress = settings.EmailAddress.Trim(),
        //        Id = Guid.NewGuid(),
        //        FirstName = settings.FirstName,
        //        LastName = settings.LastName,
        //        PasswordHash = EncryptionTools.Hash(temporaryPassword, salt),
        //        PasswordSalt = salt,
        //        TimeZone = "Eastern Standard Time"
        //    };

        //    //TODO?
        //    //TimeZone = settings.TimeZoneInfo.TimeZoneId

        //    //Find the role in the BusinessAccount that matches the name of the one passed in
        //    var role = _coreEntitiesContainer.Roles.FirstOrDefault(r => r.OwnerBusinessAccountId == businessAccount.Id && r.Name == settings.Role);
        //    user.RoleMembership.Add(role);

        //    #endregion

        //    //Add the newly created UserAccount to the database
        //    _coreEntitiesContainer.Parties.AddObject(user);

        //    if (settings.Employee.LastName != null)
        //    {
        //        var employee = _coreEntitiesContainer.Employees.FirstOrDefault(e => e.FirstName == settings.Employee.FirstName && e.LastName == settings.Employee.LastName && e.EmployerId == businessAccount.Id);

        //        user.LinkedEmployees.Add(employee);
        //    }
        //    else if (settings.Employee.FirstName == "Create")
        //    {
        //        var newEmployee = new FoundOps.Core.Models.CoreEntities.Employee
        //        {
        //            Id = Guid.NewGuid(),
        //            FirstName = settings.FirstName,
        //            LastName = settings.LastName,
        //            Employer = businessAccount
        //        };
        //        user.LinkedEmployees.Add(newEmployee);
        //    }

        //    _coreEntitiesContainer.SaveChanges();

        //    #region Send New User Email

        //    var sender = _coreEntitiesContainer.CurrentUserAccount().First().DisplayName;
        //    var recipient = user.FirstName;

        //    //Construct the email
        //    var subject = "Your FoundOPS invite from " + sender;
        //    var body = "Hi " + recipient + ", \r\n\r\n" +
        //                sender + " has created a user account for you in FoundOPS. \r\n\r\n" +
        //                "FoundOPS is an easy to use tool that helps field services teams communicate and provide the best possible service to their clients. \r\n\r\n" +
        //                "Click here to accept the invite: \r\n " + @"{0}" + "\r\n\r\n" +
        //                "If you have any difficulty accepting the invitation, email us at support@foundops.com. This invitation expires in 7 days. \r\n\r\n\r\n" +
        //                "The FoundOPS Team";

        //    _coreEntitiesMembershipProvider.ResetAccount(user.EmailAddress, subject, body, TimeSpan.FromDays(7));

        //    #endregion

        //    return Request.CreateResponse(HttpStatusCode.Created);
        //}

        ///// <summary>
        ///// This will be used when an admin user is trying to update other User Accounts
        ///// </summary>
        ///// <param name="settings">The User Settigns being changed</param>
        ///// <param name="roleId">roleId of the Admin user making the changed</param>
        ///// <returns>HttpResponseMessage</returns>
        //[System.Web.Http.AcceptVerbs("POST")]
        //public HttpResponseMessage UpdateUserSettings(UserSettings settings, Guid? roleId)
        //{
        //    //if role has value, means that they are editing as a admin -> find business off that role
        //    //only under that scenario will they be able to edit the role type
        //    //Remove them from any roles they are in in the BA and set to the new role
        //    if (!roleId.HasValue)
        //        return Request.CreateResponse(HttpStatusCode.NotAcceptable);

        //    var businessAccount = _coreEntitiesContainer.Owner(roleId.Value, new[] { RoleType.Administrator }).FirstOrDefault();

        //    if (businessAccount == null)
        //        return Request.CreateResponse(HttpStatusCode.Unauthorized);

        //    var user = _coreEntitiesContainer.Parties.OfType<UserAccount>().Include(ua => ua.RoleMembership).Include(ua => ua.LinkedEmployees).First(ua => ua.Id == settings.Id);

        //    if (UserExistsConflict(settings.EmailAddress, user.EmailAddress))
        //        return UserExistsResponse();

        //    user.FirstName = settings.FirstName;
        //    user.LastName = settings.LastName;
        //    user.EmailAddress = settings.EmailAddress;
        //    user.TimeZone = settings.TimeZoneInfo.TimeZoneId;

        //    var userRole = _coreEntitiesContainer.Roles.FirstOrDefault(r => r.OwnerBusinessAccountId == businessAccount.Id && r.MemberParties.Any(p => p.Id == user.Id));

        //    //If a new role has been selected for the user, remove all old ones and assign the new one
        //    if (userRole != null && userRole.Name != settings.Role)
        //    {
        //        user.RoleMembership.Remove(userRole);

        //        //Find the new role to be added for the user
        //        var newRole = _coreEntitiesContainer.Roles.FirstOrDefault(
        //                r => r.OwnerBusinessAccountId == businessAccount.Id && r.Name == settings.Role);

        //        //Add the new role for the user
        //        if (newRole != null)
        //            user.RoleMembership.Add(newRole);
        //    }

        //    var userLinkedEmployee = user.LinkedEmployees.FirstOrDefault(e => e.EmployerId == businessAccount.Id);

        //    var employee = userLinkedEmployee != null ? _coreEntitiesContainer.Employees.FirstOrDefault(e => userLinkedEmployee.Id == e.Id) : null;

        //    //Changing a user from one employee to another
        //    if (employee != null && settings.Employee != null && employee.Id != settings.Employee.Id
        //        //ignore the none option
        //        && settings.Employee.Id != Guid.Empty)
        //    {
        //        user.LinkedEmployees.Remove(employee);

        //        LinkEmployeeToUser(settings, user, businessAccount);
        //    }

        //    //Changing a user from an employee to no employee
        //    //If the employee Id == Guid.Empty, the none option was chosen
        //    else if (settings.Employee != null && employee != null && settings.Employee.Id == Guid.Empty)
        //    {
        //        user.LinkedEmployees.Remove(employee);
        //    }
        //    //Changing a user from no employee to an employee
        //    else if (employee == null && settings.Employee != null && settings.Employee.Id != Guid.Empty)
        //    {
        //        LinkEmployeeToUser(settings, user, businessAccount);
        //    }

        //    _coreEntitiesContainer.SaveChanges();

        //    return Request.CreateResponse(HttpStatusCode.Accepted);
        //}

        ////This will link the employee to the user account. If there is no employee, it will create a new one
        //private void LinkEmployeeToUser(UserSettings settings, UserAccount user, BusinessAccount businessAccount)
        //{
        //    var employee = _coreEntitiesContainer.Employees.FirstOrDefault(e => e.Id == settings.Employee.Id);

        //    if (employee != null)
        //        user.LinkedEmployees.Add(employee);
        //    else
        //    {
        //        employee = new FoundOps.Core.Models.CoreEntities.Employee
        //        {
        //            Id = Guid.NewGuid(),
        //            FirstName = settings.EmailAddress,
        //            LastName = settings.LastName,
        //            Employer = businessAccount
        //        };
        //        user.LinkedEmployees.Add(employee);
        //    }
        //}

        //[System.Web.Http.AcceptVerbs("POST")]
        //public HttpResponseMessage DeleteUserSettings(UserSettings settings, Guid? roleId)
        //{
        //    if (!roleId.HasValue)
        //        return Request.CreateResponse(HttpStatusCode.Unauthorized, "Invalid RoleId passed");

        //    var businessAccount = _coreEntitiesContainer.Owner(roleId.Value, new[] { RoleType.Administrator }).FirstOrDefault();

        //    //If they are not an admin, they do not have the ability to insert new Users
        //    if (businessAccount == null)
        //        return Request.CreateResponse(HttpStatusCode.Unauthorized, "User does not have Admin abilities");

        //    var user = _coreEntitiesContainer.Parties.OfType<UserAccount>().First(ua => ua.Id == settings.Id);

        //    foreach (var employee in user.LinkedEmployees)
        //    {
        //        user.LinkedEmployees.Remove(employee);
        //    }

        //    _coreEntitiesContainer.DeleteUserAccountBasedOnId(settings.Id);

        //    return Request.CreateResponse(HttpStatusCode.Accepted);
        //}

        //#endregion

        #region Business Settings

        [AcceptVerbs("GET", "POST")]
        public BusinessSettings GetBusinessSettings(Guid roleId)
        {
            var businessAccount = _coreEntitiesContainer.Owner(roleId, new[] { RoleType.Administrator }).FirstOrDefault();

            if (businessAccount == null)
                throw CommonExceptions.NotAuthorizedBusinessAccount;

            businessAccount.PartyImageReference.Load();

            var businessSettings = new BusinessSettings { Name = businessAccount.Name };

            //Load image url
            if (businessAccount.PartyImage != null)
            {
                var imageUrl = businessAccount.PartyImage.RawUrl + AzureServerHelpers.GetBlobUrlHelper(businessAccount.Id, businessAccount.PartyImage.Id);
                businessSettings.ImageUrl = imageUrl;
            }

            return businessSettings;
        }

        [AcceptVerbs("POST")]
        public HttpResponseMessage UpdateBusinessSettings(BusinessSettings settings, Guid roleId)
        {
            var businessAccount = _coreEntitiesContainer.Owner(roleId, new[] { RoleType.Administrator }).FirstOrDefault();

            if (businessAccount == null)
                return Request.CreateResponse(HttpStatusCode.Unauthorized, "User does not have Admin abilities");

            businessAccount.Name = settings.Name;
            _coreEntitiesContainer.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.Accepted);
        }

        /// <summary>
        /// Updates a business's image.
        /// The form should have 5 inputs:
        /// imageData: image file
        /// x, y, w, h: for cropping
        /// </summary>
        /// <returns>The image url, expiring in 3 hours</returns>
        [AcceptVerbs("POST")]
        public string UpdateBusinessImage(Guid roleId)
        {
            var businessAccount = _coreEntitiesContainer.Owner(roleId, new[] { RoleType.Administrator }).FirstOrDefault();

            if (businessAccount == null)
                throw CommonExceptions.NotAuthorizedBusinessAccount;

            businessAccount.PartyImageReference.Load();

            if (businessAccount.PartyImage == null)
            {
                var partyImage = new PartyImage { OwnerParty = businessAccount };
                businessAccount.PartyImage = partyImage;
            }

            return UpdatePartyImageHelper(businessAccount);
        }

        #endregion

        #region Employees

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IQueryable<Employee> GetAllEmployeesForBusiness(Guid? roleId)
        {
            //If there is no RoleId passed, we can assume that the user is not authorized to see all the User Settings
            if (!roleId.HasValue)
                throw CommonExceptions.NotAuthorizedBusinessAccount;

            var businessAccount = _coreEntitiesContainer.Owner(roleId.Value).FirstOrDefault();

            //If they are not an admin, they do not have the ability to view Users
            if (businessAccount == null)
                throw CommonExceptions.NotAuthorizedBusinessAccount;

            var employees = _coreEntitiesContainer.Employees.Where(e => e.EmployerId == businessAccount.Id).ToList();

            //Add blank employee
            var newEmployee = new FoundOps.Core.Models.CoreEntities.Employee
            {
                //Identify it with an empty guid
                Id = new Guid(),
                FirstName = "None",
                LastName = ""
            };

            employees.Add(newEmployee);

            return employees.Select(Employee.ConvertModel).OrderBy(e => e.FirstName).AsQueryable();
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Updates a party's image.
        /// The Request should send a form with 6 inputs: imageData, imageFileName, x, y, w, h
        /// </summary>
        /// <param name="partyToUpdate">The party to update</param>
        /// <returns>The image url, expiring in 1 hour</returns>
        private string UpdatePartyImageHelper(Party partyToUpdate)
        {
            var formDataTask = Request.ReadMultipartAsync(new[] { "imageFileName", "imageData" });
            formDataTask.Wait();

            var formData = formDataTask.Result;

            var imageFileNameTask = formData["imageFileName"].ReadAsStringAsync();
            var imageDataStringTask = formData["imageData"].ReadAsStringAsync();

            imageFileNameTask.Wait();
            var imageFileName = imageFileNameTask.Result;
            if (string.IsNullOrEmpty(imageFileName))
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest, "imageFileName was not set"));

            imageDataStringTask.Wait();
            var imageDataString = imageDataStringTask.Result;

            //Remove prefaced metadata ex: "data:image/png;base64"
            var metadataIndex = imageDataString.IndexOf("base64,");
            if (metadataIndex >= 0)
                imageDataString = imageDataString.Substring(metadataIndex + 7);

            var imageBytes = Convert.FromBase64String(imageDataString);

            var blob = AzureServerHelpers.GetBlobHelper(partyToUpdate.Id, partyToUpdate.PartyImage.Id);

            // Get the file extension to make sure it is an image that can be processed
            var extension = Path.GetExtension(imageFileName).ToLower();
            string[] allowedExtensions = { ".png", ".jpeg", ".jpg", ".gif" };
            if (!allowedExtensions.Contains(extension))
                throw new Exception("Cannot process files of this type.");

            blob.UploadByteArray(imageBytes);

            // Set the metadata/properties into the blob
            blob.Metadata["Submitter"] = partyToUpdate.Id.ToString();
            blob.SetMetadata();

            var contentType = "image/" + extension.Replace(".", "");
            blob.Properties.ContentType = contentType;
            blob.SetProperties();

            partyToUpdate.PartyImage.Name = imageFileName;
            _coreEntitiesContainer.SaveChanges();

            var readOnlyUrl = partyToUpdate.PartyImage.RawUrl + blob.GetSharedAccessSignature(new SharedAccessPolicy
            {
                Permissions = SharedAccessPermissions.Read,
                SharedAccessExpiryTime = DateTime.UtcNow + AzureServerHelpers.DefaultExpiration
            });

            return readOnlyUrl;
        }

        #endregion
    }
}
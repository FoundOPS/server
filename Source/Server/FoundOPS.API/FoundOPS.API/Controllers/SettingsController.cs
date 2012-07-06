using System.Collections.Generic;
using FoundOPS.API.Models;
using FoundOPS.API.Tools;
using FoundOps.Common.NET;
using FoundOps.Core.Models.Azure;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Models.CoreEntities.DesignData;
using FoundOps.Core.Tools;
using Microsoft.WindowsAzure.StorageClient;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace FoundOPS.API.Controllers
{
#if !DEBUG
    [Authorize]
#endif
    public class SettingsController : ApiController
    {
        private readonly CoreEntitiesContainer _coreEntitiesContainer;

        public SettingsController()
        {
            _coreEntitiesContainer = new CoreEntitiesContainer();
            _coreEntitiesContainer.ContextOptions.LazyLoadingEnabled = false;
        }

        #region Personal Settings

        [AcceptVerbs("GET", "POST")]
        public UserSettings GetPersonalSettings()
        {
            var user = _coreEntitiesContainer.CurrentUserAccount().Include(u => u.PartyImage).First();

            var userSettings = new UserSettings
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                EmailAddress = user.EmailAddress
            };

            //Load image url
            if (user.PartyImage != null)
            {
                var imageUrl = user.PartyImage.RawUrl + AzureServerHelpers.GetBlobUrlHelper(user.Id, user.PartyImage.Id);
                userSettings.ImageUrl = imageUrl;
            }

            return userSettings;
        }

        [AcceptVerbs("POST")]
        public HttpResponseMessage UpdatePersonalSettings(UserSettings settings)
        {
            var user = _coreEntitiesContainer.CurrentUserAccount().First();

            //The settings passed are not for the current User
            if (user.Id != settings.Id)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            //If the email address of the current user changed, check the email address is not in use yet
            if (user.EmailAddress != settings.EmailAddress &&
                _coreEntitiesContainer.Parties.OfType<UserAccount>().Any(ua => ua.EmailAddress == user.EmailAddress))
                throw new Exception("The email address is already in use");

            //Update Properties
            user.FirstName = settings.FirstName;
            user.LastName = settings.LastName;
            user.EmailAddress = settings.EmailAddress;

            //Save changes
            _coreEntitiesContainer.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.Accepted);
        }

        /// <summary>
        /// Updates a user's image.
        /// The form should have 5 inputs:
        /// imageData: image file
        /// x, y, w, h: for cropping
        /// </summary>
        /// <returns>The image url, expiring in 3 hours</returns>
        public Task<string> UpdateUserImage()
        {
            var user = _coreEntitiesContainer.CurrentUserAccount().Include(u => u.PartyImage).First();

            if (user.PartyImage == null)
            {
                var partyImage = new PartyImage { OwnerParty = user };
                user.PartyImage = partyImage;
            }

            return UpdatePartyImageHelper(user);
        }

        #endregion

        #region User Settings

        [AcceptVerbs("GET", "POST")]
        public IQueryable<UserSettings> GetAllUserSettings(Guid? roleId)
        {
            //If there is no RoleId passed, we can assume that the user is not authorized to see all the User Settings
            if (!roleId.HasValue)
                ExceptionHelper.ThrowNotAuthorizedBusinessAccount();

            var businessAccount = _coreEntitiesContainer.Owner(roleId.Value).First();

            //If they are not an admin, they do not have the ability to view Users
            if (businessAccount == null)
                ExceptionHelper.ThrowNotAuthorizedBusinessAccount();

            UserAccount[] userAccounts;
            IQueryable<UserAccount> accesibleUserAccounts;

            //If the current account is a foundops account, return all user accounts
            if (businessAccount.Id == BusinessAccountsDesignData.FoundOps.Id)
            {
                accesibleUserAccounts = _coreEntitiesContainer.Parties.OfType<UserAccount>();
            }
            else
            {
                //If not a FoundOPS account, return the current business's owned roles memberparties
                accesibleUserAccounts =
                    from role in _coreEntitiesContainer.Roles.Where(r => r.OwnerPartyId == businessAccount.Id)
                    from userAccount in _coreEntitiesContainer.Parties.OfType<UserAccount>()
                    where role.MemberParties.Any(p => p.Id == userAccount.Id)
                    select userAccount;
            }

            //If the serviceProviderId context is not empty
            //Filter only user accounts that are member parties of one of it's roles
            if (businessAccount.Id != Guid.Empty)
            {
                userAccounts = (from role in _coreEntitiesContainer.Roles.Where(r => r.OwnerPartyId == businessAccount.Id)
                                from userAccount in accesibleUserAccounts
                                where role.MemberParties.Any(p => p.Id == userAccount.Id)
                                orderby userAccount.LastName + " " + userAccount.FirstName
                                select userAccount).ToArray();
            }
            else
                userAccounts = accesibleUserAccounts.OrderBy(ua => ua.LastName + " " + ua.FirstName).ToArray();

            return userAccounts.Select(UserSettings.ConvertModel).AsQueryable();
        }

        [AcceptVerbs("POST")]
        public HttpResponseMessage InsertUserSettings(UserSettings settings, Guid? roleId)
        {
            //Check for admin abilities
            if (roleId.HasValue)
            {
                var businessAccount = _coreEntitiesContainer.Owner(roleId.Value).FirstOrDefault();

                //If they are not an admin, they do not have the ability to insert new Users
                if (businessAccount == null)
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);

                #region Create new UserAccount

                var user = new UserAccount();

                if (_coreEntitiesContainer.Parties.OfType<UserAccount>().Any(ua => ua.EmailAddress == settings.EmailAddress))
                    throw new Exception("The email address is already in use");
                
                user.EmailAddress = settings.EmailAddress;
                user.Id = Guid.NewGuid();
                user.FirstName = settings.FirstName;
                user.LastName = settings.LastName;

                //Find the role in the BusinessAccount that matches the name of the one passed in.
                var newRole = _coreEntitiesContainer.Roles.FirstOrDefault(r => r.OwnerParty.Id == businessAccount.Id && r.Name == settings.Role);

                if (newRole != null)
                    user.RoleMembership.Add(newRole);

                #endregion

                //Add the newly created UserAccount to the database
                _coreEntitiesContainer.Parties.AddObject(user);

                //Save all changes made
                _coreEntitiesContainer.SaveChanges();

                return Request.CreateResponse(HttpStatusCode.Accepted);
            }

            //User must be in an admin role to Create new Users
            return Request.CreateResponse(HttpStatusCode.Unauthorized);
        }

        /// <summary>
        /// This will be used when an admin user is trying to update other User Accounts
        /// </summary>
        /// <param name="settings">The User Settigns being changed</param>
        /// <param name="roleId">roleId of the Admin user making the changed</param>
        /// <returns>HttpResponseMessage</returns>
        [AcceptVerbs("POST")]
        public HttpResponseMessage UpdateUserSettings(UserSettings settings, Guid? roleId)
        {
            //if role has value, means that they are editing as a admin -> find business off that role
            //only under that scenario will they be able to edit the role type
            //Remove them from any roles they are in in the BA and set to the new role
            if (!roleId.HasValue)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            var businessAccount = _coreEntitiesContainer.Owner(roleId.Value).FirstOrDefault();

            if (businessAccount == null)
                return Request.CreateResponse(HttpStatusCode.Unauthorized);

            var user = _coreEntitiesContainer.Parties.OfType<UserAccount>().First(ua => ua.Id == settings.Id);

            var emailExists = _coreEntitiesContainer.Parties.OfType<UserAccount>().Any(ua => ua.EmailAddress == settings.EmailAddress);

            //If the email address of the current user changed, check the email address is not in use yet
            if (user.EmailAddress != settings.EmailAddress && emailExists)
                throw new Exception("The email address is already in use");

            user.FirstName = settings.FirstName;
            user.LastName = settings.LastName;
            user.EmailAddress = settings.EmailAddress;

            //If a new role has been selected for the user, remove all old ones and assign the new one
            if (!user.RoleMembership.Select(r => r.Name).Contains(settings.Role))
            {
                //Check each role the user has and remove all where the OwnerParty is this BuinessAccount
                foreach (var role in user.RoleMembership)
                {
                    if (role.OwnerParty == businessAccount)
                        user.RoleMembership.Remove(role);
                }

                //Find the new role to be added for the user
                var newRole = _coreEntitiesContainer.Roles.FirstOrDefault(
                        r => r.OwnerParty.Id == businessAccount.Id && r.Name == settings.Role);

                //Add the new role for the user
                if (newRole != null)
                    user.RoleMembership.Add(newRole);
            }

            _coreEntitiesContainer.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.Accepted);
        }

        #endregion

        #region Business Settings

        [AcceptVerbs("GET", "POST")]
        public BusinessSettings GetBusinessSettings(Guid roleId)
        {
            var businessAccount = _coreEntitiesContainer.Owner(roleId).Include(u => u.PartyImage).FirstOrDefault();

            if (businessAccount == null)
                ExceptionHelper.ThrowNotAuthorizedBusinessAccount();

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
        public HttpResponseMessage UpdateBusinessSettings(Guid roleId, BusinessSettings settings)
        {
            var businessAccount = _coreEntitiesContainer.Owner(roleId).FirstOrDefault();

            if (businessAccount == null)
                ExceptionHelper.ThrowNotAuthorizedBusinessAccount();

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
        public Task<string> UpdateBusinessImage(Guid roleId)
        {
            var businessAccount = _coreEntitiesContainer.Owner(roleId).Include(ba => ba.PartyImage).FirstOrDefault();

            if (businessAccount == null)
                ExceptionHelper.ThrowNotAuthorizedBusinessAccount();

            if (businessAccount.PartyImage == null)
            {
                var partyImage = new PartyImage { OwnerParty = businessAccount };
                businessAccount.PartyImage = partyImage;
            }

            return UpdatePartyImageHelper(businessAccount);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Updates a party's image.
        /// The Request should send a form with 6 inputs: imageData, imageFileName, x, y, w, h
        /// </summary>
        /// <param name="partyToUpdate">The party to update</param>
        /// <returns>The image url, expiring in 3 hours</returns>
        private async Task<string> UpdatePartyImageHelper(Party partyToUpdate)
        {
            var formData = await Request.ReadMultipartAsync(new[] { "imageFileName", "imageData"});

            var imageFileName = await formData["imageFileName"].ReadAsStringAsync();
            var imageDataString = await formData["imageData"].ReadAsStringAsync();

            if (string.IsNullOrEmpty(imageFileName))
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest, "imageFileName was not set"));

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
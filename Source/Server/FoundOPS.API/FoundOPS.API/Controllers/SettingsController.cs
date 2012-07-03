using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Web;
using FoundOPS.API.Models;
using FoundOps.Common.Composite.Tools;
using FoundOps.Common.NET;
using FoundOps.Core.Models.Azure;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Tools;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.WindowsAzure.StorageClient;

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

        #region Helpers

        /// <summary>
        /// Updates a party's image.
        /// </summary>
        /// <param name="partyToUpdate">The party to update</param>
        /// <param name="imageData">The image data</param>
        /// <param name="x">For cropping</param>
        /// <param name="y">For cropping</param>
        /// <param name="w">For cropping</param>
        /// <param name="h">For cropping</param>
        /// <returns>The image url, expiring in 3 hours</returns>
        public string UpdatePartyImageHelper(Party partyToUpdate, HttpPostedFileBase imageData, int x, int y, int w, int h)
        {
            var blob = AzureServerHelpers.GetBlobHelper(partyToUpdate.Id, partyToUpdate.PartyImage.Id);

            // Get the file extension
            var extension = Path.GetExtension(imageData.FileName).ToLower();
            string[] allowedExtensions = { ".png", ".jpeg", ".jpg", ".gif" }; // Make sure it is an image that can be processed
            if (!allowedExtensions.Contains(extension))
                throw new Exception("Cannot process files of this type.");

            var imageBytes = imageData.InputStream.ReadFully();

            //If the user selected a crop area, crop the image
            if (w != 0 && h != 0)
                imageBytes = ImageTools.CropImage(imageBytes, extension, x, y, w, h);

            blob.UploadByteArray(imageBytes);

            // Set the metadata/properties into the blob
            blob.Metadata["Submitter"] = partyToUpdate.Id.ToString();
            blob.SetMetadata();

            blob.Properties.ContentType = imageData.ContentType;
            blob.SetProperties();

            _coreEntitiesContainer.SaveChanges();

            var readOnlyUrl = blob.GetSharedAccessSignature(new SharedAccessPolicy
            {
                Permissions = SharedAccessPermissions.Read,
                SharedAccessExpiryTime = DateTime.UtcNow + AzureServerHelpers.DefaultExpiration
            });

            return readOnlyUrl;
        }

        #endregion

        [AcceptVerbs("GET", "POST")]
        public UserSettings GetUserSettings()
        {
            var user = AuthenticationLogic.CurrentUserAccountQueryable(_coreEntitiesContainer).Include(u => u.PartyImage).First();
            var userSettings = new UserSettings
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                EmailAddress = user.EmailAddress
            };

            //Load image url
            if (user.PartyImage != null)
            {
                var imageUrl = AzureServerHelpers.GetBlobUrlHelper(user.Id, user.PartyImage.Id);
                userSettings.ImageUrl = imageUrl;
            }

            return userSettings;
        }

        [AcceptVerbs("POST")]
        public HttpResponseMessage UpdateUserSettings(UserSettings settings)
        {
            var user = AuthenticationLogic.CurrentUserAccountQueryable(_coreEntitiesContainer).First();

            //If the email address of the current user changed, check the email address is not in use yet
            if (user.EmailAddress != settings.EmailAddress &&
                _coreEntitiesContainer.Parties.OfType<UserAccount>().Any(ua => ua.EmailAddress == user.EmailAddress))
                throw new Exception("The email address is already in use");

            user.FirstName = settings.FirstName;
            user.LastName = settings.LastName;
            user.EmailAddress = settings.EmailAddress;

            _coreEntitiesContainer.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.Accepted);
        }

        /// <summary>
        /// Updates a user's image.
        /// </summary>
        /// <param name="imageData">The image data</param>
        /// <param name="x">For cropping</param>
        /// <param name="y">For cropping</param>
        /// <param name="w">For cropping</param>
        /// <param name="h">For cropping</param>
        /// <returns>The image url, expiring in 3 hours</returns>
        public string UpdateUserImage(HttpPostedFileBase imageData, int x, int y, int w, int h)
        {
            var user = AuthenticationLogic.CurrentUserAccountQueryable(_coreEntitiesContainer).Include(u => u.PartyImage).First();

            if (user.PartyImage == null)
                user.PartyImage = new PartyImage();

            return UpdatePartyImageHelper(user, imageData, x, y, w, h);
        }

        [AcceptVerbs("GET", "POST")]
        public BusinessSettings GetBusinessSettings(Guid roleId)
        {
            var businessAccount = _coreEntitiesContainer.BusinessAccountOwnerOfRoleQueryable(roleId).FirstOrDefault();

            if (businessAccount == null)
                ExceptionHelper.ThrowNotAuthorizedBusinessAccount();

            var businessSettings = new BusinessSettings { Name = businessAccount.Name };

            //Load image url
            if (businessAccount.PartyImage != null)
            {
                var imageUrl = AzureServerHelpers.GetBlobUrlHelper(businessAccount.Id, businessAccount.PartyImage.Id);
                businessSettings.ImageUrl = imageUrl;
            }

            return businessSettings;
        }

        [AcceptVerbs("POST")]
        public HttpResponseMessage UpdateBusinessSettings(Guid roleId, BusinessSettings settings)
        {
            var businessAccount = _coreEntitiesContainer.BusinessAccountOwnerOfRoleQueryable(roleId).FirstOrDefault();

            if (businessAccount == null)
                ExceptionHelper.ThrowNotAuthorizedBusinessAccount();

            businessAccount.Name = settings.Name;
            _coreEntitiesContainer.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.Accepted);
        }

        /// <summary>
        /// Updates a user's image.
        /// </summary>
        /// <param name="roleId">The current role id</param>
        /// <param name="imageData">The image data</param>
        /// <param name="x">For cropping</param>
        /// <param name="y">For cropping</param>
        /// <param name="w">For cropping</param>
        /// <param name="h">For cropping</param>
        /// <returns>The image url, expiring in 3 hours</returns>
        [AcceptVerbs("POST")]
        public string UpdateBusinessImage(Guid roleId, HttpPostedFileBase imageData, int x, int y, int w, int h)
        {
            var businessAccount = _coreEntitiesContainer.BusinessAccountOwnerOfRoleQueryable(roleId).Include(ba => ba.PartyImage).FirstOrDefault();

            if (businessAccount == null)
                ExceptionHelper.ThrowNotAuthorizedBusinessAccount();

            if (businessAccount.PartyImage == null)
                businessAccount.PartyImage = new PartyImage();

            return UpdatePartyImageHelper(businessAccount, imageData, x, y, w, h);
        }
    }
}
using System.Diagnostics;
using System.Threading.Tasks;
using FoundOPS.API.Models;
using FoundOPS.API.Tools;
using FoundOps.Common.Composite.Tools;
using FoundOps.Common.NET;
using FoundOps.Core.Models.Azure;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Tools;
using Microsoft.WindowsAzure.StorageClient;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
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
        /// Updates a party's image.
        /// The Request should send a form with 6 inputs: imageData, imageFileName, x, y, w, h
        /// </summary>
        /// <param name="partyToUpdate">The party to update</param>
        /// <returns>The image url, expiring in 3 hours</returns>
        private async Task<string> UpdatePartyImageHelper(Party partyToUpdate)
        {
            var formData = await Request.ReadMultipartAsync(new[] { "imageFileName", "imageData", "x", "y", "w", "h" });

            var imageFileName = await formData["imageFileName"].ReadAsStringAsync();
            var imageDataStream = await formData["imageData"].ReadAsStreamAsync();
            var x = Convert.ToInt32(await formData["x"].ReadAsStringAsync());
            var y = Convert.ToInt32(await formData["y"].ReadAsStringAsync());
            var w = Convert.ToInt32(await formData["w"].ReadAsStringAsync());
            var h = Convert.ToInt32(await formData["h"].ReadAsStringAsync());

            var imageBytes = imageDataStream.ReadFully();

            var blob = AzureServerHelpers.GetBlobHelper(partyToUpdate.Id, partyToUpdate.PartyImage.Id);

            // Get the file extension to make sure it is an image that can be processed
            var extension = Path.GetExtension(imageFileName).ToLower();
            string[] allowedExtensions = { ".png", ".jpeg", ".jpg", ".gif" };
            if (!allowedExtensions.Contains(extension))
                throw new Exception("Cannot process files of this type.");

            //If the user selected a crop area, crop the image
            if (w != 0 && h != 0)
                imageBytes = ImageTools.CropImage(imageBytes, extension, x, y, w, h);

            blob.UploadByteArray(imageBytes);

            // Set the metadata/properties into the blob
            blob.Metadata["Submitter"] = partyToUpdate.Id.ToString();
            blob.SetMetadata();

            var contentType = "image/" + extension.Replace(".", "");
            blob.Properties.ContentType = contentType;
            blob.SetProperties();

            _coreEntitiesContainer.SaveChanges();

            var readOnlyUrl = blob.GetSharedAccessSignature(new SharedAccessPolicy
            {
                Permissions = SharedAccessPermissions.Read,
                SharedAccessExpiryTime = DateTime.UtcNow + AzureServerHelpers.DefaultExpiration
            });

            return readOnlyUrl;
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
            var user = AuthenticationLogic.CurrentUserAccountQueryable(_coreEntitiesContainer).Include(u => u.PartyImage).First();

            if (user.PartyImage == null)
                user.PartyImage = new PartyImage();

            return UpdatePartyImageHelper(user);
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
        /// Updates a business's image.
        /// The form should have 5 inputs:
        /// imageData: image file
        /// x, y, w, h: for cropping
        /// </summary>
        /// <returns>The image url, expiring in 3 hours</returns>
        [AcceptVerbs("POST")]
        public Task<string> UpdateBusinessImage(Guid roleId)
        {
            var businessAccount = _coreEntitiesContainer.BusinessAccountOwnerOfRoleQueryable(roleId).Include(ba => ba.PartyImage).FirstOrDefault();

            if (businessAccount == null)
                ExceptionHelper.ThrowNotAuthorizedBusinessAccount();

            if (businessAccount.PartyImage == null)
                businessAccount.PartyImage = new PartyImage();

            return UpdatePartyImageHelper(businessAccount);
        }
    }
}
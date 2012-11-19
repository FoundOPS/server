using FoundOps.Api.Tools;
using FoundOps.Core.Models.Azure;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Tools;
using Microsoft.WindowsAzure.StorageClient;
using System;
using System.IO;
using System.Linq;

namespace FoundOps.Api.Controllers.Rest
{
    public class PartyImageController : BaseApiController
    {
        // POST api/partyImage
        /// <summary>
        /// Updates a party's image.
        /// The Request should send a form with 6 inputs: imageData, imageFileName, x, y, w, h
        /// </summary>
        /// <returns>The image url, expiring in 1 hour</returns>
        public string Post(Guid roleId, Guid id)
        {
            var party = CoreEntitiesContainer.Parties.First(p => p.Id == id);
            party.PartyImageReference.Load();
            if (party.PartyImage == null)
            {
                var partyImage = new PartyImage { OwnerParty = party };
                party.PartyImage = partyImage;
            }

            //check the user has access to the party
            //a) if the party is a user account, make sure it is the current user
            //b) if the party is a business account, make sure the current user has admin or regular privelages for that business
            if (party as UserAccount != null)
            {
                if (party.Id != CoreEntitiesContainer.CurrentUserAccount().Id)
                    throw Request.NotAuthorized();
            }
            else if (party as BusinessAccount != null)
            {
                if (!CoreEntitiesContainer.BusinessAccount(party.Id).Any())
                    throw Request.NotAuthorized();
            }
            else
            {
                throw Request.NotFound();
            }

            var formDataTask = Request.ReadMultipartAsync(new[] { "imageFileName", "imageData" });
            formDataTask.Wait();

            var formData = formDataTask.Result;

            var imageFileNameTask = formData["imageFileName"].ReadAsStringAsync();
            var imageDataStringTask = formData["imageData"].ReadAsStringAsync();

            imageFileNameTask.Wait();
            var imageFileName = imageFileNameTask.Result;
            if (string.IsNullOrEmpty(imageFileName))
                throw Request.BadRequest("imageFileName was not set");

            imageDataStringTask.Wait();
            var imageDataString = imageDataStringTask.Result;

            //Remove prefaced metadata ex: "data:image/png;base64"
            var metadataIndex = imageDataString.IndexOf("base64,");
            if (metadataIndex >= 0)
                imageDataString = imageDataString.Substring(metadataIndex + 7);

            var imageBytes = Convert.FromBase64String(imageDataString);

            party.PartyImageReference.Load();
            //the party image file id is the same as the party id
            var blob = AzureServerHelpers.GetBlobHelper(id, id);

            // Get the file extension to make sure it is an image that can be processed
            var extension = Path.GetExtension(imageFileName);
            string[] allowedExtensions = { ".png", ".jpeg", ".jpg", ".gif" };
            if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension.ToLower()))
                throw new Exception("Cannot process files of this type.");

            blob.UploadByteArray(imageBytes);

            // Set the metadata/properties into the blob
            blob.Metadata["Submitter"] = id.ToString();
            blob.SetMetadata();

            var contentType = "image/" + extension.Replace(".", "");
            blob.Properties.ContentType = contentType;
            blob.SetProperties();

            party.PartyImage.Name = imageFileName;
            party.PartyImage.CreatedDate = DateTime.UtcNow;

            SaveWithRetry();

            var readOnlyUrl = party.PartyImage.RawUrl + blob.GetSharedAccessSignature(new SharedAccessPolicy
            {
                Permissions = SharedAccessPermissions.Read,
                SharedAccessExpiryTime = DateTime.UtcNow + AzureServerHelpers.DefaultExpiration
            });

            return readOnlyUrl;
        }
    }
}

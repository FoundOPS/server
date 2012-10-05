using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using FoundOps.Core.Models.Azure;
using FoundOps.Core.Models.CoreEntities;
using Microsoft.WindowsAzure.StorageClient;

namespace FoundOps.Api.Tools
{
    public static class SettingsTools
    {
        private static readonly CoreEntitiesContainer CoreEntitiesContainer;

        static SettingsTools()
        {
            CoreEntitiesContainer = new CoreEntitiesContainer();
            CoreEntitiesContainer.ContextOptions.LazyLoadingEnabled = false;
        }

        /// <summary>
        /// Will return true if the email already exists.
        /// </summary>
        /// <param name="newEmail">The new email address</param>
        /// <param name="oldEmail">If the oldEmail and newEmail match this will return no conflict.</param>
        public static bool UserExistsConflict(string newEmail, string oldEmail)
        {
            return oldEmail != newEmail && UserExistsConflict(newEmail);
        }

        public static bool UserExistsConflict(string newEmail)
        {
            return CoreEntitiesContainer.Parties.OfType<FoundOps.Core.Models.CoreEntities.UserAccount>().Any(ua => ua.EmailAddress.Trim() == newEmail.Trim());
        }

        /// <summary>
        /// The user exists response.
        /// </summary>
        public static HttpResponseMessage UserExistsResponse(HttpRequestMessage request)
        {
            return request.CreateResponse(HttpStatusCode.Conflict, "This email address already exists");
        }

        /// <summary>
        /// Updates a party's image.
        /// The Request should send a form with 6 inputs: imageData, imageFileName, x, y, w, h
        /// </summary>
        /// <param name="partyToUpdate">The party to update</param>
        /// <returns>The image url, expiring in 1 hour</returns>
        public static string UpdatePartyImageHelper(Party partyToUpdate, HttpRequestMessage request)
        {
            var formDataTask = request.ReadMultipartAsync(new[] { "imageFileName", "imageData" });
            formDataTask.Wait();

            var formData = formDataTask.Result;

            var imageFileNameTask = formData["imageFileName"].ReadAsStringAsync();
            var imageDataStringTask = formData["imageData"].ReadAsStringAsync();

            imageFileNameTask.Wait();
            var imageFileName = imageFileNameTask.Result;
            if (string.IsNullOrEmpty(imageFileName))
                throw new HttpResponseException(request.CreateResponse(HttpStatusCode.BadRequest, "imageFileName was not set"));

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

            try
            {
                CoreEntitiesContainer.SaveChanges();
            }
            catch (Exception)
            {
                try
                {
                    //try one more time
                    CoreEntitiesContainer.SaveChanges();
                }
                catch (Exception)
                {
                    throw request.NotSaving();
                }
            }

            var readOnlyUrl = partyToUpdate.PartyImage.RawUrl + blob.GetSharedAccessSignature(new SharedAccessPolicy
            {
                Permissions = SharedAccessPermissions.Read,
                SharedAccessExpiryTime = DateTime.UtcNow + AzureServerHelpers.DefaultExpiration
            });

            return readOnlyUrl;
        }
    }
}
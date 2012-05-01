using FoundOps.Common.Tools;
using FoundOps.Core.Models.Azure;
using FoundOps.Core.Tools;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace FoundOps.Server.Controllers
{
    public class FileController : AsyncController
    {
        private const int TimeoutMilliseconds = 5000;

        /// <summary>
        /// Gets the url for retrieving a blob.
        /// </summary>
        /// <param name="ownerPartyId">The owner role id.</param>
        /// <param name="fileGuid">The file id's guid.</param>
        //TODO: Add Authorize attribute (if current user is in roleId)
        [AsyncTimeout(TimeoutMilliseconds)]
        [HandleError(ExceptionType = typeof(TaskCanceledException), View = "TimedOut")]
        public Task<string> GetBlobUrl(Guid ownerPartyId, Guid fileGuid)
        {
            return GetBlobUrlHelper(ownerPartyId, fileGuid);
        }

        //Allows other server code to use the GetBlobUrl logic.
        internal static Task<string> GetBlobUrlHelper(Guid ownerPartyId, Guid fileGuid)
        {
            return AsyncHelper.RunAsync(() =>
            {
                //Create service client for credentialed access to the Blob service.
                var blobClient = new CloudBlobClient(AzureTools.BlobStorageUrl, new StorageCredentialsAccountAndKey(AzureTools.AccountName, AzureConstants.AccountKey)) { Timeout = TimeSpan.FromMilliseconds(TimeoutMilliseconds) };

                //Get a reference to a container, which may or may not exist.
                var blobContainer = blobClient.GetContainerReference(AzureTools.BuildContainerUrl(ownerPartyId));

                //Create a new container, if it does not exist
                blobContainer.CreateIfNotExist(new BlobRequestOptions { Timeout = TimeSpan.FromMilliseconds(TimeoutMilliseconds) });

                //Setup cross domain policy so Silverlight can access the server
                CreateSilverlightPolicy(blobClient);

                //Get a reference to a blob, which may or may not exist.
                var blob = blobContainer.GetBlobReference(fileGuid.ToString());

                return blob.GetSharedAccessSignature(new SharedAccessPolicy { Permissions = SharedAccessPermissions.Read, SharedAccessExpiryTime = DateTime.UtcNow + new TimeSpan(0, 30, 0) });
            });
        }

        /// <summary>
        /// Gets the insert url for a blob.
        /// </summary>
        /// <param name="ownerPartyId">The owner role id.</param>
        /// <param name="fileGuid">The file id's guid.</param>
        //TODO: Add Authorize attribute (if current user is in roleId)
        [AsyncTimeout(TimeoutMilliseconds)]
        [HandleError(ExceptionType = typeof(TaskCanceledException), View = "TimedOut")]
        public Task<string> InsertBlobUrl(Guid ownerPartyId, Guid fileGuid)
        {
            return InsertBlobUrlHelper(ownerPartyId, fileGuid);
        }

        //Allows other server code to use the InsertBlobUrl logic.
        //Maximum blob size is 32mb.
        internal static Task<string> InsertBlobUrlHelper(Guid ownerPartyId, Guid fileGuid)
        {
            return AsyncHelper.RunAsync(() =>
            {
                //Create service client for credentialed access to the Blob service.
                var blobClient = new CloudBlobClient(AzureTools.BlobStorageUrl,
                    new StorageCredentialsAccountAndKey(AzureTools.AccountName, AzureConstants.AccountKey)) { Timeout = TimeSpan.FromMilliseconds(TimeoutMilliseconds) };

                //Get a reference to a container, which may or may not exist.
                var blobContainer = blobClient.GetContainerReference(AzureTools.BuildContainerUrl(ownerPartyId));
                //Create a new container, if it does not exist
                blobContainer.CreateIfNotExist(new BlobRequestOptions { Timeout = TimeSpan.FromMilliseconds(TimeoutMilliseconds) });

                //Setup cross domain policy so Silverlight can access the server
                CreateSilverlightPolicy(blobClient);

                //Get a reference to a blob, which may or may not exist.
                var blob = blobContainer.GetBlobReference(fileGuid.ToString());

                blob.UploadByteArray(new byte[] { });

                // Set the metadata into the blob
                blob.Metadata["Submitter"] = ownerPartyId.ToString();
                blob.SetMetadata();

                // Set the properties
                blob.Properties.ContentType = "application/octet-stream";
                blob.SetProperties();

                return blob.GetSharedAccessSignature(new SharedAccessPolicy { Permissions = SharedAccessPermissions.Write, SharedAccessExpiryTime = DateTime.UtcNow + new TimeSpan(0, 30, 0) });
            });
        }

        /// <summary>
        /// Deletes the blob.
        /// </summary>
        /// <param name="ownerPartyId">The owner role id.</param>
        /// <param name="fileGuid">The file id's guid.</param>
        //TODO: Add Authorize attribute (if current user is in roleId)
        [HttpPost]
        [AsyncTimeout(TimeoutMilliseconds)]
        [HandleError(ExceptionType = typeof(TaskCanceledException), View = "TimedOut")]
        public Task<bool> DeleteBlob(Guid ownerPartyId, Guid fileGuid)
        {
            return DeleteBlobHelper(ownerPartyId, fileGuid);
        }

        //Allows other server code to use the DeleteBlob logic.
        internal static Task<bool> DeleteBlobHelper(Guid ownerPartyId, Guid fileGuid)
        {
            return AsyncHelper.RunAsync(() =>
            {
                //Create service client for credentials access to the Blob service.
                var blobClient = new CloudBlobClient(AzureTools.BlobStorageUrl,
                    new StorageCredentialsAccountAndKey(AzureTools.AccountName, AzureConstants.AccountKey)) { Timeout = TimeSpan.FromMilliseconds(TimeoutMilliseconds) };

                //Get a reference to a container, which may or may not exist.
                var blobContainer = blobClient.GetContainerReference(AzureTools.BuildContainerUrl(ownerPartyId));

                //Get a reference to a blob, which may or may not exist.
                var blob = blobContainer.GetBlobReference(fileGuid.ToString());

                //Delete the blob if it exists
                return blob.DeleteIfExists();
            });
        }

        private static void CreateSilverlightPolicy(CloudBlobClient blobs)
        {
            blobs.GetContainerReference("$root").CreateIfNotExist();
            blobs.GetContainerReference("$root").SetPermissions(
                new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
                });
            var blob = blobs.GetBlobReference("clientaccesspolicy.xml");
            blob.Properties.ContentType = "text/xml";
            blob.UploadText(
            @"<?xml version=""1.0"" encoding=""utf-8""?>
                <access-policy>
                    <cross-domain-access>
                    <policy>
                        <allow-from http-methods=""*"" http-request-headers=""*"">
                        <domain uri=""*"" />
                        <domain uri=""http://*"" />
                        </allow-from>
                        <grant-to>
                        <resource path=""/"" include-subpaths=""true"" />
                        </grant-to>
                    </policy>
                    </cross-domain-access>
                </access-policy>");
        }
    }
}

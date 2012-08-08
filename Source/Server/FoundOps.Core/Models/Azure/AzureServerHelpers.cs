using FoundOps.Common.NET;
using FoundOps.Common.Tools;
using System;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace FoundOps.Core.Models.Azure
{
    /// <summary>
    /// Contains constant values and helpers for Azure.
    /// Only should be used from the server, never for a client.
    /// </summary>
    public static class AzureServerHelpers
    {
        /// <summary>
        /// The AccountKey for the storage account (depending on the solution configuration).
        /// </summary>
        public static string AccountKey = ConfigWrapper.ConnectionString("AzureAccountKey");

        /// <summary>
        /// The Azure storage connection string.
        /// </summary>
        public static string StorageConnectionString = "DefaultEndpointsProtocol=http;AccountName=" + SharedConstants.AzureAccountName + ";AccountKey=" + AccountKey;

        /// <summary>
        /// The default expiration of a shared access key. Cannot be more than 1 hour without a signed identifier.
        /// </summary>
        public static TimeSpan DefaultExpiration = TimeSpan.FromMinutes(59);

        /// <summary>
        /// The default timeout.
        /// </summary>
        public static TimeSpan DefaultTimeout = TimeSpan.FromSeconds(5);

        /// <summary>
        /// Returns the TrackPoint azure storage table name for a business account.
        /// </summary>
        /// <param name="businessAccountId">The business account id.</param>
        public static string TrackPointTableName(this Guid businessAccountId)
        {
            //Table Names must start with a letter. They also must be alphanumeric. http://msdn.microsoft.com/en-us/library/windowsazure/dd179338.aspx
            return "tp" + businessAccountId.ToString().Replace("-", "");
        }

        /// <summary>
        /// Gets a shared access key read only url for a file. Expires in 1 hours
        /// </summary>
        /// <param name="ownerPartyId">The owner party of the file (the container name)</param>
        /// <param name="fileGuid">The file id (name)</param>
        public static string GetBlobUrlHelper(Guid ownerPartyId, Guid fileGuid)
        {
            //Create service client for credentialed access to the Blob service
            var blobClient = new CloudBlobClient(SharedConstants.BlobStorageUrl, new StorageCredentialsAccountAndKey(SharedConstants.AzureAccountName, AccountKey)) { Timeout = DefaultTimeout };

            //Get a reference to a container, which may or may not exist
            var blobContainer = blobClient.GetContainerReference(AzureTools.BuildContainerUrl(ownerPartyId));
            if (blobContainer == null)
                return null;

            //Get a reference to a blob, which may or may not exist
            var blob = blobContainer.GetBlobReference(fileGuid.ToString());
            if (blob == null)
                return null;

            var url = blob.GetSharedAccessSignature(new SharedAccessPolicy { SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-1), Permissions = SharedAccessPermissions.Read, SharedAccessExpiryTime = DateTime.UtcNow + DefaultExpiration });

            return url;
        }

        /// <summary>
        /// Gets a blob file for getting/inserting/editing a file
        /// </summary>
        /// <param name="ownerPartyId">The owner party id of the file</param>
        /// <param name="fileGuid">The file id</param>
        /// <returns>The blob file</returns>
        public static CloudBlob GetBlobHelper(Guid ownerPartyId, Guid fileGuid)
        {
            //Create service client for credentialed access to the Blob service.
            var blobClient = new CloudBlobClient(SharedConstants.BlobStorageUrl,
                new StorageCredentialsAccountAndKey(SharedConstants.AzureAccountName, AccountKey)) { Timeout = DefaultTimeout };

            //Get a reference to a container, which may or may not exist
            var blobContainer = blobClient.GetContainerReference(AzureTools.BuildContainerUrl(ownerPartyId));
            //Create a new container, if it does not exist
            var newContainer = blobContainer.CreateIfNotExist(new BlobRequestOptions { Timeout = DefaultTimeout });

            //Setup cross domain policy so Silverlight can access the server
            if(newContainer)
                CreateSilverlightPolicy(blobClient);

            //Get a reference to a blob, which may or may not exist.
            var blob = blobContainer.GetBlobReference(fileGuid.ToString());

            return blob;
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

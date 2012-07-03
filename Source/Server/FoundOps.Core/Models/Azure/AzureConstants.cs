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
        public static string StorageConnectionString = "DefaultEndpointsProtocol=http;AccountName=" + AzureTools.AccountName + ";AccountKey=" + AccountKey;

        /// <summary>
        /// The default expiration of a shared access key.
        /// </summary>
        public static TimeSpan DefaultExpiration = TimeSpan.FromHours(3);

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
        /// Gets a read only url for a file. Expires in 3 hours
        /// </summary>
        /// <param name="ownerPartyId">The owner party of the file (the container name)</param>
        /// <param name="fileGuid">The file id (name)</param>
        public static string GetBlobUrlHelper(Guid ownerPartyId, Guid fileGuid)
        {
            //Create service client for credentialed access to the Blob service
            var blobClient = new CloudBlobClient(AzureTools.BlobStorageUrl, new StorageCredentialsAccountAndKey(AzureTools.AccountName, AccountKey)) { Timeout = DefaultTimeout };

            //Get a reference to a container, which may or may not exist
            var blobContainer = blobClient.GetContainerReference(AzureTools.BuildContainerUrl(ownerPartyId));
            if (blobContainer == null)
                return null;
            
            //Get a reference to a blob, which may or may not exist
            var blob = blobContainer.GetBlobReference(fileGuid.ToString());
            if (blob == null)
                return null;

            var url = blob.GetSharedAccessSignature(new SharedAccessPolicy { Permissions = SharedAccessPermissions.Read, SharedAccessExpiryTime = DateTime.UtcNow + DefaultExpiration });
            
            return url;
        }
    }
}

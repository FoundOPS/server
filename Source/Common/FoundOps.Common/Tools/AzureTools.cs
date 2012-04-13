using System;

namespace FoundOps.Common.Tools
{
    public static class AzureTools
    {
#if DEBUG
        public const string BlobStorageUrl = @"http://opsappdebug.blob.core.windows.net/";
#else
        public const string BlobStorageUrl = @"http://fstoreroledata.blob.core.windows.net/";
#endif

        /// <summary>
        /// Builds the container URL. 
        /// Example url: http://fstoreroledata.blob.core.windows.net/5B15B601-D082-4CAF-BF41-562A20CE1ABB
        /// </summary>
        /// <param name="ownerPartyId">The owner party id.</param>
        public static string BuildContainerUrl(Guid ownerPartyId)
        {
            return string.Format(@"{0}{1}", BlobStorageUrl, ownerPartyId);
        }

        /// <summary>
        /// Builds the file URL.
        /// Example url: http://fstoreroledata.blob.core.windows.net/5B15B601-D082-4CAF-BF41-562A20CE1ABB/filenameaccesskeyhere
        /// </summary>
        /// <param name="ownerPartyId">The owner party id.</param>
        /// <param name="fileId">The file id.</param>
        /// <param name="accessKey">The access key.</param>
        public static string BuildFileUrl(Guid ownerPartyId, Guid fileId, string accessKey)
        {
            return BuildContainerUrl(ownerPartyId) + "/" + fileId + accessKey;
        }
    }
}
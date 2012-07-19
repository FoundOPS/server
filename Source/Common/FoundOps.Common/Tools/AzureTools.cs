using System;

namespace FoundOps.Common.Tools
{
    /// <summary>
    /// A shared Azure tools class with the server and client.
    /// Do not store any sensitive information here.
    /// </summary>
    public static class AzureTools
    {
#if DEBUG
        public const string AccountName = "opsappdebug";
        private const string BlobStorageSubDomain = "bd";
#elif TESTRELEASE
        public const string AccountName = "opsapptest";
        private const string BlobStorageSubDomain = "bt";
#elif RELEASE
        public const string AccountName = "opsapplive";
        private const string BlobStorageSubDomain = "bp";
#endif

        public const string BlobStorageUrl = @"http://" + BlobStorageSubDomain + @".foundops.com/";

        /// <summary>
        /// Builds the container URL. 
        /// Example url: http://bd.foundops.com/5B15B601-D082-4CAF-BF41-562A20CE1ABB
        /// </summary>
        /// <param name="ownerPartyId">The owner party id.</param>
        public static string BuildContainerUrl(Guid ownerPartyId)
        {
            return string.Format(@"{0}{1}", BlobStorageUrl, ownerPartyId);
        }

        /// <summary>
        /// Builds the file URL.
        /// Example url: http://bd.foundops.com/5B15B601-D082-4CAF-BF41-562A20CE1ABB/filenameaccesskeyhere
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
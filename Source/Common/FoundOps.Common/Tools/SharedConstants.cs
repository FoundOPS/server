namespace FoundOps.Common.Tools
{
    public class SharedConstants
    {
#if DEBUG
        private const string BlobStorageSubDomain = "bd";
        public static string AzureAccountName = "opsappdebug";
#elif TESTRELEASE
        private const string BlobStorageSubDomain = "bt";
        public static string AzureAccountName = "opsapptest";
#elif RELEASE
        private const string BlobStorageSubDomain = "bp";
        public static string AzureAccountName = "opsapplive";
#endif

        public static string BlobStorageUrl = "http://" + BlobStorageSubDomain + ".foundops.com/";
    }
}

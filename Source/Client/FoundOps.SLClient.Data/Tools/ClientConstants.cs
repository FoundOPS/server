namespace FoundOps.SLClient.Data.Tools
{
    public class ClientConstants
    {
#if DEBUG || DEBUGNODE
        public static string RootDomainServiceUrl = "http://localhost:31820/ClientBin";
#elif TESTRELEASE
        public static string RootDomainServiceUrl = "http://test.foundops.com";
#elif RELEASE
        public static string RootDomainServiceUrl = "http://app.foundops.com";
#endif
    }
}
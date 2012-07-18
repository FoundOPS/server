namespace FoundOps.SLClient.Data.Tools
{
    public class ClientConstants
    {
#if DEBUG || DEBUGNODE
        public static string RootDomainServiceUrl = "http://localhost:31820/ClientBin";
            //In testrelease and release mode setup the endpoint to be HTTPS
#elif TESTRELEASE
        public static string RootDomainServiceUrl = "https://test.foundops.com";
#elif RELEASE
        public static string RootDomainServiceUrl = "https://app.foundops.com";
#endif
    }
}
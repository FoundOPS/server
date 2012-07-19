namespace FoundOps.Core.Models
{
    /// <summary>
    /// Containts development constants for the FoundOPS.Core server project.
    /// </summary>
    public class ServerConstants
    {
        public static string RootFrontSiteUrl = "http://foundops.com";

#if DEBUG
        public static string RootApplicationUrl = "http://localhost:31820";
        public static string RootApiUrl = "http://localhost:9711";
#elif TESTRELEASE
        public static string RootApplicationUrl = "https://test.foundops.com";
        public static string RootApiUrl = "http://testapi.foundops.com";
#elif RELEASE
        public static string RootApplicationUrl = "https://app.foundops.com";
        public static string RootApiUrl = "http://api.foundops.com";
#endif

#if DEBUG
        /// <summary>
        /// If set to true it will automatically login a FoundOPS Admin.
        /// </summary>
        public static bool AutomaticLoginFoundOPSAdmin = true;
#else
        public static bool AutomaticLoginOPSManager = false;
        public static bool AutomaticLoginFoundOPSAdmin = false;
#endif

        /// <summary>
        /// The root directory of the server projects.
        /// </summary>
        public static string RootDirectory = @"C:\FoundOps\GitHub\Source\Server";
    }
}

﻿namespace FoundOps.Core.Models
{
    /// <summary>
    /// Shared constants for the Server and Client
    /// </summary>
    public static class AppConstants
    {
        public static string RootFrontSiteUrl = "http://foundops.com";
        public static string MobileFrontSiteUrl = "http://m.foundops.com";

        public static string RootApplicationUrl;
        public static string RootApiUrl;

        public static string ApplicationUrl;

#if DEBUG
        public static string LocalApplicationServer = "http://localhost:3501";
#endif

        static AppConstants()
        {
#if DEBUG
            RootApplicationUrl = "http://localhost:31820";
            RootApiUrl = "http://localhost:9711";
#elif TESTRELEASE
            RootApplicationUrl = "http://test.foundops.com";
            RootApiUrl = "http://testapi.foundops.com";
#elif RELEASE
            RootApplicationUrl = "http://bp.foundops.com";
            RootApiUrl = "http://api.foundops.com";
#endif

            ApplicationUrl = RootApplicationUrl + "/app/index.html";
        }
    }
}

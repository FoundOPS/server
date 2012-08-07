﻿using FoundOps.Common.NET;
using FoundOps.Common.Tools;

namespace FoundOps.Core.Models
{
    /// <summary>
    /// Containts development constants for the FoundOPS.Core server project.
    /// </summary>
    public static class ServerConstants
    {
        public static string RootFrontSiteUrl = "http://foundops.com";

#if DEBUG
        public static string RootApplicationUrl = "http://localhost:31820";
        public static string RootApiUrl = "http://localhost:9711";

#elif TESTRELEASE
        public static string RootApplicationUrl = "http://test.foundops.com";
        public static string RootApiUrl = "http://testapi.foundops.com";

#elif RELEASE
        public static string RootApplicationUrl = "http://app.foundops.com";
        public static string RootApiUrl = "http://api.foundops.com";
#endif
        public static string ApplicationUrl = RootApplicationUrl + "/App/Index";

#if DEBUG
        public enum AutoLogin
        {
            None,
            //Auto login an admin
            Admin,
            //Auto login a mobile user
            Mobile
        }

        /// <summary>
        /// If set to true it will automatically login a FoundOPS Admin.
        /// </summary>
        public static AutoLogin LoginMode = AutoLogin.Mobile;
#endif

        /// <summary>
        /// The root directory of the server projects.
        /// </summary>
        public static string RootDirectory = @"C:\FoundOps\GitHub\Source\Server";

        public static readonly string SqlConnectionString = ConfigWrapper.ConnectionString("CoreConnectionString");
        public static readonly string ContainerConnectionString = ConfigWrapper.ConnectionString("CoreEntitiesContainer");
    }
}

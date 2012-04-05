using System;
using System.Configuration;

namespace FoundOps.Common.Server
{
    public static class UserSpecificResourcesWrapper
    {
        public static bool GetBool(string resourceKey)
        {
            return Convert.ToBoolean(ConfigurationManager.AppSettings[resourceKey]);
        }

        public static string GetString(string resourceKey)
        {
            return ConfigurationManager.AppSettings[resourceKey];
        }

        public static string ConnectionString(string key)
        {
            return ConfigurationManager.ConnectionStrings[key].ConnectionString;
        }
    }
}

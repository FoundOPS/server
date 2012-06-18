using System;
using System.Configuration;

namespace FoundOps.Common.NET
{
    /// <summary>
    /// Tools for extracting information from the App.config or Web.config
    /// </summary>
    public static class ConfigWrapper
    {
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

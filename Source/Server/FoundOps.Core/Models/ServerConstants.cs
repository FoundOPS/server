using FoundOps.Common.NET;

namespace FoundOps.Core.Models
{
    /// <summary>
    /// Constants for use only by the server
    /// </summary>
    public static class ServerConstants
    {
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
        public static AutoLogin LoginMode = AutoLogin.Admin;
#endif

        /// <summary>
        /// The root directory of the server projects.
        /// </summary>
        public static string RootDirectory = @"C:\FoundOps\GitHub\Source\Server";

        public static readonly string SqlConnectionString = ConfigWrapper.ConnectionString("CoreConnectionString");
        public static readonly string ContainerConnectionString = ConfigWrapper.ConnectionString("CoreEntitiesContainer");
    }
}

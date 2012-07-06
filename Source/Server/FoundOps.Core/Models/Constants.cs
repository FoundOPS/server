namespace FoundOps.Core.Models
{
    /// <summary>
    /// Containts development constants for the FoundOPS.Core server project.
    /// </summary>
    public class ServerConstants
    {
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

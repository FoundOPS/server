namespace FoundOps.Core.Models
{
    /// <summary>
    /// Containts development constants for the FoundOPS.Core server project.
    /// </summary>
    public class ServerConstants
    {
        /// <summary>
        /// If set to true it will automatically login an operations manager.
        /// </summary>
        public static bool AutomaticLoginOPSManager = true;

        /// <summary>
        /// If set to true it will automatically login a FoundOPS Admin.
        /// </summary>
        public static bool AutomaticLoginFoundOPSAdmin = false;

        /// <summary>
        /// The root directory of the server projects.
        /// </summary>
        public static string RootDirectory = @"C:\FoundOps\GitHub\Source\Server";
    }
}

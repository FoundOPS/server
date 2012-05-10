using FoundOps.Common.Tools;
using FoundOps.Core.Models.CoreEntities;

namespace FoundOps.Core.Models.Azure
{
    /// <summary>
    /// Contains constant values and helpers for Azure.
    /// </summary>
    public class AzureHelpers
    {
#if DEBUG
        /// <summary>
        /// The AccountKey for the opsappdebug storage account.
        /// </summary>
        public static string AccountKey = "Wbs5xOmAKdNw8ef9XgRZF2lhE+DYH1uN0qgETVKSCLqIXaaTRjiFIj4sT2cf0iQxUdOAYEej2VI4aPBr7TVOYA==";
#elif TESTRELEASE
          /// <summary>
        /// The AccountKey for the opsapptest storage account.
        /// </summary>
        public static string AccountKey = "dlAT4qEtm7gsWrOKuIIRgyJvYkwimkBwRo9mWvyhDGF6aISFu3jBYnCn/qgsHwsn0+3chbZ/mEWgDyAYI2gyPA==";
#elif RELEASE
        /// <summary>
        /// The AccountKey for the opsapplive storage account.
        /// </summary>
        public static string AccountKey = "j9CJzOyyBkPmVccwc269o52l283iyGFgX8UMlpyfIMTdF0+Ka14kAAlHJiRC/WhdcrVdO0Kd+MRG/iTdddJATw==";
#endif

        /// <summary>
        /// The Azure storage connection string.
        /// </summary>
        public static string StorageConnectionString = "DefaultEndpointsProtocol=http;AccountName=" + AzureTools.AccountName + ";AccountKey=" + AzureHelpers.AccountKey;

        /// <summary>
        /// Returns the TrackPoint azure storage table name for a business account.
        /// </summary>
        /// <param name="businessAccount">The business account.</param>
        public static string TrackPointTableName(BusinessAccount businessAccount)
        {
            //Table Names must start with a letter. They also must be alphanumeric. http://msdn.microsoft.com/en-us/library/windowsazure/dd179338.aspx
            return "tp" + businessAccount.Id.ToString().Replace("-", "");
        }
    }
}

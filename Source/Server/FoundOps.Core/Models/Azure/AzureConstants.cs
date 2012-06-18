using FoundOps.Common.NET;
using FoundOps.Common.Tools;
using System;

namespace FoundOps.Core.Models.Azure
{
    /// <summary>
    /// Contains constant values and helpers for Azure.
    /// </summary>
    public static class AzureHelpers
    {
        /// <summary>
        /// The AccountKey for the storage account (depending on the solution configuration).
        /// </summary>
        public static string AccountKey = ConfigWrapper.ConnectionString("AzureAccountKey");

        /// <summary>
        /// The Azure storage connection string.
        /// </summary>
        public static string StorageConnectionString = "DefaultEndpointsProtocol=http;AccountName=" + AzureTools.AccountName + ";AccountKey=" + AzureHelpers.AccountKey;

        /// <summary>
        /// Returns the TrackPoint azure storage table name for a business account.
        /// </summary>
        /// <param name="businessAccountId">The business account id.</param>
        public static string TrackPointTableName(this Guid businessAccountId)
        {
            //Table Names must start with a letter. They also must be alphanumeric. http://msdn.microsoft.com/en-us/library/windowsazure/dd179338.aspx
            return "tp" + businessAccountId.ToString().Replace("-", "");
        }
    }
}

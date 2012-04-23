using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace FoundOps.Core.Models.Azure
{
    public class TrackPointsHistoryContext : TableServiceContext
    {
        public TrackPointsHistoryContext(string baseAddress, StorageCredentials credentials)
            : base(baseAddress, credentials)
        {
        }
    }
}

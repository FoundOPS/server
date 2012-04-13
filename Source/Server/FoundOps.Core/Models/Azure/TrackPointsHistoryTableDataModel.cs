using System;
using Microsoft.WindowsAzure.StorageClient;

namespace FoundOps.Core.Models.Azure
{
    public class TrackPointsHistoryTableDataModel : TableServiceEntity
    {
        public TrackPointsHistoryTableDataModel(string partitionKey, string rowKey)
            : base(partitionKey, rowKey)
        {
        }

        public TrackPointsHistoryTableDataModel()
            : this(Guid.NewGuid().ToString(), String.Empty)
        {
        }

        public Guid TrackPointId { get; set; }
        public Guid EmployeetId { get; set; }
        public Guid VehicleId { get; set; }
        public DateTime TimeStamp { get; set; }
        public Double Latitude { get; set; }
        public Double Longitude { get; set; }

    }
}

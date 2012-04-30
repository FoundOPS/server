using System;
using System.Runtime.Serialization;
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

        public Guid? EmployeetId { get; set; }
        public Guid? VehicleId { get; set; }
        public Guid RouteId { get; set; }

        [Obsolete("Do not use setter method")]
        public DateTime TimeStampDate { get; set; }

        private DateTime _lastTimeStamp;
        public DateTime LastTimeStamp
        {
            get { return _lastTimeStamp; }
            set
            {
                _lastTimeStamp = value;
                TimeStampDate = LastTimeStamp.Date;
            }
        }

        public Double Latitude { get; set; }
        public Double Longitude { get; set; }

    }
}

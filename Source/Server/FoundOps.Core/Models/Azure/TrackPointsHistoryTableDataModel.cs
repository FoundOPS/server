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

        public Guid? EmployeetId { get; set; }
        public Guid? VehicleId { get; set; }
        public Guid RouteId { get; set; }
        private DateTime _timeStampDate;
        public DateTime TimeStampDate
        {
            get { return _timeStampDate; }
            set
            {
                _timeStampDate = value;
                TimeStampDate = _timeStampDate.Date;

            }
        }

        public DateTime LastTimeStamp { get; set; }
        public Double Latitude { get; set; }
        public Double Longitude { get; set; }
        
    }
}

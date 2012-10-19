using FoundOps.Common.Tools;
using Microsoft.WindowsAzure.StorageClient;
using System;

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

        /// <summary>
        /// The tracked employee's id. Null if VehicleId is set.
        /// </summary>
        public Guid? EmployeeId { get; set; }

        /// <summary>
        /// The tracked vehicle's id. Null if EmployeeId is set.
        /// </summary>
        public Guid? VehicleId { get; set; }

        /// <summary>
        /// The route the resource was on when this TrackPoint was collected.
        /// </summary>
        public Guid RouteId { get; set; }

        [Obsolete("Do not use setter method. It is automatically set.")]
        public DateTime CollectedDate { get; set; }

        private DateTime _collectedTimeStamp;
        /// <summary>
        /// The time this was collected. It sets TimeStampDate
        /// </summary>
        public DateTime CollectedTimeStamp
        {
            get { return _collectedTimeStamp; }
            set
            {
                _collectedTimeStamp = value;
                CollectedDate = CollectedTimeStamp.Date;
            }
        }

        /// <summary>
        /// The latitude where this was collected.
        /// </summary>
        public double? Latitude { get; set; }

        /// <summary>
        /// The longitude where this was collected.
        /// </summary>
        public double? Longitude { get; set; }

        /// <summary>
        /// The Accuracy of the GPS coordinates from PhoneGap
        /// In Meters
        /// </summary>
        public int Accuracy { get; set; }
    }
}
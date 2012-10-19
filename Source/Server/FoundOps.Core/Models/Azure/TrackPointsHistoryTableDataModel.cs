using FoundOps.Common.Tools;
using Microsoft.WindowsAzure.StorageClient;
using System;

namespace FoundOps.Core.Models.Azure
{
    public class TrackPointsHistoryTableDataModel : TableServiceEntity, IGeoLocation
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

        #region Implementation of IGeoLocation

        double IGeoLocation.Latitude { get { return Latitude.Value; } }
        double IGeoLocation.Longitude { get { return Longitude.Value; } }
        public double LatitudeRad { get { return GeoLocation.DegreeToRadian(Latitude.Value); } }
        public double LongitudeRad { get { return GeoLocation.DegreeToRadian(Longitude.Value); } }
        
        #endregion

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
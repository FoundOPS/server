using FoundOps.Common.Tools;
using FoundOps.Core.Models.Azure;
using System;

namespace FoundOps.Api.Models
{
    public class TrackPoint : IGeoLocation
    {
        /// <summary>
        /// The Id of this TrackPoint
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The Date of the TrackPoint. On Get, it pulls LastTimeStamp.Date
        /// </summary>
        public DateTime CollectedDate
        {
            get { return CollectedTimeStamp.Date; }
        }

        /// <summary>
        /// The DateTime of this TrackPoint
        /// </summary>
        public DateTime CollectedTimeStamp { get; set; }

        /// <summary>
        /// The compass heading of this TrackPoint
        /// </summary>
        public Int32? Heading { get; set; }

        /// <summary>
        /// The latitude of this TrackPoint
        /// </summary>
        public decimal? Latitude { get; set; }

        /// <summary>
        /// The longitude of this TrackPoint
        /// </summary>
        public decimal? Longitude { get; set; }

        /// <summary>
        /// The speed of this TrackPoint
        /// </summary>
        public decimal? Speed { get; set; }

        /// <summary>
        /// The source of this TrackPoint (iPhone, Android, WindowsPhone, etc.)
        /// </summary>
        public String Source { get; set; }

        /// <summary>
        /// The Id of the Route that this TrackPoint belongs to
        /// </summary>
        public Guid? RouteId { get; set; }

        /// <summary>
        /// The accuracy of the GPS coordinates from PhoneGap
        /// In Meters
        /// </summary>
        public int Accuracy { get; set; }

        #region Implementation of IGeoLocation

        double IGeoLocation.Latitude { get { return (double) Latitude.Value; } }
        double IGeoLocation.Longitude { get { return (double) Longitude.Value; } }
        public double LatitudeRad { get { return GeoLocation.DegreeToRadian((double) Latitude.Value); } }
        public double LongitudeRad { get { return GeoLocation.DegreeToRadian((double) Longitude.Value); } }

        #endregion

        public static TrackPoint ConvertToModel(TrackPointsHistoryTableDataModel modelTrackPoint)
        {
            var trackPointId = modelTrackPoint.EmployeeId ?? modelTrackPoint.VehicleId;

            if (trackPointId == null)
                return null;

            var trackPoint = new TrackPoint
            {
                Heading = null,
                Id = (Guid)trackPointId,
                CollectedTimeStamp = modelTrackPoint.CollectedTimeStamp,
                Latitude = (decimal?)modelTrackPoint.Latitude,
                Longitude = (decimal?)modelTrackPoint.Longitude,
                Speed = null,
                Source = null,
                RouteId = modelTrackPoint.RouteId,
                Accuracy = modelTrackPoint.Accuracy
            };

            return trackPoint;
        }
    }
}
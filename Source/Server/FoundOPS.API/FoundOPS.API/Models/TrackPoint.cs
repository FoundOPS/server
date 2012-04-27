using System;
using FoundOps.Core.Models.Azure;

namespace FoundOPS.API.Models
{
    public class TrackPoint
    {
        /// <summary>
        /// The Id of this TrackPoint
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The Date of the TrackPoint. On Get, it pulls LastTimeStamp.Date
        /// </summary>
        public DateTime TimeStampDate
        {
            get { return LastTimeStamp.Date; }
        }

        /// <summary>
        /// The DateTime of this TrackPoint
        /// </summary>
        public DateTime LastTimeStamp { get; set; }

        /// <summary>
        /// The compass heading of this TrackPoint
        /// </summary>
        public Int32? CompassDirection { get; set; }

        /// <summary>
        /// The latitude of this TrackPoint
        /// </summary>
        public Double Latitude { get; set; }

        /// <summary>
        /// The longitude of this TrackPoint
        /// </summary>
        public Double Longitude { get; set; }

        /// <summary>
        /// The speed of this TrackPoint
        /// </summary>
        public Double? Speed { get; set; }

        /// <summary>
        /// The source of this TrackPoint (iPhone, Android, WindowsPhone, etc.)
        /// </summary>
        public String Source { get; set; }

        public static TrackPoint ConvertToModel(TrackPointsHistoryTableDataModel modelTrackPoint)
        {
            var trackPointId = modelTrackPoint.EmployeetId ?? modelTrackPoint.VehicleId;

            if(trackPointId == null) 
                return null;

            var trackPoint = new TrackPoint
                                 {
                                     CompassDirection = null,
                                     Id = (Guid) trackPointId,
                                     LastTimeStamp = modelTrackPoint.LastTimeStamp,
                                     Latitude = modelTrackPoint.Latitude,
                                     Longitude = modelTrackPoint.Longitude,
                                     Speed = null,
                                     Source = null
                                 };

            return trackPoint;
        }
    }
}
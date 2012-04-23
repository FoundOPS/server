using System;
using FoundOps.Core.Models.Azure;

namespace FoundOPS.API.Models
{
    public class TrackPoint
    {
        public Guid Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public Int32? CompassDirection { get; set; }
        public Double Latitude { get; set; }
        public Double Longitude { get; set; }
        public Double? Speed { get; set; }
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
                                     TimeStamp = modelTrackPoint.TimeStamp,
                                     Latitude = modelTrackPoint.Latitude,
                                     Longitude = modelTrackPoint.Longitude,
                                     Speed = null,
                                     Source = null
                                 };

            return trackPoint;
        }
    }
}
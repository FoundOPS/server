using System;

namespace FoundOPS.API.Models
{
    public class TrackPoint
    {
        public Guid Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public Int32 CompassDirection { get; set; }
        public Double Latitude { get; set; }
        public Double Longitude { get; set; }
        public Double Speed { get; set; }
        public String Source { get; set; }

        public static FoundOps.Core.Models.CoreEntities.TrackPoint ConvertFromModel(TrackPoint modelTrackPoint)
        {
            var trackPoint = new FoundOps.Core.Models.CoreEntities.TrackPoint
                                 {
                                     CompassDirection = modelTrackPoint.CompassDirection,
                                     Id = modelTrackPoint.Id,
                                     TimeStamp = modelTrackPoint.TimeStamp,
                                     Latitude = modelTrackPoint.Latitude,
                                     Longitude = modelTrackPoint.Longitude,
                                     Speed = modelTrackPoint.Speed,
                                     Source = modelTrackPoint.Source
                                 };

            return trackPoint;
        }
    }
}
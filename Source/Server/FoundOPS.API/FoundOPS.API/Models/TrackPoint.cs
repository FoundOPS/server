﻿using FoundOps.Core.Models.Azure;
using System;

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

        /// <summary>
        /// The Id of the Route that this TrackPoint belongs to
        /// </summary>
        public Guid? RouteId { get; set; }

        public static TrackPoint ConvertToModel(TrackPointsHistoryTableDataModel modelTrackPoint) 
        {
            var trackPointId = modelTrackPoint.EmployeeId ?? modelTrackPoint.VehicleId;

            if(trackPointId == null) 
                return null;

            var trackPoint = new TrackPoint
                                 {
                                     CompassDirection = null,
                                     Id = (Guid) trackPointId,
                                     CollectedTimeStamp = modelTrackPoint.CollectedTimeStamp,
                                     Latitude = modelTrackPoint.Latitude,
                                     Longitude = modelTrackPoint.Longitude,
                                     Speed = null,
                                     Source = null,
                                     RouteId = modelTrackPoint.RouteId
                                 };

            return trackPoint;
        }
    }
}
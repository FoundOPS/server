using FoundOps.Core.Models.CoreEntities;
using System;

namespace FoundOPS.API.Models
{
    public class ModelResourceWithLastPoint
    {
        public int? CompassHeading { get; set; }
        public Guid? EmployeeId { get; set; }
        public string EntityName { get; set; }
        public DateTime LastTimeStamp { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public Guid? RouteId { get; set; }
        public decimal? Speed { get; set; }
        public string TrackSource { get; set; }
        public Guid? VehicleId { get; set; }

        public static ModelResourceWithLastPoint ConvertToModel(ResourceWithLastPoint modelResource)
        {
            var resource = new ModelResourceWithLastPoint
            {
                CompassHeading = modelResource.CompassHeading,
                EmployeeId = modelResource.EmployeeId,
                EntityName = modelResource.EntityName,
                LastTimeStamp = modelResource.LastTimeStamp,
                Latitude = modelResource.Latitude,
                Longitude = modelResource.Longitude,
                RouteId = modelResource.RouteId,
                Speed = modelResource.Speed,
                TrackSource = modelResource.TrackSource,
                VehicleId = modelResource.VehicleId
            };

            return resource;
        }
    }
}
using FoundOps.Core.Models.CoreEntities;
using System;

namespace FoundOPS.API.Models
{
    public class ModelResourceWithLastPoint
    {
        public int? CompassHeading { set; get; }
        public Guid? EmployeeId { set; get; }
        public string EntityName { set; get; }
        public DateTime LastTimeStamp { set; get; }
        public decimal? Latitude { set; get; }
        public decimal? Longitude { set; get; }
        public Guid? RouteId { set; get; }
        public decimal? Speed { set; get; }
        public string TrackSource { set; get; }
        public Guid? VehicleId { set; get; }

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
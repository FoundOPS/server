using System;

namespace FoundOPS.API.Models
{
    public class ResourceWithLastPoint : TrackPoint
    {
        /// <summary>
        /// The Id of the Employee being tracked
        /// Null if the object being tracked is a Vehicle
        /// </summary>
        public Guid? EmployeeId { get; set; }
        
        /// <summary>
        /// The name of the object being tracked (ex: Bob Block or License Plate of Vehicle)
        /// </summary>
        public string EntityName { get; set; }
        
        /// <summary>
        /// The Id of the Vehicle being tracked
        /// Null if the object being tracked is a Employee
        /// </summary>
        public Guid? VehicleId { get; set; }

        public static ResourceWithLastPoint ConvertToModel(FoundOps.Core.Models.CoreEntities.ResourceWithLastPoint modelResource)
        {
            var resource = new ResourceWithLastPoint
            {
                Heading = modelResource.Heading,
                EmployeeId = modelResource.EmployeeId,
                EntityName = modelResource.EntityName,
                CollectedTimeStamp = modelResource.CollectedTimeStamp,
                Latitude = modelResource.Latitude,
                Longitude = modelResource.Longitude,
                RouteId = modelResource.RouteId,
                Speed = modelResource.Speed,
                Source = modelResource.TrackSource,
                VehicleId = modelResource.VehicleId,
                Accuracy = modelResource.Accuracy
            };

            return resource;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;

namespace FoundOps.Api.Models
{
    public class Route : ITrackable
    {
        public Guid Id { get; set; }

        public DateTime CreatedDate { get; private set; }
        public DateTime? LastModified { get; private set; }
        public Guid? LastModifyingUserId { get; private set; }

        public Guid BusinessAccountId { get; set; }
        public string Name { get; set; }

        public List<RouteDestination> RouteDestinations { get; set; }

        public Route()
        {
            CreatedDate = DateTime.UtcNow;
            RouteDestinations = new List<RouteDestination>();
        }

        /// <summary>
        /// Converts the model.
        /// </summary>
        /// <param name="routeModel">The route model.</param>
        /// <returns>The corresponding API model for Route.</returns>
        public static Route ConvertModel(FoundOps.Core.Models.CoreEntities.Route routeModel)
        {
            var route = new Route
                {
                    Id = routeModel.Id,
                    CreatedDate = routeModel.CreatedDate,
                    Name = routeModel.Name,
                    BusinessAccountId = routeModel.OwnerBusinessAccountId
                };

            route.SetLastModified(routeModel.LastModified, routeModel.LastModifyingUserId);

            foreach (var routeDestinationModel in routeModel.RouteDestinations.OrderBy(rd => rd.OrderInRoute))
                route.RouteDestinations.Add(RouteDestination.ConvertModel(routeDestinationModel));

            return route;
        }

        public void SetLastModified(DateTime? lastModified, Guid? userId)
        {
            LastModified = lastModified;
            LastModifyingUserId = userId;
        }
    }
}
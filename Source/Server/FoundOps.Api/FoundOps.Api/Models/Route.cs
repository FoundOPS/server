using System;
using System.Collections.Generic;
using System.Linq;

namespace FoundOps.Api.Models
{
    public class Route
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid BusinessAccountId { get; set; }

        public List<RouteDestination> RouteDestinations { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public Guid? LastModifyingUserId { get; set; }

        public Route()
        {
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
                    Name = routeModel.Name, 
                    BusinessAccountId = routeModel.OwnerBusinessAccountId,
                    CreatedDate = routeModel.CreatedDate,
                    LastModifiedDate = routeModel.LastModifiedDate,
                    LastModifyingUserId = routeModel.LastModifyingUserId
                };

            foreach (var routeDestinationModel in routeModel.RouteDestinations.OrderBy(rd => rd.OrderInRoute))
                route.RouteDestinations.Add(RouteDestination.ConvertModel(routeDestinationModel));

            return route;
        }
    }
}
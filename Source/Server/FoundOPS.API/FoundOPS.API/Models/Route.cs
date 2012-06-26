using System;
using System.Collections.Generic;
using System.Linq;

namespace FoundOPS.API.Models
{
    public class Route
    {
        /// <summary>
        /// The Id of the Route
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The name of the Route
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The list of RouteDestinations associated with this Route
        /// </summary>
        public List<RouteDestination> RouteDestinations { get; set; }

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
            var route = new Route { Id = routeModel.Id, Name = routeModel.Name };

            foreach (var routeDestinationModel in routeModel.RouteDestinations.OrderBy(rd => rd.OrderInRoute))
                route.RouteDestinations.Add(RouteDestination.ConvertModel(routeDestinationModel));

            return route;
        }
    }
}
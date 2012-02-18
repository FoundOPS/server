using System;
using System.Collections.Generic;

namespace FoundOPS.API.Models
{
    public class Route
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<RouteDestination> RouteDestinations { get; set; } 

        public Route()
        {
            RouteDestinations= new List<RouteDestination>();
        }

        /// <summary>
        /// Converts the model.
        /// </summary>
        /// <param name="routeModel">The route model.</param>
        /// <returns>The corresponding API model for Route.</returns>
        public static Route ConvertModel(FoundOps.Core.Models.CoreEntities.Route routeModel)
        {
            var route = new Route { Id=routeModel.Id, Name = routeModel.Name };
         
            foreach(var routeDestinationModel in routeModel.RouteDestinations)
                route.RouteDestinations.Add(RouteDestination.ConvertModel(routeDestinationModel));

            return route;
        }
    }
}
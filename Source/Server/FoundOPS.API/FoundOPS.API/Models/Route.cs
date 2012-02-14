using System.Collections.Generic;

namespace FoundOPS.API.Models
{
    public class Route
    {
        public string Name { get; set; }
        public List<RouteDestination> RouteDestinations { get; set; } 

        public Route()
        {
            RouteDestinations= new List<RouteDestination>();
        }

        public static Route ConvertModel(FoundOps.Core.Models.CoreEntities.Route routeModel)
        {
            var route = new Route {Name = routeModel.Name};
         
            foreach(var routeDestinationModel in routeModel.RouteDestinations)
                route.RouteDestinations.Add(RouteDestination.ConvertModel(routeDestinationModel));

            return route;
        }
    }
}
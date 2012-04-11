﻿namespace FoundOPS.API.Models
{
    public class RouteDestination
    {
        public int OrderInRoute { get; set; }

        public Client Client { get; set; }
        public Location Location { get; set; }

        public static RouteDestination ConvertModel(FoundOps.Core.Models.CoreEntities.RouteDestination routeDestinationModel)
        {
            var routeDestination = new RouteDestination {OrderInRoute = routeDestinationModel.OrderInRoute};

            if (routeDestinationModel.Client != null)
                routeDestination.Client = Client.ConvertModel(routeDestinationModel.Client);

            if (routeDestinationModel.Location != null)
                routeDestination.Location = Location.ConvertModel(routeDestinationModel.Location);

            return routeDestination;
        }
    }
}
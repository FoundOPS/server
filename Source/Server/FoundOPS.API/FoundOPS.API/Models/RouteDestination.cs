using System;

namespace FoundOPS.API.Models
{
    public class RouteDestination
    {
        public Guid Id { get; set; }

        /// <summary>
        /// The place occupied by this Destinationin the Route
        /// </summary>
        public int OrderInRoute { get; set; }

        /// <summary>
        /// The Client associated with the Destination
        /// </summary>
        public Client Client { get; set; }

        /// <summary>
        /// The location of the RouteDestination
        /// </summary>
        public Location Location { get; set; }

        public static RouteDestination ConvertModel(FoundOps.Core.Models.CoreEntities.RouteDestination routeDestinationModel)
        {
            var routeDestination = new RouteDestination {Id = routeDestinationModel.Id, OrderInRoute = routeDestinationModel.OrderInRoute};

            if (routeDestinationModel.Client != null)
                routeDestination.Client = Client.ConvertModel(routeDestinationModel.Client);

            if (routeDestinationModel.Location != null)
                routeDestination.Location = Location.ConvertModel(routeDestinationModel.Location);

            return routeDestination;
        }
    }
}

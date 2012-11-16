using System;
using System.Collections.Generic;
using System.Linq;

namespace FoundOps.Api.Models
{
    public class RouteDestination
    {
        public Guid Id { get; set; }

        /// <summary>
        /// The place occupied by this Destination the Route
        /// </summary>
        public int OrderInRoute { get; set; }

        public Client Client { get; set; }

        public Location Location { get; set; }

        public List<RouteTask> RouteTasks { get; set; }

        public RouteDestination()
        {
            RouteTasks = new List<RouteTask>();
        }

        public DateTime CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public Guid? LastModifyingUserId { get; set; }

        public static RouteDestination ConvertModel(FoundOps.Core.Models.CoreEntities.RouteDestination routeDestinationModel)
        {
            var routeDestination = new RouteDestination
                {
                    Id = routeDestinationModel.Id, 
                    OrderInRoute = routeDestinationModel.OrderInRoute,
                    CreatedDate = routeDestinationModel.CreatedDate,
                    LastModifiedDate = routeDestinationModel.LastModifiedDate,
                    LastModifyingUserId = routeDestinationModel.LastModifyingUserId
                };

            foreach (var routeTaskModel in routeDestinationModel.RouteTasks.OrderBy(rt => rt.Name))
                routeDestination.RouteTasks.Add(RouteTask.ConvertModel(routeTaskModel));

            if (routeDestinationModel.Client != null)
                routeDestination.Client = Client.ConvertModel(routeDestinationModel.Client);

            if (routeDestinationModel.Location != null)
                routeDestination.Location = Location.ConvertModel(routeDestinationModel.Location);

            return routeDestination;
        }
    }
}

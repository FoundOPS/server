using System;
using System.Collections.Generic;
using System.Linq;

namespace FoundOps.Api.Models
{
    public class RouteDestination : ITrackable
    {
        public Guid Id { get; set; }

        /// <summary>
        /// The place occupied by this Destination the Route
        /// </summary>
        public int OrderInRoute { get; set; }

        public Client Client { get; set; }

        public Location Location { get; set; }

        public List<RouteTask> RouteTasks { get; set; }

        public RouteDestination(DateTime createdDate)
        {
            RouteTasks = new List<RouteTask>();
            CreatedDate = createdDate;
        }

        public DateTime CreatedDate { get; private set; }
        public DateTime? LastModified { get; private set; }
        public Guid? LastModifyingUserId { get; private set; }
        
        public void SetLastModified(DateTime? lastModified, Guid? userId)
        {
            LastModified = lastModified;
            LastModifyingUserId = userId;
        }

        public static RouteDestination ConvertModel(FoundOps.Core.Models.CoreEntities.RouteDestination routeDestinationModel)
        {
            var routeDestination = new RouteDestination(routeDestinationModel.CreatedDate)
                {
                    Id = routeDestinationModel.Id, 
                    OrderInRoute = routeDestinationModel.OrderInRoute
                };

            routeDestination.SetLastModified(routeDestinationModel.LastModified, routeDestinationModel.LastModifyingUserId);

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

using System;
using System.Linq;
using System.Collections.Generic;
using FoundOps.Common.Composite.Tools;
using FoundOps.Core.Models.CoreEntities;

namespace FoundOps.SLClient.Data.Tools
{
    /// <summary>
    /// A calculator for organizing routes.
    /// </summary>
    public class SimpleRouteCalculator
    {
        /// <summary>
        /// Populates the routes.
        /// </summary>
        /// <param name="unroutedRouteTasks">The unrouted route tasks.</param>
        /// <param name="routesToPopulate">The routes to populate.</param>
        /// <returns>The tasks put into routes.</returns>
        public static IEnumerable<RouteTask> PopulateRoutes(IEnumerable<RouteTask> unroutedRouteTasks, IEnumerable<Route> routesToPopulate)
        {
            var routedTasks = new List<RouteTask>();

            //Collection of Unique ServiceTemplate Names (types) from unroutedRouteTasks
            var distinctRouteTaskServiceTemplates =
                unroutedRouteTasks.Where(rt => rt.Service != null).Select(rt => rt.Service.ServiceTemplate.Name).Distinct().ToArray();

            //2D collection of RouteTasks organized by ServiceTemplate Name (only those with a Location and a Service/ServiceTemplate)
            var routeTaskCollections =
                distinctRouteTaskServiceTemplates.Select(serviceTemplateName => unroutedRouteTasks.Where(rt => rt.Location != null && rt.Service != null && rt.Service.ServiceTemplate.Name == serviceTemplateName));

            //Go through each route tasks service template collection and route them
            foreach (var routeTaskCollection in routeTaskCollections)
            {
                var serviceType = routeTaskCollection.First().Service.ServiceTemplate.Name;

                //Only organize route tasks with lat/longs
                var unorganizedRouteTasks = routeTaskCollection.Where(rt => rt.Location.TelerikLocation.HasValue).ToList();

                var organizedRouteTasks = new List<RouteTask>();
                var calculator = new OrthodromicDistanceCalculator();

                //TODO: Allow the depot to be set
                //Initializes the start location of GotGrease? Warehouse
                const double lastLatitude = 37.724942;
                const double lastLongitude = 122.460465;

                var lastLatLon = new GeoLocation
                {
                    Latitude = lastLatitude,
                    Longitude = lastLongitude
                };

                //Order the unorganized route tasks by location
                while (unorganizedRouteTasks.Count > 0)
                {
                    var nextRouteTaskToAdd =
                        unorganizedRouteTasks.MinBy(
                            rt => calculator.OrthodromicDistance(lastLatLon, new GeoLocation
                                                                                 {
                                                                                     Latitude = Convert.ToDouble(rt.Location.Latitude),
                                                                                     Longitude = Convert.ToDouble(rt.Location.Longitude)
                                                                                 }, OrthodromicDistanceCalculator.FormulaType.SphericalLawOfCosinesFormula));

                    //Add the nextRouteTaskToAdd to the organized list and remove it from the unorganized list
                    unorganizedRouteTasks.Remove(nextRouteTaskToAdd);
                    organizedRouteTasks.Add(nextRouteTaskToAdd);

                    lastLatLon = new GeoLocation
                    {
                        Latitude = Convert.ToDouble(nextRouteTaskToAdd.Location.Latitude),
                        Longitude = Convert.ToDouble(nextRouteTaskToAdd.Location.Longitude)
                    };
                }

                var routesOfServiceType = routesToPopulate.Where(route => route.RouteType == serviceType);
                routedTasks.AddRange(PutTasksIntoRoutes(routesOfServiceType, organizedRouteTasks));
            }

            return routedTasks;
        }

        /// <summary>
        /// Puts the tasks into routes.
        /// </summary>
        /// <param name="routesWithServiceType">Type of the routes with service.</param>
        /// <param name="organizedRouteTasks">The organized route tasks.</param>
        /// <returns>The routed tasks.</returns>
        private static IEnumerable<RouteTask> PutTasksIntoRoutes(IEnumerable<Route> routesWithServiceType, IEnumerable<RouteTask> organizedRouteTasks)
        {
            var routedTasks = new List<RouteTask>();

            routesWithServiceType = routesWithServiceType.ToArray();
            organizedRouteTasks = organizedRouteTasks.ToArray();

            var routeCount = routesWithServiceType.Count();
            var routeTasksCount = organizedRouteTasks.Count();

            //Can only put tasks into routes if there is at least one route and one task
            if (routeCount <= 0 || routeTasksCount <= 0) return new List<RouteTask>();

            var normalRouteTaskCount = routeTasksCount / routeCount;
            var numberOfRoutesWithExtraTask = routeTasksCount % routeCount;

            int currentRouteTaskIndex = 0;

            //Go through each route, and add it's equal share of organized route tasks
            for (var currentRouteIndex = 0; currentRouteIndex < routeCount; currentRouteIndex++)
            {
                //Get the current route
                var route = routesWithServiceType.ElementAt(currentRouteIndex);

                //Choose the number of tasks, based on if this route has an extra task or not
                var currentRouteTaskCount = currentRouteIndex >= routeCount - numberOfRoutesWithExtraTask ? normalRouteTaskCount + 1 : normalRouteTaskCount;

                //Go through the number of route tasks for this route
                for (var i = 0; i < currentRouteTaskCount; i++)
                {
                    //Add a RouteDestination with the task to the route
                    var routeTask = organizedRouteTasks.ElementAt(currentRouteTaskIndex);
                    //Move the route task index forward
                    currentRouteTaskIndex++;

                    //Create a new route destination and add the task to it
                    var newRouteDestination = new RouteDestination
                    {
                        Id = Guid.NewGuid(),
                        OrderInRoute = route.RouteDestinations.Count + 1,
                        Client = routeTask.Client,
                        Location = routeTask.Location
                    };
                    newRouteDestination.RouteTasks.Add(routeTask);

                    //Add the route destination to the route
                    route.RouteDestinationsListWrapper.Add(newRouteDestination);

                    routedTasks.Add(routeTask);
                }
            }

            return routedTasks;
        }
    }
}

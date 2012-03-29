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
        /// <param name="unroutedTaskHolders"> </param>
        /// <param name="routesToPopulate">The routes to populate.</param>
        /// <returns>The tasks put into routes.</returns>
        public static IEnumerable<TaskHolder> PopulateRoutes(IEnumerable<TaskHolder> unroutedTaskHolders, IEnumerable<Route> routesToPopulate)
        {
            var routedTaskHolders = new List<TaskHolder>();

            //Organize the unroutedTaskHolders by ServiceTemplateName

            //a) get a collection of unique ServiceTemplate Names (types) from unroutedTaskHolders
            var distinctServiceTemplates =
                unroutedTaskHolders.Where(th => th.ServiceName != null).Select(th => th.ServiceName).Distinct().ToArray();

            //b) organize the unroutedTaskHolders into a 2d collection by ServiceTemplate Name 
            //   only choose task holders with a LocationId and a ServiceName
            var routeTaskHolderCollections =
                distinctServiceTemplates.Select(serviceTemplateName => unroutedTaskHolders.Where(th => th.LocationId != null && th.ServiceName == serviceTemplateName));

            //Go through each routeTaskHolderCollection and route them
            //(Before routing them, convert them into RouteTasks)
            foreach (var routeTaskHolderCollection in routeTaskHolderCollections)
            {
                var serviceType = routeTaskHolderCollection.First().ServiceName;

                //Only route tasks with lat/longs
                var unorganizedTaskHolders = routeTaskHolderCollection.Where(th => th.Longitude.HasValue && th.Latitude.HasValue).ToList();

                //Before being added to this collection, the TaskHolder will be converted to a RouteTask
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
                while (unorganizedTaskHolders.Count > 0)
                {
                    var nextTaskHolderToAdd =
                        unorganizedTaskHolders.MinBy(
                            th => calculator.OrthodromicDistance(lastLatLon, new GeoLocation
                            {
                                Latitude = Convert.ToDouble(th.Latitude),
                                Longitude = Convert.ToDouble(th.Longitude)
                            }, OrthodromicDistanceCalculator.FormulaType.SphericalLawOfCosinesFormula));

                    //Add the nextRouteTaskToAdd to the organized list and remove it from the unorganized list
                    unorganizedTaskHolders.Remove(nextTaskHolderToAdd);

                    var routeTask = TaskHolder.ConvertToRouteTask(nextTaskHolderToAdd);

                    organizedRouteTasks.Add(routeTask);

                    lastLatLon = new GeoLocation
                    {
                        Latitude = Convert.ToDouble(nextTaskHolderToAdd.Latitude),
                        Longitude = Convert.ToDouble(nextTaskHolderToAdd.Longitude)
                    };
                }

                //Take the organizedRouteTask collection and put it into routes
                var routedRouteTasks = PutTasksIntoRoutes(routesToPopulate.Where(route => route.RouteType == serviceType), organizedRouteTasks);

                //Return the RouteTaskHolders of the routed RouteTasks
                routedTaskHolders.AddRange(routedRouteTasks.Select(rt => rt.ParentRouteTaskHolder));
            }

            return routedTaskHolders;
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
                        OrderInRoute = route.RouteDestinations.Count + 1,
                        ClientId = routeTask.ClientId,
                        LocationId = routeTask.LocationId
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

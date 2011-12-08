using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FoundOps.Common.Composite.Tools;
using FoundOps.Core.Models.CoreEntities;
using System.Linq;

namespace FoundOps.Core.Context.Tools
{
    public class SimpleRouteCalculator
    {
        /// <summary>
        /// Populates the routes.
        /// </summary>
        /// <param name="unroutedRouteTasks">The unrouted route tasks.</param>
        /// <param name="routesToPopulate">The routes to populate.</param>
        /// <returns>
        /// The tasks that were not routed.
        /// </returns>
        public static IEnumerable<RouteTask> PopulateRoutes(IEnumerable<RouteTask> unroutedRouteTasks, IEnumerable<Route> routesToPopulate)
        {
            //Initializes the start location of GotGrease? Warehouse
            const double lastLatitude = 37.724942;
            const double lastLongitude = 122.460465;

            var lastLatLon = new GeoLocation
                                 {
                                     Latitude = lastLatitude,
                                     Longitude = lastLongitude
                                 };

            //Collection of Unique ServiceTemplate Names (types) from unroutedRouteTasks
            var distinctServiceTemplateNames =
                unroutedRouteTasks.Where(rt => rt.Service != null).Select(rt => rt.Service.ServiceTemplate.Name).Distinct().ToArray();

            //2D collection of RouteTasks organized by ServiceTemplate Name (only those with a Location)
            var routeTaskCollections =
                distinctServiceTemplateNames.Select(serviceTemplateName => unroutedRouteTasks.Where(rt => rt.Service != null && rt.Location != null && rt.Service.ServiceTemplate.Name == serviceTemplateName));

            var listOfRoutes = routesToPopulate;
            var remainingUnroutedTasks = unroutedRouteTasks.ToList();

            foreach (var serviceTemplateName in distinctServiceTemplateNames)
            {
                var firstUnorganizedRouteTaskCollection =  routeTaskCollections.FirstOrDefault(rtc => rtc.Any(rt => String.CompareOrdinal(rt.Service.ServiceTemplate.Name, serviceTemplateName) == 0));

                if (firstUnorganizedRouteTaskCollection == null)
                    continue;

                List<RouteTask> unorganizedRouteTasks = firstUnorganizedRouteTaskCollection.ToList();

                var organizedRouteTasks = new List<RouteTask>();
                var calculator = new OrthodromicDistanceCalculator();

                while (unorganizedRouteTasks.Count > 0)
                {
                    var nextRouteTaskToAdd =
                        unorganizedRouteTasks.MinBy(
                            rt => calculator.OrthodromicDistance(lastLatLon, new GeoLocation
                                                                                 {
                                                                                     Latitude = Convert.ToDouble(rt.Location.Latitude),
                                                                                     Longitude = Convert.ToDouble(rt.Location.Longitude)
                                                                                 }, OrthodromicDistanceCalculator.FormulaType.SphericalLawOfCosinesFormula));
                    unorganizedRouteTasks.Remove(nextRouteTaskToAdd);
                    organizedRouteTasks.Add(nextRouteTaskToAdd);
                    lastLatLon = new GeoLocation
                                     {
                                         Latitude = Convert.ToDouble(nextRouteTaskToAdd.Location.Latitude),
                                         Longitude = Convert.ToDouble(nextRouteTaskToAdd.Location.Longitude)
                                     };
                }

                var listOfRoutesWithServiceType = FindListOfRoutesWithServiceType(serviceTemplateName, listOfRoutes);
                var numberOfRoutesofType = listOfRoutesWithServiceType.Count();
                var numberOfRouteTasks = organizedRouteTasks.Count;

                if (numberOfRoutesofType != 0)
                {
                    var numberOfRouteTasksPerRoute = numberOfRouteTasks / numberOfRoutesofType;
                    var numberOfRoutesWithExtraTask = numberOfRouteTasks % numberOfRoutesofType;

                    PutTasksIntoRoutes(listOfRoutesWithServiceType, numberOfRouteTasksPerRoute, numberOfRoutesWithExtraTask, organizedRouteTasks, remainingUnroutedTasks);
                }
            }


            return remainingUnroutedTasks;
        }

        private static void PutTasksIntoRoutes(IEnumerable<Route> listOfRoutesWithServiceType, int numberOfRouteTasksPerRoute, int numberOfRoutesWithExtraTask, IEnumerable<RouteTask> organizedRouteTasks, List<RouteTask> remainingUnroutedTasks)
        {
            var routeCount = 0;

            foreach (Route route in listOfRoutesWithServiceType)
            {
                //Set at 1 to start because if the number of tasks doesnt evenly fit into the number of routes,
                //count is set to 0 to allow for the extra task to be added
                var count = 1;

                if (routeCount < numberOfRoutesWithExtraTask)
                    count = 0;

                routeCount++;

                foreach (RouteTask routeTask in organizedRouteTasks)
                {
                    if (count <= numberOfRouteTasksPerRoute && routeTask.RouteDestination == null)
                    {
                        var newRouteDestination = new RouteDestination
                        {
                            Id = Guid.NewGuid(),
                            OrderInRoute = route.RouteDestinations.Count + 1,
                            Client = routeTask.Client,
                            Location = routeTask.Location
                        };
                        newRouteDestination.RouteTasks.Add(routeTask);
                        route.RouteDestinationsListWrapper.Add(newRouteDestination);
                        remainingUnroutedTasks.Remove(routeTask);
                        count++;
                    }
                }
            }
        }

        private static IEnumerable<Route> FindListOfRoutesWithServiceType(string serviceTemplateName, IEnumerable<Route> listOfRoutes)
        {
            var tempListOfRoutes = new ObservableCollection<Route>();

            foreach (Route route in listOfRoutes.Where(route => route.RouteType == serviceTemplateName))
            {
                tempListOfRoutes.Add(route);
            }

            return tempListOfRoutes;
        }
    }
}

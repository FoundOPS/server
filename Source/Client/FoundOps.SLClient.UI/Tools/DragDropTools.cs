using System;
using System.Collections.Generic;
using System.Linq;
using FoundOps.Core.Models.CoreEntities;
using Telerik.Windows.Controls.DragDrop;
using Telerik.Windows.Controls.TreeView;

namespace FoundOps.SLClient.UI.Tools
{
    /// <summary>
    /// The position dragged to.
    /// </summary>
    public enum DropPlacement
    {
        None,
        Before,
        In,
        After
    }

    /// <summary>
    /// Tools for Dragging and Dropping
    /// </summary>
    public class DragDropTools
    {
        /// <summary>
        /// Finds the drop placement.
        /// </summary>
        /// <param name="e">The <see cref="Telerik.Windows.Controls.DragDrop.DragDropEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        public static DropPlacement FindDropPlacement(DragDropEventArgs e)
        {
            //The text that would show up on a Drag to tell you whether you are going
            //to drop something before, in or after the item you are hovering over
            var dragActionString = ((String)((TreeViewDragCue)e.Options.DragCue).DragActionContent);

            if (dragActionString != null)
            {
                if (dragActionString.Contains("before"))
                    return DropPlacement.Before;
                if (dragActionString.Contains("in"))
                    return DropPlacement.In;
                if (dragActionString.Contains("after"))
                    return DropPlacement.After;
            }

            return DropPlacement.None;
        }

        /// <summary>
        /// Adds the dragged item to route task.
        /// </summary>
        /// <param name="destination">The destination.</param>
        /// <param name="routeTask">The route task.</param>
        /// <param name="dropPlacement">The drop placement.</param>
        public static void AddToRouteTask(RouteTask destination, RouteTask routeTask, DropPlacement dropPlacement)
        {
            var destinationRouteTask = destination;

            var placeInDestination = destinationRouteTask.OrderInRouteDestination;

            //Makes adjustments to the placeInDestination based on where routeTask is being dropped
            if (dropPlacement == DropPlacement.After)
                placeInDestination++;

            if (dropPlacement == DropPlacement.Before && placeInDestination > 0)
                placeInDestination--;

            //Add the RouteTask to the RouteDestination found above
            destinationRouteTask.RouteDestination.RouteTasksListWrapper.Insert(placeInDestination, routeTask);
        }

        /// <summary>
        /// Adds the draggedItem to either the Route or RouteDestination it was dragged to.
        /// </summary>
        /// <param name="routeTask">The route task.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="placeInRoute">The place in route.</param>
        /// <param name="dropPlacement"> </param>
        public static void AddToDestinationOrRoute(RouteTask routeTask, object destination, int placeInRoute, DropPlacement dropPlacement)
        {
            //If the user drops the task on a destination, just add it to the end of its list of RouteTasks
            if ((destination is RouteDestination) && dropPlacement == DropPlacement.In)
            {
                ((RouteDestination)destination).RouteTasksListWrapper.Add(routeTask);

                //No need to do any of the other logic below, skip to the next iteration of the loop
                return;
            }

            //Create new destination
            var newDestination = new RouteDestination
            {
                Id = Guid.NewGuid(),
                Location = routeTask.Location,
                Client = routeTask.Client,

            };

            //Add the tasks to the destination
            newDestination.RouteTasks.Add(routeTask);

            if (destination is RouteDestination)
            {
                //Get the Destinations, Route
                var route = ((RouteDestination)destination).Route;

                //Add the new destination to the Route
                route.RouteDestinationsListWrapper.Insert(placeInRoute, newDestination);
            }
            if (destination is Route)
            {
                //Add the new destination to the Route
                ((Route)destination).RouteDestinationsListWrapper.Insert(placeInRoute, newDestination);
            }
        }

        /// <summary>
        /// Gets the drop placement.
        /// </summary>
        /// <param name="destination">The destination.</param>
        /// <param name="dropPlacement">The drop placement.</param>
        /// <returns>The index that the object will be dropped at</returns>
        public static int GetDropPlacement(object destination, DropPlacement dropPlacement)
        {
            var routeDraggedTo = new Route();
            var placeInRoute = 0;

            if (destination is Route)
            {
                placeInRoute = routeDraggedTo.RouteDestinationsListWrapper.Count;

                //Telerik does not allow you to drop above the root node in RadTreeView
                //We also know that the only time you could possibly drop into a Route and meet the condition below will be
                //if you try to drop above the root node. Therfore, if this occurs,
                //we reset placeInRoute to '0' so the drop occurs where the user anticipated 
                if (dropPlacement == DropPlacement.None || dropPlacement == DropPlacement.In)
                    placeInRoute = 0;
            }

            if (destination is RouteDestination || destination is RouteTask)
                placeInRoute =
                        destination is RouteDestination
                    //Choose the RouteDestination's OrderInRoute
                            ? ((RouteDestination)destination).OrderInRoute
                    //Choose the RouteTasks parent RouteDestination's OrderInRoute
                            : ((RouteTask)destination).RouteDestination.OrderInRoute;


            return placeInRoute;
        }

        /// <summary>
        /// Gets the type of the route.
        /// </summary>
        /// <param name="payloadCheck">The payload check.</param>
        /// <returns></returns>
        public static string GetRouteType(object payloadCheck)
        {
            if (payloadCheck is RouteDestination)
            {
                var routeDestination = ((RouteDestination)payloadCheck).RouteTasks.FirstOrDefault();
                if (routeDestination == null || routeDestination.Service == null || routeDestination.Service.ServiceTemplate == null)
                    return null;

                return routeDestination.Service.ServiceTemplate.Name;
            }
            if (payloadCheck is RouteTask)
            {
                if (((RouteTask)payloadCheck).Service == null)
                    return null;

                return ((RouteTask)payloadCheck).Service.ServiceTemplate.Name;
            }

            return null;
        }

        public static object CheckItemsForService(IEnumerable<object> payloadCollection)
        {
            var payloadCheck = payloadCollection.FirstOrDefault();
            
            int count = 0;

            foreach (var serviceObject in payloadCollection.OfType<RouteTask>().TakeWhile(serviceObject => (serviceObject).Service == null))
            {
                payloadCheck = (payloadCollection.ElementAt(count));
                count++;
            }
            return payloadCheck;
        }
    }
}

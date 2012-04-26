using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.SLClient.UI.Controls.Dispatcher;
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

    public struct CommonErrorConstants
    {
        public const string Valid = "Valid";

        public const string RouteLacksCapabilities = "Route Lacks Service Capabilities";

        public const string InvalidLocation = "Invalid Location";
    }

    /// <summary>
    /// Tools for Dragging and Dropping
    /// </summary>
    public class DragDropTools
    {
        /// <summary>
        /// Finds the drop placement (before, in or after).
        /// </summary>
        /// <param name="e">The <see cref="Telerik.Windows.Controls.DragDrop.DragDropEventArgs"/> instance containing the event data.</param>
        /// <returns>The DropPlacement Enum</returns>
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
        public static void AddRouteTaskToDestinationOrRoute(RouteTask routeTask, object destination, int placeInRoute, DropPlacement dropPlacement)
        {
            //If the user drops the task on a destination, just add it to the end of its list of RouteTasks
            if ((destination is RouteDestination) && dropPlacement == DropPlacement.In)
            {
                ((RouteDestination)destination).RouteTasksListWrapper.Add(routeTask);
                //TODO:add analytic

                //No need to do any of the other logic below, skip to the next iteration of the loop
                return;
            }

            routeTask.RemoveRouteDestination();

            //Create new destination
            var newDestination = new RouteDestination
            {
                Id = Guid.NewGuid(),
                LocationId =  routeTask.LocationId,
                ClientId = routeTask.ClientId
            };

            //Add the tasks to the destination
            newDestination.RouteTasks.Add(routeTask);

            if (destination is RouteDestination)
            {
                //Get the Destinations, Route
                var route = ((RouteDestination)destination).Route;

                //Add the new destination to the Route
                route.RouteDestinationsListWrapper.Insert(placeInRoute, newDestination);
                //TODO:add analytic
            }
            if (destination is Route)
            {
                //Add the new destination to the Route
                ((Route)destination).RouteDestinationsListWrapper.Insert(placeInRoute, newDestination);
                //TODO:add analytic
            }
        }

        /// <summary>
        /// Finds placeInRoute based on destination of the drop and the dropPlacement found earlier.
        /// </summary>
        /// <param name="destination">The destination.</param>
        /// <param name="dropPlacement">The drop placement.</param>
        /// <returns>The index that the object will be dropped at</returns>
        public static int GetDropPlacement(object destination, DropPlacement dropPlacement)
        {
            var placeInRoute = 0;

            if (destination is Route)
            {
                placeInRoute = ((Route)destination).RouteDestinationsListWrapper.Count;

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
        /// <returns> Route type</returns>
        public static string GetRouteType(object payloadCheck)
        {
            if (payloadCheck is RouteDestination)
            {
                var routetask = ((RouteDestination)payloadCheck).RouteTasks.FirstOrDefault();
                if (routetask == null || routetask.ParentRouteTaskHolder.ServiceName == null)
                    return null;

                return routetask.ParentRouteTaskHolder.ServiceName;
            }
            if (payloadCheck is RouteTask)
            {
                if (((RouteTask)payloadCheck).ParentRouteTaskHolder.ServiceName == null)
                    return null;

                return ((RouteTask)payloadCheck).ParentRouteTaskHolder.ServiceName;
            }

            return null;
        }

        /// <summary>
        /// Checks the items to be sure that their services are the same.
        /// </summary>
        /// <param name="payloadCollection">The payload collection.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Adds the route task to a route in the correct destination. This also redirects to another method which will actually add it to the correct place
        /// </summary>
        /// <param name="routeTask">The route task.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="placeInRoute">The place in route.</param>
        /// <param name="dropPlacement">The drop placement.</param>
        public static void AddRouteTaskToRoute(RouteTask routeTask, object destination, int placeInRoute, DropPlacement dropPlacement)
        {
            //IF the drag destination is a RouteTask, add the draggedItem to that RouteTask
            var routeTaskDestination = destination as RouteTask;
            if (routeTaskDestination != null)
            {
                AddToRouteTask(routeTaskDestination, routeTask, dropPlacement);
                //TODO:add analytic

                //No need to do any of the other logic below, skip to the next iteration of the loop
                return;
            }

            AddRouteTaskToDestinationOrRoute(routeTask, destination, placeInRoute, dropPlacement);
            //TODO:add analytic
        }

        /// <summary>
        /// Creates the drag cue when an error message needs to be displayed.
        /// </summary>
        /// <param name="cue">The cue.</param>
        /// <param name="errorString">The error string.</param>
        /// <param name="dataTemplate">The data template.</param>
        /// <returns></returns>
        public static TreeViewDragCue CreateErrorDragCue(TreeViewDragCue cue, string errorString, DataTemplate dataTemplate)
        {
            cue = SetPreviewAndToolTipVisabilityAndDropPossible(cue, Visibility.Collapsed, Visibility.Visible, false);
            cue.DragTooltipContentTemplate = dataTemplate;
            cue.DragActionContent = String.Format(errorString);

            return cue;
        }

        /// <summary>
        /// Sets the preview and tool tip visability as well as whether a drop is actually possible based on error codes.
        /// </summary>
        /// <param name="cue">The cue.</param>
        /// <param name="previewVisibility">The preview visibility.</param>
        /// <param name="toolTipVisibility">The tool tip visibility.</param>
        /// <param name="dropPossible">Bool value that tells us if the drop is possible </param>
        /// <returns></returns>
        public static TreeViewDragCue SetPreviewAndToolTipVisabilityAndDropPossible(TreeViewDragCue cue, Visibility previewVisibility, Visibility toolTipVisibility, bool dropPossible)
        {
            cue.DragPreviewVisibility = previewVisibility;
            cue.DragTooltipVisibility = toolTipVisibility;
            cue.IsDropPossible = dropPossible;

            return cue;
        }

        /// <summary>
        /// Checks the type of the route against they type of route required by the dragged items.
        /// </summary>
        /// <param name="routeType">Type of the route.</param>
        /// <param name="payloadCheck">The payload check.</param>
        /// <returns>Either returns an error string or 'valid' if no error was found</returns>
        public static string CheckRouteType(string routeType, object payloadCheck)
        {
            var draggedItemsType = GetRouteType(payloadCheck);

            if (draggedItemsType == null)
                return "";

            //If the Route's type and the type required by the draggedItems do not match, return an error
            if (String.CompareOrdinal(routeType, draggedItemsType) != 0)
                return CommonErrorConstants.RouteLacksCapabilities;

            return CommonErrorConstants.Valid;
        }
    }
}

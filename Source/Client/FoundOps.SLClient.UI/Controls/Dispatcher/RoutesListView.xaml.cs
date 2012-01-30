using System;
using System.Linq;
using System.Windows;
using System.Collections;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media;
using FoundOps.SLClient.UI.Tools;
using Telerik.Windows.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FoundOps.SLClient.UI.ViewModels;
using FoundOps.Core.Models.CoreEntities;
using Telerik.Windows.Controls.DragDrop;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Controls.TreeView;
using System.Windows.Controls.Primitives;
using ListBox = System.Windows.Controls.ListBox;

namespace FoundOps.SLClient.UI.Controls.Dispatcher
{
    /// <summary>
    /// UI for displaying Routes as lists
    /// </summary>
    public partial class RoutesListView
    {
        /// <summary>
        /// Constants for drag cues that require error strings
        /// </summary>
        public struct ErrorConstants
        {
            public const string DifferentLocations = "Items Selected Have Different Locations";

            public const string InvalidSelection = "Invalid Drag Selections";

            public const string RouteLacksCapability = "Route Lacks Service Capability";

            public const string CantMergeDestinations = "Can't Merge Destinations";

            public const string InvalidLocation = "Invalid Location";

        }
        #region Properties and Variables

        /// <summary>
        /// Gets the routes VM.
        /// </summary>
        public RoutesVM RoutesVM { get { return VM.Routes; } }

        /// <summary>
        /// Gets the routes drag drop VM.
        /// </summary>
        public RoutesDragDropVM RoutesDragDropVM { get { return VM.RoutesDragDrop; } }

        #endregion

        /// <summary>
        /// Gets the public routes list box.
        /// </summary>
        public ListBox PublicRoutesListBox
        {
            get { return RoutesListBox; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoutesListView"/> class.
        /// </summary>
        public RoutesListView()
        {
            InitializeComponent();
        }

        private void RouteTreeViewLoaded(object sender, RoutedEventArgs e)
        {
            RadDragAndDropManager.AddDragQueryHandler(RoutesListBox, OnDragQuery);
            RadDragAndDropManager.AddDropQueryHandler(RoutesListBox, OnDropQuery);
            RadDragAndDropManager.AddDragInfoHandler(RoutesListBox, OnDragInfo);
            RadDragAndDropManager.AddDragInfoHandler(RoutesListBox, OnDropInfo);
        }

        // OnDragQuery event handler
        private void OnDragQuery(object sender, DragDropQueryEventArgs e)
        {
            e.QueryResult = true;
            e.Handled = true;
        }
        // OnDropQuery event handler
        private void OnDropQuery(object sender, DragDropQueryEventArgs e)
        {
            //var draggedItems = e.Options.Payload as ICollection;
            e.QueryResult = true;
            e.Handled = true;
        }

        // OnDragInfo event handler
        //Nothing needs to be done here in this case, everything is handled in OnDragInfo
        private void OnDragInfo(object sender, DragDropEventArgs e)
        {
            if(e.Options.Payload == null)
                return;

            // Get the drag cue that the TreeView or we have created
            var cue = e.Options.DragCue as TreeViewDragCue;

            //Setup DragCue here
            var errorString = DragCueSetter(e);

            if (errorString == "")
            {
                cue.DragTooltipVisibility = Visibility.Collapsed;
                cue.DragPreviewVisibility = Visibility.Visible;
                cue.IsDropPossible = true;
            }
            else
            {
                cue.DragTooltipVisibility = Visibility.Visible;
                cue.DragPreviewVisibility = Visibility.Collapsed;
                cue.DragTooltipContentTemplate = this.Resources["DragCueTemplate"] as DataTemplate; ;
                cue.DragActionContent = String.Format(errorString);
                cue.IsDropPossible = false;
            }

            e.Options.DragCue = cue;
        }

        // OnDropInfo event handler
        private void OnDropInfo(object sender, DragDropEventArgs e)
        {
            //Prevents you from dragging no where
            if (e.Options.Destination == null || e.Options.Payload == null)
                return;

            var destination = e.Options.Destination.DataContext;

            var draggedItems = e.Options.Payload as IEnumerable<object>;

            // Get the drag cue that the TreeView or we have created
            var cue = e.Options.DragCue as TreeViewDragCue;

            switch (e.Options.Status)
            {
                //Sets the Drag cue if a drop is possible
                case DragStatus.DropPossible:
                    cue.DragTooltipVisibility = Visibility.Collapsed;
                    cue.IsDropPossible = true;

                    //Setup DragCue here
                    var errorString = DragCueSetter(e);

                    if (errorString == "")
                    {
                        cue.DragTooltipVisibility = Visibility.Collapsed;
                        cue.DragPreviewVisibility = Visibility.Visible;
                        cue.IsDropPossible = true;
                    }
                    else
                    {
                        cue.DragTooltipVisibility = Visibility.Visible;
                        cue.DragPreviewVisibility = Visibility.Collapsed;
                        cue.DragTooltipContentTemplate = this.Resources["DragCueTemplate"] as DataTemplate; ;
                        cue.DragActionContent = String.Format(errorString);
                        cue.IsDropPossible = false;
                    }

                    if (e.Options.Destination is TaskBoard)
                    {
                        cue.IsDropPossible = true;
                        cue.DragPreviewVisibility = Visibility.Visible;
                        //Adds an 's' to "item" in DragActionContent if more than one item is being dragged
                        cue.DragActionContent = String.Format(draggedItems.Count() > 1 ? "Add items to Task Board" : "Add item to Task Board");

                        cue.DragTooltipVisibility = Visibility.Visible;
                    }
                    break;
                //Sets the Drag cue if a drop is not possible
                case DragStatus.DropImpossible:
                    cue.IsDropPossible = false;
                    break;
                case DragStatus.DragComplete:
                    {
                        //Return if an error string is being displayed 
                        if (cue.IsDropPossible == false) return;

                        //The text that would show up on a Drag to tell you whether you are going
                        //to drop something before, in or after the item you are hovering over
                        var dragActionString = ((String)((TreeViewDragCue)e.Options.DragCue).DragActionContent);

                        //Variables that help to determine the correct placement for the drop
                        #region Setup and Check "isDropIn" "isDropBefore" and "isDropAfter"

                        var isDropIn = false;
                        var isDropAfter = false;
                        var isDropBefore = false;

                        if (dragActionString != null)
                        {
                            isDropIn = dragActionString.Contains("in");
                            isDropAfter = dragActionString.Contains("after");
                            isDropBefore = dragActionString.Contains("before");
                        }

                        #endregion

                        #region Find routeDraggedTo and the position it was dropped

                        var routeDraggedTo = new Route();
                        var placeInRoute = 0;

                        if (e.Options.Destination.DataContext is Route)
                        {
                            routeDraggedTo = (Route)e.Options.Destination.DataContext;

                            placeInRoute = routeDraggedTo.RouteDestinationsListWrapper.Count;

                            //Telerik does not allow you to drop above the root node in RadTreeView
                            //We also know that the only time you could possibly drop into a Route and meet the condition below will be
                            //if you try to drop above the root node. Therfore, if this occurs,
                            //we reset placeInRoute to '0' so the drop occurs where the user anticipated 
                            if (!isDropAfter && !isDropBefore)
                                placeInRoute = 0;
                        }
                        if (e.Options.Destination.DataContext is RouteDestination)
                        {
                            routeDraggedTo = ((RouteDestination)(e.Options.Destination.DataContext)).Route;

                            placeInRoute = ((RouteDestination)e.Options.Destination.DataContext).OrderInRoute;
                        }

                        if (e.Options.Destination.DataContext is RouteTask)
                        {
                            routeDraggedTo = ((RouteTask)(e.Options.Destination.DataContext)).RouteDestination.Route;

                            placeInRoute = ((RouteTask)e.Options.Destination.DataContext).RouteDestination.OrderInRoute;
                        }

                        #endregion

                        #region Modify placeInRoute depending on where the task was dropped

                        if (placeInRoute > 0)
                            placeInRoute--;

                        if (isDropAfter)
                            placeInRoute++;

                        if (isDropBefore && placeInRoute > 0)
                            placeInRoute--;

                        #endregion

                        foreach (var draggedItem in draggedItems.Reverse())
                        {

                            if (e.Options.Destination is TaskBoard)
                            {
                                AddToTaskBoard(draggedItem);
                                RemoveFromRoute(draggedItem);
                            }
                            else
                            {
                                AddToRoute(e, draggedItem, destination, routeDraggedTo, placeInRoute, isDropIn, isDropAfter, isDropBefore);
                            }
                        }
                    }
                    break;
            }

            e.Options.DragCue = cue;

            e.Handled = true;
        }

        #region Methods used in OnDropInfo

        /// <summary>
        /// Adds tasks to the TaskBoard.
        /// </summary>
        /// <param name="draggedItem">The dragged item.</param>
        private void AddToTaskBoard(object draggedItem)
        {
            //If the draggedItem is a RouteDestination, add all its RouteTasks to the TaskBoard
            if (draggedItem is RouteDestination)
                foreach (var task in ((RouteDestination)draggedItem).RouteTasks)
                    VM.Routes.UnroutedTasks.Add(task);

            //Id the draggedItem is a RouteTask, simply add it to the TaskBoard
            if (draggedItem is RouteTask)
                VM.Routes.UnroutedTasks.Add((RouteTask)draggedItem);
        }

        /// <summary>
        /// Removes from task board.
        /// </summary>
        /// <param name="draggedItem">The dragged item.</param>
        private void RemoveFromRoute(object draggedItem)
        {
            //If you are dragging the RouteDestination into the TaskBoard -> delete the RouteDestination completely
            if (draggedItem is RouteDestination)
                VM.Routes.DeleteRouteDestination((RouteDestination) draggedItem);

            if (draggedItem is RouteTask)
            {
                var oldRouteDestination = ((RouteTask) draggedItem).RouteDestination;
            
                ((RouteTask) draggedItem).RemoveRouteDestination();

                //if the old route destination has 0 tasks, delete it
                if (oldRouteDestination.RouteTasks.Count == 0)
                    VM.Routes.DeleteRouteDestination(oldRouteDestination);

            }
        }

        /// <summary>
        /// Adds to route.
        /// </summary>
        /// <param name="e">The <see cref="Telerik.Windows.Controls.DragDrop.DragDropEventArgs"/> instance containing the event data.</param>
        /// <param name="draggedItem">The dragged item.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="routeDraggedTo">The route dragged to.</param>
        /// <param name="placeInRoute">The place in route.</param>
        /// <param name="isDropIn">if set to <c>true</c> [is drop in].</param>
        /// <param name="isDropAfter">if set to <c>true</c> [is drop after].</param>
        /// <param name="isDropBefore">if set to <c>true</c> [is drop before].</param>
        private void AddToRoute(DragDropEventArgs e, object draggedItem, object destination, Route routeDraggedTo, int placeInRoute, bool isDropIn, bool isDropAfter, bool isDropBefore)
        {
            //If the current draggedItem is a RouteDestination -> Add it to the Route in the correct position
            var routeDestination = draggedItem as RouteDestination;
            if (routeDestination != null)
                routeDraggedTo.RouteDestinationsListWrapper.Insert(placeInRoute, (RouteDestination)draggedItem);


            //If the current draggedItem is a RouteTask -> Either add it to the RouteDestination, or create a new RouteDestination
            var routeTask = draggedItem as RouteTask;
            if (routeTask != null)
            {
                var oldRouteDestination = routeTask.RouteDestination;

                //if the old route destination only has this one task delete it
                if (oldRouteDestination.RouteTasks.Count == 1)
                    VM.Routes.DeleteRouteDestination(oldRouteDestination);

                //Check if the destination is a RouteDestination and the DragActionString is 'Drop in'
                if (isDropIn && e.Options.Destination.DataContext is RouteDestination)
                {
                    ((RouteDestination) e.Options.Destination.DataContext).RouteTasksListWrapper.Add(routeTask);
                }
                else
                {
                    var task = destination as RouteTask;
                    if (task != null)
                    {
                        #region Add the RouteTask to the RouteDestination that it was dragged to

                        //Gets the OrderInRouteDestination for the destination
                        var placeInDestination = task.OrderInRouteDestination;

                        //Makes adjustments to the placeInDestination based on where routeTask is being dropped
                        if (isDropAfter)
                            placeInDestination++;

                        if (isDropBefore && placeInDestination > 0)
                            placeInDestination--;

                        //Get the Tasks, RouteDestination
                        var routeTasksDestination = task.RouteDestination;

                        //Add the RouteTask to the RouteDestination found above
                        routeTasksDestination.RouteTasksListWrapper.Insert(placeInDestination, routeTask);

                        //No need to do any of the other logic below, skip to the next iteration of the loop
                        return;

                        #endregion
                    }

                    //Does this if you drag a task anywhere other than inside a current RouteDestination
                    #region Create new RouteDestination and add it to the Route

                    //Create new RouteDestination
                    var newDestination = new RouteDestination
                    {
                        Id = Guid.NewGuid(),
                        Location = routeTask.Location,
                        Client = routeTask.Client
                    };

                    if (destination is Route)
                    {
                        //Add the new destination to the Route
                        ((Route)destination).RouteDestinationsListWrapper.Insert(placeInRoute, newDestination);

                    }
                    if (destination is RouteDestination)
                    {
                        //Get the Destinations, Route
                        var route = ((RouteDestination)destination).Route;

                        //Add the new destination to the Route
                        route.RouteDestinationsListWrapper.Insert(placeInRoute, newDestination);
                    }

                    #endregion
                }
            }
        }

        private static string GetRouteType(object payloadCheck)
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

        /// <summary>
        ///Sets the drag cue while object is being dragged if the drop is not allowed to be dropped
        ///If the drag can be dropped then the defult drag cue is used
        ///If stopDragDrop is false it means drop is not allowed and then is cancelled
        /// </summary>
        private static string DragCueSetter(DragDropEventArgs e)
        {
            var destination = e.Options.Destination;
            var payloadCollection = (IEnumerable<object>)e.Options.Payload;

            var payloadCheck = payloadCollection.FirstOrDefault();

            //Variables that help to determine the correct placement for the drop
            #region Setup and Check "isDropIn" "isDropBefore" and "isDropAfter"

            //The text that would show up on a Drag to tell you whether you are going
            //to drop something before, in or after the item you are hovering over
            var dragActionString = ((String)((TreeViewDragCue)e.Options.DragCue).DragActionContent);

            var isDropIn = false;
            var isDropAfter = false;

            if (dragActionString != null)
            {
                isDropIn = dragActionString.Contains("in");
                isDropAfter = dragActionString.Contains("after");
            }

            #endregion

            #region Do all the checks for Mulit-Drag/Drop
            if (payloadCollection.Count() > 1)
            {
                if (payloadCheck is RouteTask)
                {
                    foreach (object draggedItem in payloadCollection)
                    {
                        //FirstOrDefault might be null but the second might have a service 
                        #region Check All Items Dragged for A Service
                        //Need to find out if the fist item dragged has a null 
                        int count = 0;
                        foreach (object serviceObject in payloadCollection)
                        {
                            //No Service means no location as well
                            if (((RouteTask)serviceObject).Service != null)
                                break;
                            payloadCheck = (payloadCollection.ElementAt(count));
                            count++;
                        }
                        #endregion

                        //Locations of selections dont match
                        if (draggedItem is RouteTask && ((RouteTask)draggedItem).Location != ((RouteTask)payloadCheck).Location && destination != null &&
                        (destination.DataContext is RouteTask || (destination.DataContext is RouteDestination && (!isDropAfter || isDropIn))))
                            return ErrorConstants.DifferentLocations;

                        //User Selection Contains Both Tasks and Destinations
                        if (draggedItem is RouteDestination)
                            return ErrorConstants.InvalidSelection;

                    }
                }
                else if (payloadCollection.FirstOrDefault() is RouteDestination)
                {
                    foreach (object draggedItem in payloadCollection)
                    {
                        //Locations of selections dont match
                        var routeDestination = (RouteDestination)payloadCheck;
                        if (routeDestination != null && (draggedItem is RouteDestination && ((RouteDestination)draggedItem).Location != routeDestination.Location) && destination != null &&
                            (destination.DataContext is RouteTask || (destination.DataContext is RouteDestination && (!isDropAfter || isDropIn))))
                            return ErrorConstants.DifferentLocations;

                        //User Selection Contains Both Tasks and Destinations
                        if (draggedItem is RouteTask)
                            return ErrorConstants.InvalidSelection;

                    }
                }

            }
            #endregion

            //Trying to drop a Destination into another Destination
            if (destination != null && destination.DataContext is RouteTask)
            {
                #region Check For Route Type
                var routeType = ((RouteTask)destination.DataContext).RouteDestination.Route.RouteType;

                var draggedItemsType = GetRouteType(payloadCheck);
                if (draggedItemsType == null)
                    return "";

                if (String.CompareOrdinal(routeType, draggedItemsType) != 0)
                    return ErrorConstants.RouteLacksCapability;

                #endregion

                if (payloadCheck is RouteDestination)
                    return ErrorConstants.CantMergeDestinations;

                //Trying to drop a Task into a Destination with a different Location
                var routeTask = payloadCheck as RouteTask;
                if (routeTask != null && (payloadCollection.FirstOrDefault() is RouteTask &&
                                                        ((routeTask.Location != ((RouteTask)destination.DataContext).Location))
                                                        && (routeTask.Location != null)))

                    return ErrorConstants.InvalidLocation;

            }

            if (destination != null && destination.DataContext is RouteDestination)
            {
                #region Check For Route Type
                var routeType = ((RouteDestination)destination.DataContext).Route.RouteType;

                var draggedItemsType = GetRouteType(payloadCheck);

                if (draggedItemsType == null)
                    return "";

                if (String.CompareOrdinal(routeType, draggedItemsType) != 0)
                    return ErrorConstants.RouteLacksCapability;

                #endregion

                if (payloadCheck is RouteDestination && isDropIn)
                    return ErrorConstants.CantMergeDestinations;



                var routeTask = payloadCheck as RouteTask;
                if (routeTask != null && (payloadCollection.FirstOrDefault() is RouteTask &&
                                                        ((routeTask.Location != ((RouteDestination)destination.DataContext).Location)
                                                         && (routeTask.Location != null)) &&
                                                        (!isDropAfter || isDropIn)))
                    return ErrorConstants.InvalidLocation;

            }
            if (destination != null && destination.DataContext is Route)
            {
                #region Check For Route Type
                var routeType = ((Route)destination.DataContext).RouteType;

                var draggedItemsType = GetRouteType(payloadCheck);
                if (draggedItemsType == null)
                    return "";

                if (String.CompareOrdinal(routeType, draggedItemsType) != 0)
                    return ErrorConstants.RouteLacksCapability;

                #endregion
            }
            return "";
        }

        #endregion

        private void RouteTreeView_PreviewDragEnded(object sender, RadTreeViewDragEndedEventArgs e)
        {
            //Fixes issue that EntityCollection does not implement IList
            //Cancelling the default drag-drop action for the TreeView is done by handling the PreviewDragEnded event.
            //http://www.telerik.com/community/forums/silverlight/drag-and-drop/draganddrop-with-radtreeview-and-entitycollection.aspx
            e.Handled = true;
        }

        //Manually handle RouteTreeViewSelected for RouteTask because multibindings will not work inside a RouteTreeView's Hierarchy
        private void RouteTreeViewSelected(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            var radTreeViewItem = e.Source as RadTreeViewItem;


            if (radTreeViewItem != null)
            {
                //Set the SelectedTask to the last selected RouteTask
                if (radTreeViewItem.Item is RouteTask)
                    this.RoutesVM.SelectedTask = (RouteTask)radTreeViewItem.Item;
                //Set the SelectedRouteDestination to the last selected RouteDestination
                if (radTreeViewItem.Item is RouteDestination)
                    this.RoutesVM.SelectedRouteDestination = (RouteDestination)radTreeViewItem.Item;
            }
        }

        //Manually handle RouteTreeViewUnselected for RouteTask because multibindings will not work inside a RouteTreeView's Hierarchy
        private void RouteTreeViewUnselected(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            var radTreeViewItem = e.Source as RadTreeViewItem;
            if (radTreeViewItem == null) return;
            if (radTreeViewItem.Item is RouteTask)
            {
                var routeTask = radTreeViewItem.Item as RouteTask;

                //If the RouteTask is the SelectedTask and it was unselected, clear SelectedTask
                if (routeTask != null && routeTask == this.RoutesVM.SelectedTask)
                    this.RoutesVM.SelectedTask = null;
            }

            if (radTreeViewItem.Item is RouteDestination)
            {
                var routeDestination = radTreeViewItem.Item as RouteDestination;

                //If the RouteDestination is the SelectedRouteDestination and it was unselected, clear SelectedRouteDestination
                if (routeDestination != null && routeDestination == this.RoutesVM.SelectedRouteDestination)
                    this.RoutesVM.SelectedRouteDestination = null;
            }
        }
    }

    public class TaskHierarchyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // We are binding a task
            var task = (RouteTask)value;
            return task;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

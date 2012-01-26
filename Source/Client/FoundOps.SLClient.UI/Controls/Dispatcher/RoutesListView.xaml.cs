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

        // OnDropInfo event handler
        private void OnDropInfo(object sender, DragDropEventArgs e)
        {
            //Prevents you from dragging no where
            if (e.Options.Destination == null)
                return;

            var destination = e.Options.Destination.DataContext;

            var draggedItems = e.Options.Payload as IEnumerable<object>;

            // Get the drag cue that the TreeView or we have created
            var cue = e.Options.DragCue as TreeViewDragCue;

            //Setup DragCue here

            switch (e.Options.Status)
            {
                //Sets the Drag cue if a drop is possible
                case DragStatus.DropPossible:
                    cue.DragTooltipVisibility = Visibility.Collapsed;
                    cue.IsDropPossible = true;

                    if (e.Options.Destination is TaskBoard)
                    {
                        cue.IsDropPossible = true;

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
                            //Used in Remove methods
                            var routeDestination = draggedItem as RouteDestination;
                            var routeTask = draggedItem as RouteTask;

                            if (e.Options.Destination is TaskBoard)
                            {
                                AddToTaskBoard(draggedItem);
                                RemoveFromRoute(draggedItem);
                            }
                            else
                            {
                                AddToRoute(e, draggedItem, destination, routeDraggedTo, placeInRoute, isDropIn, isDropAfter, isDropBefore);
                                //RemoveFromRoute(routeTask, routeDestination, e);
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
                VM.Routes.DeleteRouteDestination((RouteDestination)draggedItem);

            if (draggedItem is RouteTask)
                ((RouteTask)draggedItem).RemoveRouteDestination();
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


            #region If the current draggedItem is a RouteTask -> Either add it to the RouteDestination, or create a new RouteDestination

            var routeTask = draggedItem as RouteTask;
            if (routeTask != null)
            {
                //Check if the destination is a RouteDestination and the DragActionString is 'Drop in'
                if (isDropIn && e.Options.Destination.DataContext is RouteDestination)
                    ((RouteDestination)e.Options.Destination.DataContext).RouteTasksListWrapper.Add(routeTask);
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

                    newDestination.RouteTasks.Add(routeTask);

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

            #endregion
        }

        ///// <summary>
        ///// Removes from route.
        ///// </summary>
        ///// <param name="draggedItem">The dragged item.</param>
        ///// <param name="e">The <see cref="Telerik.Windows.Controls.DragDrop.DragDropEventArgs"/> instance containing the event data.</param>
        //private void RemoveFromRoute(RouteTask routeTask, RouteDestination routeDestination, DragDropEventArgs e)
        //{
        //    //If the draggedItem was a RouteDestination
        //    //Remove the RouteDestination from its Route
        //    if (routeDestination != null)
        //        VM.Routes.DeleteRouteDestination(routeDestination);

        //    //If the draggedItem was a RouteTask
        //    //Remove the RouteTask from its RouteDestination and check if there are any more RouteTasks in it,
        //    //If not, remove the RouteDestination
        //    if (routeTask != null)
        //    {
        //        var tasksDestination = routeTask.RouteDestination;

        //        //Remove the task from the current destination
        //        routeTask.RemoveRouteDestination();

        //        //If there are no RouteTasks remaining on the destination, delete it
        //        if (tasksDestination.RouteTasks.Count == 0)
        //            VM.Routes.DeleteRouteDestination(tasksDestination);
        //    }
        //}

        #endregion

        // OnDragInfo event handler
        //Nothing needs to be done here in this case, everything is handled in OnDragInfo
        private void OnDragInfo(object sender, DragDropEventArgs e)
        {
        }

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

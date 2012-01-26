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
                        //Here we will be adding the dragged items to somewhere, but to date we have no idea where
                        #region Adding to Route or TaskBoard

                        var routeDraggedTo = new Route();

                        var placeInRoute = 0;

                        #region Find RouteDraggedTo and the position it was dropped

                        if (e.Options.Destination.DataContext is Route)
                        {
                            routeDraggedTo = (Route)e.Options.Destination.DataContext;

                            var lastOrDefault = routeDraggedTo.RouteDestinationsListWrapper.LastOrDefault();
                            if (lastOrDefault != null)
                                placeInRoute = lastOrDefault.OrderInRoute;
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

                        var dragActionString = ((String)((TreeViewDragCue)e.Options.DragCue).DragActionContent);

                        var isDropIn = false;
                        var isDropAfter = false;
                        var isDropBefore = false;

                        if (dragActionString != null)
                        {
                            isDropIn = dragActionString.Contains("in");
                            isDropAfter = dragActionString.Contains("after");
                            isDropBefore = dragActionString.Contains("before");
                        }

                        var placeHolderReseter = placeInRoute;

                        //Iterate through the collection backwards. This keeps the order correct while dragging
                        foreach (var draggedItem in draggedItems.Reverse())
                        {
                            var routesVM = this.RoutesVM;

                            #region Adding to the TaskBoard happens here

                            //Check if you are dropping into the TaskBoard
                            if (e.Options.Destination is TaskBoard)
                            {

                                if (draggedItem is RouteDestination)
                                {
                                    foreach (var task in ((RouteDestination)draggedItem).RouteTasks)
                                    {
                                        routesVM.UnroutedTasks.Add(task);
                                    }
                                }

                                if (draggedItem is RouteTask)
                                    routesVM.UnroutedTasks.Add(draggedItem as RouteTask);

                                continue;
                            }

                            #endregion

                            //Used to reset the placeInRoute to the original drop position
                            placeInRoute = placeHolderReseter;

                            var routeDestination = draggedItem as RouteDestination;
                            if (routeDestination != null)
                            {
                                if (placeInRoute > 0)
                                    placeInRoute--;

                                if (isDropAfter)
                                    placeInRoute++;

                                if (isDropBefore)
                                    placeInRoute--;

                                routeDraggedTo.RouteDestinationsListWrapper.Insert(placeInRoute, (RouteDestination)draggedItem);
                            }

                            var routeTask = draggedItem as RouteTask;
                            if (routeTask != null)
                            {
                                //Check where you are dropping it...if it is not inside a valid destination one must be created
                                if (isDropIn)
                                    ((RouteTask)e.Options.Destination.DataContext).RouteDestination.RouteTasks.Add(routeTask);
                                else
                                {
                                    if (destination is RouteTask)
                                    {
                                        //Get the Tasks, RouteDestination
                                        var routeTasksDestination = ((RouteTask)destination).RouteDestination;

                                        //Add the RouteTask to the RouteDestination found above
                                        routeTasksDestination.RouteTasks.Add(routeTask);

                                        //No need to do any of the other logic below, skip to the next iteration of the loop
                                        continue;
                                    }

                                    //Create new RouteDestination
                                    var newDestination = new RouteDestination
                                                             {
                                                                 Id = Guid.NewGuid(),
                                                                 Location = routeTask.Location,
                                                                 Client = routeTask.Client
                                                             };

                                    newDestination.RouteTasks.Add(routeTask);

                                    if (placeInRoute > 0)
                                        placeInRoute--;

                                    if (isDropAfter)
                                        placeInRoute++;

                                    if (isDropBefore)
                                        placeInRoute--;

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
                                }
                            }
                        }

                        #endregion
                    }
                    break;
            }

            e.Options.DragCue = cue;

            e.Handled = true;
        }

        // OnDragInfo event handler
        private void OnDragInfo(object sender, DragDropEventArgs e)
        {
            var draggedItems = e.Options.Payload as IEnumerable<object>;

            if (e.Options.Status == DragStatus.DragComplete)
            {
                //Get the Route that the items were dragged from
                var routeDraggedFrom = SetRouteDraggedFrom(draggedItems, e);

                if (routeDraggedFrom == null)
                    return;

                var routesVM = this.RoutesVM;

                foreach (var draggedItem in draggedItems)
                {
                    var routeDestination = draggedItem as RouteDestination;
                    if (routeDestination != null)
                    {
                        routeDestination.Route.RouteDestinationsListWrapper.Remove(routeDestination);
                    }

                    var routeTask = draggedItem as RouteTask;
                    if (routeTask != null)
                    {
                        if (routeTask.RouteDestination.RouteTasks.Count == 1)
                        {
                            var destination = routeTask.RouteDestination;

                            routeTask.RemoveRouteDestination();

                            destination.Route.RouteDestinationsListWrapper.Remove(destination);

                            routesVM.DeleteRouteDestination(destination);

                            continue;
                        }

                        routeTask.RouteDestination.RouteTasks.Remove(routeTask);
                    }
                }
            }
        }


        #region Methods used in OnDragInfo

        /// <summary>
        /// Sets the route dragged from.
        /// </summary>
        private static Route SetRouteDraggedFrom(IEnumerable<object> payloadCollection, DragDropEventArgs e)
        {
            var firstOrDefault = payloadCollection.FirstOrDefault();

            if (firstOrDefault is RouteDestination)
                return ((RouteDestination)firstOrDefault).Route;

            var routeTask = ((RouteTask)e.Options.Source.DataContext);

            if (routeTask.RouteDestination == null)
                return null;

            var route = routeTask.RouteDestination.Route;

            return route;
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

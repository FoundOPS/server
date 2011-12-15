using System;
using System.Linq;
using System.Windows;
using System.Collections;
using System.Windows.Data;
using System.Globalization;
using Telerik.Windows.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FoundOps.SLClient.UI.ViewModels;
using FoundOps.Core.Models.CoreEntities;
using Telerik.Windows.Controls.DragDrop;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Controls.TreeView;
using System.Windows.Controls.Primitives;
using FoundOps.Common.Silverlight.UI.Tools;
using ListBox = System.Windows.Controls.ListBox;

namespace FoundOps.SLClient.UI.Controls.Dispatcher
{
    public partial class RoutesListView
    {
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

        /// <summary>
        /// Gets the routes VM.
        /// </summary>
        public RoutesVM RoutesVM
        {
            get { return (RoutesVM)this.DataContext; }
        }

        /// <summary>
        /// Gets the routes drag drop VM.
        /// </summary>
        public RoutesDragDropVM RoutesDragDropVM
        {
            get { return (RoutesDragDropVM)RoutesDragDropVMHolder.DataContext; }
        }

        private void RouteTreeViewLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            RadDragAndDropManager.AddDragQueryHandler(this.RoutesListBox, OnDragQuery);
            RadDragAndDropManager.AddDropQueryHandler(this.RoutesListBox, OnDropQuery);
            RadDragAndDropManager.AddDragInfoHandler(this.RoutesListBox, OnDragInfo);
            RadDragAndDropManager.AddDragInfoHandler(this.RoutesListBox, OnDropInfo);
        }

        private void OnDragInfo(object sender, DragDropEventArgs e)
        {
            RoutesDragDropVM.OriginalDragSource = DragSource.RotuesListBox;

            var payloadCollection = (IEnumerable<object>)e.Options.Payload;

            var draggedItems = SetDraggedItems(payloadCollection, e);

            if (draggedItems == null)
                return;

            if (e.Options.Status == DragStatus.DropPossible || e.Options.Status == DragStatus.DragInProgress)
            {
                DragCueSetter(e);
            }

            var dragCue = e.Options.DragCue as TreeViewDragCue;

            //If drop is completed, remove draggedItems from old route and add them to the new destination
            if (e.Options.Status == DragStatus.DragComplete && dragCue.IsDropPossible)
            {
                //if the drop location is the task board
                if (e.Options.Destination is GridViewGroupPanel)
                {
                    foreach (var draggedItem in draggedItems)
                    {
                        //Analytics - to track when a task is dragged from a route to the task board
                        if (RoutesVM.AutoAssignButtonHasBeenClicked)
                        {
                            Data.Services.Analytics.AddTaskToTaskBoardFromRouteAfterAutoDispatch(draggedItem.ToString());
                        }
                        else
                        {
                            Data.Services.Analytics.AddTaskToTaskBoardFromRoute(draggedItem.ToString());
                        }
                    }
                }

                var source = (RoutesVM)this.DataContext;

                var routeDraggedFrom = SetRouteDraggedFrom(payloadCollection, e);

                RemoveFromRoute(source, routeDraggedFrom, draggedItems);

                AddToRoute(e, draggedItems);
            }

            e.Handled = true;
        }

        #region Methods Called In OnDragInfo

        /// <summary>
        /// Sets the dragged items.
        /// </summary>
        private static IEnumerable SetDraggedItems(IEnumerable<object> payloadCollection, DragDropEventArgs e)
        {
            IEnumerable draggedItems;

            //Sets draggedItems to be either the RouteTask being dragged or the list of RouteTasks in the RouteDestination being dragged
            if (payloadCollection != null && payloadCollection.FirstOrDefault() is RouteDestination)
                draggedItems = ((RouteDestination)payloadCollection.FirstOrDefault()).Tasks;
            else
                draggedItems = e.Options.Payload as IEnumerable;

            return draggedItems;
        }

        /// <summary>
        /// Sets the route dragged from.
        /// </summary>
        private static Route SetRouteDraggedFrom(IEnumerable<object> payloadCollection, DragDropEventArgs e)
        {
            if (payloadCollection.FirstOrDefault() is RouteDestination)
                return ((RouteDestination)e.Options.Source.DataContext).Route;

            return ((RouteTask)e.Options.Source.DataContext).RouteDestination.Route;
        }

        /// <summary>
        ///Sets the drag cue while object is being dragged if the drop is not allowed to be dropped
        ///If the drag can be dropped then the defult drag cue is used
        ///If stopDragDrop is false it means drop is not allowed and then is cancelled
        /// </summary>
        private static void DragCueSetter(DragDropEventArgs e)
        {
            var destination = e.Options.Destination;
            var payloadCollection = (IEnumerable<object>)e.Options.Payload;
            var payloadCheck = payloadCollection.FirstOrDefault();

            var isDropIn = String.Compare(((String)((TreeViewDragCue)e.Options.DragCue).DragActionContent), "Drop In");
            var isDropAfter = String.Compare(((String)((TreeViewDragCue)e.Options.DragCue).DragActionContent), "Drop after");

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
                            payloadCheck = ((RouteTask)payloadCollection.ElementAt(count));
                            count++;
                        }
                        #endregion

                        //Locations of selections dont match
                        if (draggedItem is RouteTask && ((RouteTask)draggedItem).Location != ((RouteTask)payloadCheck).Location && destination != null &&
                        (destination.DataContext is RouteTask || (destination.DataContext is RouteDestination && (isDropAfter != 1 || isDropIn == 1))))
                        {
                            const string errorString = "Items Selected Have Different Locations";
                            e.Options.DragCue = CreateDragCue(e, errorString);
                            break;
                        }
                        //User Selection Contains Both Tasks and Destinations
                        if (draggedItem is RouteDestination)
                        {
                            const string errorString = "Invalid Drag Selections";
                            e.Options.DragCue = CreateDragCue(e, errorString);
                            break;
                        }
                    }
                }
                else if (payloadCollection.FirstOrDefault() is RouteDestination)
                {
                    foreach (object draggedItem in payloadCollection)
                    {
                        //Locations of selections dont match
                        if (draggedItem is RouteDestination && ((RouteDestination)draggedItem).Location != ((RouteDestination)payloadCheck).Location)
                        {
                            const string errorString = "Items Selected Have Different Locations";
                            e.Options.DragCue = CreateDragCue(e, errorString);
                            break;
                        }
                        //User Selection Contains Both Tasks and Destinations
                        if (draggedItem is RouteTask)
                        {
                            const string errorString = "Invalid Drag Selections";
                            e.Options.DragCue = CreateDragCue(e, errorString);
                            break;
                        }
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
                    return;

                if (String.CompareOrdinal(routeType, draggedItemsType) != 0)
                {
                    const string errorString = "Route Lacks Service Capability";
                    e.Options.DragCue = CreateDragCue(e, errorString);
                }
                #endregion

                if (payloadCheck is RouteDestination)
                {
                    const string errorString = "Can't Merge Destinations";
                    e.Options.DragCue = CreateDragCue(e, errorString);
                }
                //Trying to drop a Task into a Destination with a different Location
                if (payloadCollection.FirstOrDefault() is RouteTask &&
                    ((((RouteTask)payloadCheck).Location != ((RouteTask)destination.DataContext).Location))
                    && (((RouteTask)payloadCheck).Location != null))
                {
                    const string errorString = "Invalid Location";
                    e.Options.DragCue = CreateDragCue(e, errorString);
                }
            }

            if (destination != null && destination.DataContext is RouteDestination)
            {
                #region Check For Route Type
                var routeType = ((RouteDestination)destination.DataContext).Route.RouteType;

                var draggedItemsType = GetRouteType(payloadCheck);

                if (draggedItemsType == null)
                    return;

                if (String.CompareOrdinal(routeType, draggedItemsType) != 0)
                {
                    const string errorString = "Route Lacks Service Capability";
                    e.Options.DragCue = CreateDragCue(e, errorString);
                }
                #endregion

                if (payloadCheck is RouteDestination && isDropIn == 1)
                {
                    const string errorString = "Can't Merge Destinations";
                    e.Options.DragCue = CreateDragCue(e, errorString);
                }


                if (payloadCollection.FirstOrDefault() is RouteTask &&
                    ((((RouteTask)payloadCheck).Location != ((RouteDestination)destination.DataContext).Location)
                    && (((RouteTask)payloadCheck).Location != null)) &&
                    (isDropAfter != 1 || isDropIn == 1))
                {
                    const string errorString = "Invalid Location";
                    e.Options.DragCue = CreateDragCue(e, errorString);
                }
            }
            if (destination != null && destination.DataContext is Route)
            {
                #region Check For Route Type
                var routeType = ((Route)destination.DataContext).RouteType;

                var draggedItemsType = GetRouteType(payloadCheck);
                if (draggedItemsType == null)
                    return;

                if (String.CompareOrdinal(routeType, draggedItemsType) != 0)
                {
                    const string errorString = "Route Lacks Service Capability";
                    e.Options.DragCue = CreateDragCue(e, errorString);
                }
                #endregion
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

        private static object CreateDragCue(DragDropEventArgs e, string errorString)
        {
            var cue = e.Options.DragCue as TreeViewDragCue;
            cue.DragTooltipContent = null;
            cue.DragActionContent = String.Format(errorString);
            cue.IsDropPossible = false;

            return cue;
        }

        /// <summary>
        /// Removes draggedRouteTasks from the existing route. Then deletes any empty RouteDestination
        /// </summary>
        private static void RemoveFromRoute(RoutesVM routesVMSource, Route routeDraggedFrom, IEnumerable draggedItems)
        {
            foreach (var draggedRouteTask in draggedItems.OfType<RouteTask>())
                draggedRouteTask.RouteDestination = null;

            //Then deletes any empty RouteDestinations in the routeDraggedFrom
            foreach (var routeDestination in
                routeDraggedFrom.RouteDestinations.Where(routeDestination => routeDestination.RouteTasks.Count <= 0))
            {
                routesVMSource.DeleteRouteDestination(routeDestination);
            }
        }

        /// <summary>
        /// Adds what ever is being dragged to the specified Route by creating a new RouteDestination.
        /// </summary>
        private void AddToRoute(DragDropEventArgs e, IEnumerable draggedItems)
        {
            var destination = e.Options.Destination.DataContext;
            var isDropIn = String.Compare(((String)((TreeViewDragCue)e.Options.DragCue).DragActionContent), "Drop In");
            var isDropBefore = String.Compare(((String)((TreeViewDragCue)e.Options.DragCue).DragActionContent), "Drop before");

            //When a something is dragged to a new RouteDestination
            if (destination is RouteDestination && isDropIn != 1)
            {
                foreach (var task in draggedItems.OfType<RouteTask>())
                {
                    var newRouteDestination = new RouteDestination
                    {
                        Id = Guid.NewGuid(),
                    };

                    if (task.Client != null)
                        newRouteDestination.Client = task.Client;
                    if (task.Location != null)
                        newRouteDestination.Location = task.Location;

                    task.RemoveRouteDestination();
                    newRouteDestination.RouteTasks.Add(task);

                    if (isDropBefore == 1)
                        (((RouteDestination)destination).Route).RouteDestinationsListWrapper.Insert(((RouteDestination)destination).OrderInRoute - 1, newRouteDestination);
                    else
                        (((RouteDestination)destination).Route).RouteDestinationsListWrapper.Insert(((RouteDestination)destination).OrderInRoute, newRouteDestination);
                }
            }

            //When a task is dragged to be dropped into a destination
            if (destination is RouteDestination && isDropIn == 1)
            {
                foreach (var task in draggedItems.OfType<RouteTask>())
                {
                    task.RemoveRouteDestination();
                    ((RouteDestination)destination).RouteTasks.Add(task);

                    //Analytics - Drag and Drop. When dragging a task from another route to a route destination
                    if (RoutesVM.AutoAssignButtonHasBeenClicked)
                    {
                        Data.Services.Analytics.AddToRouteDestinationFromRouteAfterAutoDispatch(task.Name);
                    }
                    else
                    {
                        Data.Services.Analytics.AddToRouteDestinationFromRoute(task.Name);
                    }
                }
            }

            //When RouteTask is dragged into existing RouteTask, will not accept RouteDestination
            if (destination is RouteTask)
            {
                var newRouteDestination = ((RouteTask)destination).RouteDestination;

                foreach (var task in draggedItems.OfType<RouteTask>())
                {
                    task.RemoveRouteDestination();
                    newRouteDestination.RouteTasks.Add(task);

                    //Analytics - Drag and Drop. When dragging a task from another route to a route task
                    if (RoutesVM.AutoAssignButtonHasBeenClicked)
                    {
                        Data.Services.Analytics.AddToRouteTaskFromRouteAfterAutoDispatch(task.Name);
                    }
                    else
                    {
                        Data.Services.Analytics.AddToRouteTaskFromRoute(task.Name);
                    }
                }
            }

            //When a something is dragged to a new Route
            if (destination is Route)
            {
                var routeDestinations =
                    (ObservableCollection<RouteDestination>)((RadTreeView)e.Options.Destination).ItemsSource;
                var newOrderInRoute = routeDestinations.Count;

                //For new RouteDestinations
                foreach (var task in draggedItems.OfType<RouteTask>())
                {
                    var newRouteDestination = new RouteDestination
                    {
                        Id = Guid.NewGuid(),
                        OrderInRoute = newOrderInRoute
                    };

                    if (task.Client != null)
                        newRouteDestination.Client = task.Client;

                    if (task.Location != null)
                        newRouteDestination.Location = task.Location;

                    newRouteDestination.RouteTasks.Add(task);

                    newOrderInRoute++;

                    //Set to 0 initially because you might be dropping into an empty route
                    var lastDestinationOrderInRoute = 0;

                    //Checks to see if the route is empty. If not its sets lastDestinationOrderInRoute appropriately 
                    if (((Route)destination).RouteDestinationsListWrapper.Count != 0)
                        lastDestinationOrderInRoute = ((Route)destination).RouteDestinationsListWrapper.LastOrDefault().OrderInRoute;

                    ((Route)destination).RouteDestinationsListWrapper.Insert(lastDestinationOrderInRoute, newRouteDestination);

                    //Analytics - Drag and Drop. When dragging a task from another route to a route
                    if (RoutesVM.AutoAssignButtonHasBeenClicked)
                    {
                        Data.Services.Analytics.AddToRouteFromRouteAfterAutoDispatch(task.Name);
                    }
                    else
                    {
                        Data.Services.Analytics.AddToRouteFromRoute(task.Name);
                    }
                }
            }
        }

        #endregion

        #region Other Methods Required By Telerik for Drag/Drop

        private void OnDragQuery(object sender, DragDropQueryEventArgs e)
        {
            if (e.Options.Status == DragStatus.DragQuery)
            {
                var element = e.GetElement<ScrollBar>(e.Options.MouseClickPoint);
                e.QueryResult = e.QueryResult.HasValue && e.QueryResult.Value && element == null;
            }

            if (RoutesDragDropVM.OriginalDragSource == DragSource.UnroutedTaskBoard)
                DragCueSetterDrop(e.Options.Payload as IEnumerable, e);

            e.QueryResult = true;
            e.Handled = true;
        }

        private void OnDropQuery(object sender, DragDropQueryEventArgs e)
        {
            if (RoutesDragDropVM.OriginalDragSource == DragSource.UnroutedTaskBoard)
                DragCueSetterDrop(e.Options.Payload as IEnumerable, e);
            e.QueryResult = true;
            e.Handled = true;
        }

        /// <summary>
        /// Sets the DragCue for the specified item being dragged and its current potential drop location.
        /// Doesnt change the DragCue if drop location is valid
        /// </summary>
        private void DragCueSetterDrop(IEnumerable draggedItems, DragDropEventArgs e)
        {
            if (draggedItems == null)
                return;

            var payloadCollection = (IEnumerable<object>)e.Options.Payload;
            var destination = e.Options.Destination;

            //Here we need to choose a template for the items:
            var cue = new TreeViewDragCue();
            cue.ItemTemplate = this.Resources["DragCueTemplate"] as DataTemplate;
            cue.ItemsSource = draggedItems.OfType<RouteTask>();
            cue.DragTooltipContent = (draggedItems.OfType<RouteTask>()).FirstOrDefault().Name;
            cue.IsDropPossible = true;

            var isDropIn = String.Compare(((String)cue.DragActionContent), "Drop In");
            var isDropAfter = String.Compare(((String)cue.DragActionContent), "Drop after");
            var payloadCheck = ((RouteTask)payloadCollection.FirstOrDefault());

            #region Setting Drag Cue off Different Conditions
            if (payloadCollection.Count() > 1)
            {
                //FirstOrDefault might be null but the second might have a service 
                #region Check All Items Dragged for A Service
                //Need to find out if the fist item dragged has a null 
                int count = 1;
                foreach (object serviceObject in payloadCollection)
                {
                    //No Service means no location as well
                    if (((RouteTask)serviceObject).Service != null)
                        break;
                    if ((payloadCollection.Count()) >= count)
                        break;

                    payloadCheck = ((RouteTask)payloadCollection.ElementAt(count));
                    count++;
                }
                #endregion

                foreach (object draggedItem in payloadCollection)
                {
                    //Checks to be sure the 
                    if (((RouteTask)draggedItem).Service != null && String.CompareOrdinal(((RouteTask)draggedItem).Service.ServiceTemplate.Name, payloadCheck.Service.ServiceTemplate.Name) != 0)
                    {
                        const string errorString = "Items Selected Have Different Services";
                        e.Options.DragCue = CreateDragCue(errorString, cue);
                        break;
                    }
                }
            }

            if (destination != null && destination.DataContext is RouteTask)
            {
                if (payloadCheck.Service != null)
                {
                    #region Check For Route Type

                    var routeType = ((RouteTask)destination.DataContext).RouteDestination.Route.RouteType;
                    var draggedItemsType = payloadCheck.Service.ServiceTemplate.Name;

                    if (String.CompareOrdinal(routeType, draggedItemsType) != 0)
                    {
                        const string errorString = "Route Lacks Service Capability";
                        e.Options.DragCue = CreateDragCue(errorString, cue);
                    }

                    #endregion
                }

                //Trying to drop a Task into a Destination with a different Location
                if (((payloadCheck.Location != ((RouteTask)destination.DataContext).Location)))
                {
                    if (payloadCheck.Service != null || ((RouteTask)destination.DataContext).Service != null)
                    {
                        const string errorString = "Invalid Location";
                        e.Options.DragCue = CreateDragCue(errorString, cue);
                    }
                }
            }
            else if (destination != null && destination.DataContext is RouteDestination)
            {
                if (payloadCheck.Service != null)
                {
                    #region Check For Route Type

                    var routeType = ((RouteDestination)destination.DataContext).Route.RouteType;
                    var draggedItemsType = payloadCheck.Service.ServiceTemplate.Name;

                    if (String.CompareOrdinal(routeType, draggedItemsType) != 0)
                    {
                        const string errorString = "Route Lacks Service Capability";
                        e.Options.DragCue = CreateDragCue(errorString, cue);
                    }

                    #endregion
                }

                if (((payloadCheck.Location != ((RouteDestination)destination.DataContext).Location)
                    && (payloadCheck.Location != null)) && (isDropAfter != 1 || isDropIn == 1))
                {
                    const string errorString = "Invalid Location";
                    e.Options.DragCue = CreateDragCue(errorString, cue);
                }
            }

            else if (destination != null && destination.DataContext is Route)
            {
                if (payloadCheck.Service != null)
                {
                    #region Check For Route Type

                    var routeType = ((Route)destination.DataContext).RouteType;
                    var draggedItemsType = payloadCheck.Service.ServiceTemplate.Name;

                    if (String.CompareOrdinal(routeType, draggedItemsType) != 0)
                    {
                        const string errorString = "Route Lacks Service Capability";
                        //e.Options.DragCue = CreateDragCue(errorString, cue);
                        ((TreeViewDragCue)e.Options.DragCue).DragTooltipContent = null;
                        ((TreeViewDragCue)e.Options.DragCue).DragActionContent = String.Format(errorString);
                        ((TreeViewDragCue)e.Options.DragCue).IsDropPossible = false;
                    }

                    #endregion
                }
            }

            #endregion
            if (e.Options.DragCue == null)
                e.Options.DragCue = cue;
        }

        private static object CreateDragCue(string errorString, TreeViewDragCue cue)
        {
            cue.DragTooltipContent = null;
            cue.DragActionContent = String.Format(errorString);
            cue.IsDropPossible = false;

            return cue;
        }

        private void OnDropInfo(object sender, DragDropEventArgs e)
        {
            e.Handled = true;
        }

        #endregion

        #region Stuff Needed To Make Drag/Drop Work

        private void RouteTreeView_PreviewDragEnded(object sender, RadTreeViewDragEndedEventArgs e)
        {
            //Fixes issue that EntityCollection does not implement IList
            //http://www.telerik.com/community/forums/silverlight/drag-and-drop/draganddrop-with-radtreeview-and-entitycollection.aspx
            e.Handled = true;
        }

        private void RouteTreeView_DragStarted(object sender, RadTreeViewDragEventArgs e)
        {
            e.Handled = true;
        }

        #endregion

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

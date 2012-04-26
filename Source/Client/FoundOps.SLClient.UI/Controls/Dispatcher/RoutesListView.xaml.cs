using FoundOps.Common.Silverlight.UI.Tools.ExtensionMethods;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.SLClient.Data.Services;
using FoundOps.SLClient.UI.Tools;
using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using FoundOps.SLClient.UI.ViewModels;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.DragDrop;
using Telerik.Windows.Controls.TreeView;

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

            public const string CantMergeDestinations = "Can't Merge Destinations";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoutesListView"/> class.
        /// </summary>
        public RoutesListView()
        {
            InitializeComponent();

            //Adds the Mouse Click event to the possible events because for some reason the one provided does not work as it should
            //For more info: http://en.csharp-online.net/WPF_Concepts%E2%80%94Routed_Events_in_Action
            this.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(RoutesListBox_MouseLeftButtonDown), true);

            //Whenever the selected route changes, clear the treeview selection for other routes
            Observable2.FromPropertyChangedPattern(VM.Routes, x => x.SelectedEntity).Where(se => se != null)
            .ObserveOnDispatcher().Subscribe(selectedRoute =>
            {
                var routeTreeViewsToUnselect = RoutesListBox.GetDescendants<RadTreeView>().Where(rtv => ((Route)rtv.DataContext).Id != selectedRoute.Id);
                foreach (var routeTreeViewToUnselect in routeTreeViewsToUnselect)
                    routeTreeViewToUnselect.SelectedItems.Clear();
            });

            //Whenever the locations are loaded fixup the background colors
            ReactiveUI.MessageBus.Current.Listen<TaskLocationsUpdated>()
                //throttle by a half second to allow the assocations to hookup
                .Throttle(TimeSpan.FromSeconds(.5))
                .ObserveOnDispatcher().Subscribe(_ =>
                {
                    //Reset Grid background bindings to force the region colors to update
                    var gridsExpressionsToUpdate = this.GetDescendants<RadTreeView>().SelectMany(treeView => treeView.GetDescendants<Grid>())
                        .Select(grid => new Tuple<Grid, BindingExpression>(grid, grid.GetBindingExpression(Panel.BackgroundProperty))).Where(b => b.Item2 != null);

                    foreach (var ge in gridsExpressionsToUpdate)
                    {
                        var bind = ge.Item2.ParentBinding;
                        ge.Item1.SetBinding(Panel.BackgroundProperty, bind);
                    }
                });
        }

        #region Logic

        private void RouteTreeViewLoaded(object sender, RoutedEventArgs e)
        {
            RadDragAndDropManager.AddDragQueryHandler(RoutesListBox, OnDragQuery);
            RadDragAndDropManager.AddDropQueryHandler(RoutesListBox, OnDropQuery);
            RadDragAndDropManager.AddDragInfoHandler(RoutesListBox, OnDragInfo);
            RadDragAndDropManager.AddDragInfoHandler(RoutesListBox, OnDropInfo);
        }

        /// <summary>
        ///This gets called:
        ///1) an item is dragged over a potential drop location 
        ///2) again after the mouse button has been released
        /// </summary>
        private void OnDropInfo(object sender, DragDropEventArgs e)
        {
            //Prevents you from dragging no where
            if (e.Options.Destination == null || e.Options.Payload == null)
                return;

            var destination = e.Options.Destination.DataContext;

            var draggedItems = (IEnumerable<object>)e.Options.Payload;

            var cue = SetUpDragCue(e, draggedItems);

            e.Options.DragCue = cue;

            //Checks to be sure that the drag has been completed and that the drop would be on a valid location
            if (e.Options.Status == DragStatus.DragComplete && ((TreeViewDragCue)e.Options.DragCue).IsDropPossible)
            {
                //Get the values for the variables below
                var dropPlacement = DragDropTools.FindDropPlacement(e);

                var placeInRoute = DragDropTools.GetDropPlacement(destination, dropPlacement);


                #region Modify placeInRoute

                if (placeInRoute > 0)
                    placeInRoute--;

                if (dropPlacement == DropPlacement.After)
                    placeInRoute++;

                #endregion

                //if(((RouteDestination)((RouteTask)draggedItems.FirstOrDefault())).RouteTasks.Count == 1)
                //if(e.Options.Destination is TaskBoard)
                //{
                //    var destinations = draggedItems.OfType<RouteDestination>().ToArray();
                //    var routeTasks = draggedItems.OfType<RouteTask>().ToArray();
                //    var taskHolders = routeTasks.Select(rt => rt.ParentRouteTaskHolder);

                //    var allTaskHoldersToAdd = taskHolders.Union(destinations.SelectMany(rd => rd.RouteTasks.Select(rt => rt.ParentRouteTaskHolder)));

                //    //Add the TaskHolder back to VM.TaskBoard.LoadedTaskHolders 
                //    ((ObservableCollection<TaskHolder>)VM.TaskBoard.CollectionView.SourceCollection).AddRange(allTaskHoldersToAdd);

                //    //Delete the Route, RouteDestinations, and RouteTasks
                //    foreach (var routeDestination in destinations)
                //        VM.Routes.DeleteRouteDestination(routeDestination);
                //    foreach (var routeTask in routeTasks)
                //        VM.Routes.DeleteRouteTask(routeTask);
                //}
                //else
                //{
                //Go in reverse to preserve the order that the objects were previously in
                foreach (var draggedItem in draggedItems.Reverse())
                {
                    if (e.Options.Destination is TaskBoard)
                    {
                        AddToTaskBoard(draggedItem);
                        RemoveFromRoute(draggedItem);
                    }
                    else
                    {
                        AddToRoute(draggedItem, destination, placeInRoute, dropPlacement);
                    }
                }
                //}
            }

            e.Handled = true;

            VM.Routes.DispatcherSave();
        }

        #region Methods used in OnDropInfo

        /// <summary>
        /// Sets up the drag cue.
        /// </summary>
        /// <param name="e">The <see cref="Telerik.Windows.Controls.DragDrop.DragDropEventArgs"/> instance containing the event data.</param>
        /// <param name="draggedItems">The dragged items.</param>
        /// <returns></returns>
        private TreeViewDragCue SetUpDragCue(DragDropEventArgs e, IEnumerable<object> draggedItems)
        {
            // Get the drag cue that the TreeView or we have created
            var cue = e.Options.DragCue as TreeViewDragCue ?? new TreeViewDragCue();


            var status = e.Options.Status;

            if (status == DragStatus.DropImpossible)
            {
                //Sets the Drag cue and we assume that a drop is not possible unless notified otherwise
                cue = DragDropTools.SetPreviewAndToolTipVisabilityAndDropPossible(cue, Visibility.Collapsed, Visibility.Visible, false);
                cue.DragActionContent = "Cannot drop here";
            }

            //Sets the Drag cue if a drop is possible
            if (status == DragStatus.DropPossible)
            {
                cue = DragDropTools.SetPreviewAndToolTipVisabilityAndDropPossible(cue, Visibility.Visible, Visibility.Collapsed, true);
                cue.IsDropPossible = true;

                //Setup the error string here
                var errorString = DragCueErrorStringHelper(e);

                //Check errorString, an empty string means that there is not an issue with the current drop location
                if (errorString != "")
                    cue = DragDropTools.CreateErrorDragCue(cue, errorString, this.Resources["DragCueTemplate"] as DataTemplate);

                if (errorString == "")
                    cue = DragDropTools.SetPreviewAndToolTipVisabilityAndDropPossible(cue, Visibility.Visible, Visibility.Collapsed, true);


                if (e.Options.Destination is TaskBoard)
                {
                    cue = DragDropTools.SetPreviewAndToolTipVisabilityAndDropPossible(cue, Visibility.Visible, Visibility.Visible, true);
                    //Adds an 's' to "item" in DragActionContent if more than one item is being dragged
                    cue.DragActionContent =
                        String.Format(draggedItems.Count() > 1
                                          ? "Add items to Task Board"
                                          : "Add item to Task Board");

                }
            }

            return cue;
        }

        /// <summary>
        ///Sets the drag cue while object is being dragged if the drop is not allowed to be dropped
        ///If the drag can be dropped then the defult drag cue is used
        ///If stopDragDrop is false it means drop is not allowed and then is cancelled
        /// </summary>
        private static string DragCueErrorStringHelper(DragDropEventArgs e)
        {
            var destination = e.Options.Destination;

            //Collection of dragged items
            var payloadCollection = (IEnumerable<object>)e.Options.Payload;

            //The first item, used in various checks to be sure that all dragged items have the same service, location, etc.
            var payloadCheck = payloadCollection.FirstOrDefault();

            //Place that the dragged items were dropped (before, in, after)
            var dropPlacement = DragDropTools.FindDropPlacement(e);

            #region Setting Drag Cue off Different Conditions

            #region Do all the checks for Mulit-Drag/Drop

            if (payloadCollection.Count() > 1)
            {
                var routeTasks = payloadCollection.OfType<RouteTask>();
                var routeDestinations = payloadCollection.OfType<RouteDestination>();

                var routeTasksExist = routeTasks.Any();
                var routeDestinationsExist = routeDestinations.Any();

                //User Selection Contains Both Tasks and Destinations
                if (routeTasksExist && routeDestinationsExist)
                    return ErrorConstants.InvalidSelection;
                //Checks whether there draggedItems are tasks and then checks to be sure they all have the same location if they are not being dropped in a route
                if (routeTasksExist)
                {
                    var locations = routeTasks.Select(rt => rt.Location).Distinct();

                    if (locations.Count() > 1 &&
                        (destination.DataContext is RouteTask ||
                         (destination.DataContext is RouteDestination && dropPlacement == DropPlacement.In)))
                        return ErrorConstants.DifferentLocations;
                }

                //FirstOrDefault might be null but the second might have a service 
                if (payloadCheck is RouteTask)
                    payloadCheck = DragDropTools.CheckItemsForService(payloadCollection);
            }

            #endregion

            if (destination == null)
                return "";

            #region Drag Destination is a RouteTask

            if (destination.DataContext is RouteTask)
            {
                #region Check For Route Type
                var routeType = ((RouteTask)destination.DataContext).RouteDestination.Route.RouteType;

                var returnString = DragDropTools.CheckRouteType(routeType, payloadCheck);

                if (returnString != CommonErrorConstants.Valid)
                    return returnString;

                #endregion

                if (payloadCheck is RouteDestination)
                    return ErrorConstants.CantMergeDestinations;

                //Trying to drop a Task into a Destination with a different Location
                var routeTask = payloadCheck as RouteTask;

                if (routeTask == null)
                    return "";

                //Checks the RouteTasks location to be sure you are able to drop it in the specified RouteDestination
                if (((routeTask.Location != ((RouteTask)destination.DataContext).Location)) && (routeTask.Location != null))
                    return CommonErrorConstants.InvalidLocation;

            }

            #endregion

            #region Drag Destination is a RouteDestination

            else if (destination.DataContext is RouteDestination)
            {
                #region Check For Route Type
                var routeType = ((RouteDestination)destination.DataContext).Route.RouteType;

                var returnString = DragDropTools.CheckRouteType(routeType, payloadCheck);

                if (returnString != CommonErrorConstants.Valid)
                    return returnString;

                #endregion

                //Disallows the merging of destinations
                if (payloadCheck is RouteDestination && dropPlacement == DropPlacement.In)
                    return ErrorConstants.CantMergeDestinations;

                var routeTask = payloadCheck as RouteTask;

                if (routeTask == null)
                    return "";

                //Checks the RouteTasks location to be sure you are able to drop it on the specified RouteDestination
                if ((((routeTask.Location != ((RouteDestination)destination.DataContext).Location) && (routeTask.Location != null)) && dropPlacement != DropPlacement.After))
                    return CommonErrorConstants.InvalidLocation;

            }

            #endregion

            #region Drag Destination is a Route

            else if (destination.DataContext is Route)
            {
                #region Check For Route Type
                var routeType = ((Route)destination.DataContext).RouteType;

                var returnString = DragDropTools.CheckRouteType(routeType, payloadCheck);

                if (returnString != CommonErrorConstants.Valid)
                    return returnString;

                #endregion
            }

            #endregion

            return "";

            #endregion
        }

        /// <summary>
        /// Adds tasks to the TaskBoard.
        /// </summary>
        /// <param name="draggedItem">The dragged item.</param>
        private void AddToTaskBoard(object draggedItem)
        {
            //If the draggedItem is a RouteDestination, add all its RouteTasks to the TaskBoard
            if (draggedItem is RouteDestination)
            {
                foreach (var task in ((RouteDestination) draggedItem).RouteTasks)
                    CreateNewRouteTaskAndAddToTaskBoard(task);
                //TODO:add analytic
            }
            //Id the draggedItem is a RouteTask, simply add it to the TaskBoard
            if (draggedItem is RouteTask)
            {
                CreateNewRouteTaskAndAddToTaskBoard(((RouteTask) draggedItem));
                //TODO:add analytic
            }
        }

        private void CreateNewRouteTaskAndAddToTaskBoard(RouteTask routeTask)
        {
            var oldRouteTask = routeTask;

            //Create a new RouteTask to be saved as the ChildRouteTask of the TaskHolder
            var newRouteTask = new RouteTask
            {
                Id = Guid.NewGuid(),
                BusinessAccountId = oldRouteTask.BusinessAccountId,
                Client = oldRouteTask.Client,
                ClientId = oldRouteTask.ClientId,
                Date = oldRouteTask.Date,
                EstimatedDuration = oldRouteTask.EstimatedDuration,
                Location = oldRouteTask.Location,
                LocationId = oldRouteTask.LocationId,
                Name = oldRouteTask.Name,
                OwnerBusinessAccount = oldRouteTask.OwnerBusinessAccount,
                ParentRecurringService = oldRouteTask.ParentRecurringService,
                ParentRouteTaskHolder = oldRouteTask.ParentRouteTaskHolder,
                RecurringServiceId = oldRouteTask.RecurringServiceId,
                RouteDestination = null,
                RouteDestinationId = null,
                Service = oldRouteTask.Service,
                ServiceId = oldRouteTask.ServiceId,
                Status = Status.Created
            };

            VM.Routes.DeleteRouteTask(oldRouteTask);

            var taskHolder = routeTask.ParentRouteTaskHolder;

            taskHolder.ChildRouteTask = null;
            taskHolder.ChildRouteTask = newRouteTask;

            Manager.Data.DetachEntities(new[] { newRouteTask });

            ((ObservableCollection<TaskHolder>)VM.TaskBoard.CollectionView.SourceCollection).Add(routeTask.ParentRouteTaskHolder);
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
            {
                var oldRouteDestination = ((RouteTask)draggedItem).RouteDestination;

                ((RouteTask)draggedItem).RemoveRouteDestination();

                //if the old route destination has 0 tasks, delete it
                if (oldRouteDestination.RouteTasks.Count == 0)
                    VM.Routes.DeleteRouteDestination(oldRouteDestination);
            }
        }

        /// <summary>
        /// Adds to route.
        /// </summary>
        /// <param name="draggedItem">The dragged item.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="placeInRoute">The place in route.</param>
        /// <param name="dropPlacement">The drag placement.</param>
        private void AddToRoute(object draggedItem, object destination, int placeInRoute, DropPlacement dropPlacement)
        {
            //If the current draggedItem is a RouteDestination -> Add it to the Route in the correct position
            var draggedRouteDestination = draggedItem as RouteDestination;
            if (draggedRouteDestination != null)
            {
                if (destination is RouteDestination)
                    ((RouteDestination)destination).Route.RouteDestinationsListWrapper.Insert(placeInRoute, draggedRouteDestination);
                //TODO:add analytic

                if (destination is Route && dropPlacement == DropPlacement.After)
                    ((Route)destination).RouteDestinationsListWrapper.Add((RouteDestination)draggedItem);
                //TODO:add analytic
                else if (destination is Route)
                    ((Route)destination).RouteDestinationsListWrapper.Insert(placeInRoute, (RouteDestination)draggedItem);
                //TODO:add analytic
            }

            //If the current draggedItem is a RouteTask -> Either add it to the RouteDestination, or create a new RouteDestination
            var routeTask = draggedItem as RouteTask;
            if (routeTask != null)
            {
                //if the old route destination only has this one task, move the whole RouteDestination instead of the RouteTask
                if (routeTask.RouteDestination.RouteTasks.Count == 1)
                {
                    if (destination is Route && dropPlacement == DropPlacement.After)
                        ((Route)destination).RouteDestinationsListWrapper.Add(routeTask.RouteDestination);
                    //TODO:add analytic
                    else if (destination is Route)
                        ((Route)destination).RouteDestinationsListWrapper.Insert(placeInRoute, routeTask.RouteDestination);
                    //TODO:add analytic
                }
                //This will check the destination and call the correct method to add the RouteTask to the appropriate place
                else
                {
                    DragDropTools.AddRouteTaskToRoute(routeTask, destination, placeInRoute, dropPlacement);
                }
            }
        }

        #endregion

        #region Non-Drag/Drop Methods

        private void RouteTreeViewPreviewDragEnded(object sender, RadTreeViewDragEndedEventArgs e)
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
                //Set the SelectedRouteTask to the last selected RouteTask
                if (radTreeViewItem.Item is RouteTask)
                    VM.Routes.SelectedRouteTask = (RouteTask)radTreeViewItem.Item;
                //Set the SelectedRouteDestination to the last selected RouteDestination
                if (radTreeViewItem.Item is RouteDestination)
                    VM.Routes.SelectedRouteDestination = (RouteDestination)radTreeViewItem.Item;
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

                //If the RouteTask is the routeTask and it was unselected, clear SelectedRouteTask
                if (routeTask == VM.Routes.SelectedRouteTask)
                    VM.Routes.SelectedRouteTask = null;
            }

            if (radTreeViewItem.Item is RouteDestination)
            {
                var routeDestination = radTreeViewItem.Item as RouteDestination;

                //If the RouteDestination is the SelectedRouteDestination and it was unselected, clear SelectedRouteDestination
                if (routeDestination != null && routeDestination == VM.Routes.SelectedRouteDestination)
                    VM.Routes.SelectedRouteDestination = null;
            }
        }

        #endregion

        #region Un-Used Methods Required By Drag/Drop

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
        //Nothing needs to be done here in this case, everything is handled in OnDropInfo
        private void OnDragInfo(object sender, DragDropEventArgs e)
        {
        }

        #endregion

        /// <summary>
        /// Handles the MouseLeftButtonDown event of the RoutesListBox control.
        /// Overriders the actual Mouse button click event because the one provided does not work for some reason
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void RoutesListBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Checks to be sure that the click happened on a route
            var border = e.OriginalSource as Border;

            if (border == null)
                return;

            //Further checks to be sure that the click happened on a click
            var route = (border).DataContext as Route;

            //Sets the SelectedEntity to the route that was clicked on
            if (route != null)
                VM.Routes.SelectedEntity = route;
        }

        #endregion
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

using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Collections;
using System.Windows.Media;
using System.Reactive.Linq;
using FoundOps.Common.Tools;
using System.Windows.Controls;
using FoundOps.SLClient.UI.Controls.Dispatcher;
using FoundOps.SLClient.UI.Tools;
using Telerik.Windows.Controls;
using System.IO.IsolatedStorage;
using System.Collections.Generic;
using FoundOps.SLClient.Data.Tools;
using System.Collections.ObjectModel;
using FoundOps.SLClient.UI.ViewModels;
using FoundOps.Core.Models.CoreEntities;
using Telerik.Windows.Controls.DragDrop;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Controls.TreeView;
using FoundOps.Common.Silverlight.Blocks;
using Analytics = FoundOps.SLClient.Data.Services.Analytics;

namespace FoundOps.SLClient.Navigator.Panes.Dispatcher
{
    /// <summary>
    /// The main page of the dispatcher.
    /// </summary>
    [ExportPage("Dispatcher")]
    public partial class MainPage
    {
        #region Properties and Variables

        /// <summary>
        /// Gets the RegionsVM.
        /// </summary>
        public RegionsVM RegionsVM { get { return VM.Regions; } }

        /// <summary>
        /// Gets the RoutesVM.
        /// </summary>
        public RoutesVM RoutesVM { get { return VM.Routes; } }

        /// <summary>
        /// Gets the routes drag drop VM.
        /// </summary>
        public RoutesDragDropVM RoutesDragDropVM { get { return VM.RoutesDragDrop; } }

        #endregion

        /// <summary>
        /// Constants for drag cues that require error strings
        /// </summary>
        public struct ErrorConstants
        {
            /// <summary>
            /// Error String if you try to drag two things with different ServiceTemplates
            /// </summary>
            public const string DifferentService = "Items Selected Have Different Services";
            /// <summary>
            /// Error String if you try to drop something on a Route without the correct capability
            /// </summary>
            public const string RouteLacksCapabilities = "Route Lacks Service Capabilities";
            /// <summary>
            /// Error String if you try to drop something on a destination with a different location
            /// </summary>
            public const string InvalidLocation = "Invalid Location";

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();

            //var manager = new Manager();
            //_analytics = manager.Analytics;

            //Loads the layout from Isolated Storage
            LoadLayout();

            //set the click event for the reset button in DispatcherToolBar
            this.DispatcherToolBar.ResetLayoutButton.Click += ResetLayoutButtonClick;

            this.DependentWhenVisible(RoutesVM);
            this.DependentWhenVisible(RegionsVM);

            RadDragAndDropManager.AddDragQueryHandler(this.TaskBoard, OnDragQuery);
            RadDragAndDropManager.AddDropQueryHandler(this.TaskBoard, OnDropQuery);
            RadDragAndDropManager.AddDragInfoHandler(this.TaskBoard, OnDragInfo);
            RadDragAndDropManager.AddDropInfoHandler(this.TaskBoard, OnDropInfo);

            RadDragAndDropManager.SetAllowDrag(this.TaskBoard, true);
            RadDragAndDropManager.SetAllowDrop(this.TaskBoard, true);

            //Whenever a route, route destination, or task is selected select the appropriate details pane
            this.RoutesVM.FromAnyPropertyChanged()
                .Where(e => e.PropertyName == "SelectedEntity" || e.PropertyName == "SelectedRouteDestination" || e.PropertyName == "SelectedTask")
                .Throttle(new TimeSpan(0, 0, 0, 0, 250)).ObserveOnDispatcher()
                .Subscribe(pe =>
                {
                    switch (pe.PropertyName)
                    {
                        case "SelectedEntity":
                            DestinationDetailsPane.IsSelected = false;
                            TaskDetailsPane.IsSelected = false;
                            RouteDetailsPane.IsSelected = true;
                            break;
                        case "SelectedRouteDestination":
                            RouteDetailsPane.IsSelected = false;
                            TaskDetailsPane.IsSelected = false;
                            DestinationDetailsPane.IsSelected = true;
                            break;
                        case "SelectedTask":
                            RouteDetailsPane.IsSelected = false;
                            DestinationDetailsPane.IsSelected = false;
                            TaskDetailsPane.IsSelected = true;
                            break;
                    }
                });
        }

        #region Logic

        #region Analytics

        //Tracks the selected pane in the group containing the route list, schedule view and map view
        private void MainGroupSelectionChanged(object sender, RoutedEventArgs e)
        {
            //if (mainGroup == null)
            //    return;
            //
            //TrackEventAction.Track("RoutePanes", "RoutePanesSelectionChanged", mainGroup.SelectedPane.ToString());
        }

        private void AddNewRouteTaskButtonClick(object sender, RoutedEventArgs e)
        {
            //Analytics - Track when a new route task is created
            Analytics.AddNewRouteTask();
        }

        private void DeleteRouteTaskButtonClick(object sender, RoutedEventArgs e)
        {
            //Analytics - Track when a route task is deleted
            Analytics.DeleteRouteTask();
        }

        #endregion

        #region DragAndDrop

        private void OnDropInfo(object sender, DragDropEventArgs e)
        {
            //Nothing needs to be done here in this case, everything is handled in OnDragInfo
        }

        private void OnDragInfo(object sender, DragDropEventArgs e)
        {
            if (CheckSourceForHeaderCell(e.Options.Source))
                return;

            var draggedItems = e.Options.Payload as IEnumerable;

            if (draggedItems == null) return;

            // Get the drag cue that the TreeView or we have created if one previously didnt exist
            var cue = e.Options.DragCue as TreeViewDragCue ?? new TreeViewDragCue();

            //Set up a drag cue and save it
            var errorString = DragCueSetter(draggedItems, e);

            if (errorString == "")
            {
                var itemNames = ((IEnumerable<object>)draggedItems).Select(i => ((RouteTask)i).Name);
                cue.DragPreviewVisibility = Visibility.Visible;
                cue.DragTooltipVisibility = Visibility.Collapsed;
                cue.ItemsSource = itemNames;
                e.Options.DragCue = cue;
            }
            else
            {
                cue.DragTooltipVisibility = Visibility.Visible;
                cue.DragTooltipContentTemplate = this.Resources["DragCueTemplate"] as DataTemplate; ;
                cue.DragActionContent = String.Format(errorString);
                cue.IsDropPossible = false;
                e.Options.DragCue = cue;
            }

            //Drag has completed and has been placed at a valid location
            if (e.Options.Status == DragStatus.DragComplete && ((TreeViewDragCue)e.Options.DragCue).IsDropPossible)
            {
                RemoveFromTaskBoard(draggedItems);

                AddToTreeView(e, draggedItems);
            }
            e.Handled = true;
        }

        #region Methods used in Drag/Drop Info

        /// <summary>
        /// Sets the DragCue based on all possible errors.
        /// </summary>
        /// <param name="draggedItems">The dragged items.</param>
        /// <param name="e">The <see cref="Telerik.Windows.Controls.DragDrop.DragDropEventArgs"/> instance containing the event data.</param>
        /// <returns>Empty string if there is no error. Otherwise, it returns the errorString</returns>
        private string DragCueSetter(IEnumerable draggedItems, DragDropEventArgs e)
        {
            if (!draggedItems.OfType<RouteTask>().Any()) return "";

            //Checks to see if the destination is null
            var destinationCheck = e.Options.Destination;
            if (destinationCheck == null) return "";

            //If its not null we set the destination to e.Options.Destination.DataContext
            var destination = destinationCheck.DataContext;

            var payloadCheck = (draggedItems.OfType<RouteTask>().FirstOrDefault());

            #region Setting Drag Cue off Different Conditions

            #region Check For Multiple Tasks Being Dragged

            if (draggedItems.OfType<RouteTask>().Count() > 1)
            {
                //FirstOrDefault might be null but the second might have a service 
                #region Check All Items Dragged for A Service
                //Need to find out if the fist item dragged has a null 
                int count = 0;
                foreach (var serviceObject in draggedItems.Cast<object>().TakeWhile(serviceObject => ((RouteTask)serviceObject).Service == null))
                {
                    payloadCheck = draggedItems.OfType<RouteTask>().ElementAt(count);
                    count++;
                }
                #endregion

                if (draggedItems.OfType<RouteTask>().Where(draggedItem => (draggedItem).Service != null &&
                            String.CompareOrdinal((draggedItem).Service.ServiceTemplate.Name, payloadCheck.Service.ServiceTemplate.Name) != 0)
                            .Any(draggedItem => (draggedItem).Service != null))
                    return ErrorConstants.DifferentService;

            }

            #endregion

            #region Drag Destination is RouteTask

            if (destination != null && destination is RouteTask)
            {
                if (payloadCheck.Service != null)
                {
                    #region Check For Route Type

                    var routeType = ((RouteTask)destination).RouteDestination.Route.RouteType;

                    var shouldReturn = CheckRouteType(routeType, payloadCheck, e);

                    if (shouldReturn) return ErrorConstants.RouteLacksCapabilities;

                    #endregion
                }

                //Trying to drop a Task into a Destination with a different Location
                if (((payloadCheck.Location != ((RouteTask)destination).Location)))
                    if (payloadCheck.Service != null || ((RouteTask)destination).Service != null)
                        return ErrorConstants.InvalidLocation;
            }

            #endregion

            #region Drag Destination is RouteDestination

            else if (destination != null && destination is RouteDestination)
            {
                if (payloadCheck.Service != null)
                {
                    #region Check For Route Type

                    var routeType = ((RouteDestination)destination).Route.RouteType;

                    bool shouldReturn = CheckRouteType(routeType, payloadCheck, e);

                    if (shouldReturn) return ErrorConstants.RouteLacksCapabilities;

                    #endregion
                }

                #region Set isDropIn

                var dragActionString = ((String)((TreeViewDragCue)e.Options.DragCue).DragActionContent);

                var isDropIn = false;

                if (dragActionString != null)
                {
                    isDropIn = dragActionString.Contains("in");
                }

                #endregion

                //Checks to see if the dragged locations match the destinations location. Also makes sure that you are trying to drop in the destination before throwing an error.
                if (((payloadCheck.Location != ((RouteDestination)destination).Location)) && isDropIn)
                    return ErrorConstants.InvalidLocation;

            }

            #endregion

            #region Drag Destination is Route

            else if (destination != null && destination is Route)
            {
                if (payloadCheck.Service != null)
                {
                    #region Check For Route Type

                    var routeType = ((Route)destination).RouteType;

                    bool shouldReturn = CheckRouteType(routeType, payloadCheck, e);

                    if (shouldReturn) return ErrorConstants.RouteLacksCapabilities;


                    #endregion
                }
            }

            #endregion

            return "";

            #endregion
        }

        /// <summary>
        /// Adds the draggedItems to tree view(Route).
        /// </summary>
        /// <param name="e">The <see cref="Telerik.Windows.Controls.DragDrop.DragDropEventArgs"/> instance containing the event data.</param>
        /// <param name="draggedItems">The dragged items.</param>
        private void AddToTreeView(DragDropEventArgs e, IEnumerable draggedItems)
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

                var lastOrDefault = routeDraggedTo.RouteDestinationsListWrapper.LastOrDefault();
                if (lastOrDefault != null)
                    //This will ensure that the new RouteDestination is palced at the end of the Route
                    placeInRoute = lastOrDefault.OrderInRoute;

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

            var destination = e.Options.Destination.DataContext;

            draggedItems = draggedItems as IEnumerable<object>;

            foreach (var draggedItem in draggedItems)
            {
                //We know they are all RouteTasks becuase they come from the TaskBoard
                var routeTask = draggedItem as RouteTask;

                //This is done first because we dont need to create a new RouteDestination
                if (destination is RouteTask)
                {
                    #region Add the RouteTask to the RouteDestination it was dragged to

                    var destinationRouteTask = destination as RouteTask;

                    var placeInDestination = destinationRouteTask.OrderInRouteDestination;

                    //Makes adjustments to the placeInDestination based on where routeTask is being dropped
                    if (isDropAfter)
                        placeInDestination++;

                    if (isDropBefore && placeInDestination > 0)
                        placeInDestination--;

                    //Add the RouteTask to the RouteDestination found above
                    destinationRouteTask.RouteDestination.RouteTasksListWrapper.Insert(placeInDestination, routeTask);

                    //No need to do any of the other logic below, skip to the next iteration of the loop
                    continue;

                    #endregion
                }

                //If the user drops the task on a destination, just add it to the end of its list of RouteTasks
                if ((destination is RouteDestination) && isDropIn)
                {
                    ((RouteDestination)destination).RouteTasksListWrapper.Add(routeTask);

                    //No need to do any of the other logic below, skip to the next iteration of the loop
                    continue;
                }

                #region Create new RouteDestination and add it to the Route

                var newDestination = new RouteDestination
                {
                    Id = Guid.NewGuid(),
                    Location = routeTask.Location,
                    Client = routeTask.Client,
                    
                };

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

                #endregion
            }
        }

        /// <summary>
        /// Removes from task board.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="draggedItems">The dragged items.</param>
        private void RemoveFromTaskBoard(IEnumerable draggedItems)
        {
            foreach (var draggedItem in draggedItems.OfType<RouteTask>())
                VM.Routes.UnroutedTasks.Remove(draggedItem);
        }

        /// <summary>
        /// Checks the type of the route.
        /// </summary>
        /// <param name="routeType">Type of the route.</param>
        /// <param name="payloadCheck">The payload check.</param>
        /// <param name="e">The <see cref="Telerik.Windows.Controls.DragDrop.DragDropEventArgs"/> instance containing the event data.</param>
        /// <returns>True if the Capabilities dont match</returns>
        private bool CheckRouteType(string routeType, RouteTask payloadCheck, DragDropEventArgs e)
        {
            var draggedItemsType = payloadCheck.Service.ServiceTemplate.Name;

            return String.CompareOrdinal(routeType, draggedItemsType) != 0;
        }

        /// <summary>
        /// Checks the source for header cell.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>True if the source is a GridViewHeaderCell and false if not</returns>
        private bool CheckSourceForHeaderCell(FrameworkElement source)
        {
            if (source is GridViewHeaderCell)
            {
                return true;
            }
            return false;
        }

        #endregion

        //These two methods are needed to make Drag and Drop work, but they simply tell the DragDropManager that a drag and drop are possible
        #region Drag/Drop Query

        private void OnDropQuery(object sender, DragDropQueryEventArgs e)
        {
            if (CheckSourceForHeaderCell(e.Options.Source))
                return;

            e.QueryResult = true;
            e.Handled = true;
        }

        private void OnDragQuery(object sender, DragDropQueryEventArgs e)
        {
            if (CheckSourceForHeaderCell(e.Options.Source))
                return;

            e.QueryResult = false;

            var gridView = ((TaskBoard)sender).PublicTaskBoardRadGridView;

            if (gridView != null)
            {
                IList selectedItems = gridView.SelectedItems.ToList();

                //Will return true if the number of selected tasks is more than 0
                e.QueryResult = selectedItems.Count > 0;

                e.Options.Payload = selectedItems;
            }

            e.Handled = true;
        }

        #endregion

        #endregion

        #region Layout

        #region Loading and Saving Layout

        /// <summary>
        /// Loads the layout from isolated storage.
        /// </summary>
        private void LoadLayout()
        {
            try
            {
                using (var storage = IsolatedStorageFile.GetUserStoreForApplication())
                using (var isoStream = storage.OpenFile("RadDocking_Layout.xml", FileMode.Open))
                    this.radDocking.LoadLayout(isoStream);
            }
            catch
            {
                LoadDefaultLayout();
                SaveLayout();
            }
        }

        /// <summary>
        /// Resets the layout to the stored default.
        /// </summary>
        public void LoadDefaultLayout()
        {
            var stream = GetType().Assembly.GetManifestResourceStream("FoundOps.SLClient.Navigator.Panes.Dispatcher.DefaultDispatcherLayout.xml");
            this.radDocking.LoadLayout(stream);
        }

        /// <summary>
        /// Saves the current layout. Called every time the layout is changed.
        /// </summary>
        /// <returns>The layout xml.</returns>
        private string SaveLayout()
        {
            string xml;
            // Save your layout
            using (var storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (var isoStream = storage.OpenFile("RadDocking_Layout.xml", FileMode.OpenOrCreate))
                {
                    this.radDocking.SaveLayout(isoStream);
                    isoStream.Seek(0, SeekOrigin.Begin);
                    var reader = new StreamReader(isoStream);
                    xml = reader.ReadToEnd();

                    ////Save to local file system (for FoundOPS use when resetting default layout)
                    //SaveToFileSystem(xml);
                }
            }
            // Return the generated XML
            return xml;
        }

        //Save to local file system (for FoundOPS use when resetting default layout)
        private void SaveToFileSystem(string xml)
        {
            var saveFileDialog = new SaveFileDialog
            {
                DefaultExt = "xml",
                Filter = "XML Files (*.xml)|*.xml|All files (*.*)|*.*",
                FilterIndex = 1
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                using (var stream = saveFileDialog.OpenFile())
                {
                    var sw = new StreamWriter(stream, System.Text.Encoding.UTF8);
                    sw.Write(xml);
                    sw.Close();

                    stream.Close();
                }
            }
        }

        #endregion

        #region Layout Changes

        /// <summary>
        /// Called whenever the RadDockingLayout is changed.
        /// </summary>
        private void RadDockingLayoutChangeEnded(object sender, EventArgs e)
        {
            //save the Layout whenever it changes
            SaveLayout();

            //call the analytic for handling layout changing in dispatcher
            Analytics.DispatcherLayoutChanged();
        }

        //Insures the route map stays the correct size
        private void RouteMapPaneSizeChanged(object sender, SizeChangedEventArgs e)
        {
            //if (RouteMapView.Content as FrameworkElement == null) return;

            //((FrameworkElement)RouteMapView.Content).Width = RouteMapPane.Width * .99;
            //((FrameworkElement)RouteMapView.Content).Height = RouteMapPane.Height * .99;
        }

        #endregion

        /// <summary>
        /// Resets the layout.
        /// </summary>
        private void ResetLayoutButtonClick(object sender, RoutedEventArgs e)
        {
            LoadDefaultLayout();
            SaveLayout();

            //call the analytic for handling layout changing in dispatcher
            Analytics.DispatcherLayoutReset();
        }

        #endregion

        #endregion
    }
}

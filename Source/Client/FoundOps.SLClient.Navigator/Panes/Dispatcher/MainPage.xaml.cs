using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Collections;
using System.Reactive.Linq;
using FoundOps.Common.Tools;
using System.Windows.Controls;
using FoundOps.SLClient.UI.Tools;
using System.IO.IsolatedStorage;
using System.Collections.Generic;
using FoundOps.SLClient.Data.Tools;
using FoundOps.SLClient.UI.ViewModels;
using FoundOps.Core.Models.CoreEntities;
using Telerik.Windows.Controls.DragDrop;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Controls.TreeView;
using FoundOps.Common.Silverlight.Blocks;
using FoundOps.SLClient.UI.Controls.Dispatcher;
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
            public const string DifferentService = "Items Selected Have Different Services";
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

            this.DependentWhenVisible(VM.DispatcherFilter);
            this.DependentWhenVisible(RoutesVM);
            this.DependentWhenVisible(RegionsVM);

            //RadDragAndDropManager.AddDragQueryHandler(this.TaskBoard, OnDragQuery);
            //RadDragAndDropManager.AddDropQueryHandler(this.TaskBoard, OnDropQuery);
            //RadDragAndDropManager.AddDragInfoHandler(this.TaskBoard, OnDragInfo);
            //RadDragAndDropManager.AddDropInfoHandler(this.TaskBoard, OnDropInfo);

            //RadDragAndDropManager.SetAllowDrag(this.TaskBoard, true);
            //RadDragAndDropManager.SetAllowDrop(this.TaskBoard, true);

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

        //Nothing needs to be done here in this case, everything is handled in OnDragInfo
        private void OnDropInfo(object sender, DragDropEventArgs e)
        {
        }

        private void OnDragInfo(object sender, DragDropEventArgs e)
        {
            if (CheckSourceForHeaderCell(e.Options.Source))
                return;

            var draggedItems = e.Options.Payload as IEnumerable;

            if (draggedItems == null) return;

            SetupDragCue(e, draggedItems);

            //Drag has completed and has been placed at a valid location
            if (e.Options.Status == DragStatus.DragComplete && ((TreeViewDragCue)e.Options.DragCue).IsDropPossible)
            {
                //Find out whether to drop it in, after, or before
                var dropPlacement = DragDropTools.FindDropPlacement(e);

                var destination = e.Options.Destination.DataContext;

                var placeInRoute = DragDropTools.GetDropPlacement(destination, dropPlacement);

                #region Modify placeInRoute

                if (dropPlacement == DropPlacement.Before)
                    placeInRoute--;

                //In case we somehow messed up. Set the place to the first position
                if (placeInRoute < 0)
                    placeInRoute = 0;

                #endregion

                draggedItems = draggedItems as IEnumerable<object>;

                foreach (var draggedItem in draggedItems)
                    AddToRouteRemoveFromTaskBoard(draggedItem, destination, dropPlacement, placeInRoute);
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

            //Collection of dragged items
            var payloadCollection = (IEnumerable<object>)e.Options.Payload;

            //The first item, used in various checks to be sure that all dragged items have the same service, location, etc.
            var payloadCheck = (RouteTask)(payloadCollection.FirstOrDefault());

            #region Setting Drag Cue off Different Conditions

            #region Check For Multiple Tasks Being Dragged

            var routeTasks = draggedItems.OfType<RouteTask>();

            if (routeTasks.Count() > 1)
            {
                var services = routeTasks.Select(rt => rt.Service).Where(s => s != null).Distinct();

                if (services.Count() > 1)
                    return ErrorConstants.DifferentService;

                //FirstOrDefault might be null but the second might have a service 
                payloadCheck = (RouteTask)DragDropTools.CheckItemsForService(payloadCollection);
            }

            #endregion

            if (destination == null)
                return "";

            #region Drag Destination is RouteTask

            if (destination is RouteTask)
            {
                if (payloadCheck.Service != null)
                {
                    #region Check For Route Type

                    var routeType = ((RouteTask)destination).RouteDestination.Route.RouteType;

                    var returnString = DragDropTools.CheckRouteType(routeType, payloadCheck);

                    if (returnString != CommonErrorConstants.Valid)
                        return returnString;

                    #endregion
                }

                //Trying to drop a Task into a Destination with a different Location
                if (((payloadCheck.Location != ((RouteTask)destination).Location)))
                    if (payloadCheck.Service != null || ((RouteTask)destination).Service != null)
                        return CommonErrorConstants.InvalidLocation;
            }

            #endregion

            #region Drag Destination is RouteDestination

            else if (destination is RouteDestination)
            {
                #region Check For Route Type

                var routeType = ((RouteDestination)destination).Route.RouteType;

                var returnString = DragDropTools.CheckRouteType(routeType, payloadCheck);

                if (returnString != CommonErrorConstants.Valid)
                    return returnString;

                #endregion

                var dragActionString = ((String)((TreeViewDragCue)e.Options.DragCue).DragActionContent);

                //Checks to see if the dragged locations match the destinations location. Also makes sure that you are trying to drop in the destination before throwing an error.
                if (((payloadCheck.Location != ((RouteDestination)destination).Location)) && dragActionString != null && dragActionString.Contains("in"))
                    return CommonErrorConstants.InvalidLocation;
            }

            #endregion

            #region Drag Destination is Route

            else if (destination is Route)
            {
                #region Check For Route Type

                var routeType = ((Route)destination).RouteType;

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
        /// Sets up the drag cue.
        /// </summary>
        /// <param name="e">The <see cref="Telerik.Windows.Controls.DragDrop.DragDropEventArgs"/> instance containing the event data.</param>
        /// <param name="draggedItems">The dragged items.</param>
        private void SetupDragCue(DragDropEventArgs e, IEnumerable draggedItems)
        {
            // Get the drag cue that the TreeView or we have created if one previously didnt exist
            var cue = e.Options.DragCue as TreeViewDragCue ?? new TreeViewDragCue();

            //Set up a drag cue and save it
            var errorString = DragCueSetter(draggedItems, e);

            if (errorString == "")
            {
                var itemNames = ((IEnumerable<object>)draggedItems).Select(i => ((RouteTask)i).Name);
                cue = DragDropTools.SetPreviewAndToolTipVisabilityAndDropPossible(cue, Visibility.Visible, Visibility.Collapsed, true);
                cue.ItemsSource = itemNames;
            }
            else if (errorString != "")
                cue = DragDropTools.CreateErrorDragCue(cue, errorString, this.Resources["DragCueTemplate"] as DataTemplate);

            e.Options.DragCue = cue;
        }

        /// <summary>
        /// Adds to Route remove from task board.
        /// </summary>
        /// <param name="draggedItem">The dragged item.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="dropPlacement">The drop placement.</param>
        /// <param name="placeInRoute">The place in route.</param>
        private void AddToRouteRemoveFromTaskBoard(object draggedItem, object destination, DropPlacement dropPlacement, int placeInRoute)
        {
            //We know they are all RouteTasks becuase they come from the TaskBoard
            var routeTask = draggedItem as RouteTask;

            //This will check the destination and call the correct method to add the RouteTask to the appropriate place
            DragDropTools.AddRouteTaskToRoute(routeTask, destination, placeInRoute, dropPlacement);

            //Remove the RouteTask from the TaskBoard
            //VM.Routes.UnroutedTasks.Remove((RouteTask)draggedItem);

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

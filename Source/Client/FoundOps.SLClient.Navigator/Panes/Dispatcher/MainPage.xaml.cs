using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Collections;
using System.Windows.Media;
using System.Reactive.Linq;
using FoundOps.Common.Tools;
using System.Windows.Controls;
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

        /// <summary>
        /// Adds draggedItems to route.
        /// </summary>
        private void AddToRoute(DragDropEventArgs e, IEnumerable draggedItems)
        {
            var destination = e.Options.Destination.DataContext;
            if (destination is RouteDestination)
            {
                var newRouteDestination = new RouteDestination
                {
                    Id = Guid.NewGuid(),
                    Client = draggedItems.OfType<RouteTask>().First().Client,
                    Location = draggedItems.OfType<RouteTask>().First().Location
                };

                foreach (var task in draggedItems.OfType<RouteTask>())
                {
                    newRouteDestination.RouteTasks.Add(task);

                    //Analytics - Drag and Drop. When dragging a task from the task board to a route destination
                    if (RoutesVM.AutoAssignButtonHasBeenClicked)
                        Analytics.AddToRouteDestinationFromTaskBoardAfterAutoDispatch(task.ToString());
                    else
                        Analytics.AddToRouteDestinationFromTaskBoard(task.ToString());
                }

                (((RouteDestination)destination).Route).RouteDestinationsListWrapper.Insert(((RouteDestination)destination).OrderInRoute, newRouteDestination);
            }

            if (destination is RouteTask)
            {
                var newRouteDestination = ((RouteTask)destination).RouteDestination;

                foreach (var task in draggedItems.OfType<RouteTask>())
                {
                    newRouteDestination.RouteTasks.Add(task);

                    //Analytics - Drag and Drop. When dragging a task from the task board to a route task
                    if (RoutesVM.AutoAssignButtonHasBeenClicked)
                        Analytics.AddToRouteTaskFromTaskBoardAfterAutoDispatch(task.ToString());
                    else
                        Analytics.AddToRouteTaskFromTaskBoard(task.ToString());
                }
            }

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
                    {
                        var lastOrDefault = ((Route)destination).RouteDestinationsListWrapper.LastOrDefault();
                        if (lastOrDefault != null)
                            lastDestinationOrderInRoute = lastOrDefault.OrderInRoute;
                    }

                    ((Route)destination).RouteDestinationsListWrapper.Insert(lastDestinationOrderInRoute, newRouteDestination);

                    //Analytics - Drag and Drop. When dragging a task from the task board to a route
                    if (RoutesVM.AutoAssignButtonHasBeenClicked)
                    {
                        Analytics.AddToRouteFromTaskBoardAfterAutoDispatch(task.ToString());
                    }
                    else
                    {
                        Analytics.AddToRouteFromTaskBoard(task.ToString());
                    }

                }
            }
        }

        #region Dragging

        private static object CreateDragCue(string errorString, TreeViewDragCue cue)
        {
            cue.DragTooltipContent = null;
            cue.DragActionContent = String.Format(errorString);
            cue.IsDropPossible = false;

            return cue;
        }

        /// <summary>
        /// Sets the DragCue for the specified item being dragged and its current potential drop location.
        /// Doesnt change the DragCue if drop location is valid
        /// </summary>
        private void DragCueSetter(IEnumerable draggedItems, DragDropEventArgs e)
        {
            var payloadCollection = (IEnumerable<object>)e.Options.Payload;
            var destination = e.Options.Destination;

            var draggedRouteTasks = draggedItems.OfType<RouteTask>();

            if (draggedRouteTasks.Count() <= 0) return;

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

        private void OnDragInfo(object sender, DragDropEventArgs e)
        {
            if (e.Options.Source is GridViewHeaderCell)
            {
                return;
            }

            RoutesDragDropVM.OriginalDragSource = DragSource.UnroutedTaskBoard;

            var draggedItems = e.Options.Payload as IEnumerable;
            var cue = e.Options.DragCue;

            DragCueSetter(draggedItems, e);

            cue = e.Options.DragCue;

            if (e.Options.Status == DragStatus.DragComplete && ((TreeViewDragCue)cue).IsDropPossible)
            {
                //Gets hit if you try and drop a task onto the Taskboard
                //Checks to be sure you are trying to drop into one of the Routes
                if (!(e.Options.Destination is RadTreeView)) return;

                var source = this.TaskBoard.PublicTaskBoardRadGridView.ItemsSource as IList;

                RemoveFromTaskBoard(source, draggedItems);

                AddToRoute(e, draggedItems);
            }
        }

        private void OnDragQuery(object sender, DragDropQueryEventArgs e)
        {
            if (e.Options.Source is GridViewHeaderCell)
            {
                return;
            }

            if (this.TaskBoard.PublicTaskBoardRadGridView != null)
            {
                IList selectedItems = this.TaskBoard.PublicTaskBoardRadGridView.SelectedItems.ToList();
                e.QueryResult = selectedItems.Count > 0;
                e.Options.Payload = selectedItems;
            }

            var draggedItems = e.Options.Payload as IEnumerable;

            DragCueSetter(draggedItems, e);

            e.QueryResult = true;
            e.Handled = true;
        }

        #endregion

        #region Dropping

        /// <summary>
        /// Sets up the initial DragCue
        /// DragCue potentially gets changed in OnDragInfo if drop location is invalid
        /// </summary>
        private void OnDropInfo(object sender, DragDropEventArgs e)
        {
            if (e.Options.Source is GridViewHeaderCell)
            {
                return;
            }

            var draggedItems = e.Options.Payload as ICollection;

            // Get the drag cue that the TreeView or we have created
            var cue = e.Options.DragCue as TreeViewDragCue;

            if (cue == null)
                cue = new TreeViewDragCue();

            if (e.Options.Status == DragStatus.DropPossible)
            {
                // Set a suitable text:
                cue.DragActionContent = String.Format("Add item to Task Board");
                cue.IsDropPossible = true;
                e.Options.DragCue = cue;
            }
            else if (e.Options.Status == DragStatus.DropImpossible)
            {
                cue.DragActionContent = null;
                cue.IsDropPossible = false;
            }

            if (e.Options.Status != DragStatus.DropPossible)
            {
                this.TaskBoard.PublicTaskBoardRadGridView.Background = new SolidColorBrush(Colors.White);
            }
        }

        private static void OnDropQuery(object sender, DragDropQueryEventArgs e)
        {
            if (e.Options.Source is GridViewHeaderCell)
            {
                return;
            }

            e.QueryResult = true;
            e.Handled = true;
        }

        #endregion

        /// <summary>
        /// Removes DraggedItems from the TaskBoard.
        /// </summary>
        private static void RemoveFromTaskBoard(IList source, IEnumerable draggedItems)
        {
            foreach (RouteTask draggedItem in draggedItems.OfType<RouteTask>())
            {
                if (source != null)
                {
                    source.Remove(draggedItem);
                }
            }
        }

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

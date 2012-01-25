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

        private void OnDropInfo(object sender, DragDropEventArgs e)
        {
            if(CheckSourceForHeaderCell(e.Options.Source))
                return;


            var draggedItems = e.Options.Source.DataContext as IEnumerable<object>;

            // Get the drag cue that the TreeView or we have created
            var cue = e.Options.DragCue as TreeViewDragCue ?? new TreeViewDragCue {IsDropPossible = false};

            if (cue.IsDropPossible)
            {
                // Set a suitable text:

            }
            else if (!cue.IsDropPossible)
            {
                cue.DragActionContent = null;
                cue.IsDropPossible = false;
            }

            e.Options.DragCue = cue;

            if (e.Options.Status != DragStatus.DropPossible)
            {
            }

            if (e.Options.Status == DragStatus.DragComplete)
            {
            }
            e.Handled = true;
        }

        private void OnDragInfo(object sender, DragDropEventArgs e)
        {
            if (CheckSourceForHeaderCell(e.Options.Source))
                return;

            var draggedItems = e.Options.Payload as IEnumerable<object>;

            var cue = e.Options.DragCue as TreeViewDragCue;

            DragCueSetter(draggedItems, e);

            e.Options.DragCue = cue;

            if (e.Options.Status == DragStatus.DragInProgress)
            {
                //////Set up a drag cue:

            }
            else if (e.Options.Status == DragStatus.DragComplete)
            {

            }
        }
        
        #region Methods used in Drag/Drop Info

        private void DragCueSetter(IEnumerable draggedItems, DragDropEventArgs e)
        {
            var draggedRouteTasks = (IEnumerable<RouteTask>)draggedItems;

            if (!draggedRouteTasks.Any()) return;

            var destination = e.Options.Destination.DataContext;

            //Here we need to choose a template for the items:
            var cue = new TreeViewDragCue
            {
                ItemTemplate = this.Resources["DragCueTemplate"] as DataTemplate,
                ItemsSource = draggedRouteTasks,
                IsDropPossible = true
            };

            var payloadCheck = (draggedRouteTasks.FirstOrDefault());

            #region Setting Drag Cue off Different Conditions

            #region Check For Multiple Tasks Being Dragged

            if (draggedRouteTasks.Count() > 1)
            {
                //FirstOrDefault might be null but the second might have a service 
                #region Check All Items Dragged for A Service
                //Need to find out if the fist item dragged has a null 
                int count = 0;
                foreach (var serviceObject in draggedRouteTasks.Cast<object>().TakeWhile(serviceObject => ((RouteTask)serviceObject).Service == null))
                {
                    payloadCheck = (draggedRouteTasks.ElementAt(count));
                    count++;
                }
                #endregion

                if (draggedRouteTasks.Where(draggedItem => (draggedItem).Service != null &&
                    String.CompareOrdinal((draggedItem).Service.ServiceTemplate.Name, payloadCheck.Service.ServiceTemplate.Name) != 0)
                    .Any(draggedItem => (draggedItem).Service != null))
                {
                    const string errorString = "Items Selected Have Different Services";
                    e.Options.DragCue = CreateDragCue(errorString, cue);
                }
            }

            #endregion

            #region Drag Destination is RouteTask

            if (destination != null && destination is RouteTask)
            {
                if (payloadCheck.Service != null)
                {
                    #region Check For Route Type

                    var routeType = ((RouteTask)destination).RouteDestination.Route.RouteType;

                    bool shouldReturn = CheckRouteType(routeType, payloadCheck, e, cue);

                    if (shouldReturn) return;

                    #endregion
                }

                //Trying to drop a Task into a Destination with a different Location
                if (((payloadCheck.Location != ((RouteTask)destination).Location)))
                {
                    if (payloadCheck.Service != null || ((RouteTask)destination).Service != null)
                    {
                        const string errorString = "Invalid Location";
                        e.Options.DragCue = CreateDragCue(errorString, cue);
                    }
                }
            }

            #endregion

            #region Drag Destination is RouteDestination

            else if (destination != null && destination is RouteDestination)
            {
                if (payloadCheck.Service != null)
                {
                    #region Check For Route Type

                    var routeType = ((RouteDestination)destination).Route.RouteType;

                    bool shouldReturn = CheckRouteType(routeType, payloadCheck, e, cue);

                    if (shouldReturn) return;

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
                {
                    const string errorString = "Invalid Location";
                    e.Options.DragCue = CreateDragCue(errorString, cue);
                }
            }

            #endregion

            #region Drag Destination is Route

            else if (destination != null && destination is Route)
            {
                if (payloadCheck.Service != null)
                {
                    #region Check For Route Type

                    var routeType = ((Route)destination).RouteType;

                    bool shouldReturn = CheckRouteType(routeType, payloadCheck, e, cue);

                    if (shouldReturn) return;


                    #endregion
                }
            }

            #endregion

            #endregion

            if (e.Options.DragCue == null)
                e.Options.DragCue = cue;
        }

        private bool CheckRouteType(string routeType, RouteTask payloadCheck, DragDropEventArgs e, TreeViewDragCue cue)
        {
            var draggedItemsType = payloadCheck.Service.ServiceTemplate.Name;

            if (String.CompareOrdinal(routeType, draggedItemsType) != 0)
            {
                const string errorString = "Route Lacks Service Capability";
                e.Options.DragCue = CreateDragCue(errorString, cue);
                return true;
            }

            return false;
        }

        private TreeViewDragCue CreateDragCue(string errorString, TreeViewDragCue cue)
        {
            cue.DragTooltipContent = null;
            cue.DragActionContent = String.Format(errorString);
            cue.IsDropPossible = false;

            return cue;
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

            e.QueryResult = true;
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

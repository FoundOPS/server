using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using FoundOps.Common.Silverlight.Extensions.Telerik;
using FoundOps.Common.Silverlight.Tools;
using FoundOps.SLClient.UI.ViewModels;
using FoundOps.Core.Models.CoreEntities;
using GalaSoft.MvvmLight.Messaging;
using ReactiveUI;
using Telerik.Windows.Controls.Map;

namespace FoundOps.SLClient.UI.Controls.Dispatcher
{
    public partial class RouteMapView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RouteMapView"/> class.
        /// </summary>
        public RouteMapView()
        {
            InitializeComponent();

            //Initializes the MapView to the RoadView setting via OSM
            this.MapTypeSelector.SelectedIndex = 0;

            InformationLayer.CenterMapBasedOnIpInfo();

            MessageBus.Current.Listen<RefreshRouteMapView>().Subscribe(_ => RefreshRouteMapView());
            MessageBus.Current.Listen<RouteMapVM.RouteDestinationSetMessage>().Subscribe(OnRouteDestinationSet);
        }

        /// <summary>
        /// Gets the route map VM.
        /// </summary>
        public RouteMapVM RouteMapVM
        {
            get { return (RouteMapVM)this.DataContext; }
        }

        public class MapShapeItem
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="MapShapeItem"/> class.
            /// </summary>
            public MapShapeItem()
            {
                this.Points = new LocationCollection();
            }
  
            public LocationCollection Points
            {
                get;
                private set;
            }

            /// <summary>
            /// Gets or sets the route.
            /// </summary>
            /// <value>
            /// The route.
            /// </value>
            public Route Route { get; set; }
        }

        private ObservableCollection<MapShapeItem> collection = new ObservableCollection<MapShapeItem>();

        /// <summary>
        /// Refreshes the RouteMapView with PinPoints for all of the ShownRoutes
        /// </summary>
        protected void RefreshRouteMapView()
        {
            foreach (var route in RouteMapVM.ShownRoutes)
            {
                //Create a MapPolyline for each route
                var mapPolyline = new MapShapeItem {Route = route};

                //Only show pinpoints for RouteDestinations that have locations
                foreach (var routeDestination in route.RouteDestinations.Where(rd => rd.Location != null))
                {
                    var routeDestinationControl = new ContentControl
                                                      {
                                                          ContentTemplate = (DataTemplate) this.Resources["RouteDestination"],
                                                          Content = routeDestination,
                                                          VerticalAlignment = VerticalAlignment.Top
                                                      };

                    if (routeDestination.Location.TelerikLocation == null) return;
                        MapLayer.SetLocation(routeDestinationControl, routeDestination.Location.TelerikLocation.Value);
                        InformationLayer.Items.Add(routeDestinationControl);

                        //Add a point to the polyline for each routeDestination
                        mapPolyline.Points.Add(routeDestination.Location.TelerikLocation.Value);
                }
                collection.Add(mapPolyline);
                
                InformationLayer2.ItemsSource = collection;
            }

            InformationLayer.SetBestView();
        }

        private void OnRouteDestinationSet(RouteMapVM.RouteDestinationSetMessage routeDestinationSetMessage)
        {
            Map.Center = routeDestinationSetMessage.SetRouteDestination;
            Map.ZoomLevel = 14;
            Map.ZoomLevel = 15;
        }

        private void MapTypeSelector_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (this.MapTypeSelector.SelectedIndex == 0)
                Map.Provider = new OpenStreetMapProvider();  
            else
                Map.Provider = new YahooMapsProvider(MapMode.Aerial);
        }
    }
}

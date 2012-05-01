using FoundOps.Common.Silverlight.Tools.Location;
using FoundOps.Common.Silverlight.Extensions.Telerik;
using FoundOps.Common.Silverlight.UI.Controls;
using FoundOps.Common.Tools;
using FoundOps.SLClient.UI.ViewModels;
using FoundOps.SLClient.UI.Tools;
using ReactiveUI;
using System;
using System.Reactive.Linq;
using System.Windows;
using Telerik.Windows.Controls.Map;

namespace FoundOps.SLClient.UI.Controls.Locations
{
    /// <summary>
    /// Allows you to select a Location's address and latitude/longitude
    /// </summary>
    public partial class LocationEdit
    {
        //private LocationsVM LocationsVM { get { return VM.Locations; } }

        #region LocationVM Dependency Property

        /// <summary>
        /// LocationVM
        /// </summary>
        public LocationVM LocationVM
        {
            get { return (LocationVM)GetValue(LocationVMProperty); }
            set
            {
                LocationVMPropertyChanged(value);
                SetValue(LocationVMProperty, value);
            }
        }

        private void LocationVMPropertyChanged(LocationVM locationVM)
        {
            // Zoom to BestView when selection changed on  geocode results (when there is more than just the one default option).
            var georesultselectionchanged = Observable.FromEventPattern<System.Windows.Controls.SelectionChangedEventArgs>(GeocoderResultsListBox, "SelectionChanged")
                .Where(_ => GeocoderResultsListBox.Items.Count > 1).AsGeneric();

            // Zoom to BestView when Geocoding is completed (i.e. When Search completes)
            locationVM.GeocodeCompletion.AsGeneric().Merge(georesultselectionchanged)
                .Throttle(new TimeSpan(0, 0, 0, 1)).ObserveOnDispatcher().Subscribe(_ => InformationLayer.SetBestView());

            // Subscribe to changes of latitude/longitude by changing visual state according to validity.
            locationVM.ValidLatitudeLongitudeState.Throttle(new TimeSpan(0, 0, 0, 0, 500))
                .ObserveOnDispatcher().Subscribe(validstate =>
                {
                    if (validstate)
                        VisualStateManager.GoToState(this, "MapDetails", false);
                    else
                        VisualStateManager.GoToState(this, "MapSearch", true);
                });
        }

        /// <summary>
        /// LocationVM Dependency Property.
        /// </summary>
        public static readonly DependencyProperty LocationVMProperty =
            DependencyProperty.Register(
                "LocationVM",
                typeof(LocationVM),
                typeof(LocationEdit),
                new PropertyMetadata(null));

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="LocationEdit"/> class.
        /// </summary>
        public LocationEdit()
        {
            InitializeComponent();
            if (System.ComponentModel.DesignerProperties.IsInDesignTool) return;

            //Initializes the MapView to the RoadView setting via OSM
            this.MapTypeSelector.SelectedIndex = 0;

            InformationLayer.CenterMapBasedOnIpInfo();

            MessageBus.Current.Listen<LocationSetMessage>().Subscribe(OnLocationSet);
        }

        /// <summary>
        /// Selects the latitude longitude when manually selecting the latitude and longitude by clicking the map.
        /// </summary>
        private void MapSelectLatitudeLongitude(object sender, MapMouseRoutedEventArgs eventArgs)
        {
            var latitudeLongitude = new Tuple<decimal, decimal>((decimal)eventArgs.Location.Latitude,
                                                                (decimal)eventArgs.Location.Longitude);

            //In MapEditMode call ManuallySelectLatitudeLongitude command
            if ((VisualStateGroup.CurrentState == null || VisualStateGroup.CurrentState.Name != "MapDetails") &&
                LocationVM.ManuallySetLatitudeLongitude.CanExecute(latitudeLongitude))
                LocationVM.ManuallySetLatitudeLongitude.Execute(latitudeLongitude);
        }

        private void OnLocationSet(LocationSetMessage locationSetMessage)
        {
            VisualStateManager.GoToState(this, "MapDetails", true);
            Map.Center = locationSetMessage.SetLocation;
            Map.ZoomLevel = 14;
            Map.ZoomLevel = 15;
        }

        private void EditLocationButtonClick(object sender, RoutedEventArgs e)
        {
            InformationLayer.SetBestView();
            VisualStateManager.GoToState(this, "MapSearch", true);
        }

        private void MoreDetailsButtonClick(object sender, RoutedEventArgs e)
        {
            //LocationVM.MoveToDetailsView.Execute(null);
        }

        private void MapTypeSelectorSelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (this.MapTypeSelector.SelectedIndex == 0)
                Map.Provider = new OpenStreetMapProvider();
            else
                Map.Provider = new YahooMapsProvider(MapMode.Aerial);
        }
    }
}

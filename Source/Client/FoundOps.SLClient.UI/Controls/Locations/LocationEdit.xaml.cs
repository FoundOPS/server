using System;
using ReactiveUI;
using System.Windows;
using System.Reactive.Linq;
using Telerik.Windows.Controls.Map;
using FoundOps.SLClient.UI.ViewModels;
using FoundOps.Common.Silverlight.Tools.Location;
using FoundOps.Common.Silverlight.Extensions.Telerik;

namespace FoundOps.SLClient.UI.Controls.Locations
{
    /// <summary>
    /// Allows you to select a Location's address and latitude/longitude
    /// </summary>
    public partial class LocationEdit
    {
        private LocationsVM LocationsVM { get { return (LocationsVM)this.DataContext; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocationEdit"/> class.
        /// </summary>
        public LocationEdit()
        {
            InitializeComponent();
            if (System.ComponentModel.DesignerProperties.IsInDesignTool) return;


            // Zoom to BestView when selection changed on  geocode results.
            var georesultselectionchanged = Observable.FromEventPattern<System.Windows.Controls.SelectionChangedEventArgs>(GeocoderResultsListBox, "SelectionChanged");
            georesultselectionchanged.ObserveOnDispatcher().Subscribe(_=>InformationLayer.SetBestView());

            // Zoom to BestView when Geocoding is completed (i.e. When Search completes)
            LocationsVM.SelectedLocationVMObservable.Where(lvm => lvm != null)
                .SelectMany(lvm => lvm.GeocodeCompletion).ObserveOnDispatcher().Subscribe(_ =>
                {
                    if (InformationLayer.Items.Count > 0)
                        InformationLayer.SetBestView();
                });


            // Subscribe to changes of latitude/longitude by changing visual state according to validity.
            LocationsVM.SelectedLocationVMObservable.Where(lvm => lvm != null)
                .SelectMany(lvm => lvm.ValidLatitudeLongitudeState)
                .Throttle(new TimeSpan(0, 0, 0, 0, 500))
                .ObserveOnDispatcher().Subscribe(validstate =>
            {
                if (validstate)
                    VisualStateManager.GoToState(this, "MapDetails", false);
                else
                    VisualStateManager.GoToState(this, "MapSearch", true);
            });

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
                LocationsVM.SelectedLocationVM.ManuallySetLatitudeLongitude.CanExecute(latitudeLongitude))
                LocationsVM.SelectedLocationVM.ManuallySetLatitudeLongitude.Execute(latitudeLongitude);
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
            LocationsVM.MoveToDetailsView.Execute(null);
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

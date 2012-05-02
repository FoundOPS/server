using FoundOps.Common.Silverlight.Tools.Location;
using FoundOps.Common.Silverlight.Extensions.Telerik;
using FoundOps.Common.Tools;
using FoundOps.SLClient.UI.Tools;
using FoundOps.SLClient.UI.ViewModels;
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
        #region Public Properties

        #region LocationVM Dependency Property

        /// <summary>
        /// LocationVM
        /// </summary>
        public LocationVM LocationVM
        {
            get { return (LocationVM)GetValue(LocationVMProperty); }
            set { SetValue(LocationVMProperty, value); }
        }

        /// <summary>
        /// LocationVM Dependency Property.
        /// </summary>
        public static readonly DependencyProperty LocationVMProperty =
            DependencyProperty.Register(
                "LocationVM",
                typeof(LocationVM),
                typeof(LocationEdit),
                new PropertyMetadata(LocationVMChanged));

        private static void LocationVMChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as LocationEdit;
            if (c != null)
            {
                c.SetupLocationVMLogic(e.NewValue as LocationVM);
            }
        }

        #endregion

        #endregion

        #region Locals

        //LocationVM disposables
        private IDisposable _setBestViewDisposable = null;
        private IDisposable _setMapStateDisposable = null;

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

            //Center the map based on the location of the ip address
            InformationLayer.CenterMapBasedOnIpInfo();

            //When the LocationSetMessage is sent update the UI for the newly set location
            MessageBus.Current.Listen<LocationSetMessage>().Subscribe(locationSetMessage =>
            {
                //Change to MapDetails state
                VisualStateManager.GoToState(this, "MapDetails", true);
                //Center on the set location
                Map.Center = locationSetMessage.SetLocation;
                Map.ZoomLevel = 14;
                Map.ZoomLevel = 15;
            });
        }

        #region Logic

        /// <summary>
        /// Sets up the set best view and state logic that relies on the LocationVM
        /// </summary>
        /// <param name="newLocationVM">The new LocationVM</param>
        private void SetupLocationVMLogic(LocationVM newLocationVM)
        {
            //Dispose old LocationVM subscriptions
            if (_setBestViewDisposable != null)
            {
                _setBestViewDisposable.Dispose();
                _setBestViewDisposable = null;
            }

            if (_setMapStateDisposable != null)
            {
                _setMapStateDisposable.Dispose();
                _setMapStateDisposable = null;
            }

            if (newLocationVM == null)
                return;

            // Zoom to BestView when selection changed on  geocode results (when there is more than just the one default option).
            var georesultselectionchanged = Observable.FromEventPattern<System.Windows.Controls.SelectionChangedEventArgs>(GeocoderResultsListBox, "SelectionChanged")
                .Where(_ => GeocoderResultsListBox.Items.Count > 1).AsGeneric();

            // Zoom to BestView when Geocoding is completed (i.e. When Search completes)
            _setBestViewDisposable = newLocationVM.GeocodeCompletion.AsGeneric().Merge(georesultselectionchanged)
                   .Throttle(new TimeSpan(0, 0, 0, 1)).ObserveOnDispatcher().Subscribe(_ => InformationLayer.SetBestView());

            if (newLocationVM.ValidLatitudeLongitudeState != null)
                // Subscribe to changes of latitude/longitude by changing visual state according to validity.
                _setMapStateDisposable = newLocationVM.ValidLatitudeLongitudeState.Throttle(new TimeSpan(0, 0, 0, 0, 500))
                     .ObserveOnDispatcher().Subscribe(validstate =>
                     {
                         if (validstate)
                             VisualStateManager.GoToState(this, "MapDetails", false);
                         else
                             VisualStateManager.GoToState(this, "MapSearch", true);
                     });
        }

        #region UI Interactions

        private void EditLocationButtonClick(object sender, RoutedEventArgs e)
        {
            InformationLayer.SetBestView();
            VisualStateManager.GoToState(this, "MapSearch", true);
        }

        private void MoreDetailsButtonClick(object sender, RoutedEventArgs e)
        {
            if (VM.Locations.SelectedLocationVM == LocationVM)
                VM.Locations.MoveToDetailsView.Execute(null);
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

        /// <summary>
        /// Chooses the map provider.
        /// </summary>
        private void MapTypeSelectorSelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (this.MapTypeSelector.SelectedIndex == 0)
                Map.Provider = new OpenStreetMapProvider();
            else
                Map.Provider = new YahooMapsProvider(MapMode.Aerial);
        }

        #endregion

        #endregion
    }
}

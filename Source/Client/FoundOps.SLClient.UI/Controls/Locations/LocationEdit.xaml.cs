using System;
using System.Windows;
using System.Windows.Controls;
using FoundOps.Common.Silverlight.Tools;
using FoundOps.SLClient.UI.ViewModels;
using ReactiveUI;
using Telerik.Windows.Controls.Map;
using GalaSoft.MvvmLight.Messaging;
using FoundOps.Common.Silverlight.Extensions.Telerik;

namespace FoundOps.SLClient.UI.Controls.Locations
{
    public partial class LocationEdit : UserControl
    {
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
                new PropertyMetadata(new PropertyChangedCallback(LocationVMChanged)));

        private static void LocationVMChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as LocationEdit;
            if (c != null)
            {
                var oldVM = (LocationVM)e.OldValue;
                if (oldVM != null)
                    oldVM.PropertyChanged -= c.LocationVMPropertyChanged;
                var newVM = (LocationVM)e.NewValue;
                if (newVM != null)
                    newVM.PropertyChanged += c.LocationVMPropertyChanged;
            }
        }

        void LocationVMPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "GeocoderResults")
            {
                if (_onLocationSetHandled)
                    InformationLayer.SetBestView();
                else
                    _onLocationSetHandled = true;
            }
        }

        private bool _onLocationSetHandled = true;

        #endregion

        public LocationEdit()
        {
            InitializeComponent();

            //Initializes the MapView to the RoadView setting via OSM
            this.MapTypeSelector.SelectedIndex = 0;

            InformationLayer.CenterMapBasedOnIpInfo();

            MessageBus.Current.Listen<LocationSetMessage>().Subscribe(OnLocationSet);
            MessageBus.Current.Listen<ResetMapMessage>().Subscribe(OnLocationNotSet);
        }

        private void MapSelectLatitudeLongitude(object sender, MapMouseRoutedEventArgs eventArgs)
        {
            var latitudeLongitude = new Tuple<decimal, decimal>((decimal)eventArgs.Location.Latitude,
                                                                (decimal)eventArgs.Location.Longitude);

            if ((VisualStateGroup.CurrentState == null || VisualStateGroup.CurrentState.Name != "MapViewOnly") && LocationVM.ManuallySetLatitudeLongitude.CanExecute(latitudeLongitude))
                LocationVM.ManuallySetLatitudeLongitude.Execute(latitudeLongitude);
        }

        private void OnLocationSet(LocationSetMessage locationSetMessage)
        {
            VisualStateManager.GoToState(this, "MapViewOnly", true);
            Map.Center = locationSetMessage.SetLocation;
            Map.ZoomLevel = 14;
            Map.ZoomLevel = 15;
            _onLocationSetHandled = false;
        }

        private void OnLocationNotSet(ResetMapMessage obj)
        {
            VisualStateManager.GoToState(this, "MapEdit", true);
            InformationLayer.CenterMapBasedOnIpInfo();
        }

        private void EditLocationButtonClick(object sender, RoutedEventArgs e)
        {
            InformationLayer.SetBestView();
            VisualStateManager.GoToState(this, "MapEdit", true);
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

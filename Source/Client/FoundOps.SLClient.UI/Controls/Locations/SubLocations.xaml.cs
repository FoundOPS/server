﻿using System;
using ReactiveUI;
using System.Windows;
using Telerik.Windows.Controls.Map;
using FoundOps.SLClient.UI.ViewModels;
using FoundOps.Common.Silverlight.Tools.Location;
using FoundOps.Common.Silverlight.Extensions.Telerik;

namespace FoundOps.SLClient.UI.Controls.Locations
{
    /// <summary>
    /// 
    /// </summary>
    public partial class SubLocations
    {
        #region SubLocationsVM Dependency Property

        /// <summary>
        /// SubLocationsVM
        /// </summary>
        public SubLocationsVM SubLocationsVM
        {
            get { return (SubLocationsVM)GetValue(SubLocationsVMProperty); }
            set { SetValue(SubLocationsVMProperty, value); }
        }

        /// <summary>
        /// SubLocationsVM Dependency Property.
        /// </summary>
        public static readonly DependencyProperty SubLocationsVMProperty =
            DependencyProperty.Register(
                "SubLocationsVM",
                typeof(SubLocationsVM),
                typeof(SubLocations),
                new PropertyMetadata(new PropertyChangedCallback(SubLocationsVMChanged)));

        private static void SubLocationsVMChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as SubLocations;
            if (c != null)
            {
                c.InformationLayer.SetBestView();
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="SubLocations"/> class.
        /// </summary>
        public SubLocations()
        {
            InitializeComponent();
            if (System.ComponentModel.DesignerProperties.IsInDesignTool) return;
            //Initializes the MapView to the RoadView setting via OSM
            this.MapTypeSelector.SelectedIndex = 0;

            InformationLayer.CenterMapBasedOnIpInfo();

            MessageBus.Current.Listen<SubLocationsVM.SubLocationSetMessage>().Subscribe(OnSubLocationSet);
            MessageBus.Current.Listen<LocationSetMessage>().Subscribe(OnLocationSet);
        }

        //Whenever a location is set, we want the SubLocation's map to zoom to that location (to make it easier to add sublocations)
        private void OnLocationSet(LocationSetMessage locationSetMessage)
        {
            Map.Center = locationSetMessage.SetLocation;
            Map.ZoomLevel = 14;
            Map.ZoomLevel = 15;
        }

        private void MapSelectLatitudeLongitude(object sender, MapMouseRoutedEventArgs eventArgs)
        {
            var latitudeLongitude = new Tuple<decimal, decimal>((decimal)eventArgs.Location.Latitude,
                                                                (decimal)eventArgs.Location.Longitude);

            if (this.SubLocationsVM.ManuallySetLatitudeLongitude.CanExecute(latitudeLongitude))
                this.SubLocationsVM.ManuallySetLatitudeLongitude.Execute(latitudeLongitude);
        }

        private void OnSubLocationSet(SubLocationsVM.SubLocationSetMessage subLocationSetMessage)
        {
            Map.Center = subLocationSetMessage.SetSubLocation;
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

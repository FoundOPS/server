﻿#pragma checksum "C:\FoundOps\Agile5\Source-DEV\Client\FoundOps.SLClient.UI\Controls\Regions\RegionLarge.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "DA9800404378DBAE689CA6A507DBA648"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.488
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using FoundOps.SLClient.UI.Controls.Locations;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Map;


namespace FoundOps.SLClient.UI.Controls.Regions {
    
    
    public partial class RegionLarge : System.Windows.Controls.UserControl {
        
        internal System.Windows.Controls.UserControl myRegionLarge;
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Grid GeocoderLargeGrid;
        
        internal Telerik.Windows.Controls.RadMap Map;
        
        internal Telerik.Windows.Controls.Map.InformationLayer InformationLayer;
        
        internal Telerik.Windows.Controls.RadComboBox MapTypeSelector;
        
        internal Telerik.Windows.Controls.RadComboBoxItem OSMRoad;
        
        internal Telerik.Windows.Controls.RadComboBoxItem YahooSatellite;
        
        internal FoundOps.SLClient.UI.Controls.Locations.LocationsGrid LocationsGrid;
        
        internal System.Windows.Controls.StackPanel AddExistingLocationComboBox;
        
        internal Telerik.Windows.Controls.RadComboBox ExistingLocationsComboBox;
        
        internal System.Windows.Controls.Button AddLocationButton;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/FoundOps.SLClient.UI;component/Controls/Regions/RegionLarge.xaml", System.UriKind.Relative));
            this.myRegionLarge = ((System.Windows.Controls.UserControl)(this.FindName("myRegionLarge")));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.GeocoderLargeGrid = ((System.Windows.Controls.Grid)(this.FindName("GeocoderLargeGrid")));
            this.Map = ((Telerik.Windows.Controls.RadMap)(this.FindName("Map")));
            this.InformationLayer = ((Telerik.Windows.Controls.Map.InformationLayer)(this.FindName("InformationLayer")));
            this.MapTypeSelector = ((Telerik.Windows.Controls.RadComboBox)(this.FindName("MapTypeSelector")));
            this.OSMRoad = ((Telerik.Windows.Controls.RadComboBoxItem)(this.FindName("OSMRoad")));
            this.YahooSatellite = ((Telerik.Windows.Controls.RadComboBoxItem)(this.FindName("YahooSatellite")));
            this.LocationsGrid = ((FoundOps.SLClient.UI.Controls.Locations.LocationsGrid)(this.FindName("LocationsGrid")));
            this.AddExistingLocationComboBox = ((System.Windows.Controls.StackPanel)(this.FindName("AddExistingLocationComboBox")));
            this.ExistingLocationsComboBox = ((Telerik.Windows.Controls.RadComboBox)(this.FindName("ExistingLocationsComboBox")));
            this.AddLocationButton = ((System.Windows.Controls.Button)(this.FindName("AddLocationButton")));
        }
    }
}


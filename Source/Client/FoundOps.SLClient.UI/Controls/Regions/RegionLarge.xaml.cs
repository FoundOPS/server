using System;
using ReactiveUI;
using System.ComponentModel;
using System.Windows.Controls;
using FoundOps.SLClient.UI.ViewModels;
using Telerik.Windows.Controls.Map;
using FoundOps.Common.Silverlight.Tools;
using FoundOps.Common.Silverlight.Tools.Location;
using FoundOps.Common.Silverlight.Extensions.Telerik;

namespace FoundOps.SLClient.UI.Controls.Regions
{
    public partial class RegionLarge: UserControl
    {
        public RegionLarge()
        {
            InitializeComponent();

#if DEBUG
            if (DesignerProperties.IsInDesignTool)
                return;
#endif

            //Initializes the MapView to the RoadView setting via OSM
            this.MapTypeSelector.SelectedIndex = 0;

            InformationLayer.CenterMapBasedOnIpInfo();

            MessageBus.Current.Listen<LocationSetMessage>().Subscribe(OnLocationSet);
            InformationLayer.SetBestView();
        }

        private void OnLocationSet(LocationSetMessage locationSetMessage)
        {
            Map.Center = locationSetMessage.SetLocation;
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

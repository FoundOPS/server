using System;
using System.Windows;
using System.Threading;
using Telerik.Windows.Controls.Map;

namespace FoundOps.Common.Silverlight.Tools.Location
{
    public static class MapTools
    {
        public static void SetBestView(this InformationLayer informationLayer)
        {
            if (informationLayer.Items.Count <= 0) return;

            var map = informationLayer.MapControl;

            //Wait 1 second (to let the PinPoints draw) then set the best view for the pinpoints
            Timer timer = null;
            timer = new Timer(afterTwoSecondsCallback =>
            {
                informationLayer.Dispatcher.BeginInvoke(() =>
                {
                    var bestViewRectangle =
                        informationLayer.GetBestView(informationLayer.Items);
                    bestViewRectangle.MapControl = map;
                    map.Center = bestViewRectangle.Center;
                    map.ZoomLevel = bestViewRectangle.ZoomLevel;
                });
                timer.Dispose();
            }, null, 1000, 1000);
        }

        public static void CenterMapBasedOnIpInfo(this InformationLayer informationLayer)
        {
            var map = informationLayer.MapControl;

            var ipInformation = ((string)Application.Current.Resources["IPInformation"]);
            if (ipInformation == null) return;
            var ipInformationDelimited = ipInformation.Split(';');
            var latitude = Convert.ToDouble(ipInformationDelimited[8]);
            var longitude = Convert.ToDouble(ipInformationDelimited[9]);
            map.Center = new Telerik.Windows.Controls.Map.Location(latitude, longitude);
            map.ZoomLevel = 16;
        }
    }
}

using FoundOps.Common.Silverlight.Tools.ExtensionMethods;
using Telerik.Windows.Controls.Map;
using System;
using System.Linq;
using System.Windows;

namespace FoundOps.Common.Silverlight.Tools.Location
{
    public static class MapTools
    {
        private static IDisposable _setBestViewDisposable;
        public static void SetBestView(this InformationLayer informationLayer)
        {
            if (informationLayer.Items.Count <= 0) return;

            var map = informationLayer.MapControl;

            if (_setBestViewDisposable!=null)
            {
                _setBestViewDisposable.Dispose();
                _setBestViewDisposable = null;
            }

            //Wait 1/2 second (to let the PinPoints draw) then set the best view for the pinpoints
            _setBestViewDisposable = Rxx3.RunDelayed(TimeSpan.FromSeconds(.5), () =>
            {
                if (informationLayer.Items.Count <= 0)
                    return;

                if (informationLayer.Items.Count == 1)
                {
                    informationLayer.ScrollToCenterOfView(informationLayer.Items.First());
                    return;
                }

                var bestViewRectangle = informationLayer.GetBestView(informationLayer.Items);
                bestViewRectangle.MapControl = map;

                map.ZoomLevel = bestViewRectangle.ZoomLevel;
                map.Center = bestViewRectangle.Center;
            });
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

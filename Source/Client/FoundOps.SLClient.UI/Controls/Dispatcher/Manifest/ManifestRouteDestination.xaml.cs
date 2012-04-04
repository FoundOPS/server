using System;
using System.Net;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.SLClient.Data.Converters;
using FoundOps.SLClient.UI.Tools;

namespace FoundOps.SLClient.UI.Controls.Dispatcher.Manifest
{
    /// <summary>
    /// The UI for diplaying a RouteDestination in the manifest.
    /// </summary>
    public partial class ManifestRouteDestination
    {
        /// <summary>
        /// Occurs when the [image loaded].
        /// This is to allow proper Pdf generation.
        /// </summary>
        public event EventHandler ImageLoaded;

        #region RouteDestination Dependency Property

        /// <summary>
        /// RouteDestination
        /// </summary>
        public RouteDestination RouteDestination
        {
            get { return (RouteDestination)GetValue(RouteDestinationProperty); }
            set { SetValue(RouteDestinationProperty, value); }
        }

        /// <summary>
        /// RouteDestination Dependency Property.
        /// </summary>
        public static readonly DependencyProperty RouteDestinationProperty =
            DependencyProperty.Register(
                "RouteDestination",
                typeof(RouteDestination),
                typeof(ManifestRouteDestination),
                new PropertyMetadata(RouteDestinationChanged));

        private static void RouteDestinationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as ManifestRouteDestination;
            if (c == null) return;
            var routeDestination = e.NewValue as RouteDestination;

            Location location = null;
            if (routeDestination != null)
                location = routeDestination.Location;

            ////If the 2D Barcode is not going to be displayed call ImageLoaded and hide the BarcodeImage
            //if (!VM.Routes.RouteManifestVM.RouteManifestSettings.Is2DBarcodeVisible || 
            //    routeDestination == null || location == null || !location.Latitude.HasValue || !location.Longitude.HasValue)
            //{
            //    c.BarcodeImage.Visibility = Visibility.Collapsed;

            //    if (c.ImageLoaded != null)
            //        c.ImageLoaded(c, null);
            //}
            //else
            //{
            //Load the Image, and raise the ImageLoaded event when it is completed
            var client = new WebClient();

            var urlConverter = new LocationToUrlConverter();
            if (location == null)
            {
                if (c.ImageLoaded != null)
                    c.ImageLoaded(c, null);
                return;
            }

            client.OpenReadObservable((Uri)urlConverter.Convert(location, null, null, null)).ObserveOnDispatcher()
                .Subscribe(
                    stream =>
                    {
                        var image = new BitmapImage();
                        image.SetSource(stream);
                        c.BarcodeImage.Source = image;
                        if (c.ImageLoaded != null)
                            c.ImageLoaded(c, null);
                    });
            //}
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ManifestRouteDestination"/> class.
        /// </summary>
        public ManifestRouteDestination()
        {
            InitializeComponent();
        }
    }
}
using FoundOps.Core.Models.CoreEntities;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows;

namespace FoundOps.SLClient.UI.Controls.Dispatcher.Manifest
{
    /// <summary>
    /// The UI for diplaying a RouteDestination in the manifest.
    /// </summary>
    public partial class ManifestRouteDestination
    {
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
                new PropertyMetadata(new PropertyChangedCallback(RouteDestinationChanged)));

        private static void RouteDestinationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as ManifestRouteDestination;
            if (c == null) return;
            var routeDestination = e.NewValue as RouteDestination;

            //If the barcode image is null hide the barcode image
            if (routeDestination == null || routeDestination.Location == null || routeDestination.Location.BarcodeImage == null)
            {
                c.BarcodeImage.Visibility = Visibility.Collapsed;
                return;
            }

            c.SetImage(routeDestination.Location.BarcodeImage);
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ManifestRouteDestination"/> class.
        /// </summary>
        public ManifestRouteDestination()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Sets the barcode image
        /// </summary>
        private void SetImage(byte[] imageBytes)
        {
            var image = new BitmapImage();
            image.SetSource(new MemoryStream(imageBytes));
            BarcodeImage.Source = image;
        }
    }
}
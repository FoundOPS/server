using FoundOps.Core.Models.CoreEntities;
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
                new PropertyMetadata(null));

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
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using FoundOps.SLClient.UI.ViewModels;

namespace FoundOps.SLClient.UI.Controls.Dispatcher
{
    /// <summary>
    /// A filter for Dispatching
    /// </summary>
    public partial class Filter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Filter"/> class.
        /// </summary>
        public Filter()
        {
            InitializeComponent();
        }

        private RoutesVM RoutesVM
        {
            get { return (RoutesVM) this.DataContext; }
        }

        //Manually handle RouteType checked
        private void RouteTypeCheckBoxChecked(object sender, RoutedEventArgs e)
        {
            var routeTypeToAdd = (string)((CheckBox)sender).DataContext;

            if (routeTypeToAdd == null)
                return;
            
            if (!RoutesVM.SelectedRouteTypes.Any(c => c == routeTypeToAdd))
                RoutesVM.SelectedRouteTypes.Add(routeTypeToAdd);
        }

        //Manually handle RouteType unchecked
        private void RouteTypeCheckBoxUnchecked(object sender, RoutedEventArgs e)
        {
            var routeTypeToRemove = (string)((CheckBox)sender).DataContext;

            if (routeTypeToRemove == null)
                return;

            if (this.RoutesVM.SelectedRouteTypes.Any(c => c == routeTypeToRemove))
                RoutesVM.SelectedRouteTypes.Remove(routeTypeToRemove);
        }

        //Manually handle Region checked
        private void RegionsCheckBoxChecked(object sender, RoutedEventArgs e)
        {
            var regionToAdd = (string)((CheckBox)sender).DataContext;

            if (regionToAdd == null)
                return;

            if (!RoutesVM.SelectedRegions.Any(c => c == regionToAdd))
                RoutesVM.SelectedRegions.Add(regionToAdd);
        }

        //Manually handle Region unchecked
        private void RegionsCheckBoxUnchecked(object sender, RoutedEventArgs e)
        {
            var regionToRemove = (string)((CheckBox)sender).DataContext;

            if (regionToRemove == null)
                return;

            if (this.RoutesVM.SelectedRegions.Any(c => c == regionToRemove))
                RoutesVM.SelectedRegions.Remove(regionToRemove);
        }
    }
}

using System.Linq;
using System.Windows;
using System.Windows.Controls;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.SLClient.UI.Tools;
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


        //Manually handle RouteType checked
        private void RouteTypeCheckBoxChecked(object sender, RoutedEventArgs e)
        {
            var serviceTemplate = ((ServiceTemplate) ((Telerik.Windows.Controls.HeaderedItemsControl)(((Telerik.Windows.RadRoutedEventArgs)(e)).OriginalSource)).Header);
            VM.DispatcherFilter.SelectedRouteTypes.Add(serviceTemplate.Name);
        }

        //Manually handle RouteType unchecked
        private void RouteTypeCheckBoxUnchecked(object sender, RoutedEventArgs e)
        {
            var serviceTemplate = ((ServiceTemplate)((Telerik.Windows.Controls.HeaderedItemsControl)(((Telerik.Windows.RadRoutedEventArgs)(e)).OriginalSource)).Header);
            VM.DispatcherFilter.SelectedRouteTypes.Remove(serviceTemplate.Name);
        }

        //Manually handle Region checked
        private void RegionsCheckBoxChecked(object sender, RoutedEventArgs e)
        {
            var region = ((Region)((Telerik.Windows.Controls.HeaderedItemsControl)(((Telerik.Windows.RadRoutedEventArgs)(e)).OriginalSource)).Header);
            VM.DispatcherFilter.SelectedRegions.Remove(region.Name);
        }

        //Manually handle Region unchecked
        private void RegionsCheckBoxUnchecked(object sender, RoutedEventArgs e)
        {
            var region = ((Region)((Telerik.Windows.Controls.HeaderedItemsControl)(((Telerik.Windows.RadRoutedEventArgs)(e)).OriginalSource)).Header);
            VM.DispatcherFilter.SelectedRegions.Remove(region.Name);
        }
    }
}

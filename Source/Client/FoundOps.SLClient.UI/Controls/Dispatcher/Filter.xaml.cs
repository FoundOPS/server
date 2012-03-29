using FoundOps.SLClient.UI.Tools;

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

        private void RouteCapabilitiesTreeViewChecked(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            VM.DispatcherFilter.TriggerFilterUpdated();
        }

        private void RouteCapabilitiesTreeViewUnchecked(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            VM.DispatcherFilter.TriggerFilterUpdated();
        }

        private void RegionsTreeViewChecked(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            VM.DispatcherFilter.TriggerFilterUpdated();
        }

        private void RegionsTreeViewUnchecked(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            VM.DispatcherFilter.TriggerFilterUpdated();
        }
    }
}

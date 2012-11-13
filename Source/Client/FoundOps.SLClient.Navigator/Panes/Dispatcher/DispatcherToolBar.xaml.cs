using Analytics = FoundOps.SLClient.Data.Services.Analytics;
using System.Windows;

namespace FoundOps.SLClient.Navigator.Panes.Dispatcher
{
    /// <summary>
    /// The toolbar at the top of the dispatcher main page
    /// </summary>
    public partial class DispatcherToolBar
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DispatcherToolBar"/> class.
        /// </summary>
        public DispatcherToolBar()
        {
            InitializeComponent();
        }

        private void AddNewRouteClick(object sender, RoutedEventArgs routedEventArgs)
        {
            Analytics.Track("Add Route");
        }

        private void AutoCalcRoutesButtonClick(object sender, RoutedEventArgs e)
        {
            Analytics.Track("Auto Assign Jobs");
        }

        private void DateChanged(object sender, Telerik.Windows.Controls.SelectionChangedEventArgs e)
        {
            Analytics.Track("Change Date");
        }
    }
}

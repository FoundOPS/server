using System.Windows.Controls;
using FoundOps.SLClient.UI.ViewModels;
using FoundOps.Common.Silverlight.UI.Tools;

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

        private void AddNewRouteClick(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            //Analytics - Track when a new route is created
            TrackEventAction.Track("Dispatcher", "AddNewRoute", 1);
        }

        private void AddNewRouteTaskClick(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            //Analytics - Track when a new route task is created
            TrackEventAction.Track("Dispatcher", "AddNewRouteTask", 1);
        }

        private void AutoCalcRoutesButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            //Analytics - Set AutoAsignButtonClickedOn to true
            ((RoutesVM)DataContext).AutoAsignButtonClickedOn = (Button)sender;

            //Analytics - Track when a AutoAsignJobs button is clicked
            TrackEventAction.Track("Dispatcher", "AutoAsignJobs", 1);
        }
    }
}

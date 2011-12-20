using System.Windows.Controls;
using FoundOps.SLClient.Data.Services;
using FoundOps.SLClient.UI.ViewModels;

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
            Analytics.AddNewRoute();
        }

        private void AddNewRouteTaskClick(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            //Analytics - Track when a new route task is created
            Analytics.AddNewRouteTask();
        }

        private void AutoCalcRoutesButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            //Analytics - Set AutoAsignButtonClickedOn to true
            ((RoutesVM)DataContext).AutoAsignButtonClickedOn = (Button)sender;

            //Analytics - Track when a AutoAsignJobs button is clicked
            Analytics.AutoAsignJobs();
        }

        private void PreviousDayButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            //Analytics - Track when a AutoAsignJobs button is clicked
            Analytics.PreviousDay();
        }

        private void NextDayButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            //Analytics - Track when a NextDay button is clicked
            Analytics.NextDay();
        }
    }
}

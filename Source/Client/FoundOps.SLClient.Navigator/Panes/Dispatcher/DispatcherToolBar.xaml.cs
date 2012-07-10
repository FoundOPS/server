using Analytics = FoundOps.SLClient.Data.Services.Analytics;
using FoundOps.SLClient.Data.Services;
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
            //Analytics - Track when a new route is created
            Analytics.Track(Event.AddRoute);
        }

        private void AutoCalcRoutesButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            //Analytics - Track when a AutoAsignJobs button is clicked
            Analytics.Track(Event.AutoAssignJobs);
        }

        private void PreviousDayButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            //Analytics - Track when a AutoAsignJobs button is clicked
            Analytics.Track(Event.DispatcherPreviousDay);
        }

        private void NextDayButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            //Analytics - Track when a NextDay button is clicked
            Analytics.Track(Event.DispatcherNextDay);
        }

        private void DateChanged(object sender, Telerik.Windows.Controls.SelectionChangedEventArgs e)
        {
            //Analytics - Track when the date is changed with the RadDatePicker
            Analytics.Track(Event.DispatcherChangeDate);
        }
    }
}

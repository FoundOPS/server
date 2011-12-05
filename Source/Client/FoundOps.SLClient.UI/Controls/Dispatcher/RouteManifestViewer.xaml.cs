using System;
using System.Windows;
using System.Reactive.Linq;
using FoundOps.SLClient.UI.ViewModels;
using FoundOps.Common.Silverlight.UI.Tools;

namespace FoundOps.SLClient.UI.Controls.Dispatcher
{
    /// <summary>
    /// Displays RouteManifest
    /// </summary>
    public partial class RouteManifestViewer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RouteManifestViewer"/> class.
        /// </summary>
        public RouteManifestViewer()
        {
            InitializeComponent();

            //There is a problem when binding to Maximum
            Observable2.FromPropertyChangedPattern(this.RouteManifest, x => x.PageCount).SubscribeOnDispatcher()
                .Subscribe(pageCount => PageUpDown.Maximum = pageCount);
        }

        private RouteManifestVM RouteManifestVM
        {
            get { return ((RoutesVM)this.DataContext).RouteManifestVM; }
        }

        private void PrintManifest(object sender, RoutedEventArgs routedEventArgs)
        {
            RouteManifestVM.Printer.Print();

            #region Analytics
            
            //get current time
            var currentTimeOfDay = DateTime.Now.ToShortTimeString();

            //check for route manifest options
            if (RouteManifestVM.RouteManifestSettings.IsHeaderVisible)
            {
                TrackEventAction.Track("RouteManifest", "RouteManifestOptionSelected", "Header", 1);
                if (RouteManifestVM.RouteManifestSettings.IsRouteNameVisible)
                    TrackEventAction.Track("RouteManifest", "RouteManifestOptionSelected", "RouteName", 1);
                if (RouteManifestVM.RouteManifestSettings.IsRouteDateVisible)
                    TrackEventAction.Track("RouteManifest", "RouteManifestOptionSelected", "RouteDate", 1);
                if (RouteManifestVM.RouteManifestSettings.IsAssignedVehiclesVisible)
                    TrackEventAction.Track("RouteManifest", "RouteManifestOptionSelected", "AssignedVehicles", 1);
            }

            if (RouteManifestVM.RouteManifestSettings.IsSummaryVisible)
            {
                TrackEventAction.Track("RouteManifest", "RouteManifestOptionSelected", "Summary", 1);
                if (RouteManifestVM.RouteManifestSettings.IsRouteSummaryVisible)
                    TrackEventAction.Track("RouteManifest", "RouteManifestOptionSelected", "RouteSummary", 1);
                if (RouteManifestVM.RouteManifestSettings.IsScheduledStartTimeVisible)
                    TrackEventAction.Track("RouteManifest", "RouteManifestOptionSelected", "StartTime", 1);
                if (RouteManifestVM.RouteManifestSettings.IsScheduledEndTimeVisible)
                    TrackEventAction.Track("RouteManifest", "RouteManifestOptionSelected", "EndTime", 1);
                if (RouteManifestVM.RouteManifestSettings.IsDestinationsSummaryVisible)
                    TrackEventAction.Track("RouteManifest", "RouteManifestOptionSelected", "DestinationsSummary", 1);
                if (RouteManifestVM.RouteManifestSettings.IsNumberofDestinationsVisible)
                    TrackEventAction.Track("RouteManifest", "RouteManifestOptionSelected", "NumberofDestinations", 1);
                if (RouteManifestVM.RouteManifestSettings.IsTaskSummaryVisible)
                    TrackEventAction.Track("RouteManifest", "RouteManifestOptionSelected", "TaskSummary", 1);
                if (RouteManifestVM.RouteManifestSettings.IsNumberOfTasksVisible)
                    TrackEventAction.Track("RouteManifest", "RouteManifestOptionSelected", "NumberOfTasks", 1);
            }

            if (RouteManifestVM.RouteManifestSettings.IsDestinationsVisible)
            {
                TrackEventAction.Track("RouteManifest", "RouteManifestOptionSelected", "Destinations", 1);
                if (RouteManifestVM.RouteManifestSettings.IsAddressVisible)
                    TrackEventAction.Track("RouteManifest", "RouteManifestOptionSelected", "Address", 1);
                if (RouteManifestVM.RouteManifestSettings.IsContactInfoVisible)
                    TrackEventAction.Track("RouteManifest", "RouteManifestOptionSelected", "ContactInfo", 1);
                if (RouteManifestVM.RouteManifestSettings.IsRouteTasksVisible)
                    TrackEventAction.Track("RouteManifest", "RouteManifestOptionSelected", "RouteTasks", 1);
                if (RouteManifestVM.RouteManifestSettings.Is2DBarcodeVisible)
                    TrackEventAction.Track("RouteManifest", "RouteManifestOptionSelected", "2DBarcode", 1);
            }

            if (RouteManifestVM.RouteManifestSettings.IsFooterVisible)
            {
                TrackEventAction.Track("RouteManifest", "RouteManifestOptionSelected", "Footer", 1);
                if (RouteManifestVM.RouteManifestSettings.IsPageNumbersVisible)
                    TrackEventAction.Track("RouteManifest", "RouteManifestOptionSelected", "PageNumbers", 1);
                if (RouteManifestVM.RouteManifestSettings.IsCustomMessageVisible)
                    TrackEventAction.Track("RouteManifest", "RouteManifestOptionSelected", "CustomMessage", 1);
            }

            //Analytics - Track when manifests are printed
            TrackEventAction.Track("RouteManifest", "RouteManifestPrinted", "Time: " + currentTimeOfDay, 1);

            #endregion
        }

        private void MyRouteManifestViewerClosed(object sender, System.EventArgs e)
        {
            //Dispose of Printer
            RouteManifestVM.Printer = null;

            //On closing the manifest: update and save the route manifest settings
            RouteManifestVM.UpdateSaveRouteManifestSettings();
        }
    }
}

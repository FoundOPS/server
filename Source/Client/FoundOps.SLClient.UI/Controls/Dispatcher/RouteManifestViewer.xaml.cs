using System;
using System.Windows;
using FoundOps.SLClient.UI.ViewModels;
using FoundOps.SLClient.UI.Controls.Dispatcher.Manifest;

namespace FoundOps.SLClient.UI.Controls.Dispatcher
{
    /// <summary>
    /// Displays RouteManifest
    /// </summary>
    public partial class RouteManifestViewer
    {
        private int _pageIndex;

        /// <summary>
        /// 
        /// </summary>
        public int PageIndex
        {
            get { return _pageIndex; }
            set
            {
                if (_routeManifest.Pages.Count <= _pageIndex + 1) return;

                _pageIndex = value;
                this.CurrentManifestPageViewbox.Child = _routeManifest.Pages[PageIndex];
            }
        }

        private readonly RouteManifest _routeManifest;

        /// <summary>
        /// Initializes a new instance of the <see cref="RouteManifestViewer"/> class.
        /// </summary>
        public RouteManifestViewer()
        {
            this.Loaded += (s, e) =>
            {
                _routeManifest.UpdateControl();
                this.PageIndex = 0;
            };

            _routeManifest = new RouteManifest {PrintedHeight = 1056, PrintedWidth = 816};

            InitializeComponent();

            ////There is a problem when binding to Maximum
            //Observable2.FromPropertyChangedPattern(this.RouteManifest, x => x.PageCount).SubscribeOnDispatcher()
            //    .Subscribe(pageCount => PageUpDown.Maximum = pageCount);
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
                Data.Services.Analytics.Header();
                if (RouteManifestVM.RouteManifestSettings.IsRouteNameVisible)
                    Data.Services.Analytics.RouteName();
                if (RouteManifestVM.RouteManifestSettings.IsRouteDateVisible)
                    Data.Services.Analytics.RouteDate();
                if (RouteManifestVM.RouteManifestSettings.IsAssignedVehiclesVisible)
                    Data.Services.Analytics.AssignedVehicles();
            }

            if (RouteManifestVM.RouteManifestSettings.IsSummaryVisible)
            {
                Data.Services.Analytics.Summary();
                if (RouteManifestVM.RouteManifestSettings.IsRouteSummaryVisible)
                    Data.Services.Analytics.RouteSummary();
                if (RouteManifestVM.RouteManifestSettings.IsScheduledStartTimeVisible)
                    Data.Services.Analytics.StartTime();
                if (RouteManifestVM.RouteManifestSettings.IsScheduledEndTimeVisible)
                    Data.Services.Analytics.EndTime();
                if (RouteManifestVM.RouteManifestSettings.IsDestinationsSummaryVisible)
                    Data.Services.Analytics.DestinationsSummary();
                if (RouteManifestVM.RouteManifestSettings.IsNumberofDestinationsVisible)
                    Data.Services.Analytics.NumberofDestinations();
                if (RouteManifestVM.RouteManifestSettings.IsTaskSummaryVisible)
                    Data.Services.Analytics.TaskSummary();
                if (RouteManifestVM.RouteManifestSettings.IsNumberOfTasksVisible)
                    Data.Services.Analytics.NumberOfTasks();
            }

            if (RouteManifestVM.RouteManifestSettings.IsDestinationsVisible)
            {
                Data.Services.Analytics.Destinations();
                if (RouteManifestVM.RouteManifestSettings.IsAddressVisible)
                    Data.Services.Analytics.Address();
                if (RouteManifestVM.RouteManifestSettings.IsContactInfoVisible)
                    Data.Services.Analytics.ContactInfo();
                if (RouteManifestVM.RouteManifestSettings.IsRouteTasksVisible)
                    Data.Services.Analytics.RouteTasks();
                if (RouteManifestVM.RouteManifestSettings.Is2DBarcodeVisible)
                    Data.Services.Analytics.Barcode();
            }

            if (RouteManifestVM.RouteManifestSettings.IsFooterVisible)
            {
                Data.Services.Analytics.Footer();
                if (RouteManifestVM.RouteManifestSettings.IsPageNumbersVisible)
                    Data.Services.Analytics.PageNumbers();
                if (RouteManifestVM.RouteManifestSettings.IsCustomMessageVisible)
                    Data.Services.Analytics.CustomMessage();
            }

            //Analytics - Track when manifests are printed
            Data.Services.Analytics.RouteManifestPrinted(currentTimeOfDay);

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

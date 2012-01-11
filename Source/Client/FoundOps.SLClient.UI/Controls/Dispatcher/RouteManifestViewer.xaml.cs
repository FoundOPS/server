using System;
using System.Reactive.Linq;
using System.Windows;
using System.ComponentModel;
using FoundOps.Common.Silverlight.UI.Controls.Printing;
using FoundOps.SLClient.UI.Tools;
using FoundOps.SLClient.UI.Controls.Dispatcher.Manifest;

namespace FoundOps.SLClient.UI.Controls.Dispatcher
{
    /// <summary>
    /// Displays RouteManifest
    /// </summary>
    public partial class RouteManifestViewer : INotifyPropertyChanged, IPagedViewer
    {
        #region Public Events and Properties

        // Implementation of INotifyPropertyChanged
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #region Implementation of IPagedViewer

        public bool IsFirstPage { get { return PageIndex == 0; } }

        public bool IsLastPage { get { return PageIndex == this.RouteManifest.PageCount - 1; } }

        private int _pageIndex;
        /// <summary>
        /// The current Manifest page.
        /// </summary>
        public int PageIndex
        {
            get { return _pageIndex; }
            set
            {
                //If there are not enough pages, return
                if (RouteManifest.PageCount < value + 1) return;

                _pageIndex = value;
                this.CurrentManifestPageViewboxGrid.Children.Add(RouteManifest.Pages[PageIndex]);

                this.CurrentManifestPageViewbox.UpdateLayout();

                RaisePropertyChanged("PageIndex");
                RaisePropertyChanged("IsFirstPage");
                RaisePropertyChanged("IsLastPage");
            }
        }

        #endregion

        /// <summary>
        /// The route manifest.
        /// </summary>
        public RouteManifest RouteManifest { get; private set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="RouteManifestViewer"/> class.
        /// </summary>
        public RouteManifestViewer()
        {
            RouteManifest = new RouteManifest { PrintedHeight = 1056, PrintedWidth = 816 };

            //Update the control every time the manifest viewer is loaded because the routes might have changed
            //Then set this paged viewer to the first page
            this.Loaded += (s, e) => RouteManifest.ForceUpdate(() => this.PageIndex = 0);

            InitializeComponent();

            ////There is a problem when binding to Maximum
            Observable2.FromPropertyChangedPattern(this.RouteManifest, x => x.PageCount).SubscribeOnDispatcher()
                .Subscribe(pageCount => PageUpDown.Maximum = pageCount);

            //Set the RouteManifestVM's Printer
            VM.RouteManifest.Printer = RouteManifest;
            VM.RouteManifest.Viewer = this;
        }

        #region Logic

        #region Implementation of INotifyPropertyChanged

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        private void MyRouteManifestViewerClosed(object sender, System.EventArgs e)
        {
            //Dispose of Printer
            VM.RouteManifest.Printer = null;
            VM.RouteManifest.Viewer = null;

            //On closing the manifest: update and save the route manifest settings
            VM.RouteManifest.UpdateSaveRouteManifestSettings();
        }

        private void PrintManifest(object sender, RoutedEventArgs routedEventArgs)
        {
            VM.RouteManifest.Printer.Print();

            #region Analytics

            //get current time
            var currentTimeOfDay = DateTime.Now.ToShortTimeString();

            //check for route manifest options
            if (VM.RouteManifest.RouteManifestSettings.IsHeaderVisible)
            {
                Data.Services.Analytics.Header();
                if (VM.RouteManifest.RouteManifestSettings.IsRouteNameVisible)
                    Data.Services.Analytics.RouteName();
                if (VM.RouteManifest.RouteManifestSettings.IsRouteDateVisible)
                    Data.Services.Analytics.RouteDate();
                if (VM.RouteManifest.RouteManifestSettings.IsAssignedVehiclesVisible)
                    Data.Services.Analytics.AssignedVehicles();
            }

            if (VM.RouteManifest.RouteManifestSettings.IsSummaryVisible)
            {
                Data.Services.Analytics.Summary();
                if (VM.RouteManifest.RouteManifestSettings.IsRouteSummaryVisible)
                    Data.Services.Analytics.RouteSummary();
                if (VM.RouteManifest.RouteManifestSettings.IsScheduledStartTimeVisible)
                    Data.Services.Analytics.StartTime();
                if (VM.RouteManifest.RouteManifestSettings.IsScheduledEndTimeVisible)
                    Data.Services.Analytics.EndTime();
                if (VM.RouteManifest.RouteManifestSettings.IsDestinationsSummaryVisible)
                    Data.Services.Analytics.DestinationsSummary();
                if (VM.RouteManifest.RouteManifestSettings.IsNumberofDestinationsVisible)
                    Data.Services.Analytics.NumberofDestinations();
                if (VM.RouteManifest.RouteManifestSettings.IsTaskSummaryVisible)
                    Data.Services.Analytics.TaskSummary();
                if (VM.RouteManifest.RouteManifestSettings.IsNumberOfTasksVisible)
                    Data.Services.Analytics.NumberOfTasks();
            }

            if (VM.RouteManifest.RouteManifestSettings.IsDestinationsVisible)
            {
                Data.Services.Analytics.Destinations();
                if (VM.RouteManifest.RouteManifestSettings.IsAddressVisible)
                    Data.Services.Analytics.Address();
                if (VM.RouteManifest.RouteManifestSettings.IsContactInfoVisible)
                    Data.Services.Analytics.ContactInfo();
                if (VM.RouteManifest.RouteManifestSettings.IsRouteTasksVisible)
                    Data.Services.Analytics.RouteTasks();
                if (VM.RouteManifest.RouteManifestSettings.Is2DBarcodeVisible)
                    Data.Services.Analytics.Barcode();
            }

            if (VM.RouteManifest.RouteManifestSettings.IsFooterVisible)
            {
                Data.Services.Analytics.Footer();
                if (VM.RouteManifest.RouteManifestSettings.IsPageNumbersVisible)
                    Data.Services.Analytics.PageNumbers();
                if (VM.RouteManifest.RouteManifestSettings.IsCustomMessageVisible)
                    Data.Services.Analytics.CustomMessage();
            }

            //Analytics - Track when manifests are printed
            Data.Services.Analytics.RouteManifestPrinted(currentTimeOfDay);

            #endregion
        }

        #endregion
    }
}

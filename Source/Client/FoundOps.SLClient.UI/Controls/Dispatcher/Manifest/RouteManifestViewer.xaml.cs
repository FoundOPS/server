using System;
using System.Windows;
using System.Reactive.Linq;
using FoundOps.Common.Tools;
using System.Windows.Controls;
using FoundOps.SLClient.UI.Tools;
using Telerik.Windows.Documents.UI;
using Telerik.Windows.Documents.Model;
using Telerik.Windows.Documents.Layout;
using Telerik.Windows.Documents.FormatProviders.Pdf;

namespace FoundOps.SLClient.UI.Controls.Dispatcher.Manifest
{
    /// <summary>
    /// Displays RouteManifest
    /// </summary>
    public partial class RouteManifestViewer
    {
        #region Locals

        //Keeps track if the current window is open
        private bool _isOpen;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="RouteManifestViewer"/> class.
        /// </summary>
        public RouteManifestViewer()
        {
            InitializeComponent();

            //Update the Manifest when
            //a) the SelectedEntity changes
            //b) the RouteManifestSettings properties change
            VM.Routes.SelectedEntityObservable.AsGeneric()
                .Merge(VM.RouteManifest.RouteManifestSettings.FromAnyPropertyChanged().AsGeneric().Throttle(TimeSpan.FromSeconds(.75)))
              .Throttle(TimeSpan.FromMilliseconds(200)).ObserveOnDispatcher()
                //Update the Manifest
              .Subscribe(a => UpdateDocument());

            //Update the manifest when this is opened
            this.Loaded += (s, e) =>
            {
                this._isOpen = true;
                UpdateDocument();
            };
            this.Closed += (s, e) => _isOpen = false;

            //this.ManifestRichTextBox.CurrentVisiblePageChanged+= Update textbox
        }

        #region Logic

        private void UpdateDocument()
        {
            //Only load the manifest when the current viewer is open
            if (!_isOpen) return;

            var bodyParagraph = new Paragraph();

            var margin = new Thickness(25, 2, 0, 0);
            //If there is a selected route setup the manifest
            if (VM.Routes.SelectedEntity != null)
            {
                //Keep track of the images loaded

                #region Add the Header

                if (VM.RouteManifest.RouteManifestSettings.IsHeaderVisible)
                {
                    //We only want the header to show up once, so just add it first to the body paragraph.
                    var manifestHeader = new ManifestHeader { Margin = new Thickness(350, 0, 0, 0) };
                    manifestHeader.Measure(new Size(double.MaxValue, double.MaxValue));

                    bodyParagraph.Inlines.Add(new InlineUIContainer
                    {
                        Height = manifestHeader.DesiredSize.Height,
                        Width = manifestHeader.DesiredSize.Width,
                        UiElement = manifestHeader
                    });
                }

                #endregion

                #region Add the Summary

                if (VM.RouteManifest.RouteManifestSettings.IsSummaryVisible)
                {
                    //We only want the summary to show up once, so just add it first to the body paragraph.
                    var manifestSummary = new ManifestSummary { Margin = margin };
                    manifestSummary.Measure(new Size(double.MaxValue, double.MaxValue));

                    bodyParagraph.Inlines.Add(new InlineUIContainer
                    {
                        Height = manifestSummary.DesiredSize.Height,
                        Width = manifestSummary.DesiredSize.Width,
                        UiElement = manifestSummary
                    });
                }

                #endregion

                #region Add the RouteDestinations

                foreach (var routeDestination in VM.Routes.SelectedEntity.RouteDestinationsListWrapper)
                {
                    var manifestRouteDestination = new ManifestRouteDestination { Margin = margin, RouteDestination = routeDestination };
                    manifestRouteDestination.Measure(new Size(double.MaxValue, double.MaxValue));

                    var container = new InlineUIContainer
                    {
                        Height = manifestRouteDestination.DesiredSize.Height,
                        Width = manifestRouteDestination.DesiredSize.Width,
                        UiElement = manifestRouteDestination
                    };

                    bodyParagraph.Inlines.Add(container);
                }

                #endregion

                ////Fixup the layout in half a second
                //Observable.Interval(TimeSpan.FromMilliseconds(500)).Take(1).SubscribeOnDispatcher().Subscribe(_ => ManifestRichTextBox.UpdateLayout());
            }

            ////Setup the main section
            var mainSection = new Section { FooterBottomMargin = 0, HeaderTopMargin = 0, ActualPageMargin = new Padding(0) };
            mainSection.Blocks.Add(bodyParagraph);

            //Setup the Document
            var document = new RadDocument { LayoutMode = DocumentLayoutMode.Paged };
            document.Sections.Add(mainSection);

            ManifestRichTextBox.Document = document;
        }

        private void MyRouteManifestViewerClosed(object sender, System.EventArgs e)
        {
            //On closing the manifest: update and save the route manifest settings
            VM.RouteManifest.UpdateSaveRouteManifestSettings();
        }

        //#region Analytics

        ////get current time
        //var currentTimeOfDay = DateTime.Now.ToShortTimeString();

        ////check for route manifest options
        //if (VM.RouteManifest.RouteManifestSettings.IsHeaderVisible)
        //{
        //    Data.Services.Analytics.Header();
        //    if (VM.RouteManifest.RouteManifestSettings.IsRouteNameVisible)
        //        Data.Services.Analytics.RouteName();
        //    if (VM.RouteManifest.RouteManifestSettings.IsRouteDateVisible)
        //        Data.Services.Analytics.RouteDate();
        //    if (VM.RouteManifest.RouteManifestSettings.IsAssignedVehiclesVisible)
        //        Data.Services.Analytics.AssignedVehicles();
        //}

        //if (VM.RouteManifest.RouteManifestSettings.IsSummaryVisible)
        //{
        //    Data.Services.Analytics.Summary();
        //    if (VM.RouteManifest.RouteManifestSettings.IsRouteSummaryVisible)
        //        Data.Services.Analytics.RouteSummary();
        //    if (VM.RouteManifest.RouteManifestSettings.IsScheduledStartTimeVisible)
        //        Data.Services.Analytics.StartTime();
        //    if (VM.RouteManifest.RouteManifestSettings.IsScheduledEndTimeVisible)
        //        Data.Services.Analytics.EndTime();
        //    if (VM.RouteManifest.RouteManifestSettings.IsDestinationsSummaryVisible)
        //        Data.Services.Analytics.DestinationsSummary();
        //    if (VM.RouteManifest.RouteManifestSettings.IsNumberofDestinationsVisible)
        //        Data.Services.Analytics.NumberofDestinations();
        //    if (VM.RouteManifest.RouteManifestSettings.IsTaskSummaryVisible)
        //        Data.Services.Analytics.TaskSummary();
        //    if (VM.RouteManifest.RouteManifestSettings.IsNumberOfTasksVisible)
        //        Data.Services.Analytics.NumberOfTasks();
        //}

        //if (VM.RouteManifest.RouteManifestSettings.IsDestinationsVisible)
        //{
        //    Data.Services.Analytics.Destinations();
        //    if (VM.RouteManifest.RouteManifestSettings.IsAddressVisible)
        //        Data.Services.Analytics.Address();
        //    if (VM.RouteManifest.RouteManifestSettings.IsContactInfoVisible)
        //        Data.Services.Analytics.ContactInfo();
        //    if (VM.RouteManifest.RouteManifestSettings.IsRouteTasksVisible)
        //        Data.Services.Analytics.RouteTasks();
        //    if (VM.RouteManifest.RouteManifestSettings.Is2DBarcodeVisible)
        //        Data.Services.Analytics.Barcode();
        //}

        //if (VM.RouteManifest.RouteManifestSettings.IsFooterVisible)
        //{
        //    Data.Services.Analytics.Footer();
        //    if (VM.RouteManifest.RouteManifestSettings.IsPageNumbersVisible)
        //        Data.Services.Analytics.PageNumbers();
        //    if (VM.RouteManifest.RouteManifestSettings.IsCustomMessageVisible)
        //        Data.Services.Analytics.CustomMessage();
        //}

        ////Analytics - Track when manifests are printed
        //Data.Services.Analytics.RouteManifestPrinted(currentTimeOfDay);

        //#endregion

        #endregion

        private string GetFileName()
        {
            return String.Format("{0} Manifest", VM.Routes.SelectedEntity != null ? VM.Routes.SelectedEntity.Name : "");
        }

        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            //TODO: Need to scroll through to force generation of items controls

            //Open a SaveFileDialog to let the user save the document as a PDF
            var saveDialog = new SaveFileDialog { DefaultExt = ".pdf", Filter = "PDF|*.pdf", DefaultFileName = GetFileName() + ".pdf" };

            var dialogResult = saveDialog.ShowDialog();
            if (dialogResult != true) return;

            var provider = new PdfFormatProvider();
            using (var output = saveDialog.OpenFile())
                provider.Export(this.ManifestRichTextBox.Document, output);
        }

        private void BtnPrint_OnClick(object sender, RoutedEventArgs e)
        {
            ManifestRichTextBox.Print(GetFileName(), PrintMode.Native);
        }
    }
}
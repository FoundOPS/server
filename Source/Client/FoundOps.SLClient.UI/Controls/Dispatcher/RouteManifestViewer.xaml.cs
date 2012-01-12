using System;
using System.IO;
using System.Windows;
using System.Reactive.Linq;
using FoundOps.Common.Tools;
using System.Reactive.Subjects;
using FoundOps.SLClient.UI.Tools;
using Telerik.Windows.Documents.Model;
using Telerik.Windows.Documents.Fixed;
using Telerik.Windows.Documents.Layout;
using Telerik.Windows.Documents.FormatProviders.Pdf;
using FoundOps.SLClient.UI.Controls.Dispatcher.Manifest;

namespace FoundOps.SLClient.UI.Controls.Dispatcher
{
    /// <summary>
    /// Displays RouteManifest
    /// </summary>
    public partial class RouteManifestViewer
    {
        //Used to update the Pdf
        private readonly Subject<bool> _updatePdfObservable = new Subject<bool>();

        /// <summary>
        /// Initializes a new instance of the <see cref="RouteManifestViewer"/> class.
        /// </summary>
        public RouteManifestViewer()
        {
            InitializeComponent();

            //Update the Manifest when
            //a) the SelectedEntity changes & now
            //b) the RouteManifestSettings properties change
            VM.Routes.SelectedEntityObservable.AsGeneric().AndNow()
                .Merge(VM.RouteManifest.RouteManifestSettings.FromAnyPropertyChanged().AsGeneric().Throttle(TimeSpan.FromSeconds(.75)))
              .Throttle(TimeSpan.FromMilliseconds(200)).ObserveOnDispatcher()
                //Update the Manifest
              .Subscribe(a => UpdateDocument());

            _updatePdfObservable.Throttle(TimeSpan.FromSeconds(.25)).ObserveOnDispatcher().Subscribe(_ => UpdatePdf());
        }

        #region Logic

        private void UpdateDocument()
        {
            var doc = new RadDocument { LayoutMode = DocumentLayoutMode.Paged };



            //Keep track of the images loaded
            var routeDestinationImagesCompleted = new BehaviorSubject<int>(0);

            var section = new Section { FooterBottomMargin = 0, HeaderTopMargin = 0, ActualPageMargin = new Padding(0) };
            doc.Sections.Add(section);

            var paragraph = new Paragraph { LeftIndent = 0, RightIndent = 0 };
            section.Blocks.Add(paragraph);

            //If there is a selected route setup the manifest
            if (VM.Routes.SelectedEntity != null)
            {
                //Update the Pdf when all the images are loaded
                routeDestinationImagesCompleted.Where(imageLoaded => imageLoaded == VM.Routes.SelectedEntity.RouteDestinations.Count)
                    .Subscribe(_ => _updatePdfObservable.OnNext(true));

                //Add the header
                var header = new ManifestHeader();
                header.Measure(new Size(double.MaxValue, double.MaxValue));

                paragraph.Inlines.Add(new InlineUIContainer
                {
                    Height = header.DesiredSize.Height,
                    Width = header.DesiredSize.Width,
                    UiElement = header
                });

                foreach (var routeDestination in VM.Routes.SelectedEntity.RouteDestinationsListWrapper)
                {
                    var manifestRouteDestination = new ManifestRouteDestination();

                    //Whenever the image is loaded increment routeDestinationImagesCompleted
                    Observable.FromEventPattern<EventArgs>(manifestRouteDestination, "ImageLoaded").AsGeneric()
                        .Subscribe(_ => routeDestinationImagesCompleted.OnNext(routeDestinationImagesCompleted.First() + 1));

                    manifestRouteDestination.RouteDestination = routeDestination;

                    manifestRouteDestination.Measure(new Size(double.MaxValue, double.MaxValue));

                    var container = new InlineUIContainer
                    {
                        Height = manifestRouteDestination.DesiredSize.Height,
                        Width = manifestRouteDestination.DesiredSize.Width,
                        UiElement = manifestRouteDestination
                    };

                    paragraph.Inlines.Add(container);
                }
            }

            ////Add the footers
            //foreach (var sectionToAddFooterTo in doc.Sections)
            //{
            //    var manifestFooter = new ManifestFooter();
            //    manifestFooter.Measure(new Size(double.MaxValue, double.MaxValue));

            //    var footerParagraph = new Paragraph();
            //    footerParagraph.Inlines.Add(new InlineUIContainer
            //    {
            //        Height = manifestFooter.DesiredSize.Height,
            //        Width = manifestFooter.DesiredSize.Width,
            //        UiElement = manifestFooter
            //    });

            //    sectionToAddFooterTo.Children.Add(footerParagraph);
            //    var footerDocument = new RadDocument();

            //    var footer = new Footer { Body = footerDocument };
            //    sectionToAddFooterTo.Footers.Default = footer;
            //}

            ManifestRichTextBox.Document = doc;

            //Update the Pdf right away if there is not a selected Route);
            if (VM.Routes.SelectedEntity == null)
                _updatePdfObservable.OnNext(true);
        }

        /// <summary>
        /// Updates the PDF.
        /// </summary>
        private void UpdatePdf()
        {
            //Scroll to bottom to force all itemscontrols to generate
            ManifestRichTextBox.Document.CaretPosition.MoveToLastPositionInDocument();

            //Give it time to draw the controls
            Observable.Interval(TimeSpan.FromSeconds(1)).Take(1).ObserveOnDispatcher().Subscribe(_ =>
            {
                var outputStream = new MemoryStream();
                var pdfFormatProvider = new PdfFormatProvider();
                pdfFormatProvider.Export(ManifestRichTextBox.Document, outputStream);

                ManifestPdfViewer.DocumentSource = new PdfDocumentSource(outputStream);
                ManifestPdfViewer.DocumentSource.Loaded += (s, e) => outputStream.Close();
            });
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
    }
}
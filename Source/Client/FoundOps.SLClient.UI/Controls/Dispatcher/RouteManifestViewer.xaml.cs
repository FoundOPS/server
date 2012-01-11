using System;
using System.IO;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using FoundOps.Common.Silverlight.UI.Tools.ExtensionMethods;
using FoundOps.Common.Tools;
using FoundOps.SLClient.Data.Models;
using FoundOps.SLClient.UI.Tools;
using FoundOps.SLClient.UI.Controls.Dispatcher.Manifest;
using Telerik.Windows.Documents.Fixed;
using Telerik.Windows.Documents.FormatProviders.Pdf;
using Telerik.Windows.Documents.Model;

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

            //Update the Manifest when
            //a) the SelectedEntity changes
            //b) RouteManifestSettings changes
            //c) this first load

            //a) the SelectedEntity changes
            VM.Routes.SelectedEntityObservable.AsGeneric().Merge(
                //b) RouteManifestSettings changes
              VM.RouteManifest.RouteManifestSettings.FromAnyPropertyChanged().AsGeneric())
                //c) this first load
              .AndNow()
              .Throttle(TimeSpan.FromMilliseconds(200)).ObserveOnDispatcher()
                //Update the Manifest
              .Subscribe(a => UpdatePDF());
        }

        #region Logic

        private void UpdatePDF()
        {
            var doc = new RadDocument();

            var section = new Section { HeaderTopMargin = 0, FooterBottomMargin = 0 };
            doc.Sections.Add(section);

            var paragraph = new Paragraph { LeftIndent = 0, RightIndent = 0 };
            section.Blocks.Add(paragraph);

            if (VM.Routes.SelectedEntity != null)
            {
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
                    var manifestRouteDestination = new ManifestRouteDestination { RouteDestination = routeDestination };
                    manifestRouteDestination.Measure(new Size(double.MaxValue, double.MaxValue));

                    var container = new InlineUIContainer
                    {
                        Height = manifestRouteDestination.DesiredSize.Height,
                        Width = manifestRouteDestination.DesiredSize.Width,
                        UiElement = manifestRouteDestination
                    };

                    paragraph.Inlines.Add(container);
                }

                var footer = new ManifestFooter();
                footer.Measure(new Size(double.MaxValue, double.MaxValue));

                paragraph.Inlines.Add(new InlineUIContainer
                {
                    Height = footer.DesiredSize.Height,
                    Width = footer.DesiredSize.Width,
                    UiElement = footer
                });
            }
            var outputStream = new MemoryStream();
            var pdfFormatProvider = new PdfFormatProvider();
            pdfFormatProvider.Export(doc, outputStream);

            this.PDFViewer.DocumentSource = new PdfDocumentSource(outputStream);
            this.PDFViewer.DocumentSource.Loaded += (s, e) => outputStream.Close();
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

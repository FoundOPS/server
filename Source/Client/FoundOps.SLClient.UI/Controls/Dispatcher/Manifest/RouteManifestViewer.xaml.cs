using System;
using System.Linq;
using System.Windows;
using System.Globalization;
using System.Reactive.Linq;
using System.Windows.Media;
using FoundOps.Common.Tools;
using System.Windows.Controls;
using FoundOps.SLClient.UI.Tools;
using Telerik.Windows.Documents;
using Telerik.Windows.Documents.DocumentStructure;
using Telerik.Windows.Documents.UI;
using Telerik.Windows.Documents.Model;
using Telerik.Windows.Documents.Layout;
using Telerik.Windows.Documents.FormatProviders.Pdf;
using Border = Telerik.Windows.Documents.Model.Border;

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
        private double _headerHeight;
        private readonly Data.Models.RouteManifestSettings _settings = VM.RouteManifest.RouteManifestSettings;

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
            Loaded += (s, e) =>
            {
                _isOpen = true;
                UpdateDocument();
            };
            Closed += (s, e) => _isOpen = false;

            //this.ManifestRichTextBox.CurrentVisiblePageChanged+= Update textbox
        }

        #region Logic

        private void UpdateDocument()
        {
            //Only load the manifest when the current viewer is open
            if (!_isOpen) return;

            var document = new RadDocument { LayoutMode = DocumentLayoutMode.Paged };
            var mainSection = new Section { FooterBottomMargin = 0, HeaderTopMargin = 0, ActualPageMargin = new Padding(0, 20, 0, 20) };
            var bodyParagraph = new Paragraph();

            //If there is a selected route setup the manifest
            if (VM.Routes.SelectedEntity != null)
            {
                //Keep track of the images loaded

                #region Add the Header

                if (_settings.IsHeaderVisible)
                {
                    //We only want the header to show up once, so just add it first to the body paragraph.
                    var manifestHeader = new ManifestHeader { Margin = new Thickness(0, 0, 0, 35) };
                    manifestHeader.Measure(new Size(double.MaxValue, double.MaxValue));

                    bodyParagraph.Inlines.Add(new InlineUIContainer
                    {
                        Height = manifestHeader.DesiredSize.Height,
                        Width = manifestHeader.DesiredSize.Width,
                        UiElement = manifestHeader
                    });

                    var numVehicles = VM.Routes.SelectedEntity.Vehicles.Count;
                    var numTechnicians = VM.Routes.SelectedEntity.Technicians.Count;
                    if(numVehicles >= numTechnicians)
                    {
                        if (_settings.IsAssignedVehiclesVisible)
                        {
                            _headerHeight = numVehicles * 16;
                        }
                        else if (_settings.IsAssignedTechniciansVisible)
                        {
                            _headerHeight = numTechnicians * 16;
                        }
                        else
                            _headerHeight = 0;
                    }
                    else
                    {
                        if (_settings.IsAssignedTechniciansVisible)
                        {
                            _headerHeight = numTechnicians * 16;
                        }
                        else if (_settings.IsAssignedVehiclesVisible)
                        {
                            _headerHeight = numVehicles * 16;
                        }
                        else
                            _headerHeight = 0;
                    }
                }
                else
                {
                    _headerHeight = 0;
                }

                #endregion

                #region Add the Summary

                if (_settings.IsSummaryVisible)
                {
                    //We only want the summary to show up once, so just add it first to the body paragraph.
                    var manifestSummary = new ManifestSummary { Margin = new Thickness(25, _headerHeight, 0, 0) };
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
                    var manifestRouteDestination = new ManifestRouteDestination { Margin = new Thickness(25, 2, 0, 10), RouteDestination = routeDestination };
                    if(routeDestination.OrderInRoute == 1)
                        manifestRouteDestination = new ManifestRouteDestination { Margin = new Thickness(25, 2, 0, 26), RouteDestination = routeDestination };
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

                //Fixup the layout in half a second
                //Observable.Interval(TimeSpan.FromMilliseconds(500)).Take(1).SubscribeOnDispatcher().Subscribe(_ => ManifestRichTextBox.UpdateLayout());
            }

            if (_settings.IsFooterVisible)
            {
                mainSection.Footers.Default = GetFooterDocument();
            }
            
            ////Setup the main section
            mainSection.Blocks.Add(bodyParagraph);

            //Setup the Document
            document.Sections.Add(mainSection);

            ManifestRichTextBox.Document = document;
            ManifestRichTextBox.IsSpellCheckingEnabled = false;
        }

        #region Add the Footer

        private Footer GetFooterDocument()
        {
            var footer = new Footer();
            var footerDoc = new RadDocument();

            var footerSection = new Section();
            footerDoc.Sections.Add(footerSection);
            var cell11Paragraph = new Paragraph();
            var cell12Paragraph = new Paragraph();
            var cell13Paragraph = new Paragraph();

            var cell11Span = new Span("Generated By FoundOPS.")
            {
                FontSize = 10,
                FontFamily = new FontFamily("Century Gothic")
            };

            cell11Paragraph.Inlines.Add(cell11Span);
            var table = new Table { Borders = new TableBorders(new Border(BorderStyle.None)) };
            var row1 = new TableRow();
            var cell11 = new TableCell { Padding = new Padding(35, 0, 0, 0) };
            cell11.Blocks.Add(cell11Paragraph);

            var cell12Span = new Span(" ");
            cell12Paragraph.Inlines.Add(cell12Span);
            var cell12 = new TableCell();
            cell12.Blocks.Add(cell12Paragraph);
            cell12.TextAlignment = RadTextAlignment.Center;

            var cell13Span = new Span
            {
                Text =
                    _settings.IsCustomMessageVisible
                        ? VM.RouteManifest.RouteManifestSettings.CustomMessage
                        : " ",
                FontSize = 10,
                FontFamily = new FontFamily("Century Gothic")
            };

            cell13Paragraph.Inlines.Add(cell13Span);
            var cell13 = new TableCell { Padding = new Padding(0, 0, 35, 0) };
            cell13.Blocks.Add(cell13Paragraph);
            cell13.TextAlignment = RadTextAlignment.Right;

            row1.Cells.Add(cell11);
            row1.Cells.Add(cell12);
            row1.Cells.Add(cell13);

            table.Rows.Add(row1);

            footerSection.Blocks.Add(table);

            footer.Body = footerDoc;

            return footer;
        }

        #endregion

        private void MyRouteManifestViewerClosed(object sender, EventArgs e)
        {
            //On closing the manifest: update and save the route manifest settings
            VM.RouteManifest.UpdateSaveRouteManifestSettings();
        }

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
                provider.Export(ManifestRichTextBox.Document, output);
        }

        private void BtnPrint_OnClick(object sender, RoutedEventArgs e)
        {
            ManifestRichTextBox.Print(GetFileName(), PrintMode.Native);

            #region Analytics

            //get current time
            var currentTimeOfDay = DateTime.Now.ToShortTimeString();

            //check for route manifest options
            if (_settings.IsHeaderVisible)
            {
                Data.Services.Analytics.Header();
                if (_settings.IsRouteNameVisible)
                    Data.Services.Analytics.RouteName();
                if (_settings.IsRouteDateVisible)
                    Data.Services.Analytics.RouteDate();
                if (_settings.IsAssignedVehiclesVisible)
                    Data.Services.Analytics.AssignedVehicles();
            }

            if (_settings.IsSummaryVisible)
            {
                Data.Services.Analytics.Summary();
                if (_settings.IsRouteSummaryVisible)
                    Data.Services.Analytics.RouteSummary();
                if (_settings.IsScheduledStartTimeVisible)
                    Data.Services.Analytics.StartTime();
                if (_settings.IsScheduledEndTimeVisible)
                    Data.Services.Analytics.EndTime();
                if (_settings.IsDestinationsSummaryVisible)
                    Data.Services.Analytics.DestinationsSummary();
                if (_settings.IsNumberofDestinationsVisible)
                    Data.Services.Analytics.NumberofDestinations();
                if (_settings.IsTaskSummaryVisible)
                    Data.Services.Analytics.TaskSummary();
                if (_settings.IsNumberOfTasksVisible)
                    Data.Services.Analytics.NumberOfTasks();
            }

            if (_settings.IsDestinationsVisible)
            {
                Data.Services.Analytics.Destinations();
                if (_settings.IsAddressVisible)
                    Data.Services.Analytics.Address();
                if (_settings.IsContactInfoVisible)
                    Data.Services.Analytics.ContactInfo();
                if (_settings.IsRouteTasksVisible)
                    Data.Services.Analytics.RouteTasks();
                if (_settings.Is2DBarcodeVisible)
                    Data.Services.Analytics.Barcode();
            }

            if (_settings.IsFooterVisible)
            {
                Data.Services.Analytics.Footer();
                if (_settings.IsPageNumbersVisible)
                    Data.Services.Analytics.PageNumbers();
                if (_settings.IsCustomMessageVisible)
                    Data.Services.Analytics.CustomMessage();
            }

            //Analytics - Track when manifests are printed
            Data.Services.Analytics.RouteManifestPrinted(currentTimeOfDay);

            #endregion
        }
    }
}
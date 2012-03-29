using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Reactive.Linq;
using System.Windows.Media;
using FoundOps.Common.Tools;
using System.Windows.Controls;
using FoundOps.SLClient.UI.Tools;
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
        private double _headerPadding;
        private readonly Data.Models.RouteManifestSettings _settings = VM.RouteManifest.RouteManifestSettings;

        private readonly ObservableCollection<InlineUIContainer> _routeDestinationUIContainers = new ObservableCollection<InlineUIContainer>();

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
                .Merge(VM.RouteManifest.RouteManifestSettings.FromAnyPropertyChanged().AsGeneric().Throttle(TimeSpan.FromMilliseconds(100)))
              .Throttle(TimeSpan.FromMilliseconds(100)).ObserveOnDispatcher()
                //Update the Manifest
              .Subscribe(a => UpdateDocument());

            //Update the manifest when this is opened
            Loaded += (s, e) =>
            {
                _isOpen = true;
                UpdateDocument();
            };

            Closed += (s, e) => _isOpen = false;

            //Refresh the InlineUIContainer's sizes after they are added to the visual tree. (This is for ItemsControls)
            _routeDestinationUIContainers.FromCollectionChanged().Throttle(TimeSpan.FromMilliseconds(500))
             .ObserveOnDispatcher().Subscribe(_ =>
             {
                 foreach(var uiContainer in _routeDestinationUIContainers)
                 {
                     //Update the Height and Width
                     uiContainer.UiElement.Measure(new Size(double.MaxValue, double.MaxValue));
                     uiContainer.Height = uiContainer.UiElement.DesiredSize.Height;
                     uiContainer.Width = uiContainer.UiElement.DesiredSize.Width;
                 }

                 ManifestRichTextBox.UpdateEditorLayout();
             });

            //this.ManifestRichTextBox.CurrentVisiblePageChanged+= Update textbox
        }

        #region Logic

        private void UpdateDocument()
        {
            _routeDestinationUIContainers.Clear();

            //Only load the manifest when the current viewer is open
            if (!_isOpen) return;
            var document = new RadDocument { LayoutMode = DocumentLayoutMode.Paged };
            var mainSection = new Section { FooterBottomMargin = 0, HeaderTopMargin = 0, ActualPageMargin = new Padding(0, 20, 0, 20) };
            var bodyParagraph = new Paragraph();

            //If there is a selected route setup the manifest
            if (VM.Routes.SelectedEntity != null)
            {
                #region Add the Header

                if (_settings.IsHeaderVisible)
                {
                    //We only want the header to show up once, so just add it first to the body paragraph.
                    var manifestHeader = new ManifestHeader { Margin = new Thickness(0, 0, 0, _headerPadding) };
                    manifestHeader.Measure(new Size(double.MaxValue, double.MaxValue));

                    var header = new InlineUIContainer
                                     {
                                         Height = manifestHeader.DesiredSize.Height,
                                         Width = manifestHeader.DesiredSize.Width,
                                         UiElement = manifestHeader
                                     };
                    bodyParagraph.Inlines.Add(header);

                    var numVehicles = VM.Routes.SelectedEntity.Vehicles.Count;
                    var numTechnicians = VM.Routes.SelectedEntity.Technicians.Count;
                    if (numVehicles >= numTechnicians)
                    {
                        if (_settings.IsAssignedVehiclesVisible)
                        {
                            _headerPadding = numVehicles * 25;
                        }
                        else if (_settings.IsAssignedTechniciansVisible)
                        {
                            _headerPadding = numTechnicians * 25;
                        }
                        else
                            _headerPadding = 25;
                    }
                    else
                    {
                        if (_settings.IsAssignedTechniciansVisible)
                        {
                            _headerPadding = numTechnicians * 25;
                        }
                        else if (_settings.IsAssignedVehiclesVisible)
                        {
                            _headerPadding = numVehicles * 25;
                        }
                        else
                            _headerPadding = 25;
                    }
                    _headerPadding += 25;

                    manifestHeader.Margin = new Thickness(0, 0, 0, _headerPadding);
                }

                #endregion

                #region Add the Summary

                if (_settings.IsSummaryVisible)
                {
                    //We only want the summary to show up once, so just add it first to the body paragraph.
                    var manifestSummary = new ManifestSummary { Margin = new Thickness(25, 0, 0, -50) };
                    manifestSummary.Measure(new Size(double.MaxValue, double.MaxValue));

                    var summary = new InlineUIContainer
                                                  {
                                                      Height = manifestSummary.DesiredSize.Height,
                                                      Width = manifestSummary.DesiredSize.Width,
                                                      UiElement = manifestSummary
                                                  };
                    bodyParagraph.Inlines.Add(summary);
                }

                #endregion

                #region Add the RouteDestinations

                foreach (var routeDestination in VM.Routes.SelectedEntity.RouteDestinationsListWrapper)
                {
                    var manifestRouteDestination = new ManifestRouteDestination { Margin = new Thickness(25, 2, 0, 10), RouteDestination = routeDestination };
                    manifestRouteDestination.Measure(new Size(double.MaxValue, double.MaxValue));

                    var destination = new InlineUIContainer
                                           {
                                               Height = manifestRouteDestination.DesiredSize.Height,
                                               Width = manifestRouteDestination.DesiredSize.Width,
                                               UiElement = manifestRouteDestination
                                           };

                    _routeDestinationUIContainers.Add(destination);
                    bodyParagraph.Inlines.Add(destination);
                }

                #endregion
            }

            if (_settings.IsFooterVisible)
                mainSection.Footers.Default = GetFooterDocument();

            ////Setup the main section
            mainSection.Blocks.Add(bodyParagraph);

            //Setup the Document
            document.Sections.Add(mainSection);
            ManifestRichTextBox.Document = document;
            ManifestRichTextBox.IsSpellCheckingEnabled = false;
            ManifestRichTextBox.Width = 870;

            //Fixup the layout
            //Observable.Interval(TimeSpan.FromMilliseconds(100)).Take(1).SubscribeOnDispatcher().Subscribe(_ => ManifestRichTextBox.UpdateLayout());
        }

        #region Add the Footer

        private Footer GetFooterDocument()
        {
            var footer = new Footer();
            var footerDoc = new RadDocument();
            var footerSection = new Section();
            footerDoc.Sections.Add(footerSection);
            var cell1Paragraph = new Paragraph();
            var cell2Paragraph = new Paragraph();
            var cell3Paragraph = new Paragraph();

            var cell1Span = new Span("Generated By FoundOPS.")
            {
                FontSize = 10,
                FontFamily = new FontFamily("Century Gothic")
            };

            cell1Paragraph.Inlines.Add(cell1Span);
            var table = new Table { Borders = new TableBorders(new Border(BorderStyle.None)) };
            var row1 = new TableRow();
            var cell1 = new TableCell { Padding = new Padding(35, 0, 0, 0) };
            cell1.Blocks.Add(cell1Paragraph);

            var cell2Span = new Span
            {
                Text =
                    " ",
                FontSize = 10,
                FontFamily = new FontFamily("Century Gothic")
            };
            cell2Paragraph.Inlines.Add(cell2Span);
            var cell2 = new TableCell();
            cell2.Blocks.Add(cell2Paragraph);
            cell2.TextAlignment = RadTextAlignment.Center;

            var cell3Span = new Span
            {
                Text =
                    _settings.IsCustomMessageVisible
                        ? VM.RouteManifest.RouteManifestSettings.CustomMessage
                        : " ",
                FontSize = 10,
                FontFamily = new FontFamily("Century Gothic")
            };

            cell3Paragraph.Inlines.Add(cell3Span);
            var cell3 = new TableCell { Padding = new Padding(0, 0, 35, 0) };
            cell3.Blocks.Add(cell3Paragraph);
            cell3.TextAlignment = RadTextAlignment.Right;

            row1.Cells.Add(cell1);
            row1.Cells.Add(cell2);
            row1.Cells.Add(cell3);

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
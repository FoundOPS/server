using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Reactive.Linq;
using System.Windows.Media;
using FoundOps.Common.Tools;
using System.Windows.Controls;
using FoundOps.SLClient.Data.Models;
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

        private double _headerPadding;

        private RouteManifestSettings Settings { get { return VM.RouteManifest.RouteManifestSettings; } }

        private readonly ObservableCollection<InlineUIContainer> _routeDestinationUIContainers = new ObservableCollection<InlineUIContainer>();

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="RouteManifestViewer"/> class.
        /// </summary>
        public RouteManifestViewer()
        {
            InitializeComponent();

            //Update the Manifest when
            //a) the RouteManifestSettings properties change
            //b) the SelectedEntity changes
            Settings.FromAnyPropertyChanged().AsGeneric()
            .Merge(VM.Routes.SelectedEntityObservable.AsGeneric())
            .Throttle(TimeSpan.FromMilliseconds(100)).ObserveOnDispatcher()
                //Update the Manifest
            .Subscribe(a => UpdateDocument());

            //Update the manifest when this is opened
            Loaded += (s, e) =>
            {
                VM.Routes.ManifestOpen = true;
                //Wait until the popup is shown before updating the document
                Observable.Interval(TimeSpan.FromMilliseconds(350)).Take(1).ObserveOnDispatcher()
                .Subscribe(_ => UpdateDocument());
            };

            Closed += (s, e) => VM.Routes.ManifestOpen = false;

            //Refresh the InlineUIContainer's sizes after they are added to the visual tree. (This is for ItemsControls)
            _routeDestinationUIContainers.FromCollectionChanged().Throttle(TimeSpan.FromMilliseconds(100))
             .ObserveOnDispatcher().Subscribe(_ =>
             {
                 foreach (var uiContainer in _routeDestinationUIContainers)
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

        /// <summary>
        /// Updates the document.
        /// </summary>
        public void UpdateDocument()
        {
            _routeDestinationUIContainers.Clear();

            //Only load the manifest when the current viewer is open
            if (!VM.Routes.ManifestOpen) return;
            var document = new RadDocument { LayoutMode = DocumentLayoutMode.Paged };
            var mainSection = new Section { FooterBottomMargin = 0, HeaderTopMargin = 0, ActualPageMargin = new Padding(0, 20, 0, 20) };
            var bodyParagraph = new Paragraph();

            //If there is a selected route setup the manifest
            if (VM.Routes.SelectedEntity != null)
            {
                #region Add the Header

                if (Settings.IsHeaderVisible)
                {
                    //We only want the header to show up once, so just add it first to the body paragraph.
                    var manifestHeader = new ManifestHeader { Margin = new Thickness(0, 0, 0, 45) };
                    manifestHeader.Measure(new Size(double.MaxValue, double.MaxValue));

                    var header = new InlineUIContainer
                                     {
                                         Height = manifestHeader.DesiredSize.Height,
                                         Width = manifestHeader.DesiredSize.Width,
                                         UiElement = manifestHeader
                                     };
                    bodyParagraph.Inlines.Add(header);
                }

                #endregion

                #region Add the Summary

                if (Settings.IsSummaryVisible)
                {
                    //We only want the summary to show up once, so just add it first to the body paragraph.
                    var manifestSummary = new ManifestSummary { Margin = new Thickness(25, 0, 0, _headerPadding) };
                    manifestSummary.Measure(new Size(double.MaxValue, double.MaxValue));

                    var summary = new InlineUIContainer
                                                  {
                                                      Height = manifestSummary.DesiredSize.Height,
                                                      Width = manifestSummary.DesiredSize.Width,
                                                      UiElement = manifestSummary
                                                  };
                    var numVehicles = VM.Routes.SelectedEntity.Vehicles.Count;
                    var numTechnicians = VM.Routes.SelectedEntity.Technicians.Count;
                    if (numVehicles >= numTechnicians)
                    {
                        if (Settings.IsAssignedVehiclesVisible)
                        {
                            _headerPadding = numVehicles * 25;
                        }
                        else if (Settings.IsAssignedTechniciansVisible)
                        {
                            _headerPadding = numTechnicians * 25;
                        }
                        else
                            _headerPadding = 25;
                    }
                    else
                    {
                        if (Settings.IsAssignedTechniciansVisible)
                        {
                            _headerPadding = numTechnicians * 25;
                        }
                        else if (Settings.IsAssignedVehiclesVisible)
                        {
                            _headerPadding = numVehicles * 25;
                        }
                        else
                            _headerPadding = 25;
                    }
                    _headerPadding -= 20;

                    manifestSummary.Margin = new Thickness(25, 0, 0, _headerPadding);

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

            if (Settings.IsFooterVisible)
                mainSection.Footers.Default = GetFooterDocument();

            ////Setup the main section
            mainSection.Blocks.Add(bodyParagraph);

            //Setup the Document
            document.Sections.Add(mainSection);
            ManifestRichTextBox.Document = document;
            ManifestRichTextBox.IsSpellCheckingEnabled = false;
            ManifestRichTextBox.Width = 870;

            //Fixup the layout
            Observable.Interval(TimeSpan.FromMilliseconds(100)).Take(1).ObserveOnDispatcher().Subscribe(_ => ManifestRichTextBox.UpdateEditorLayout());
        }

        #region Private Methods

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
                    Settings.IsCustomMessageVisible
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

        private void BtnPrint_OnClick(object sender, RoutedEventArgs e)
        {
            ManifestRichTextBox.Print(GetFileName(), PrintMode.Native);

            #region Analytics

            //get current time
            var currentTimeOfDay = DateTime.Now.ToShortTimeString();

            //check for route manifest options
            if (Settings.IsHeaderVisible)
            {
                Data.Services.Analytics.Header();
                if (Settings.IsRouteNameVisible)
                    Data.Services.Analytics.RouteName();
                if (Settings.IsRouteDateVisible)
                    Data.Services.Analytics.RouteDate();
                if (Settings.IsAssignedVehiclesVisible)
                    Data.Services.Analytics.AssignedVehicles();
            }

            if (Settings.IsSummaryVisible)
            {
                Data.Services.Analytics.Summary();
                if (Settings.IsRouteSummaryVisible)
                    Data.Services.Analytics.RouteSummary();
                if (Settings.IsScheduledStartTimeVisible)
                    Data.Services.Analytics.StartTime();
                if (Settings.IsScheduledEndTimeVisible)
                    Data.Services.Analytics.EndTime();
                if (Settings.IsDestinationsSummaryVisible)
                    Data.Services.Analytics.DestinationsSummary();
                if (Settings.IsNumberofDestinationsVisible)
                    Data.Services.Analytics.NumberofDestinations();
                if (Settings.IsTaskSummaryVisible)
                    Data.Services.Analytics.TaskSummary();
                if (Settings.IsNumberOfTasksVisible)
                    Data.Services.Analytics.NumberOfTasks();
            }

            if (Settings.IsDestinationsVisible)
            {
                Data.Services.Analytics.Destinations();
                if (Settings.IsAddressVisible)
                    Data.Services.Analytics.Address();
                if (Settings.IsContactInfoVisible)
                    Data.Services.Analytics.ContactInfo();
                if (Settings.IsRouteTasksVisible)
                    Data.Services.Analytics.RouteTasks();
                if (Settings.Is2DBarcodeVisible)
                    Data.Services.Analytics.Barcode();
            }

            if (Settings.IsFooterVisible)
            {
                Data.Services.Analytics.Footer();
                if (Settings.IsPageNumbersVisible)
                    Data.Services.Analytics.PageNumbers();
                if (Settings.IsCustomMessageVisible)
                    Data.Services.Analytics.CustomMessage();
            }

            //Analytics - Track when manifests are printed
            Data.Services.Analytics.RouteManifestPrinted(currentTimeOfDay);

            #endregion
        }

        private string GetFileName()
        {
            return String.Format("{0} Manifest", VM.Routes.SelectedEntity != null ? VM.Routes.SelectedEntity.Name : "");
        }

        private void MyRouteManifestViewerClosed(object sender, EventArgs e)
        {
            //On closing the manifest: update and save the route manifest settings
            VM.RouteManifest.UpdateSaveRouteManifestSettings();
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

        #endregion

        #endregion
    }
}
using System.Diagnostics;
using FoundOps.Common.Silverlight.Tools.ExtensionMethods;
using FoundOps.Common.Silverlight.UI.Tools.ExtensionMethods;
using FoundOps.Common.Tools;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Framework.Views.Controls.CustomFields;
using FoundOps.SLClient.Data.Models;
using FoundOps.SLClient.Data.Services;
using FoundOps.SLClient.UI.Tools;
using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using Telerik.Windows.Documents.UI;
using Telerik.Windows.Documents.Model;
using Telerik.Windows.Documents.Layout;
using Telerik.Windows.Documents.FormatProviders.Pdf;
using Telerik.Windows.Documents.UI.Layers;
using Telerik.Windows.Media.Imaging;
using Telerik.Windows.Media.Imaging.FormatProviders;
using Border = Telerik.Windows.Documents.Model.Border;
using Section = Telerik.Windows.Documents.Model.Section;

namespace FoundOps.SLClient.UI.Controls.Dispatcher.Manifest
{
    /// <summary>
    /// Displays RouteManifest
    /// </summary>
    public partial class RouteManifestViewer
    {
        #region Public Properties and Events

        private bool _canPrintOrSave;
        /// <summary>
        /// Whether or not this can print or save.
        /// This will be false when the data is loading and when the 
        /// </summary>
        public bool CanPrintOrSave
        {
            get { return _canPrintOrSave; }
            set
            {
                _canPrintOrSave = value;
                this.btnPrint.IsEnabled = value;
                this.btnSave.IsEnabled = value;
            }
        }

        #endregion

        #region Locals

        private double _summaryPadding;

        private RouteManifestSettings Settings { get { return VM.RouteManifest.RouteManifestSettings; } }

        private bool _currentlyPrinting;

        //A subject that controls when update manifest is triggered
        private readonly Subject<bool> _updateManifest = new Subject<bool>();

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="RouteManifestViewer"/> class.
        /// </summary>
        public RouteManifestViewer()
        {
            InitializeComponent();

            // Workaround for bug with RichTextBox. Fixed will be in next release (should remove then)
            this.ManifestRichTextBox.UILayersBuilder = new CustomUILayerBuilder();

            //Update the Manifest when
            //a) the RouteManifestSettings properties change (if the details are loaded)
            var settingsChanged = Settings.FromAnyPropertyChanged()
                //Throttle, if a setting is changed while the document is updating it causes a problem
                .Throttle(TimeSpan.FromSeconds(.75)).AsGeneric(); 
           
            //b) the selected entity changed (if the details are loaded)
            var selectedEntityChanged =  VM.Routes.SelectedEntityObservable.DistinctUntilChanged().AsGeneric();
            
            //c) the details are loaded
            var detailsLoadedObservable = VM.Routes.SelectedEntityObservable.WhereNotNull()
                .SelectLatest(se => Observable2.FromPropertyChangedPattern(se, x => x.ManifestDetailsLoaded)).DistinctUntilChanged();

            settingsChanged.Merge(selectedEntityChanged).Merge(detailsLoadedObservable).Subscribe(_updateManifest);

            selectedEntityChanged.Subscribe(_ => Debug.WriteLine("Selected Entity Changed"));
            detailsLoadedObservable.Subscribe(_ => Debug.WriteLine("Details Loaded"));
            settingsChanged.Subscribe(_ => Debug.WriteLine("Settings Changed"));

            //Update the manifest when this is opened
            Loaded += (s, e) =>
            {
                VM.Routes.ManifestOpen = true;

                //Set the busy indicator if the SelectedEntity != null && the SelectedEntity's manifest details are loaded
                if (VM.Routes.SelectedEntity != null && !VM.Routes.SelectedEntity.ManifestDetailsLoaded)
                    ManifestBusyIndicator.IsBusy = true;

                _updateManifest.OnNext(true);
            };

            _updateManifest.Throttle(TimeSpan.FromMilliseconds(300)).ObserveOnDispatcher().Subscribe(_ =>
            {
                try
                {
                    UpdateDocument();
                }
                catch
                {
                    //If there is an issue generating the manifest, try again
                    _updateManifest.OnNext(true);
                }
            });

            //Disable printing and saving and show the busy indicator when the details are loading
            detailsLoadedObservable.DistinctUntilChanged().ObserveOnDispatcher().Subscribe(detailsLoaded =>
            {
                ManifestBusyIndicator.IsBusy = true;
                CanPrintOrSave = detailsLoaded;
            });

            Closed += (s, e) => VM.Routes.ManifestOpen = false;

            this.ManifestRichTextBox.PrintStarted += (s, e) => _currentlyPrinting = true;
            this.ManifestRichTextBox.PrintCompleted += (s, e) => _currentlyPrinting = false;
        }

        #region Logic

        #region Setup Manifest

        /// <summary>
        /// Takes a UI element converts it to an image.
        /// </summary>
        /// <param name="uiElement">The uiElement to add to the manifest</param>
        /// <param name="customForceLayoutUpdate">A custom method to force the ui element's layout.</param>
        private ImageInline ConvertToImageInline(UIElement uiElement, Action customForceLayoutUpdate = null)
        {
            //To make sure the items controls render properly
            //add it to the BusyContent's StackPanel quickly

            //The BusyIndicator should be open when rendering manifests
            BusyContentStackPanel.Children.Add(uiElement);

            //Update the layout
            if (customForceLayoutUpdate != null)
                customForceLayoutUpdate();
            else
                BusyContentStackPanel.UpdateLayout();

            uiElement.Measure(new Size(double.MaxValue, double.MaxValue));

            ImageInline imageInline = null;

            //Try to create the image inline
            try
            {
                var writableBitmap = new WriteableBitmap((int)uiElement.DesiredSize.Width, (int)uiElement.DesiredSize.Height);
                writableBitmap.Render(uiElement, null);
                writableBitmap.Invalidate();

                var radImage = new RadBitmap(writableBitmap);

                var stream = new MemoryStream();
                var provider = new PngFormatProvider();
                provider.Export(radImage, stream);

                //Remove the uiElements from the stackpanel so the user does not see it
                BusyContentStackPanel.Children.Clear();

                return new ImageInline(stream);
            }
            catch
            {
                imageInline = new ImageInline();

                //Remove the uiElements from the stackpanel so the user does not see it
                BusyContentStackPanel.Children.Clear();
            }

            return imageInline;
        }

        /// <summary>
        /// A helper method to get the file name.
        /// </summary>
        private string GetFileName()
        {
            return String.Format("{0} Manifest", VM.Routes.SelectedEntity != null ? VM.Routes.SelectedEntity.Name : "");
        }

        /// <summary>
        /// Sets up the Footer
        /// </summary>
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

        private IDisposable _retry;
        private bool _updatingDocument = false;
        /// <summary>
        /// Updates the document.
        /// </summary>
        private void UpdateDocument()
        {
            //Only load the manifest when the viewer is open, it is not printing, and the details are loaded
            if (!VM.Routes.ManifestOpen || _currentlyPrinting || VM.Routes.SelectedEntity == null || !VM.Routes.SelectedEntity.ManifestDetailsLoaded)
                return;

            //If the manifest is currently updating retry in a second
            if (_updatingDocument)
            {
                if (_retry != null)
                    _retry.Dispose();

                _retry = Rxx3.RunDelayed(TimeSpan.FromSeconds(1), () => _updateManifest.OnNext(true));
            }

            //Disable printing and saving
            CanPrintOrSave = false;

            //Open busy indicator, used to add things to visual tree
            ManifestBusyIndicator.IsBusy = true;

            //Wait for the busy indicator to open
            Rxx3.RunDelayed(TimeSpan.FromMilliseconds(500), () =>
            {
                _updatingDocument = true;

                var document = new RadDocument { LayoutMode = DocumentLayoutMode.Paged };
                var mainSection = new Section { FooterBottomMargin = 0, HeaderTopMargin = 0, ActualPageMargin = new Padding(0, 20, 0, 20) };
                var bodyParagraph = new Paragraph();

                #region Add the Header

                if (Settings.IsHeaderVisible)
                {
                    //We only want the header to show up once, so just add it first to the body paragraph.
                    var manifestHeader = new ManifestHeader { Margin = new Thickness(0) };

                    bodyParagraph.Inlines.Add(ConvertToImageInline(manifestHeader));
                }

                #endregion

                #region Add the Summary

                if (Settings.IsSummaryVisible)
                {
                    //We only want the summary to show up once, so just add it first to the body paragraph.
                    var manifestSummary = new ManifestSummary { Margin = new Thickness(0, 0, 0, _summaryPadding) };

                    bodyParagraph.Inlines.Add(ConvertToImageInline(manifestSummary));
                }

                #endregion

                #region Add the Route Destinations

                foreach (var routeDestination in VM.Routes.SelectedEntity.RouteDestinationsListWrapper)
                {
                    var manifestRouteDestination = new ManifestRouteDestination { Margin = new Thickness(0, 6, 0, 6), RouteDestination = routeDestination };

                    bodyParagraph.Inlines.Add(ConvertToImageInline(manifestRouteDestination, () =>
                    {
                        //Force the manifestRouteDestination's itemscontrol to layout RouteTasks
                        manifestRouteDestination.UpdateLayout();
                        manifestRouteDestination.Measure(new Size(double.MaxValue, double.MaxValue));

                        //Force the FieldsControl's ItemsControls to layout fields
                        foreach (var manifestFieldsControl in manifestRouteDestination.GetDescendants<ManifestFields>())
                        {
                            var routeTask = manifestFieldsControl.DataContext as RouteTask;

                            if (routeTask == null || routeTask.ServiceHolder == null
                                || routeTask.ServiceHolder.Service == null)
                                continue;

                            var serviceTemplate = routeTask.ServiceHolder.Service.ServiceTemplate;

                            manifestFieldsControl.ServiceTemplate = serviceTemplate;
                            manifestFieldsControl.UpdateLayout();
                            manifestFieldsControl.Measure(new Size(double.MaxValue, double.MaxValue));
                        }

                        manifestRouteDestination.Measure(new Size(double.MaxValue, double.MaxValue));
                    }));
                }

                #endregion

                if (Settings.IsFooterVisible)
                    mainSection.Footers.Default = GetFooterDocument();

                ////Setup the main section
                mainSection.Blocks.Add(bodyParagraph);

                //Setup the Document
                document.Sections.Add(mainSection);
                ManifestRichTextBox.Document = document;
                ManifestRichTextBox.IsSpellCheckingEnabled = false;
                ManifestRichTextBox.Width = 870;

                //Close busy indicator
                ManifestBusyIndicator.IsBusy = false;

                //Allow printing and saving
                CanPrintOrSave = true;

                _updatingDocument = false;
            });
        }

        #endregion

        /// <summary>
        /// On closing the manifest: update and save the route manifest settings
        /// </summary>
        private void MyRouteManifestViewerClosed(object sender, EventArgs e)
        {
            VM.RouteManifest.UpdateSaveRouteManifestSettings();
        }

        #region Automatically Recenter ChildWindow

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var contentRoot = this.GetTemplateChild("ContentRoot") as FrameworkElement;
            if (contentRoot != null)
                contentRoot.LayoutUpdated += ContentRootLayoutUpdated;
        }

        void ContentRootLayoutUpdated(object sender, EventArgs e)
        {
            var contentRoot = this.GetTemplateChild("ContentRoot") as FrameworkElement;
            if (contentRoot == null) return;

            var tg = contentRoot.RenderTransform as TransformGroup;
            var tts = tg.Children.OfType<TranslateTransform>();
            foreach (var t in tts)
            {
                t.X = 0; t.Y = 0;
            }
        }

        #endregion

        #region Print and Save

        private void PrintButtonClick(object sender, RoutedEventArgs e)
        {
            ManifestRichTextBox.Print(GetFileName(), PrintMode.Native);

            #region Analytics

            //get current time
            var currentTimeOfDay = DateTime.UtcNow.ToShortTimeString();

            //check for route manifest options
            if (Settings.IsHeaderVisible)
            {
                Data.Services.Analytics.Track(Event.ManifestOption, detail: "Header");
                if (Settings.IsRouteNameVisible)
                    Data.Services.Analytics.Track(Event.ManifestOption, detail: "Route Name");
                if (Settings.IsRouteDateVisible)
                    Data.Services.Analytics.Track(Event.ManifestOption, detail: "Route Date");
                if (Settings.IsAssignedVehiclesVisible)
                    Data.Services.Analytics.Track(Event.ManifestOption, detail: "Assigned Vehicles");
            }

            if (Settings.IsSummaryVisible)
            {
                Data.Services.Analytics.Track(Event.ManifestOption, detail: "Summary");
                if (Settings.IsRouteSummaryVisible)
                    Data.Services.Analytics.Track(Event.ManifestOption, detail: "Route Summary");
                if (Settings.IsScheduledStartTimeVisible)
                    Data.Services.Analytics.Track(Event.ManifestOption, detail: "Start Time");
                if (Settings.IsScheduledEndTimeVisible)
                    Data.Services.Analytics.Track(Event.ManifestOption, detail: "End Time");
                if (Settings.IsDestinationsSummaryVisible)
                    Data.Services.Analytics.Track(Event.ManifestOption, detail: "Destinations Summary");
                if (Settings.IsNumberofDestinationsVisible)
                    Data.Services.Analytics.Track(Event.ManifestOption, detail: "Number of Destinations");
                if (Settings.IsTaskSummaryVisible)
                    Data.Services.Analytics.Track(Event.ManifestOption, detail: "Task Summary");
                if (Settings.IsNumberOfTasksVisible)
                    Data.Services.Analytics.Track(Event.ManifestOption, detail: "Number Of Tasks");
            }

            if (Settings.IsDestinationsVisible)
            {
                Data.Services.Analytics.Track(Event.ManifestOption, detail: "Destinations");
                if (Settings.IsAddressVisible)
                    Data.Services.Analytics.Track(Event.ManifestOption, detail: "Address");
                if (Settings.IsContactInfoVisible)
                    Data.Services.Analytics.Track(Event.ManifestOption, detail: "Contact Info");
                if (Settings.IsRouteTasksVisible)
                    Data.Services.Analytics.Track(Event.ManifestOption, detail: "RouteTasks");
                if (Settings.Is2DBarcodeVisible)
                    Data.Services.Analytics.Track(Event.ManifestOption, detail: "Barcode");
            }

            if (Settings.IsFooterVisible)
            {
                Data.Services.Analytics.Track(Event.ManifestOption, detail: "Footer");
                if (Settings.IsPageNumbersVisible)
                    Data.Services.Analytics.Track(Event.ManifestOption, detail: "Page Numbers");
                if (Settings.IsCustomMessageVisible)
                    Data.Services.Analytics.Track(Event.ManifestOption, detail: "Custom Message");
            }
            //Analytics - Track when manifests are printed
            Data.Services.Analytics.Track(Event.PrintedManifest, detail: currentTimeOfDay);

            #endregion
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

    /// <summary>
    /// Workaround for bug with RichTextBox. Fixed will be in next release (should remove then)
    /// </summary>
    class CustomUILayerBuilder : UILayersBuilder
    {
        protected override void BuildUILayersOverride(IUILayerContainer uiLayerContainer)
        {
            base.BuildUILayersOverride(uiLayerContainer);

            uiLayerContainer.UILayers.Remove(DefaultUILayers.TableMovementLayer);
        }
    }
}
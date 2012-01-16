using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Reactive.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using FoundOps.Common.Tools;
using System.Reactive.Subjects;
using FoundOps.SLClient.UI.Tools;
using Telerik.Windows.Documents;
using Telerik.Windows.Documents.Model;
using Telerik.Windows.Documents.Fixed;
using Telerik.Windows.Documents.Layout;
using Telerik.Windows.Documents.FormatProviders.Pdf;
using Telerik.Windows.Documents.UI;

namespace FoundOps.SLClient.UI.Controls.Dispatcher.Manifest
{
    /// <summary>
    /// Displays RouteManifest
    /// </summary>
    public partial class RouteManifestViewer : INotifyPropertyChanged
    {
        #region Public Properties and Variables

        #region Implementation of INotifyPropertyChanged

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        private bool _isLoading;
        /// <summary>
        /// A bool that is true when the manifest is being calculated.
        /// </summary>
        public bool IsLoading
        {
            get { return _isLoading; }
            private set
            {
                _isLoading = value;
                this.RaisePropertyChanged("IsLoading");
            }
        }

        #endregion

        #region Locals

        //Used to update the Pdf
        private readonly Subject<bool> _updatePdfObservable = new Subject<bool>();

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

            _updatePdfObservable.Throttle(TimeSpan.FromSeconds(.25)).ObserveOnDispatcher().Subscribe(_ => UpdatePdf());
        }

        #region Logic

        private void UpdateDocument()
        {
            //Only load the manifest when the current viewer is open
            if (!_isOpen) return;
            IsLoading = true;

            var bodyParagraph = new Paragraph();

            //If there is a selected route setup the manifest
            if (VM.Routes.SelectedEntity != null)
            {
                //Keep track of the images loaded
                var routeDestinationImagesCompleted = new BehaviorSubject<int>(0);

                //Update the Pdf when all the required images are loaded 
                //routeDestinationManifests that do not have images will fire ImageLoaded imediately
                routeDestinationImagesCompleted.Where(imageLoaded => imageLoaded == VM.Routes.SelectedEntity.RouteDestinations.Count)
                    .Subscribe(_ => _updatePdfObservable.OnNext(true));

                #region Add the Header

                //We only want the header to show up once, so just add it first to the body paragraph.
                var manifestHeader = new ManifestHeader();
                manifestHeader.Measure(new Size(double.MaxValue, double.MaxValue));

                bodyParagraph.Inlines.Add(new InlineUIContainer
                {
                    Height = manifestHeader.DesiredSize.Height,
                    Width = manifestHeader.DesiredSize.Width,
                    UiElement = manifestHeader
                });

                #endregion

                #region Add the RouteDestinations

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

                    bodyParagraph.Inlines.Add(container);
                }

                #endregion
            }

            ////Setup the main section
            var mainSection = new Section { FooterBottomMargin = 0, HeaderTopMargin = 0, ActualPageMargin = new Padding(0) };
            mainSection.Blocks.Add(bodyParagraph);

            //Setup the Document
            var document = new RadDocument() { LayoutMode = DocumentLayoutMode.Paged };
            document.Sections.Add(mainSection);

            ManifestRichTextBox.Document = document;

            //Update the Pdf right away if there is not a selected Route
            if (VM.Routes.SelectedEntity == null)
                _updatePdfObservable.OnNext(true);
        }

        /// <summary>
        /// Updates the PDF.
        /// </summary>
        private void UpdatePdf()
        {
            //Only load the manifest when the current viewer is open
            if (!_isOpen) return;

            //Scroll to bottom to force all itemscontrols to generate
            ManifestRichTextBox.Document.CaretPosition.MoveToLastPositionInDocument();

            //Give it time to draw the controls
            Observable.Interval(TimeSpan.FromSeconds(1)).Take(1).ObserveOnDispatcher().Subscribe(_ =>
            {
                var outputStream = new MemoryStream();
                var pdfFormatProvider = new PdfFormatProvider();
                pdfFormatProvider.Export(ManifestRichTextBox.Document, outputStream);

                ManifestPdfViewer.DocumentSource = new PdfDocumentSource(outputStream);
                ManifestPdfViewer.DocumentSource.Loaded += (s, e) =>
                {
                    outputStream.Close();
                    IsLoading = false;
                };
            });
        }

        private void MyRouteManifestViewerClosed(object sender, System.EventArgs e)
        {
            //On closing the manifest: update and save the route manifest settings
            VM.RouteManifest.UpdateSaveRouteManifestSettings();
        }

        private void CurrentPageBlockLoaded(object sender, RoutedEventArgs e)
        {
            DependencyObject parent = this;
            while (parent != null && !(parent is DocumentPagePresenter))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            if (parent == null) return;
            var presenter = (DocumentPagePresenter)parent;
            int pageNumber = presenter.SectionBoxIndex + 1;
            ((TextBlock)sender).Text = pageNumber.ToString();
        }

        private void PageCountBlockLoaded(object sender, RoutedEventArgs e)
        {
            DependencyObject parent = this;
            while (parent != null && !(parent is DocumentPagePresenter))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            if (parent == null) return;
            var presenter = (DocumentPagePresenter)parent;
            var position = new DocumentPosition(presenter.Owner.Document);
            position.MoveToLastPositionInDocument();

            ((TextBlock)sender).Text = (position.GetCurrentSectionBox().ChildIndex + 1).ToString();
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

        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            if (IsLoading) return;

            //Open a SaveFileDialog to let the user save the document as a PDF
            var saveDialog = new SaveFileDialog {DefaultExt = ".pdf", Filter = "PDF|*.pdf"};

            var dialogResult = saveDialog.ShowDialog();
            if (dialogResult != true) return;
            var provider = new PdfFormatProvider();
            using (var output = saveDialog.OpenFile())
                provider.Export(this.ManifestRichTextBox.Document, output);
        }
    }
}
using System;
using System.Linq;
using System.Windows;
using System.Reactive.Linq;
using System.ComponentModel;
using FoundOps.Common.Tools;
using System.Windows.Printing;
using System.Windows.Controls;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.SLClient.UI.Tools;
using FoundOps.SLClient.Data.Models;
using System.Collections.ObjectModel;
using FoundOps.Common.Silverlight.UI.Controls.Printing;

namespace FoundOps.SLClient.UI.Controls.Dispatcher.Manifest
{
    /// <summary>
    /// Displays a Manifest
    /// </summary>
    public class RouteManifest : INotifyPropertyChanged, IPagedPrinter
    {
        #region Public Events, Properties and Variables

        // Implementation of INotifyPropertyChanged
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Occurs when the [route manifest updated].
        /// </summary>
        public event EventHandler RouteManifestUpdated;

        #region Implementation of IPagedPrinter

        private int _pageCount;
        /// <summary>
        /// The number of pages.
        /// </summary>
        public int PageCount
        {
            get { return _pageCount; }
            set
            {
                _pageCount = value;
                this.RaisePropertyChanged("PageCount");
            }
        }

        private readonly ObservableCollection<FrameworkElement> _pages = new ObservableCollection<FrameworkElement>();
        /// <summary>
        /// The manifest pages.
        /// </summary>
        public ObservableCollection<FrameworkElement> Pages { get { return _pages; } }

        #endregion

        /// <summary>
        /// The height to print.
        /// </summary>
        public double PrintedHeight { get; set; }

        /// <summary>
        /// The width to print.
        /// </summary>
        public double PrintedWidth { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Manifest"/> class.
        /// </summary>
        public RouteManifest()
        {
            //Update the Manifest when
            //a) the SelectedEntity changes
            //b) RouteManifestSettings changes

            //a) the SelectedEntity changes
            VM.Routes.SelectedEntityObservable.AsGeneric().Merge(
                //b) RouteManifestVM or RouteManifestSettings changes
            VM.RouteManifest.RouteManifestSettings.FromAnyPropertyChanged()
            .Where(p => RouteManifestSettings.DestinationsProperties.Contains(p.PropertyName)).AsGeneric())
            .Throttle(TimeSpan.FromMilliseconds(200)).ObserveOnDispatcher()
            .Subscribe(a => UpdateControl());
        }

        #region Logic

        #region Implementation of IPagedPrinter

        /// <summary>
        /// Prints the manifest.
        /// </summary>
        public void Print()
        {
            if (this.PageCount < 1)
                return;

            var printDocument = new PrintDocument();

            var pageToPrint = 0;

            //Go through each page, render it, then move forward
            printDocument.PrintPage += (sender, e) =>
            {
                var currentPage = this.Pages[pageToPrint];
                var parent = currentPage.Parent;
                if (parent as Panel != null)
                    ((Panel)parent).Children.Remove(currentPage);

                //Add the page to a canvas to get around SL5 printing issues
                var canvas = new Canvas { Width = this.PrintedWidth, Height = this.PrintedHeight };
                canvas.Children.Add(currentPage);

                e.PageVisual = canvas;

                //Increase page to print
                pageToPrint++;

                //Continue printing this page if not past the last page
                e.HasMorePages = pageToPrint < this.PageCount - 1;
            };

            //SL5 Release with Vector Printing
            //var settings = new PrinterFallbackSettings { ForceVector = true };
            printDocument.Print("Route Manifest");//, settings);
        }

        #endregion

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

        /// <summary>
        /// Forces an update.
        /// </summary>
        /// <param name="onComplete">An (optional) action to perform after updating the Manifest.</param>
        public void ForceUpdate(Action onComplete = null)
        {
            //Make sure this happens on the Dispatcher to prevent interfering with other UpdateControl calls
            Application.Current.RootVisual.Dispatcher.BeginInvoke(() =>
            {
                UpdateControl();
                if (onComplete != null)
                    onComplete();
            });
        }

        /// <summary>
        /// Updates the control.
        /// </summary>
        private void UpdateControl()
        {
            //Clear the pages
            this.Pages.Clear();

            var selectedRoute = VM.Routes.SelectedEntity;
            if (selectedRoute == null) return;

            //Go through each route destination
            //Generate the pages by finding how many fit on a page
            var routeDestinations = selectedRoute.RouteDestinationsListWrapper;

            StackPanel currentPage = null;
            ObservableCollection<RouteDestination> currentPageRouteDesinations = null;

            var currentPageIndex = -1;
            var currentRouteDestinationIndex = -1;

            //Keeps track if a new page needs to be added
            var createNewPage = true;

            while (currentRouteDestinationIndex < routeDestinations.Count)
            {
                if (createNewPage)
                {
                    //Update the currentPageIndex before creating a new page
                    currentPageIndex++;

                    //Create a new page
                    currentPage = new StackPanel();

                    //If this is the first page, add the ManifestHeader
                    if (currentPageIndex == 0)
                        currentPage.Children.Add(new ManifestHeader());

                    //Setup a new collection of route destinations
                    currentPageRouteDesinations = new ObservableCollection<RouteDestination>();

                    //Add a ManifestBody to the currentPage
                    var currentBody = new ManifestBody { RouteDestinationsItemsControl = { ItemsSource = currentPageRouteDesinations } };
                    currentPage.Children.Add(currentBody);

                    //Add the new page to this PagedPrinter's collection of Pages
                    this.Pages.Add(currentPage);

                    //Set the createNewPage flag to false
                    createNewPage = false;
                }

                //Update the currentRouteDestinationIndex before adding the route destination
                currentRouteDestinationIndex++;

                //Add the route destination if there is one at this index
                if (routeDestinations.ElementAtOrDefault(currentRouteDestinationIndex) == null)
                    continue;
                currentPageRouteDesinations.Add(routeDestinations.ElementAt(currentRouteDestinationIndex));

                currentPage.Measure(new Size(this.PrintedWidth, this.PrintedHeight));

                if (currentPage.DesiredSize.Height < this.PrintedHeight)
                    continue;

                //remove the last route destination (that made it overflow)
                currentPageRouteDesinations.Remove(routeDestinations.ElementAt(currentRouteDestinationIndex));
                currentRouteDestinationIndex--;

                //update the layout on the page, now that the items are final before moving onto the next
                currentPage.Measure(new Size(this.PrintedWidth, this.PrintedHeight));
                //currentPage.UpdateLayout();

                //Set the createNewPage flag to true (to move to the next page)
                createNewPage = true;
            }

            var manifestFooter = new ManifestFooter();
            currentPage.Children.Add(manifestFooter);
            currentPage.Measure(new Size(this.PrintedWidth, this.PrintedHeight));

            if (currentPage.DesiredSize.Height >= this.PrintedHeight)
            {
                //The ManifestFooter is past the page, so remove it from the page
                currentPage.Children.Remove(manifestFooter);
                currentPage.UpdateLayout();

                //Add another page with the footer
                currentPageIndex++;
                currentPage = new StackPanel();
                currentPage.Children.Add(manifestFooter);
                currentPage.Measure(new Size(this.PrintedWidth, this.PrintedHeight));
                //currentPage.UpdateLayout();
                Pages.Add(currentPage);
            }

            PageCount = currentPageIndex + 1;

            if (RouteManifestUpdated != null)
                RouteManifestUpdated(this, null);
        }

        #endregion

        //private void UpdateControl()
        //{
        //    var numberOfRouteTasks = 0;
        //    if (((RoutesVM)this.DataContext).SelectedEntity != null)
        //    {
        //        //Manually update the number of route tasks text
        //        numberOfRouteTasks += ((RoutesVM)this.DataContext).SelectedEntity.RouteDestinationsListWrapper.Sum(routeDestination => routeDestination.RouteTasks.Count);
        //        this.NumberOfTasks.Text = numberOfRouteTasks.ToString();
        //    }

        //    ForceBindings();
        //    Dispatcher.BeginInvoke(() => UpdateCurrentItems());
        //}

        //protected override void ForceBindings()
        //{
        //    var startTime = new DateTime();

        //    //Go through each visibility property and force a "binding" to the corresponding Element
        //    //This is required because the CollectionPrinter is to slow with normal bindings
        //    //NOTE: For this to work every Element Name should be setup to correspond with the bool property in RouteManifestVM.RouteManifestSettings
        //    //NOTE: "SummaryElement" corresponds with "IsSummaryVisible"

        //    if (RoutesVM.RouteManifestVM == null) return;

        //    //TOOD: in RouteManifestSettings class. Create static public string[] of all toggleablevisibilityproperties
        //    var propertiesToForce = RoutesVM.RouteManifestVM.RouteManifestSettings.GetType().GetProperties().Where(property => property.Name.Contains("Visible"));

        //    //Get the element names and visibilities to force
        //    var elementNamesVisibilities = propertiesToForce.Select(property =>
        //    {
        //        //Setup corresponding element name. Ex. "IsSummaryVisible" => "SummaryElement"
        //        var correspondingElementName = string.Format("{0}Element", property.Name.Replace("Is", "").Replace("Visible", ""));

        //        //Set visibility to the corresponding property's value in RouteManifestSettings
        //        var visibility = ((bool)property.GetValue(RoutesVM.RouteManifestVM.RouteManifestSettings, null)) ? Visibility.Visible : Visibility.Collapsed;

        //        return new Tuple<string, Visibility>(correspondingElementName, visibility);
        //    });

        //    //Go through each RouteDestinationStackPanel 
        //    foreach (var routeDestinationStackPanel in this.RouteDestinationsItemsControl.GetChildObjects<StackPanel>("RouteDestinationStackPanel"))
        //        //Then go through each element
        //        foreach (var elementNameVisibility in elementNamesVisibilities)
        //        {
        //            //Get each element and set the proper visibility

        //            var element = routeDestinationStackPanel.GetChildObject<FrameworkElement>(elementNameVisibility.Item1);

        //            if (element != null)
        //                element.Visibility = elementNameVisibility.Item2;
        //        }

        //    //Force Summary visibility
        //    SummaryStackPanel.Visibility = this.IsFirstPage ? Visibility.Visible : Visibility.Collapsed;

        //    InvalidateArrange();

        //    //Debug.WriteLine("Time to force bindings: " + DateTime.Now.Subtract(startTime));
        //}
    }
}
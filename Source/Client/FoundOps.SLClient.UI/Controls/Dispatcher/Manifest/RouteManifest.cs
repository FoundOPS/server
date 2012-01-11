using System;
using System.Linq;
using System.Windows;
using System.Reactive.Linq;
using FoundOps.Common.Tools;
using System.Windows.Printing;
using System.Windows.Controls;
using System.Collections.Generic;
using FoundOps.SLClient.UI.Tools;
using FoundOps.SLClient.Data.Models;
using FoundOps.Common.Silverlight.UI.Controls.Printing;

namespace FoundOps.SLClient.UI.Controls.Dispatcher.Manifest
{
    /// <summary>
    /// Displays a Manifest
    /// </summary>
    public class RouteManifest : IPagedPrinter
    {
        /// <summary>
        /// The height to print.
        /// </summary>
        public double PrintedHeight { get; set; }

        /// <summary>
        /// The width to print.
        /// </summary>
        public double PrintedWidth { get; set; }

        /// <summary>
        /// The manifest pages.
        /// </summary>
        public List<FrameworkElement> Pages = new List<FrameworkElement>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Manifest"/> class.
        /// </summary>
        public RouteManifest()
        {
            //Set the RouteManifestVM's Printer
            VM.Routes.RouteManifestVM.Printer = this;

            //Update the Manifest when the RouteManifestVM or RouteManifestSettings changes
            VM.Routes.RouteManifestVM.RouteManifestSettings.FromAnyPropertyChanged()
            .Where(p => RouteManifestSettings.DestinationsProperties.Contains(p.PropertyName)).AsGeneric()
            .Throttle(TimeSpan.FromMilliseconds(200)).ObserveOnDispatcher()
            .Subscribe(a => UpdateControl());
        }

        /// <summary>
        /// Updates the control.
        /// </summary>
        public void UpdateControl()
        {
            var selectedRoute = VM.Routes.SelectedEntity;
            if (selectedRoute == null) return;

            var currentPageIndex = 0;
            var currentPage = new StackPanel();
            var currentItemIndex = 0;

            var routeDestinations = selectedRoute.RouteDestinationsListWrapper;

            var newPage = true;

            var currentItems = new List<object>();


            while (currentItemIndex < routeDestinations.Count)
            {
                ManifestBody currentBody = null;

                if (newPage)
                {
                    if (currentPageIndex == 0)
                        currentPage.Children.Add(new ManifestHeader());

                    currentBody = new ManifestBody {RouteDestinationsItemsControl = {ItemsSource = currentItems}};

                    currentPage.Children.Add(currentBody);

                    this.Pages.Add(currentPage);

                    newPage = false;
                }

                currentItems.Add(routeDestinations.ElementAt(currentItemIndex));

                currentItemIndex++;

                currentBody.RouteDestinationsItemsControl.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

                if (currentPage.Height < this.PrintedHeight)
                    continue;

                newPage = true;
                currentPageIndex++;
            }

            currentPage.Children.Add(new ManifestFooter());
        }

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

        #region Implementation of IPagedPrinter

        /// <summary>
        /// Prints the manifest.
        /// </summary>
        public void Print()
        {
            var printDocument = new PrintDocument();

            //Start on first page
            var pageToPrint = 0;

            //Go through each page, render it, then move forward
            printDocument.PrintPage += (sender, e) =>
            {
                e.PageVisual = this.Pages[pageToPrint];

                //Continue printing if not past the last page
                e.HasMorePages = pageToPrint < this.Pages.Count - 1;

                //Increase page to print
                pageToPrint++;
            };

            //Move back to the first page at the end of printing
            //printDocument.EndPrint += (s, e) => CurrentPageIndex = 0;

            //SL5 Release with Vector Printing
            var settings = new PrinterFallbackSettings { ForceVector = true };
            printDocument.Print("Route Manifest", settings);
        }

        #endregion
    }
}
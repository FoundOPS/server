using System;
using System.Linq;
using System.Windows;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Data;
using System.Reactive.Linq;
using FoundOps.Common.Tools;
using FoundOps.SLClient.Data.Models;
using FoundOps.SLClient.UI.ViewModels;
using FoundOps.Common.Silverlight.Tools;

namespace FoundOps.SLClient.UI.Controls.Dispatcher
{
    /// <summary>
    /// Displays a RouteManifest
    /// </summary>
    public partial class RouteManifest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RouteManifest"/> class.
        /// </summary>
        public RouteManifest()
        {
            InitializeComponent();
            BodyItemsControl = RouteDestinationsItemsControl;
            RoutesVM.RouteManifestVM.Printer = this;

            //Bind the ItemsSource to the SelectedEntity (a Route) 's RouteDestinationsListWrapper
            SetBinding(ItemsSourceProperty, new Binding("SelectedEntity.RouteDestinationsListWrapper") { Source = RoutesVM });

            //Update the Manifest when: RouteManifestVM or RouteManifestSettings changes or this is Loaded
            RoutesVM.RouteManifestVM.RouteManifestSettings.FromAnyPropertyChanged().Where(p => RouteManifestSettings.DestinationsProperties.Contains(p.PropertyName))
            .AsGeneric().Merge(Observable.FromEventPattern<RoutedEventArgs>(this, "Loaded").Select(r => true)).Throttle(TimeSpan.FromSeconds(.5))
            .ObserveOnDispatcher().Subscribe(a => UpdateControl());
        }

        private void UpdateControl()
        {
            var numberOfRouteTasks = 0;
            if (((RoutesVM)this.DataContext).SelectedEntity != null)
            {
                //Manually update the number of route tasks text
                numberOfRouteTasks += ((RoutesVM)this.DataContext).SelectedEntity.RouteDestinationsListWrapper.Sum(routeDestination => routeDestination.RouteTasks.Count);
                this.NumberOfTasks.Text = numberOfRouteTasks.ToString();
            }

            ForceBindings();
            Dispatcher.BeginInvoke(() => UpdateCurrentItems());
        }

        /// <summary>
        /// Exposes the RoutesVM.
        /// </summary>
        private RoutesVM RoutesVM
        {
            get
            {
                return (RoutesVM)DataContext;
            }
        }

        //Overrides the Paged Printer's ForceBindings
        protected override void ForceBindings()
        {
            var startTime = new DateTime();

            //Go through each visibility property and force a "binding" to the corresponding Element
            //This is required because the CollectionPrinter is to slow with normal bindings
            //NOTE: For this to work every Element Name should be setup to correspond with the bool property in RouteManifestVM.RouteManifestSettings
            //NOTE: "SummaryElement" corresponds with "IsSummaryVisible"

            if (RoutesVM.RouteManifestVM == null) return;

            //TOOD: in RouteManifestSettings class. Create static public string[] of all toggleablevisibilityproperties
            var propertiesToForce = RoutesVM.RouteManifestVM.RouteManifestSettings.GetType().GetProperties().Where(property => property.Name.Contains("Visible"));

            //Get the element names and visibilities to force
            var elementNamesVisibilities = propertiesToForce.Select(property =>
            {
                //Setup corresponding element name. Ex. "IsSummaryVisible" => "SummaryElement"
                var correspondingElementName = string.Format("{0}Element", property.Name.Replace("Is", "").Replace("Visible", ""));

                //Set visibility to the corresponding property's value in RouteManifestSettings
                var visibility = ((bool)property.GetValue(RoutesVM.RouteManifestVM.RouteManifestSettings, null)) ? Visibility.Visible : Visibility.Collapsed;

                return new Tuple<string, Visibility>(correspondingElementName, visibility);
            });

            //Go through each RouteDestinationStackPanel 
            foreach (var routeDestinationStackPanel in this.RouteDestinationsItemsControl.GetChildObjects<StackPanel>("RouteDestinationStackPanel"))
                //Then go through each element
                foreach (var elementNameVisibility in elementNamesVisibilities)
                {
                    //Get each element and set the proper visibility
                    
                    var element = routeDestinationStackPanel.GetChildObject<FrameworkElement>(elementNameVisibility.Item1);

                    if (element != null)
                        element.Visibility = elementNameVisibility.Item2;
                }

            //Force Summary visibility
            SummaryStackPanel.Visibility = this.IsFirstPage ? Visibility.Visible : Visibility.Collapsed;

            InvalidateArrange();

            //Debug.WriteLine("Time to force bindings: " + DateTime.Now.Subtract(startTime));
        }
    }
}
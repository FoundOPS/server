using FoundOps.Common.Silverlight.UI.Controls.InfiniteAccordion;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.SLClient.UI.Tools;
using System;
using System.Windows;
using System.ComponentModel.Composition;

namespace FoundOps.SLClient.Navigator.Panes.InfiniteAccordion
{
    /// <summary>
    /// The InfiniteAccordionPageAdapter for Blocks.
    /// </summary>
    [Export("Infinite Accordion")]
    public partial class InfiniteAccordion : IInfiniteAccordionPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InfiniteAccordion"/> class.
        /// </summary>
        [ImportingConstructor]
        public InfiniteAccordion()
        {
            InitializeComponent();
            VM.Navigation.CurrentSectionObservable.Subscribe(OnNavigateToMessageRecieved);
        }

        //This method handles when a section was chosen
        private void OnNavigateToMessageRecieved(string sectionChosen)
        {
            //Clear the ObjectDisplayTypeToDisplay
            LayoutRoot.ObjectDisplayTypeToDisplay = null;

            switch (sectionChosen)
            {
                case "Business Accounts":
                    LayoutRoot.ObjectDisplayTypeToDisplay = typeof (BusinessAccount).ToString();
                    break;
                case "Clients":
                    LayoutRoot.ObjectDisplayTypeToDisplay = typeof (Client).ToString();
                    break;
                case "Employees":
                    LayoutRoot.ObjectDisplayTypeToDisplay = typeof (Employee).ToString();
                    break;
                case "Locations":
                    LayoutRoot.ObjectDisplayTypeToDisplay = typeof (Location).ToString();
                    break;
                case "Regions":
                    LayoutRoot.ObjectDisplayTypeToDisplay = typeof (Region).ToString();
                    break;
                case "Routes":
                    LayoutRoot.ObjectDisplayTypeToDisplay = typeof (Route).ToString();
                    break;
                case "Services":
                    LayoutRoot.ObjectDisplayTypeToDisplay = typeof (ServiceHolder).ToString();
                    break;
                case "Vehicles":
                    LayoutRoot.ObjectDisplayTypeToDisplay = typeof (Vehicle).ToString();
                    break;
                case "Vehicle Maintenance":
                    LayoutRoot.ObjectDisplayTypeToDisplay = typeof (VehicleMaintenanceLogEntry).ToString();
                    break;
            }
        }

        private string _initialListDetailsControlObjectType;

        /// <summary>
        /// Sets the object to display.
        /// </summary>
        public string SelectedObjectType
        {
            get { return _initialListDetailsControlObjectType; }
            set
            {
                _initialListDetailsControlObjectType = value;
                LayoutRoot.ObjectDisplayTypeToDisplay = _initialListDetailsControlObjectType;
            }
        }

        private void ExportLocationsCSV_OnClick(object sender, RoutedEventArgs e)
        {
            VM.Locations.ExportToCSV();
        }

        private void ExportRecurringServicesCSV_OnClick(object sender, RoutedEventArgs e)
        {
            VM.RecurringServices.ExportToCSV();
        }
    }
}

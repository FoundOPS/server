using FoundOps.Common.Silverlight.UI.Messages;
using FoundOps.Common.Silverlight.UI.Controls.InfiniteAccordion;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.SLClient.UI.Tools;
using ReactiveUI;
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
            MessageBus.Current.Listen<NavigateToMessage>().Subscribe(OnNavigateToMessageRecieved);
        }

        //This method handles when a NavigateTo button was pressed
        private void OnNavigateToMessageRecieved(NavigateToMessage navigateToMessage)
        {
            var navigateToUriString = navigateToMessage.UriToNavigateTo.ToString();

            //Clear the ObjectDisplayTypeToDisplay
            LayoutRoot.ObjectDisplayTypeToDisplay = null;

            if (navigateToUriString.Contains("Business Accounts"))
                LayoutRoot.ObjectDisplayTypeToDisplay = typeof(BusinessAccount).ToString();
            else if (navigateToUriString.Contains("Clients"))
                LayoutRoot.ObjectDisplayTypeToDisplay = typeof(Client).ToString();
            else if (navigateToUriString.Contains("Employees"))
                LayoutRoot.ObjectDisplayTypeToDisplay = typeof(Employee).ToString();
            else if (navigateToUriString.Contains("Locations"))
                LayoutRoot.ObjectDisplayTypeToDisplay = typeof(Location).ToString();
            else if (navigateToUriString.Contains("Regions"))
                LayoutRoot.ObjectDisplayTypeToDisplay = typeof(Region).ToString();
            else if (navigateToUriString.Contains("Routes"))
                LayoutRoot.ObjectDisplayTypeToDisplay = typeof(Route).ToString();
            else if (navigateToUriString.Contains("Services"))
                LayoutRoot.ObjectDisplayTypeToDisplay = typeof(ServiceHolder).ToString();
            else if (navigateToUriString.Contains("Vehicles"))
                LayoutRoot.ObjectDisplayTypeToDisplay = typeof(Vehicle).ToString();
            else if (navigateToUriString.Contains("Vehicle Maintenance"))
                LayoutRoot.ObjectDisplayTypeToDisplay = typeof(VehicleMaintenanceLogEntry).ToString();
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

using System;
using ReactiveUI;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel.Composition;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Common.Silverlight.MVVM.Messages;
using FoundOps.Common.Silverlight.UI.Controls.InfiniteAccordion;

namespace FoundOps.SLClient.Navigator.Panes.InfiniteAccordion
{
    /// <summary>
    /// The InfiniteAccordionPageAdapter for Blocks.
    /// </summary>
    [Export]
    public partial class Blocks : IInfiniteAccordionPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Blocks"/> class.
        /// </summary>
        [ImportingConstructor]
        public Blocks()
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

            if (navigateToUriString.Contains("BusinessAccounts"))
                LayoutRoot.ObjectDisplayTypeToDisplay = typeof(BusinessAccount).ToString();
            else if (navigateToUriString.Contains("Clients"))
                LayoutRoot.ObjectDisplayTypeToDisplay = typeof(Client).ToString();
            else if (navigateToUriString.Contains("Contacts"))
                LayoutRoot.ObjectDisplayTypeToDisplay = typeof(Contact).ToString();
            else if (navigateToUriString.Contains("Employees"))
                LayoutRoot.ObjectDisplayTypeToDisplay = typeof(Employee).ToString();
            else if (navigateToUriString.Contains("Locations"))
                LayoutRoot.ObjectDisplayTypeToDisplay = typeof(Location).ToString();
            else if (navigateToUriString.Contains("Regions"))
                LayoutRoot.ObjectDisplayTypeToDisplay = typeof(Region).ToString();
            else if (navigateToUriString.Contains("Services"))
                LayoutRoot.ObjectDisplayTypeToDisplay = typeof(Service).ToString();
            else if (navigateToUriString.Contains("Vehicles"))
                LayoutRoot.ObjectDisplayTypeToDisplay = typeof(Vehicle).ToString();
            else if (navigateToUriString.Contains("VehicleMaintenance"))
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

        /// <summary>
        /// A method to get the ObjectDisplay for the type.
        /// </summary>
        /// <param name="objectTypeToDisplay">The object type to display.</param>
        /// <returns></returns>
        public IObjectTypeDisplay ObjectDisplayToDisplay(Type objectTypeToDisplay)
        {
            return LayoutRoot.ObjectTypeDisplay(objectTypeToDisplay.ToString());
        }

        /// <summary>
        /// Exposes the parent grid.
        /// </summary>
        public Grid ParentGrid
        {
            get { return (Grid)this.Parent; }
        }

        /// <summary>
        /// Exposes this as a UI element.
        /// </summary>
        public UIElement ThisUIElement
        {
            get { return this; }
        }
    }
}

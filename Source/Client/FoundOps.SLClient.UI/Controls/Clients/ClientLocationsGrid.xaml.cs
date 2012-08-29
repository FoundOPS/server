using System;
using System.Linq;
using System.Windows;
using Telerik.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using FoundOps.SLClient.Data.Tools;
using FoundOps.SLClient.Data.Services;
using FoundOps.SLClient.UI.ViewModels;
using FoundOps.Core.Models.CoreEntities;
using Telerik.Windows.Controls.GridView;
using FoundOps.Common.Silverlight.UI.Controls.InfiniteAccordion;

namespace FoundOps.SLClient.UI.Controls.Clients
{
    /// <summary>
    /// Contains a list of Locations for the current Client context.
    /// </summary>
    public partial class ClientLocationsGrid : INotifyPropertyChanged
    {
        #region Properties & Variables

        //Public

        /// <summary>
        /// Gets the locations VM.
        /// </summary>
        public LocationsVM LocationsVM
        {
            get { return (LocationsVM)this.DataContext; }
        }

        //Locals

        private void OnCellDoubleClick(object sender, RadRoutedEventArgs e)
        {
            ((IProvideContext)this.DataContext).MoveToDetailsView.Execute(null);
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientLocationsGrid"/> class.
        /// </summary>
        public ClientLocationsGrid()
        {
            InitializeComponent();

            this.DependentWhenVisible(LocationsVM);
            LocationsRadGridView.AddHandler(GridViewCellBase.CellDoubleClickEvent, new EventHandler<RadRoutedEventArgs>(OnCellDoubleClick), true);
        }

        #region Implementation of INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        private void BillingLocationToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            //Set the DefaultBilling location for the current Client
            var currentClient = Manager.Context.GetContext<Client>();

            var radioButton = (RadioButton)sender;
            var location = radioButton.DataContext as Location;

            if (currentClient == null || location == null) return;

            //Prevents race condition issue when changing Client context
            if (!currentClient.Locations.Any(l => l.Id == location.Id)) return;

            foreach (var l in currentClient.Locations)
                l.IsDefaultBillingLocation = false;

            location.IsDefaultBillingLocation = true;
        }
    }
}
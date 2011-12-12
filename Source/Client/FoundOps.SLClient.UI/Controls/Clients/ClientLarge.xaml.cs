using System;
using System.Windows;
using Telerik.Windows;
using System.ComponentModel;
using FoundOps.SLClient.Data.Tools;
using FoundOps.SLClient.UI.ViewModels;
using Telerik.Windows.Controls.GridView;
using PropertyMetadata = System.Windows.PropertyMetadata;

namespace FoundOps.SLClient.UI.Controls.Clients
{
    /// <summary>
    /// Displays a Client and its associations.
    /// </summary>
    public partial class ClientLarge
    {
        #region Properties

        #region ClientsListVM Dependency Property

        /// <summary>
        /// ClientsListVM
        /// </summary>
        public ClientsVM ClientsListVM
        {
            get { return (ClientsVM)GetValue(ClientsListVMProperty); }
            set { SetValue(ClientsListVMProperty, value); }
        }

        /// <summary>
        /// ClientsListVM Dependency Property.
        /// </summary>
        public static readonly DependencyProperty ClientsListVMProperty =
            DependencyProperty.Register(
                "ClientsListVM",
                typeof(ClientsVM),
                typeof(ClientLarge),
                new PropertyMetadata(null));

        #endregion

        /// <summary>
        /// Gets the clients VM.
        /// </summary>
        public ClientsVM ClientsVM { get { return (ClientsVM)this.DataContext; } }

        /// <summary>
        /// Gets the locations VM.
        /// </summary>
        public LocationsVM LocationsVM { get { return (LocationsVM) ((FrameworkElement) this.Resources["LocationsVMHolder"]).DataContext; } }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientLarge"/> class.
        /// </summary>
        public ClientLarge()
        {
            // Required to initialize variables
            InitializeComponent();

#if DEBUG
            if (DesignerProperties.IsInDesignTool)
                return;
#endif
            this.DependentWhenVisible(LocationsVM);

            //Hookup the AddDeleteLocation logic
            AddDeleteLocation.CreateNewItem = () => this.LocationsVM.StartCreationOfLocation();
            AddDeleteLocation.RemoveCurrentItem = (item) => this.LocationsVM.DeleteLocationInCreation();

            LocationsRadGridView.AddHandler(GridViewCellBase.CellDoubleClickEvent, new EventHandler<RadRoutedEventArgs>(OnCellDoubleClick), true);
        }

        /// <summary>
        /// Called whenever the Add Existing or Add New location button is clicked.
        /// </summary>
        private void AddLocationButtonClicked(object sender, RoutedEventArgs e)
        {
            //Call this to trigger the AddLocationCommand, and to close the contextmenu after adding an item
            AddDeleteLocation.AddedCurrentItem();
            //Must do this manually because the AddLocationCommand takes a LocationsVM instead of the entity as a parameter
            ClientsVM.AddLocationCommand.Execute(LocationsVM);
        }

        private void OnCellDoubleClick(object sender, RadRoutedEventArgs e)
        {
            LocationsVM.MoveToDetailsView.Execute(null);
        }
    }
}
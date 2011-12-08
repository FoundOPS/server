using System;
using Telerik.Windows;
using FoundOps.SLClient.Data.Tools;
using FoundOps.SLClient.UI.ViewModels;
using Telerik.Windows.Controls.GridView;
using FoundOps.Common.Silverlight.UI.Controls.InfiniteAccordion;

namespace FoundOps.SLClient.UI.Controls.Vehicles.Maintenance
{
    public partial class MaintenanceGrid
    {
        public bool IsMainGrid { get; set; }

        public MaintenanceGrid()
        {
            InitializeComponent();

            this.DependentWhenVisible(VehicleMaintenanceVM);

            MaintenanceRadGridView.AddHandler(GridViewCellBase.CellDoubleClickEvent, new EventHandler<RadRoutedEventArgs>(OnCellDoubleClick), true);
        }

        public VehicleMaintenanceVM VehicleMaintenanceVM
        {
            get { return (VehicleMaintenanceVM)this.DataContext; }
        }

        private void OnCellDoubleClick(object sender, RadRoutedEventArgs e)
        {
            if (!IsMainGrid)
                ((IProvideContext)this.DataContext).MoveToDetailsView.Execute(null);
        }
    }
}

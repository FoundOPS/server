using System;
using FoundOps.SLClient.UI.Tools;
using Telerik.Windows;
using Telerik.Windows.Controls.GridView;

namespace FoundOps.Framework.Views.Controls.Routes
{
    public partial class RouteLarge
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RouteLarge"/> class.
        /// </summary>
        public RouteLarge()
        {
            InitializeComponent();
            EmployeesGridView.AddHandler(GridViewCellBase.CellDoubleClickEvent, new EventHandler<RadRoutedEventArgs>(OnEmployeesCellDoubleClick), true);
            VehiclesGridView.AddHandler(GridViewCellBase.CellDoubleClickEvent, new EventHandler<RadRoutedEventArgs>(OnVehiclesCellDoubleClick), true);
        }

        private void OnEmployeesCellDoubleClick(object sender, RadRoutedEventArgs e)
        {
            VM.Employees.MoveToDetailsView.Execute(null);
        }

        private void OnVehiclesCellDoubleClick(object sender, RadRoutedEventArgs e)
        {
            VM.Vehicles.MoveToDetailsView.Execute(null);
        }
    }
}

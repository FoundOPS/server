using System;
using Telerik.Windows;
using FoundOps.SLClient.Data.Tools;
using FoundOps.SLClient.UI.ViewModels;
using Telerik.Windows.Controls.GridView;
using FoundOps.Common.Silverlight.UI.Controls.InfiniteAccordion;

namespace FoundOps.SLClient.UI.Controls.Employees.EmployeeHistory
{
    public partial class EmployeeHistoryGrid
    {
        public bool IsMainGrid { get; set; }

        public EmployeeHistoryGrid()
        {
            InitializeComponent();

            this.DependentWhenVisible(EmployeeHistoryVM);

            EmployeeHistoryRadGridView.AddHandler(GridViewCellBase.CellDoubleClickEvent, new EventHandler<RadRoutedEventArgs>(OnCellDoubleClick), true);
        }

        public EmployeeHistoryVM EmployeeHistoryVM
        {
            get { return (EmployeeHistoryVM)this.DataContext; }
        }

        private void OnCellDoubleClick(object sender, RadRoutedEventArgs e)
        {
            if (!IsMainGrid)
                ((IProvideContext)this.DataContext).MoveToDetailsView.Execute(null);
        }
    }
}

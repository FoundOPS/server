using System;
using Telerik.Windows;
using FoundOps.SLClient.Data.Tools;
using FoundOps.SLClient.UI.ViewModels;
using Telerik.Windows.Controls.GridView;
using FoundOps.Common.Silverlight.UI.Controls.InfiniteAccordion;

namespace FoundOps.SLClient.UI.Controls.Services.RecurringServices
{
    public partial class RecurringServicesGrid
    {
        public bool IsMainGrid { get; set; }

        public RecurringServicesGrid()
        {
            InitializeComponent();

            this.DependentWhenVisible(RecurringServicesVM);

            RecurringServiceRadGridView.AddHandler(GridViewCellBase.CellDoubleClickEvent, new EventHandler<RadRoutedEventArgs>(OnCellDoubleClick), true);
        }

        public RecurringServicesVM RecurringServicesVM
        {
            get { return (RecurringServicesVM) this.DataContext; }
        }

        private void OnCellDoubleClick(object sender, RadRoutedEventArgs e)
        {
            if (!IsMainGrid)
                ((IProvideContext)this.DataContext).MoveToDetailsView.Execute(null);
        }
    }
}

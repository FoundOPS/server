using FoundOps.Common.Silverlight.UI.Controls.InfiniteAccordion;
using FoundOps.SLClient.Data.Tools;
using FoundOps.SLClient.UI.Tools;
using Telerik.Windows;
using Telerik.Windows.Controls.GridView;
using System;

namespace FoundOps.SLClient.UI.Controls.Services.RecurringServices
{
    /// <summary>
    /// UI for displaying the RecurringServices for the current context.
    /// </summary>
    public partial class RecurringServicesGrid
    {
        public bool IsMainGrid { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecurringServicesGrid"/> class.
        /// </summary>
        public RecurringServicesGrid()
        {
            InitializeComponent();

            this.DependentWhenVisible(VM.RecurringServices);

            RecurringServiceRadGridView.AddHandler(GridViewCellBase.CellDoubleClickEvent, new EventHandler<RadRoutedEventArgs>(OnCellDoubleClick), true);
        }

        private void OnCellDoubleClick(object sender, RadRoutedEventArgs e)
        {
            if (!IsMainGrid)
                ((IProvideContext)this.DataContext).MoveToDetailsView.Execute(null);
        }
    }
}

using FoundOps.SLClient.Data.Tools;
using FoundOps.SLClient.UI.Tools;
using System;
using Telerik.Windows;
using Telerik.Windows.Controls.GridView;

namespace FoundOps.SLClient.UI.Controls.Dispatcher
{
    /// <summary>
    /// Contains the route log for the current context.
    /// </summary>
    public partial class RoutesLogGrid
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is main grid.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is main grid; otherwise, <c>false</c>.
        /// </value>
        public bool IsMainGrid { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="RoutesLogGrid"/> class.
        /// </summary>
        public RoutesLogGrid()
        {
            InitializeComponent();

            this.DependentWhenVisible(VM.RoutesInfiniteAccordion);

            RouteLogRadGridView.AddHandler(GridViewCellBase.CellDoubleClickEvent, new EventHandler<RadRoutedEventArgs>(OnCellDoubleClick), true);
        }

        private void OnCellDoubleClick(object sender, RadRoutedEventArgs radRoutedEventArgs)
        {
            if (!IsMainGrid)
                VM.RoutesInfiniteAccordion.MoveToDetailsView.Execute(null);
        }
    }
}

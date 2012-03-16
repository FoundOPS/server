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
            //TODO:
            //Navigate to the Dispatcher on the Service date of the selected route
        }
    }
}

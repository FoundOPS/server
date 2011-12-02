using System;
using System.Windows.Controls;
using FoundOps.SLClient.Data.Tools;
using FoundOps.SLClient.UI.ViewModels;
using Telerik.Windows;
using Telerik.Windows.Controls.GridView;

namespace FoundOps.SLClient.UI.Controls.Dispatcher
{
    public partial class RoutesLogGrid : UserControl
    {
        public RoutesLogGrid()
        {
            InitializeComponent();

            this.DependentWhenVisible(RoutesInfiniteAccordionVM);

            RouteLogRadGridView.AddHandler(GridViewCellBase.CellDoubleClickEvent, new EventHandler<RadRoutedEventArgs>(OnCellDoubleClick), true);
        }

        public RoutesInfiniteAccordionVM RoutesInfiniteAccordionVM
        {
            get
            {
                return (RoutesInfiniteAccordionVM)this.DataContext;
            }
        }

        private void OnCellDoubleClick(object sender, RadRoutedEventArgs radRoutedEventArgs)
        {
            //TODO:
            //Navigate to the Dispatcher on the Service date of the selected route
        }

    }
}

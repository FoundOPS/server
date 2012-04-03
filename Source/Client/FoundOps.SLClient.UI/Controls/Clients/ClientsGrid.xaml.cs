using System;
using FoundOps.SLClient.UI.Tools;
using Telerik.Windows;
using System.ComponentModel;
using FoundOps.SLClient.Data.Tools;
using FoundOps.SLClient.UI.ViewModels;
using Telerik.Windows.Controls.GridView;
using FoundOps.Common.Silverlight.UI.Controls.InfiniteAccordion;

namespace FoundOps.SLClient.UI.Controls.Clients
{
    /// <summary>
    /// UI to display a list of the current context's clients.
    /// </summary>
    public partial class ClientsGrid
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is main grid.
        /// </summary>
        public bool IsMainGrid { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientsGrid"/> class.
        /// </summary>
        public ClientsGrid()
        {
            InitializeComponent();

#if DEBUG
            if (DesignerProperties.IsInDesignTool)
                return;
#endif

            this.DependentWhenVisible(VM.Clients);

            ClientsRadGridView.AddHandler(GridViewCellBase.CellDoubleClickEvent, new EventHandler<RadRoutedEventArgs>(OnCellDoubleClick), true);
        }

        private void OnCellDoubleClick(object sender, RadRoutedEventArgs radRoutedEventArgs)
        {
            if (!IsMainGrid)
                ((IProvideContext)this.DataContext).MoveToDetailsView.Execute(null);
        }
    }
}
﻿using System;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using FoundOps.Common.Silverlight.Tools.ExtensionMethods;
using FoundOps.SLClient.Data.Services;
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

            this.DependentWhenVisible(ClientsVM);

            ClientsRadGridView.AddHandler(GridViewCellBase.CellDoubleClickEvent, new EventHandler<RadRoutedEventArgs>(OnCellDoubleClick), true);
        }

        private ClientsVM ClientsVM
        {
            get
            {
                return (ClientsVM)this.DataContext;
            }
        }

        private void OnCellDoubleClick(object sender, RadRoutedEventArgs radRoutedEventArgs)
        {
            if (!IsMainGrid)
                ((IProvideContext)this.DataContext).MoveToDetailsView.Execute(null);
        }
    }
}
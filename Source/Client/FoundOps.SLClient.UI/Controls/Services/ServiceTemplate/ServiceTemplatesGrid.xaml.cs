using System;
using Telerik.Windows;
using FoundOps.SLClient.UI.Tools;
using FoundOps.SLClient.Data.Tools;
using FoundOps.SLClient.UI.ViewModels;
using Telerik.Windows.Controls.GridView;

namespace FoundOps.SLClient.UI.Controls.Services.ServiceTemplate
{
    /// <summary>
    /// Displays a list of the ServiceTemplates for the current context
    /// </summary>
    public partial class ServiceTemplatesGrid
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is main grid.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is main grid; otherwise, <c>false</c>.
        /// </value>
        public bool IsMainGrid { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceTemplatesGrid"/> class.
        /// </summary>
        public ServiceTemplatesGrid()
        {
            InitializeComponent();

            this.DependentWhenVisible(ServiceTemplatesVM);
            this.DependentWhenVisible(FieldsVM);

            ServiceTemplatesRadGridView.AddHandler(GridViewCellBase.CellDoubleClickEvent, new EventHandler<RadRoutedEventArgs>(OnCellDoubleClick), true);
        }

        /// <summary>
        /// Gets the ServiceTemplatesVM.
        /// </summary>
        public ServiceTemplatesVM ServiceTemplatesVM { get { return VM.ServiceTemplates; } }

        /// <summary>
        /// Gets the FieldsVM.
        /// </summary>
        public FieldsVM FieldsVM { get { return VM.Fields; } }

        private void OnCellDoubleClick(object sender, RadRoutedEventArgs e)
        {
            //TODO: Should there be conditions?
            if (!IsMainGrid)
                ServiceTemplatesVM.MoveToDetailsView.Execute(null);
        }
    }
}

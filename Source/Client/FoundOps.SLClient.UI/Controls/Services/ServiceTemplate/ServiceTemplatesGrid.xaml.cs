using System;
using FoundOps.SLClient.Data.Tools;
using FoundOps.SLClient.UI.ViewModels;
using Telerik.Windows;
using Telerik.Windows.Controls.GridView;

namespace FoundOps.SLClient.UI.Controls.Services.ServiceTemplate
{
    /// <summary>
    /// Displays a list of the ServiceTemplates for the current context
    /// </summary>
    public partial class ServiceTemplatesGrid
    {
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

        public ServiceTemplatesVM ServiceTemplatesVM { get { return (ServiceTemplatesVM)this.DataContext; } }

        public FieldsVM FieldsVM { get { return (FieldsVM)FieldsVMHolder.DataContext; } }

        private void OnCellDoubleClick(object sender, RadRoutedEventArgs e)
        {
            //TODO: Should there be conditions?
            if (!IsMainGrid)
                ServiceTemplatesVM.MoveToDetailsView.Execute(null);
        }
    }
}

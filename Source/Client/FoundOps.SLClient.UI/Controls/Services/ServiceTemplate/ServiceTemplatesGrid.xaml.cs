using FoundOps.Core.Models.CoreEntities;
using FoundOps.SLClient.Data.Services;
using FoundOps.SLClient.Data.Tools;
using FoundOps.SLClient.UI.Tools;
using System;
using System.Reactive.Linq;
using System.Windows.Data;
using Telerik.Windows;
using Telerik.Windows.Controls;
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

            this.DependentWhenVisible(VM.ServiceTemplates);
            this.DependentWhenVisible(VM.Fields);

            ServiceTemplatesRadGridView.AddHandler(GridViewCellBase.CellDoubleClickEvent, new EventHandler<RadRoutedEventArgs>(OnCellDoubleClick), true);

            //Switch the RadGridView's ItemSource depending on whether this is in the admin console or not
            //If there is a BusinessAccountContext
            //bind the ItemsSource to the BusinessAccountContextServiceTemplates
            //Otherwise bind the ItemsSource to the ServiceTemplatesForContext
            Manager.Context.GetContextObservable<BusinessAccount>().ObserveOnDispatcher().Subscribe(businessAccountContext =>
                ServiceTemplatesRadGridView.SetBinding(DataControl.ItemsSourceProperty,
                new Binding(businessAccountContext != null ? "BusinessAccountContextServiceTemplates" : "ServiceTemplatesForContext") { Source = VM.ServiceTemplates }));
        }

        private void OnCellDoubleClick(object sender, RadRoutedEventArgs e)
        {
            //Only move to details view if this is in the admin console (there is a BusinessAccount context)
            if (Manager.Context.GetContext<BusinessAccount>() != null && !IsMainGrid)
                VM.ServiceTemplates.MoveToDetailsView.Execute(null);
        }
    }
}

using System;
using System.Windows;
using Telerik.Windows;
using System.ComponentModel;
using FoundOps.SLClient.Data.Tools;
using FoundOps.SLClient.UI.ViewModels;
using Telerik.Windows.Controls.GridView;
using PropertyMetadata = System.Windows.PropertyMetadata;
using FoundOps.Common.Silverlight.UI.Controls.InfiniteAccordion;

namespace FoundOps.SLClient.UI.Controls.Clients
{
    public partial class ClientsGrid : INotifyPropertyChanged
    {
        private bool _isMainGrid;
        public bool IsMainGrid
        {
            get { return _isMainGrid; }
            set
            {
                _isMainGrid = value;
                ClientsRadGridView.Columns["DeleteSelectedClientFromParentContextVMColumn"].IsVisible = !IsMainGrid;
            }
        }

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

        #region ParentContextVM Dependency Property

        /// <summary>
        /// ParentContextVM
        /// </summary>
        public IAddDeleteSelectedClient ParentContextVM
        {
            get { return (IAddDeleteSelectedClient)GetValue(ParentContextVMProperty); }
            set { SetValue(ParentContextVMProperty, value); }
        }

        /// <summary>
        /// ParentContextVM Dependency Property.
        /// </summary>
        public static readonly DependencyProperty ParentContextVMProperty =
            DependencyProperty.Register(
                "ParentContextVM",
                typeof(IAddDeleteSelectedClient),
                typeof(ClientsGrid),
                new PropertyMetadata(null));

        #endregion

        #region Implementation of INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
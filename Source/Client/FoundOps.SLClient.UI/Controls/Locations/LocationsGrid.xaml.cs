using System;
using System.Windows;
using Telerik.Windows;
using System.ComponentModel;
using FoundOps.SLClient.Data.Tools;
using FoundOps.SLClient.UI.ViewModels;
using Telerik.Windows.Controls.GridView;
using PropertyMetadata = System.Windows.PropertyMetadata;
using FoundOps.Common.Silverlight.UI.Controls.InfiniteAccordion;

namespace FoundOps.SLClient.UI.Controls.Locations
{
    public partial class LocationsGrid : INotifyPropertyChanged
    {
        private bool _isMainGrid;
        public bool IsMainGrid
        {
            get { return _isMainGrid; }
            set
            {
                _isMainGrid = value;
            }
        }

        public LocationsGrid()
        {
            InitializeComponent();

            this.DependentWhenVisible(LocationsVM);

            LocationsRadGridView.AddHandler(GridViewCellBase.CellDoubleClickEvent, new EventHandler<RadRoutedEventArgs>(OnCellDoubleClick), true);
        }

        public LocationsVM LocationsVM
        {
            get { return (LocationsVM) this.DataContext; }
        }

        private void OnCellDoubleClick(object sender, RadRoutedEventArgs e)
        {
            if (!IsMainGrid)
                ((IProvideContext)this.DataContext).MoveToDetailsView.Execute(null);
        }

        #region ParentContextVM Dependency Property

        /// <summary>
        /// ParentContextVM
        /// </summary>
        public IAddDeleteSelectedLocation ParentContextVM
        {
            get { return (IAddDeleteSelectedLocation)GetValue(ParentContextVMProperty); }
            set { SetValue(ParentContextVMProperty, value); }
        }

        /// <summary>
        /// ParentContextVM Dependency Property.
        /// </summary>
        public static readonly DependencyProperty ParentContextVMProperty =
            DependencyProperty.Register(
                "ParentContextVM",
                typeof(IAddDeleteSelectedLocation),
                typeof(LocationsGrid),
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

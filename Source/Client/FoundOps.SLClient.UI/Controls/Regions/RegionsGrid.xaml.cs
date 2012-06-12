using System;
using Telerik.Windows;
using System.ComponentModel;
using FoundOps.SLClient.Data.Tools;
using FoundOps.SLClient.UI.ViewModels;
using Telerik.Windows.Controls.GridView;
using FoundOps.Common.Silverlight.UI.Controls.InfiniteAccordion;

namespace FoundOps.SLClient.UI.Controls.Regions
{
    public partial class RegionsGrid : INotifyPropertyChanged
    {
        public bool IsMainGrid { get; set; }

        public RegionsGrid()
        {
            InitializeComponent();

            this.DependentWhenVisible(RegionsVM);

            RegionsRadGridView.AddHandler(GridViewCellBase.CellDoubleClickEvent, new EventHandler<RadRoutedEventArgs>(OnCellDoubleClick), true);
        }

        public RegionsVM RegionsVM
        {
            get { return (RegionsVM)this.DataContext; }
        }

        private void OnCellDoubleClick(object sender, RadRoutedEventArgs e)
        {
            if (!IsMainGrid)
                ((IProvideContext)this.DataContext).MoveToDetailsView.Execute(null);
        }

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

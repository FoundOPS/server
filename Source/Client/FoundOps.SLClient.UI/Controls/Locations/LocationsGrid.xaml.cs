using FoundOps.Common.Silverlight.UI.Controls.InfiniteAccordion;
using FoundOps.SLClient.Data.Tools;
using FoundOps.SLClient.UI.ViewModels;
using System;
using System.ComponentModel;
using Telerik.Windows;
using Telerik.Windows.Controls.GridView;

namespace FoundOps.SLClient.UI.Controls.Locations
{
    /// <summary>
    /// Contains a list of Locations.
    /// </summary>
    public partial class LocationsGrid : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is main grid.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is main grid; otherwise, <c>false</c>.
        /// </value>
        public bool IsMainGrid { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocationsGrid"/> class.
        /// </summary>
        public LocationsGrid()
        {
            InitializeComponent();
            this.DependentWhenVisible(LocationsVM);
            LocationsRadGridView.AddHandler(GridViewCellBase.CellDoubleClickEvent, new EventHandler<RadRoutedEventArgs>(OnCellDoubleClick), true);
        }

        /// <summary>
        /// Gets the locations VM.
        /// </summary>
        public LocationsVM LocationsVM
        {
            get { return (LocationsVM) this.DataContext; }
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

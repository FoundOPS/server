using System;
using System.ComponentModel;
using FoundOps.Common.Silverlight.UI.Controls.InfiniteAccordion;
using FoundOps.SLClient.Data.Tools;
using FoundOps.SLClient.UI.Tools;
using Telerik.Windows;
using Telerik.Windows.Controls.GridView;

namespace FoundOps.SLClient.UI.Controls.Vehicles
{
    /// <summary>
    /// UI for displaying Vehicles for the current context.
    /// </summary>
    public partial class VehiclesGrid
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is main grid.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is main grid; otherwise, <c>false</c>.
        /// </value>
        public bool IsMainGrid { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="VehiclesGrid"/> class.
        /// </summary>
        public VehiclesGrid()
        {
            InitializeComponent();
            this.DependentWhenVisible(VM.Vehicles);
            VehiclesRadGridView.AddHandler(GridViewCellBase.CellDoubleClickEvent, new EventHandler<RadRoutedEventArgs>(OnCellDoubleClick), true);
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

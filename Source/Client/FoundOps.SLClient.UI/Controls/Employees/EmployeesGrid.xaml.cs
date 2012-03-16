using System;
using System.Windows;
using Telerik.Windows;
using System.ComponentModel;
using FoundOps.SLClient.Data.Tools;
using FoundOps.SLClient.UI.ViewModels;
using Telerik.Windows.Controls.GridView;
using PropertyMetadata = System.Windows.PropertyMetadata;
using FoundOps.Common.Silverlight.UI.Controls.InfiniteAccordion;

namespace FoundOps.SLClient.UI.Controls.Employees
{
    /// <summary>
    /// Contains a list of Employees for the current context.
    /// </summary>
    public partial class EmployeesGrid : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is main grid.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is main grid; otherwise, <c>false</c>.
        /// </value>
        public bool IsMainGrid { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeesGrid"/> class.
        /// </summary>
        public EmployeesGrid()
        {
            InitializeComponent();

            this.DependentWhenVisible(EmployeesVM);

            EmployeesGridView.AddHandler(GridViewCellBase.CellDoubleClickEvent, new EventHandler<RadRoutedEventArgs>(OnCellDoubleClick), true);
        }

        /// <summary>
        /// Gets the employees VM.
        /// </summary>
        public EmployeesVM EmployeesVM
        {
            get { return (EmployeesVM)this.DataContext; }
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
        public IAddDeleteSelectedEmployee ParentContextVM
        {
            get { return (IAddDeleteSelectedEmployee) GetValue(ParentContextVMProperty); }
            set { SetValue(ParentContextVMProperty, value); }
        }

        /// <summary>
        /// ParentContextVM Dependency Property.
        /// </summary>
        public static readonly DependencyProperty ParentContextVMProperty =
            DependencyProperty.Register(
                "ParentContextVM",
                typeof (IAddDeleteSelectedEmployee),
                typeof (EmployeesGrid),
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

using System.Windows;
using FoundOps.SLClient.UI.ViewModels;

namespace FoundOps.SLClient.UI.Controls.Employees
{
    /// <summary>
    /// 
    /// </summary>
    public partial class SelectedEmployees
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectedEmployees"/> class.
        /// </summary>
        public SelectedEmployees()
        {
            InitializeComponent();
        }

        #region SelectedEmployeesVM Dependency Property

        /// <summary>
        /// SelectedEmployeesVM
        /// </summary>
        public SelectedEmployeesVM SelectedEmployeesVM
        {
            get { return (SelectedEmployeesVM)GetValue(SelectedEmployeesVMProperty); }
            set { SetValue(SelectedEmployeesVMProperty, value); }
        }

        /// <summary>
        /// SelectedEmployeesVM Dependency Property.
        /// </summary>
        public static readonly DependencyProperty SelectedEmployeesVMProperty =
            DependencyProperty.Register(
                "SelectedEmployeesVM",
                typeof(SelectedEmployeesVM),
                typeof(SelectedEmployees),
                new PropertyMetadata(null));

        #endregion
    }
}

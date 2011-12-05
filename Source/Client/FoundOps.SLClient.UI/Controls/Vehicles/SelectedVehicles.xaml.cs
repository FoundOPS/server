using System.Windows;
using FoundOps.SLClient.UI.ViewModels;

namespace FoundOps.SLClient.UI.Controls.Vehicles
{
    /// <summary>
    /// A control to allow adding and removing Vehicle associations to entities.
    /// </summary>
    public partial class SelectedVehicles
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectedVehicles"/> class.
        /// </summary>
        public SelectedVehicles()
        {
            InitializeComponent();
        }

        #region SelectedVehiclesVM Dependency Property

        /// <summary>
        /// SelectedVehiclesVM
        /// </summary>
        public SelectedVehiclesVM SelectedVehiclesVM
        {
            get { return (SelectedVehiclesVM)GetValue(SelectedVehiclesVMProperty); }
            set { SetValue(SelectedVehiclesVMProperty, value); }
        }

        /// <summary>
        /// SelectedVehiclesVM Dependency Property.
        /// </summary>
        public static readonly DependencyProperty SelectedVehiclesVMProperty =
            DependencyProperty.Register(
                "SelectedVehiclesVM",
                typeof(SelectedVehiclesVM),
                typeof(SelectedVehicles),
                new PropertyMetadata(null));

        #endregion
    }
}

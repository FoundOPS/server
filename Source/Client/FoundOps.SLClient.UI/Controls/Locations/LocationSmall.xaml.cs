using System.Windows;
using FoundOps.Framework.Views.Controls.Locations;
using FoundOps.SLClient.UI.ViewModels;

namespace FoundOps.SLClient.UI.Controls.Locations
{
    public partial class LocationSmall
    {
        #region LocationVM Dependency Property

        /// <summary>
        /// LocationVM
        /// </summary>
        public LocationVM LocationVM
        {
            get { return (LocationVM)GetValue(LocationVMProperty); }
            set { SetValue(LocationVMProperty, value); }
        }

        /// <summary>
        /// LocationVM Dependency Property.
        /// </summary>
        public static readonly DependencyProperty LocationVMProperty =
            DependencyProperty.Register(
                "LocationVM",
                typeof(LocationVM),
                typeof(LocationEdit),
                new PropertyMetadata(new PropertyChangedCallback(LocationVMChanged)));

        private static void LocationVMChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        #endregion

        public LocationSmall()
        {
            InitializeComponent();
        }
    }
}

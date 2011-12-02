using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using FoundOps.SLClient.UI.ViewModels;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.SLClient.Data.Tools;
using Telerik.Windows.Controls;

namespace FoundOps.SLClient.UI.Controls.CustomFields.Edit
{
    public partial class LocationFieldEdit : UserControl
    {
        public LocationFieldEdit()
        {
            InitializeComponent();

            if (!LocationsVM.IsLoading)
                SetupTwoWayBinding();

            LocationsVM.PropertyChanged += (sender, e) =>
            {
                //After the locations are loaded, setup two way binding
                if (e.PropertyName == "IsLoading")
                    if (LocationsVM.IsLoading)
                        ClearTwoWayBinding();
                    else
                        SetupTwoWayBinding();
            };

            this.DependentWhenVisible(LocationsVM);
        }

        private void ClearTwoWayBinding()
        {
            LocationFieldComboBox.SetBinding(Selector.SelectedValueProperty, new Binding("Value"));
        }
        private void SetupTwoWayBinding()
        {
            LocationFieldComboBox.SetBinding(Selector.SelectedValueProperty, new Binding("Value") { Mode = BindingMode.TwoWay });
        }

        public LocationsVM LocationsVM
        {
            get { return (LocationsVM)this.DataContext; }
        }

        #region LocationField Dependency Property

        /// <summary>
        /// LocationField
        /// </summary>
        public LocationField LocationField
        {
            get { return (LocationField)GetValue(LocationFieldProperty); }
            set { SetValue(LocationFieldProperty, value); }
        }

        /// <summary>
        /// LocationField Dependency Property.
        /// </summary>
        public static readonly DependencyProperty LocationFieldProperty =
            DependencyProperty.Register(
                "LocationField",
                typeof(LocationField),
                typeof(LocationFieldEdit),
                new PropertyMetadata(null));

        #endregion
    }
}

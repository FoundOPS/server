using System.Windows;
using System.Windows.Data;
using FoundOps.SLClient.UI.ViewModels;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.SLClient.Data.Tools;
using Telerik.Windows.Controls;

namespace FoundOps.SLClient.UI.Controls.Locations
{
    public partial class LocationLinkLarge
    {
        public LocationLinkLarge()
        {
            InitializeComponent();

            LocationsVM.PropertyChanged += (sender, e) =>
            {
                //After the locations are loaded, setup two way binding
                if (e.PropertyName == "IsLoading" && !LocationsVM.IsLoading)
                {
                    LocationsRadComboBox.SetBinding(Selector.SelectedValueProperty, new Binding("Entity") { Source = this, Mode = BindingMode.TwoWay });
                }
            };

            this.DependentWhenVisible(LocationsVM);
        }

        public LocationsVM LocationsVM
        {
            get { return (LocationsVM)this.DataContext; }
        }

        #region Entity Dependency Property

        /// <summary>
        /// The selected location entity
        /// </summary>
        public Location Entity
        {
            get { return (Location)GetValue(EntityProperty); }
            set { SetValue(EntityProperty, value); }
        }

        /// <summary>
        /// The Entity Dependency Property.
        /// </summary>
        public static readonly DependencyProperty EntityProperty =
            DependencyProperty.Register(
                "Entity",
                typeof(Location),
                typeof(LocationLinkLarge),
                new PropertyMetadata(null));

        #endregion

        #region IsReadOnly Dependency Property

        /// <summary>
        /// IsReadOnly
        /// </summary>
        public bool IsReadOnly
        {
            get { return (bool) GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        /// <summary>
        /// IsReadOnly Dependency Property.
        /// </summary>
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(
                "IsReadOnly",
                typeof (bool),
                typeof (LocationLinkLarge),
                new PropertyMetadata(new PropertyChangedCallback(IsReadOnlyChanged)));

        private static void IsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LocationLinkLarge c = d as LocationLinkLarge;
            if (c != null)
            {
                c.LocationsRadComboBox.IsEnabled = !((bool)e.NewValue);
            }
        }

        #endregion

    }
}

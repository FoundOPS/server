using System.Windows;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.SLClient.Data.Tools;
using FoundOps.SLClient.UI.Tools;

namespace FoundOps.SLClient.UI.Controls.Locations
{
    /// <summary>
    /// The UI for selecting a single Location from the loaded Locations.
    /// </summary>
    public partial class LocationLinkLarge
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocationLinkLarge"/> class.
        /// </summary>
        public LocationLinkLarge()
        {
            InitializeComponent();

            this.DependentWhenVisible(VM.Locations);
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
                new PropertyMetadata(IsReadOnlyChanged));

        private static void IsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as LocationLinkLarge;
            if (c != null)
                c.LocationsRadComboBox.IsEnabled = !((bool)e.NewValue);
        }

        #endregion
    }
}

using System;
using System.Windows;
using FoundOps.SLClient.UI.Tools;
using FoundOps.SLClient.Data.Tools;
using FoundOps.Core.Models.CoreEntities;

namespace FoundOps.SLClient.UI.Controls.CustomFields.Edit
{
    /// <summary>
    /// UI for editing and viewing LocationField information.
    /// </summary>
    public partial class LocationFieldEdit
    {
        /// <summary>
        /// A function that checks if the LocationField.Value is valid
        /// </summary>
        public Func<object, bool> IsLocationValueValid { get; private set; }


        private bool _loaded;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocationFieldEdit"/> class.
        /// </summary>
        public LocationFieldEdit()
        {
            this.Loaded += (s, e) => _loaded = true;
            this.Unloaded += (s, e) => _loaded = false;

            //Ignore attempts to set the location to null when this is not loaded
            IsLocationValueValid = location => _loaded || location != null;

            InitializeComponent();

            this.DependentWhenVisible(VM.Locations);
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

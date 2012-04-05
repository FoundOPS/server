using FoundOps.Core.Models.CoreEntities;
using FoundOps.SLClient.UI.Tools;
using FoundOps.SLClient.Data.Tools;
using System.Windows;

namespace FoundOps.SLClient.UI.Controls.CustomFields.Edit
{
    /// <summary>
    /// UI for editing and viewing LocationField information.
    /// </summary>
    public partial class LocationFieldEdit
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocationFieldEdit"/> class.
        /// </summary>
        public LocationFieldEdit()
        {
            InitializeComponent();

            this.DependentWhenVisible(VM.Locations);
            LocationsAutoCompleteBox.HookupSearchSuggestions(VM.Locations.ManuallyUpdateSuggestions);
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

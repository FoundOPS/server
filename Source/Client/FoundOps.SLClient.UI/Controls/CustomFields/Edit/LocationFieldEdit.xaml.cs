using System.Windows;
using System.Windows.Controls;
using FoundOps.SLClient.Data.Tools;
using FoundOps.SLClient.UI.ViewModels;
using FoundOps.Core.Models.CoreEntities;

namespace FoundOps.SLClient.UI.Controls.CustomFields.Edit
{
    /// <summary>
    /// UI for editing and viewing LocationField information.
    /// </summary>
    public partial class LocationFieldEdit : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocationFieldEdit"/> class.
        /// </summary>
        public LocationFieldEdit()
        {
            InitializeComponent();

            this.DependentWhenVisible(LocationsVM);
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

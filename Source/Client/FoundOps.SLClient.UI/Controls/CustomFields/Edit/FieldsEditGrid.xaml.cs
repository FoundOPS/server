using System.Windows;
using FoundOps.Core.Models.CoreEntities;

namespace FoundOps.SLClient.UI.Controls.CustomFields.Edit
{
    /// <summary>
    /// UI for editing the values of fields.
    /// </summary>
    public partial class FieldsEditGrid
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldsEditGrid"/> class.
        /// </summary>
        public FieldsEditGrid()
        {
            InitializeComponent();
        }

        #region ServiceTemplate Dependency Property

        /// <summary>
        /// ServiceTemplate
        /// </summary>
        public ServiceTemplate ServiceTemplate
        {
            get { return (ServiceTemplate) GetValue(ServiceTemplateProperty); }
            set { SetValue(ServiceTemplateProperty, value); }
        }

        /// <summary>
        /// ServiceTemplate Dependency Property.
        /// </summary>
        public static readonly DependencyProperty ServiceTemplateProperty =
            DependencyProperty.Register(
                "ServiceTemplate",
                typeof (ServiceTemplate),
                typeof (FieldsEditGrid),
                new PropertyMetadata(null));

        #endregion

        #region IsGeneratedService Dependency Property

        /// <summary>
        /// IsGeneratedService
        /// </summary>
        public bool IsGeneratedService
        {
            get { return (bool) GetValue(IsGeneratedServiceProperty); }
            set { SetValue(IsGeneratedServiceProperty, value); }
        }

        /// <summary>
        /// IsGeneratedService Dependency Property.
        /// </summary>
        public static readonly DependencyProperty IsGeneratedServiceProperty =
            DependencyProperty.Register(
                "IsGeneratedService",
                typeof (bool),
                typeof (FieldsEditGrid),
                new PropertyMetadata(false));

        #endregion
    }
}

using System.Linq;
using System.Windows;
using Telerik.Windows.Controls;
using ServiceTemplate = FoundOps.Core.Models.CoreEntities.ServiceTemplate;

// Needs to be in the same namespace, because it is a partial class
// ReSharper disable CheckNamespace
namespace FoundOps.Framework.Views.Controls.CustomFields
// ReSharper restore CheckNamespace
{
    /// <summary>
    /// UI to display a list of fields.
    /// </summary>
    public partial class ManifestFields
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldsDefineGrid"/> class.
        /// </summary>
        public ManifestFields()
        {
            InitializeComponent();

            //TODO: Decide on this?
            ////Hide LocationField (for now) from being definable
            //FieldDefineTreeListView.FilterDescriptors.Add(new FilterDescriptor<Field> { FilteringExpression = f => !(f is LocationField) });
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
                typeof(ManifestFields),
                new PropertyMetadata(null));

        #endregion
    }
}

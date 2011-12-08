using System.Windows;
using FoundOps.Core.Models.CoreEntities;
using Telerik.Windows.Data;
using LocationField = FoundOps.Core.Models.CoreEntities.LocationField;
using ServiceTemplate = FoundOps.Core.Models.CoreEntities.ServiceTemplate;

// Needs to be in the same namespace, because it is a partial class
// ReSharper disable CheckNamespace
namespace FoundOps.Framework.Views.Controls.CustomFields
// ReSharper restore CheckNamespace
{
    public partial class FieldsDefineGrid
    {
        public FieldsDefineGrid()
        {
            InitializeComponent();

            //Hide LocationField (for now) from being definable
            FieldDefineTreeListView.FilterDescriptors.Add(new FilterDescriptor<Field>() { FilteringExpression = (f) => !(f is LocationField) });
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
                typeof (FieldsDefineGrid),
                new PropertyMetadata(null));

        #endregion
    }
}

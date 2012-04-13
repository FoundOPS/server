using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using FoundOps.SLClient.Data.Converters;
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
        private static OrderByNameConverter _orderByNameConverter = new OrderByNameConverter();

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldsDefineGrid"/> class.
        /// </summary>
        public ManifestFields()
        {
            InitializeComponent();

            //TODO: Decide on this?
            ////Hide LocationField (for now) from being definable
            //FieldDefineTreeListView.FilterDescriptors.Add(new FilterDescriptor<Field> { FilteringExpression = f => !(f is LocationField) });
            //var fieldName = this.ServiceTemplate.Fields;
            //var two = this.ServiceTemplate;
        }


        #region ServiceTemplate Dependency Property

        /// <summary>
        /// ServiceTemplate
        /// </summary>
        public ServiceTemplate ServiceTemplate
        {
            get { return (ServiceTemplate)GetValue(ServiceTemplateProperty); }
            set { SetValue(ServiceTemplateProperty, value); }
        }

        /// <summary>
        /// ServiceTemplate Dependency Property.
        /// </summary>
        public static readonly DependencyProperty ServiceTemplateProperty =
            DependencyProperty.Register(
                "ServiceTemplate",
                typeof(ServiceTemplate),
                typeof(ManifestFields),
                new PropertyMetadata(ServiceTemplateChanged));

        private static void ServiceTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as ManifestFields;
            if (c != null)
            {
                var newValue = e.NewValue as ServiceTemplate;

                //Manually set the ItemsControl so this happens immediately
                c.FieldsItemsControl.ItemsSource = (IEnumerable) (newValue == null ? null : _orderByNameConverter.Convert(newValue.Fields, null, null, null));
            }
        }

        #endregion
    }
}

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Models.CoreEntities.DesignData;
using ServiceTemplate = FoundOps.Core.Models.CoreEntities.ServiceTemplate;

// Needs to be in the same namespace, because it is a partial class
// ReSharper disable CheckNamespace
namespace FoundOps.Framework.Views.Controls.CustomFields
// ReSharper restore CheckNamespace
{
    /// <summary>
    /// UI to define/edit the definition of fields.
    /// </summary>
    public partial class FieldsDefineGrid
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldsDefineGrid"/> class.
        /// </summary>
        public FieldsDefineGrid()
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
                typeof (FieldsDefineGrid),
                new PropertyMetadata(null));

        #endregion
    }

    /// <summary>
    /// This will return true if the current BusinessAccount is FoundOPS.
    /// </summary>
    public class FoundOPSBoolConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var serviceProvider = value as BusinessAccount;

            if (serviceProvider == null)
                return false;

            return serviceProvider.Id == BusinessAccountsConstants.FoundOpsId;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

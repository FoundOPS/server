using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Models.CoreEntities.DesignData;

namespace FoundOps.SLClient.UI.Controls.Services.ServiceTemplate
{
    /// <summary>
    /// The control for editing a ServiceTemplate
    /// </summary>
    public partial class ServiceTemplateEdit
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceTemplateEdit"/> class.
        /// </summary>
        public ServiceTemplateEdit()
        {
            InitializeComponent();
        }
    }

    /// <summary>
    /// This will return the proper visibility.
    /// 
    /// If the converter parameter is true:
    /// Visible: if the service provider is FoundOPS
    /// Collapsed: if service provider is NOT FoundOPS 
    /// 
    /// If the converter parameter is false:
    /// Visible: if service provider is NOT FoundOPS 
    /// Collapsed: if the service provider is FoundOPS
    /// </summary>
    public class FoundOPSVisibilityConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var showIfFoundOPS = System.Convert.ToBoolean(parameter);

            var serviceProvider = value as BusinessAccount;

            if (serviceProvider == null)
                return Visibility.Collapsed;

            //The provider is FoundOPS
            if (serviceProvider.Id == BusinessAccountsConstants.FoundOpsId)
            {
                return showIfFoundOPS ? Visibility.Visible : Visibility.Collapsed;
            }

            //The provider is not FoundOPS
            return showIfFoundOPS ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

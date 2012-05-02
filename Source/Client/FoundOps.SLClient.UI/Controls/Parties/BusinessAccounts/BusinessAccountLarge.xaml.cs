using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using FoundOps.Common.Silverlight.UI.Controls.AddEditDelete;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Models.CoreEntities.DesignData;

namespace FoundOps.SLClient.UI.Controls.Parties.BusinessAccounts
{
    /// <summary>
    /// The UI for viewing and editing BusinessAccount details.
    /// </summary>
    public partial class BusinessAccountLarge
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessAccountLarge"/> class.
        /// </summary>
        public BusinessAccountLarge()
        {
            InitializeComponent();
        }
    }

    /// <summary>
    /// A converter to return the proper AddMode for the current ServiceProvider.
    /// </summary>
    public class ServiceTemplateAddModeConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var serviceProvider = value as BusinessAccount;

            if (serviceProvider == null)
                return AddMode.None;

            return serviceProvider.Id == BusinessAccountsConstants.FoundOpsId ? AddMode.Add : AddMode.AddExisting;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    /// <summary>
    /// A converter that will return Collapsed if the guid == FoundOPS.Id
    /// </summary>
    public class FoundOPSVisibilityConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var guid = value as Guid?;
            if (!guid.HasValue)
                return Visibility.Collapsed;

            return guid.Value == BusinessAccountsConstants.FoundOpsId ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

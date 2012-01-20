using System;
using System.Windows.Data;
using System.Globalization;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Models.CoreEntities.DesignData;
using FoundOps.Common.Silverlight.UI.Controls.AddEditDelete;

namespace FoundOps.SLClient.Data.Converters
{
    /// <summary>
    /// AddDeleteMode within FoundOPS context: AddDelete, all other serviceproviders: AddItemDelete.
    /// Return as int (workaround, because SetProperty on enum sometimes messes up). 
    /// </summary>
    public class ServiceTemplatesAddDeleteModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var serviceProvider = value as BusinessAccount;

            if (serviceProvider == null)
                return AddDeleteMode.AddDelete;

            return serviceProvider.Id == BusinessAccountsDesignData.FoundOps.Id ? AddDeleteMode.AddDelete: AddDeleteMode.AddItemDelete;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

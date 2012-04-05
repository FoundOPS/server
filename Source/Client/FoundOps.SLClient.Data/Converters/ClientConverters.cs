using System;
using System.Globalization;
using System.Windows.Data;

namespace FoundOps.SLClient.UI.Converters
{
    /// <summary>
    /// Returns a 
    /// </summary>
    public class ClientTitleConverter: IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return String.Format("Client - {0}", value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

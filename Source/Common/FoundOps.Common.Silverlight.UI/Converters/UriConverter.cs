using System;
using System.Globalization;
using System.Windows.Data;

namespace FoundOps.Common.Silverlight.Converters
{
    public class UriConverter:IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new Uri(value.ToString(), UriKind.RelativeOrAbsolute);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

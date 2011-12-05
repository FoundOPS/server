using System;
using System.Windows.Data;
using System.Globalization;

namespace FoundOps.Common.Silverlight.Converters
{
    public class MailToConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new Uri(String.Format(@"Mailto:{0}", value), UriKind.RelativeOrAbsolute);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

using System;
using System.Globalization;
using System.Windows.Data;

namespace FoundOps.Common.Silverlight.Converters
{
    public class ZeroToOneIndexedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return 0;

            return ((int)value) + 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return 0;

            if (value is double)
                return (int)((double)value) - 1;
         
            return (int)value - 1;
        }
    }

    public class IntToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? 0.0 : System.Convert.ToDouble(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? 0.0 : System.Convert.ToInt32(value);
        }
    }
}

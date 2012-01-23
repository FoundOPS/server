using System;
using System.Collections;
using System.Linq;
using System.Windows.Data;
using System.Globalization;

namespace FoundOps.Common.Silverlight.UI.Converters
{
    /// <summary>
    /// Return true if the int is > than the converter parameter 
    /// </summary>
    public class GreaterThan : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isGreater = System.Convert.ToInt32(value) > System.Convert.ToInt32(parameter);
            return isGreater;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

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

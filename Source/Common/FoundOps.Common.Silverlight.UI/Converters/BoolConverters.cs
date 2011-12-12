using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Collections.Generic;

namespace FoundOps.Common.Silverlight.Converters
{
    public class ExistsBoolConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool?)
            {
                return ((bool?)value).HasValue && ((bool?)value).Value;
            }
            else if (value is IEnumerable<object>)
            {
                return ((IEnumerable<object>)value).Count() > 0;
            }
            else if (value is Visibility)
            {
                if ((Visibility)value == Visibility.Visible)
                    return true;
            }
            else if (value is DateTime?)
            {
                if (((DateTime?)value).HasValue)
                    return true;
            }
            else if (value is Guid?)
            {
                if (((Guid?)value).HasValue)
                    return true;
            }
            else if (value is Guid)
            {
                return ((Guid) value) != Guid.Empty;
            }
            else if (value is TimeSpan)
            {
                if (((TimeSpan)value).CompareTo(TimeSpan.Zero) > 0)
                {
                    return true;
                }
            }
            else if (value is int && (int)value != 0)
            {
                return true;
            }
            else if (value != null)
            {
                return true;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool?)
                return value;
            throw new NotImplementedException();
        }

        #endregion
    }

    public class NotExistsBoolConverter : IValueConverter
    {
        readonly ExistsBoolConverter _existsBoolConverter = new ExistsBoolConverter();

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return !(bool)_existsBoolConverter.Convert(value, targetType, parameter, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool?)
                return value;
            throw new NotImplementedException();
        }

        #endregion
    }

    public class TrueIfEnumValueEqualsParameter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((int)value) == (int)parameter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((bool)value)
                return parameter;
            return null;
        }

        #endregion
    }

    public class TrueIfValueEqualsParameter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if((value != null) && (parameter != null))
                return System.Convert.ToString(value) == System.Convert.ToString(parameter);
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((bool)value)
                return parameter;
            return null;
        }

        #endregion
    }
}

﻿using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Collections.Generic;

namespace FoundOps.Common.Silverlight.Converters
{
    /// <summary>
    /// If the object exists the converter will return true.
    /// </summary>
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
                return ((Guid)value) != Guid.Empty;
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

    /// <summary>
    /// If the object does not exist the converter will return true.
    /// </summary>
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
                return !(bool?)value;
            throw new NotImplementedException();
        }

        #endregion
    }

    /// <summary>
    /// Checks if the current enum value is the same as the ConverterParameter.
    /// </summary>
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

    /// <summary>
    /// Returns whether or not the value equals the parameter.
    /// </summary>
    public class EqualsConverterParameter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == parameter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

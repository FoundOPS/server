﻿using System;
using System.Windows;
using System.Windows.Data;
using System.Globalization;
using FoundOps.Common.Silverlight.Extensions;

namespace FoundOps.Common.Silverlight.Converters
{
    public class VisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool)
            {
                bool val = (bool)value;
                if (val)
                    return Visibility.Visible;
            }
            else if (value is string)
            {
                string val = (string)value;
                if (!String.IsNullOrEmpty(val))
                    return Visibility.Visible;
            }
            else if (value != null && value.GetType().IsTypeDerivedFromGenericType(typeof(Nullable<>)))
            {
                dynamic dyn = value;
                if (dyn.HasValue)
                    return Visibility.Visible;
            }
            else if (value is TimeSpan)
            {
                if (((TimeSpan)value).CompareTo(TimeSpan.Zero) > 0)
                    return Visibility.Visible;
            }
            else if (value != null)
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    public class NotVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool)
            {
                bool val = (bool)value;
                if (val)
                    return Visibility.Collapsed;
            }
            else if (value is string)
            {
                string val = (string)value;
                if (!String.IsNullOrEmpty(val))
                    return Visibility.Collapsed;
            }
            else if (value != null && GenericExtensions.IsTypeDerivedFromGenericType(value.GetType(), typeof(Nullable<>)))
            {
                dynamic dyn = value;
                if (dyn.HasValue)
                    return Visibility.Collapsed;
            }
            else if (value is TimeSpan)
            {
                if (((TimeSpan)value).CompareTo(TimeSpan.Zero) > 0)
                    return Visibility.Collapsed;
            }
            else if (value != null)
            {
                return Visibility.Collapsed;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    //All values must be true
    public class MultiVisibilityConverter : IMultiValueConverter
    {
        #region Implementation of IMultiValueConverter

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            int numberOfVariables = values.Length;

            var visibilityConverter = new VisibilityConverter();

            for (var i = 0; i < numberOfVariables; i++)
            {
                if ((Visibility)visibilityConverter.Convert(values[i], null, null, null) == Visibility.Collapsed)
                    return Visibility.Collapsed;
            }
            return Visibility.Visible;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    /// <summary>
    /// Visibility based on matching selected integer value to the given integer parameter.
    /// </summary>
    public class EqualsVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value == System.Convert.ToInt32(parameter) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    /// <summary>
    /// If the value is greater than 1 it will return Visible.
    /// </summary>
    public class GreaterThanVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (System.Convert.ToInt32(value) > System.Convert.ToInt32(parameter)) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class NotEqualsVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (System.Convert.ToInt32(value) != System.Convert.ToInt32(parameter)) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

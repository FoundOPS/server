using System;
using System.Windows;
using System.Windows.Data;
using System.Globalization;

namespace FoundOps.Common.Silverlight.UI.Converters
{
    /// <summary>
    /// Cancels a ConvertBack if the newValue is null.
    /// </summary>
    public class ConvertBackWhenExists : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //If value == null return DependencyProperty.UnsetValue
            return value ?? DependencyProperty.UnsetValue;
        }
    }
}

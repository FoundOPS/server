using System;
using System.Windows.Automation;
using System.Windows.Data;

namespace FoundOps.SLClient.UI.Converters
{
    /// <summary>
    /// Convert from true or false to a ToggleState
    /// </summary>
    public class CheckStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var result = (bool)value;
            return result ? ToggleState.On : ToggleState.Off;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var state = (ToggleState)value;
            return state == ToggleState.On;
        }
    }
}

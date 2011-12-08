using System;
using System.Globalization;
using System.Windows.Data;

namespace FoundOps.SLClient.UI.Converters
{
    /// <summary>
    /// Sets the add button text based on if the item is new or existing
    /// </summary>
    public class AddButtonTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? "Add New" : "Add Existing";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}

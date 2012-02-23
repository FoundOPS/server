using System;
using System.Globalization;
using System.Windows.Data;

namespace FoundOps.SLClient.UI.Converters
{
    /// <summary>
    /// Convert timespan (in minutes) to an int and vise versa
    /// </summary>
    public class EstimatedLengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return 0;
            var minutes = ((TimeSpan)value).Minutes;
            return System.Convert.ToInt32(minutes);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return new TimeSpan();
            return new TimeSpan(0, System.Convert.ToInt32(value), 0);
        }
    }
}

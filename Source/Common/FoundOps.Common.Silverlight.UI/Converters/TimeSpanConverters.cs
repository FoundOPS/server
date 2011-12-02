using System;
using System.Globalization;
using System.Windows.Data;

namespace FoundOps.Common.Silverlight.Converters
{
    public enum TimeSpanComponent
    {
        Days,
        Hours,
        Minutes
    }
    public class TimeSpanToComponentConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var timeSpanComponent = (TimeSpanComponent)parameter;
            var timeSpan = (TimeSpan)value;
            if (timeSpanComponent == TimeSpanComponent.Days)
                return timeSpan.Days;
            if (timeSpanComponent == TimeSpanComponent.Hours)
                return timeSpan.Hours;
            return timeSpan.Minutes;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

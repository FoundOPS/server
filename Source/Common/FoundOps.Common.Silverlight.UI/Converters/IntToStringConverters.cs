using System;
using System.Globalization;
using System.Windows.Data;

namespace FoundOps.Common.Silverlight.UI.Converters
{
    public class StatusIntToStringConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var statusInt = (int) value;

            if (statusInt == 1)
                return "Created";
            if (statusInt == 2)
                return "Routed";
            if (statusInt == 3)
                return "In Progress";
            if (statusInt == 4)
                return "On Hold";
            if (statusInt == 5)
                return "Completed";

            return "Cancelled";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

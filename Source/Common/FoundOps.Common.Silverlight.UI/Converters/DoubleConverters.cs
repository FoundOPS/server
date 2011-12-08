using System;
using System.Windows.Data;

namespace FoundOps.Common.Silverlight.Converters
{
    public class DoubleToDecimalConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is double?)
            {
                if (!((double?)value).HasValue)
                    return new decimal?();

                return System.Convert.ToDecimal(((double?) value).Value);
            }

            return System.Convert.ToDecimal(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is decimal?)
            {
                if (!((decimal?)value).HasValue)
                    return new double?();

                return System.Convert.ToDouble(((decimal?)value).Value);
            }

            return System.Convert.ToDouble(value);
        }

        #endregion
    }
}

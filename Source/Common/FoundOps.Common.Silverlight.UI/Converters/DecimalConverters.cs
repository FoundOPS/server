using System;
using System.Windows.Data;

namespace FoundOps.Common.Silverlight.Converters
{
    public class DecimalToCurrencyStringConverter: IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (parameter != null)
            {
                var hideZeroOrNulls = System.Convert.ToBoolean(parameter);
                if (hideZeroOrNulls && (value == null || System.Convert.ToDecimal(value) == 0))
                    return "";
            }
            return String.Format("{0:C}", value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class DecimalToDoubleConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is decimal?)
            {
                if (!((decimal?)value).HasValue)
                    return new double?();

                return System.Convert.ToDouble(((decimal?)value).Value);
            }

            return System.Convert.ToDouble(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is double?)
            {
                if (!((double?)value).HasValue)
                    return new decimal?();

                return new decimal?(System.Convert.ToDecimal(((double?)value).Value));
            }

            return new decimal?(System.Convert.ToDecimal(value));
        }

        #endregion
    }
}

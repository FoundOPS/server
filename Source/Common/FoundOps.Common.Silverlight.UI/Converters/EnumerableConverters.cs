using System;
using System.Linq;
using System.Windows.Data;
using System.Globalization;
using System.Collections.Generic;

namespace FoundOps.Common.Silverlight.Converters
{
    public class ElementFromEnumerableConverter :IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var enumerable = value as IEnumerable<object>;

            if (enumerable == null)
                return null;

            var index = parameter != null ? System.Convert.ToInt32(parameter) : 0;

            return enumerable.ElementAtOrDefault(index);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

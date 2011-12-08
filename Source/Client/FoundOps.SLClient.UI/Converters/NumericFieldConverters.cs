using System;
using System.Globalization;
using System.Windows.Data;
using FoundOps.Core.Models.CoreEntities;
using Telerik.Windows.Controls;

namespace FoundOps.Framework.Views.Converters
{
    public class NumericFieldToValueFormatConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((string)value == "p")
                return ValueFormat.Percentage;

            return ValueFormat.Numeric;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

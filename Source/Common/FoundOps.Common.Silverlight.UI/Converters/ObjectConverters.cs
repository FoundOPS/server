using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace FoundOps.Common.Silverlight.Converters
{
    public class PropertyConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //Example: Name
            string nameOfPropertyToReturn = (string) parameter;
            if (nameOfPropertyToReturn != "" && value != null)
            {
                var propertyInfo = value.GetType().GetProperty(nameOfPropertyToReturn);
                if (propertyInfo != null)
                {
                    var propertyToReturn = propertyInfo.GetValue(value, null);
                    return propertyToReturn;
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    /// <summary>
    /// Returns the first object that is not null
    /// </summary>
    public class FirstNotNullConverter : IMultiValueConverter
    {
        #region Implementation of IMultiValueConverter

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.FirstOrDefault(v => v != null);
        }

        #endregion

        #region Implementation of IMultiValueConverter

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

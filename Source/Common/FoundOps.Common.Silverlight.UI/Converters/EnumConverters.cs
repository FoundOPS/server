using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Windows.Data;

namespace FoundOps.Common.Silverlight.Converters
{
    /// <summary>
    /// Caches the "enum objects" for the lifetime of the application.
    /// </summary>
    public static class EnumValueCache
    {
        private static readonly IDictionary<Type, object[]> Cache = new Dictionary<Type, object[]>();

        public static object[] GetValues(Type type)
        {
            if (!type.IsEnum)
                throw new ArgumentException("Type '" + type.Name + "' is not an enum");

            object[] values;
            if (!Cache.TryGetValue(type, out values))
            {
                values = type.GetFields()
                    .Where(f => f.IsLiteral)
                    .Select(f => f.GetValue(null))
                    .ToArray();
                Cache[type] = values;
            }
            return values;
        }
    }

    /// <summary>
    /// Enum => EnumValues
    /// </summary>
    public class EnumValuesConverter : IValueConverter
    {
        #region Methods
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            return EnumValueCache.GetValues(value.GetType());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion Methods
    }

    public class LocalizedEnumValuesConverter : IValueConverter
    {
        #region Methods
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var resourceManager = (ResourceManager)value;

            if (value == null || parameter == null)
                return null;

            return
                EnumValueCache.GetValues(parameter.GetType()).Select(
                    enumValue =>
                    new LocalizedEnumItem
                        {
                            Value = enumValue,
                            LocalizedName =
                                resourceManager.GetString(String.Format("EnumName{0}", enumValue.ToString()), culture),
                            LocalizedDescription =
                                resourceManager.GetString(String.Format("EnumDescription{0}", enumValue.ToString()), culture)
                        });

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion Methods
    }

    public class LocalizedEnumToStringMultiValueConverter : System.Windows.Data.IMultiValueConverter
    {
        #region Methods
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            object value = values[0];
            ResourceManager resourceManager = (ResourceManager)values[1];

            if (value == null || parameter == null)
                return "";

            return resourceManager.GetString(String.Format("EnumName{0}", value.ToString()), culture);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion Methods
    }

    public class LocalizedEnumItem
    {
        public string LocalizedName { get; set; }
        public string LocalizedDescription { get; set; }
        public object Value { get; set; }
    }

    public class EnumBoolConverter : IValueConverter
    {
        #region Methods
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || parameter == null)
                return value;

            return value.ToString() == parameter.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || parameter == null)
                return value;
            if ((bool)value)
                return Enum.Parse(targetType, (String)parameter, true);
            return null;
        }
        #endregion Methods
    }
}

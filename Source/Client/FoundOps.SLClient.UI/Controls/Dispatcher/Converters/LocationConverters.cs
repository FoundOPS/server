using System;
using System.Globalization;
using System.Windows.Data;
using FoundOps.Core.Models.CoreEntities;

namespace FoundOps.Framework.Views.Converters
{
    public class LocationToStringConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var location = (Location) value;
            return location == null
                       ? ""
                       : String.Format("{0}, {1}, {2} {3}, {4}", location.Name, location.AddressLineOne, location.City,
                                       location.State, location.ZipCode);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class RegionToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var region = (Region)value;
            return region == null
                       ? ""
                       : String.Format("{0}", region.Name);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

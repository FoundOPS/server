using FoundOps.Core.Models.CoreEntities;
using System;
using System.Windows.Data;
using System.Globalization;

namespace FoundOps.SLClient.Data.Converters
{
    /// <summary>
    /// A converter that takes a location and returns a string with the geo uri
    /// </summary>
    public class LocationToGeoUriConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var location = value as Location;

            if (location == null || location.Latitude == null || location.Longitude == null)
                return null;

            var locationUrl = String.Format("geo:{0},{1}", location.Latitude, location.Longitude);

            return locationUrl;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    /// <summary>
    /// Returns a description of a location.
    /// </summary>
    public class LocationToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var location = (Location)value;
            return location == null
                       ? ""
                       : String.Format("{0} {1}, {2}, {3} {4}", location.Name, location.AddressLineOne, location.AdminDistrictTwo,
                                       location.AdminDistrictOne, location.PostalCode);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Displays either "Locations" or "Location" depending on the number of locations.
    /// </summary>
    public class LocationTitleConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((int)value > 1) ? "Locations" : "Location";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    /// <summary>
    /// Returns the name of a region.
    /// </summary>
    public class RegionToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var region = (Region)value;
            return region == null ? "" : String.Format("{0}", region.Name);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Windows.Data;
using System.Globalization;
using FoundOps.Common.Tools.ExtensionMethods;
using FoundOps.Core.Models.CoreEntities;

namespace FoundOps.SLClient.Data.Converters
{
    /// <summary>
    /// A converter that takes a location and returns a url with a (Location) QRCode
    /// </summary>
    public class LocationToUrlConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var location = value as Location;

            if (location == null)
                return null;

            var latitude = location.Latitude;
            var longitude = location.Longitude;

            string locationUrl = "";

            locationUrl = String.Format(UriExtensions.ThisRootUrl+ "/Helper/LocationQRCode?latitude={0}&longitude={1}", latitude, longitude);
            return new Uri(locationUrl);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

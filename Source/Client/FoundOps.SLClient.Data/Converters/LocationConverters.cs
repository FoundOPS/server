using System;
using System.Windows.Data;
using System.Globalization;
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

#if DEBUG
            locationUrl = String.Format("http://localhost:31820/Helper/LocationQRCode?latitude={0}&longitude={1}", latitude, longitude);
#else
            locationUrl = String.Format("http://www.foundops.com/Helper/LocationQRCode?latitude={0}&longitude={1}", latitude, longitude);
#endif
            return new Uri(locationUrl);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

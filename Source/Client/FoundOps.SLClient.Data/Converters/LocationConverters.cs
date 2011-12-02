using System;
using System.ComponentModel;
using System.Linq;
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

    public class FilterLocationsCollectionViewByClient : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if(values.Count()<2 || values[0] == null)
                return null;

            var locations = values[0] as ICollectionView;
            if (values[1] == null)
                return locations;

            //Only filter by the Selected Client if the current Service Template is owned by a Service
            var serviceTemplate = (ServiceTemplate)values[1];
            if(serviceTemplate.OwnerService==null)
                return locations;

            var clientFilter = serviceTemplate.OwnerService.Client;

            var filteredCollectionView = new CollectionViewSource {Source = locations.SourceCollection}.View;
            filteredCollectionView.Filter += (l) =>
                                                 {
                                                     //If there is no client we cannot determine this is part of the Client.Locations
                                                     if (clientFilter == null)
                                                         return false;

                                                     var location = (Location) l;
                                                     return location.PartyId == clientFilter.OwnedParty.Id ||
                                                                  location.PartyId == clientFilter.LinkedPartyId;
                                                 };

            return filteredCollectionView;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

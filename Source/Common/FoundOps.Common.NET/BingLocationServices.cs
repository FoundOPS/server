using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml.Linq;

//Only works in .NET (not Silverlight) for now because PlaceFinder's Cross Domain Policy is messed up
namespace FoundOps.Common.NET
{
    //http://developer.yahoo.com/geo/placefinder/guide/responses.html#supported-formats
    public static class BingLocationServices
    {
        private const string ApiUrl = "http://dev.virtualearth.net/REST/v1/Locations";

#if RELEASE
        private const string BingKey = "ApDdqZNPi95Qa91JH6HgiFVCxKhjWQwsErfTe5yPRy1SZuANNIgAuo--8Y1AsLcu";
#else
        private const string BingKey = "AtM_c5OqseUW51ynqulvwJWJ-ui0BJbHb4beSXcp--eaHZGufzIsARQJGg00K-Ec";
#endif

        public static IEnumerable<GeocoderResult> TryGeocode(string location)
        {
            var query = String.Format("{0}?query={1}", ApiUrl, location);
            return TryGeocodeUrl(query);
        }

        public static IEnumerable<GeocoderResult> TryGeocode(Address addressToGeocode)
        {
            var parameters = new[]
                {
                    addressToGeocode.State,
                    addressToGeocode.ZipCode,
                    addressToGeocode.City,
                    addressToGeocode.AddressLineOne
                };

            //replace all spaces with + and empty parameters with -
            parameters = parameters.Select(p => p.Replace(" ", "+"))
                .Select(p => string.IsNullOrEmpty(p) ? "-" : p).ToArray();

            var query = String.Format("{0}/US/{1}/{2}/{3}/{4}", ApiUrl, parameters[0], parameters[1], parameters[2], parameters[3]);

            return TryGeocodeUrl(query);
        }

        private static IEnumerable<GeocoderResult> TryGeocodeUrl(string restQueryUrl)
        {
            restQueryUrl = restQueryUrl + "&o=xml" + "&key=" + BingKey;

            // Initiate Async Network call to Bing Maps REST Service
            var geocodeService = new WebClient();
            var data = geocodeService.DownloadString(new Uri(restQueryUrl));
            return BingGeocodeServiceDownloadStringCompleted(data);
        }

        private static IEnumerable<GeocoderResult> BingGeocodeServiceDownloadStringCompleted(string data)
        {
            string xmlContent = data;
            XDocument xmlResultSet = XDocument.Parse(xmlContent);
            XNamespace xmlns = "http://schemas.microsoft.com/search/local/ws/rest/v1";

            var results =
                xmlResultSet.Descendants(xmlns + "Location")
                .Select(
                    result =>
                    {
                        var geocoderResult = new GeocoderResult();

                        var name = result.Element(xmlns + "Name");
                        if (name != null)
                            geocoderResult.Name = name.Value;

                        var precision = result.Element(xmlns + "Confidence");
                        if (precision != null)
                            geocoderResult.Precision = precision.Value;

                        var pointElement = result.Element(xmlns + "Point");
                        if (pointElement != null)
                        {
                            var latitude = pointElement.Element(xmlns + "Latitude");
                            if (latitude != null)
                                geocoderResult.Latitude = latitude.Value;

                            var longitude = pointElement.Element(xmlns + "Longitude");
                            if (longitude != null)
                                geocoderResult.Longitude = longitude.Value;
                        }

                        var addressElement = result.Element(xmlns + "Address");
                        if (addressElement != null)
                        {
                            var addressLineOne = addressElement.Element(xmlns + "AddressLine");
                            if (addressLineOne != null)
                                geocoderResult.AddressLineOne = addressLineOne.Value;

                            var city = addressElement.Element(xmlns + "Locality");
                            if (city != null)
                                geocoderResult.City = city.Value;

                            var state = addressElement.Element(xmlns + "AdminDistrict");
                            if (state != null)
                                geocoderResult.State = state.Value;

                            var zipCode = addressElement.Element(xmlns + "PostalCode");
                            if (zipCode != null)
                                geocoderResult.ZipCode = zipCode.Value;
                        }

                        return geocoderResult;
                    }).ToArray();

            return results;
        }
    }
}

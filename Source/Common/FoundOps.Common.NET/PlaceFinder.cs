using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Xml.Linq;

//Only works in .NET (not Silverlight) for now because PlaceFinder's Cross Domain Policy is messed up
namespace FoundOps.Common.NET
{
    //http://developer.yahoo.com/geo/placefinder/guide/responses.html#supported-formats
    public static class YahooPlaceFinder
    {
        private const string YahooId = "dj0yJmk9UWtYQVg3OVluUksyJmQ9WVdrOU1VcGlNWGRwTkdrbWNHbzlNVFF3TVRJM01UZzJNZy0tJnM9Y29uc3VtZXJzZWNyZXQmeD1mMA--";
        private const string GeocodeUrl = "http://where.yahooapis.com/geocode?appid=" + YahooId;

        public static IEnumerable<GeocoderResult> TryGeocode(Address addressToGeocode)
        {
            string parameters = "";

            if (!string.IsNullOrEmpty(addressToGeocode.AddressLineOne))
                parameters += String.Format("&street={0}", addressToGeocode.AddressLineOne.Replace(" ", "+"));

            if (!string.IsNullOrEmpty(addressToGeocode.City))
                parameters += String.Format("&city={0}", addressToGeocode.City.Replace(" ", "+"));

            if (!string.IsNullOrEmpty(addressToGeocode.State))
                parameters += String.Format("&state={0}", addressToGeocode.State.Replace(" ", "+"));

            if (!string.IsNullOrEmpty(addressToGeocode.ZipCode))
                parameters += String.Format("&zip={0}", addressToGeocode.ZipCode.Replace(" ", "+"));

            string restQueryUrl = GeocodeUrl + parameters;
            return TryGeocodeUrl(restQueryUrl);
        }

        public static IEnumerable<GeocoderResult> TryGeocode(string location)
        {
            var restQueryUrl = GeocodeUrl + String.Format("&location={0}", location.Replace(" ", "+"));
            return TryGeocodeUrl(restQueryUrl);
        }

        private static IEnumerable<GeocoderResult> TryGeocodeUrl(string restQueryUrl)
        {
            // Initiate Async Network call to Yahoo Geocoding Service
            var yahooGeocodeService = new WebClient();
            var data = yahooGeocodeService.DownloadString(new Uri(restQueryUrl));
            return YahooGeocodeServiceDownloadStringCompleted(data);
        }

        private static IEnumerable<GeocoderResult> YahooGeocodeServiceDownloadStringCompleted(string data)
        {
            string xmlContent = data;
            XDocument xmlResultSet = XDocument.Parse(xmlContent);
            var results =
                xmlResultSet.Elements("ResultSet").Elements("Result").Select(
                    result =>
                        new GeocoderResult
                                  {
                                      Name = result.Element("name").Value,
                                      Precision = result.Element("quality").Value,
                                      Latitude = result.Element("latitude").Value,
                                      Longitude = result.Element("longitude").Value,
                                      AddressLineOne = result.Element("line1").Value,
                                      AddressLineTwo = result.Element("line2").Value,
                                      City = result.Element("city").Value,
                                      State = result.Element("statecode").Value,
                                      ZipCode = result.Element("postal").Value
                                  });

            return results;
        }
    }

    [KnownType(typeof(GeocoderResult))]
    public class Address
    {
        [Key]
        public int Key { get; set; } //for RIA Services

        public string Name { get; set; }
        public string AddressLineOne { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }

    public class GeocoderResult : Address
    {
        public string Precision { get; set; }
        public string Radius { get; set; }
        public string AddressLineTwo { get; set; }

        public bool Equals(GeocoderResult resultToCompareTo)
        {
            bool addressIsEqual = !string.IsNullOrEmpty(AddressLineOne) &&
                                  !string.IsNullOrEmpty(resultToCompareTo.AddressLineOne) &&
                                  AddressLineOne.ToLower() == resultToCompareTo.AddressLineOne.ToLower();

            bool zipCodeIsEqual = !string.IsNullOrEmpty(ZipCode) && !string.IsNullOrEmpty(resultToCompareTo.ZipCode) &&
                                  ZipCode.Length >= 5 && resultToCompareTo.ZipCode.Length >= 5 &&
                                  resultToCompareTo.ZipCode.Substring(0, 5) == ZipCode.Substring(0, 5);

            bool cityAndStateEqual = !string.IsNullOrEmpty(City) && !string.IsNullOrEmpty(State) &&
                                     !string.IsNullOrEmpty(resultToCompareTo.City) &&
                                     !string.IsNullOrEmpty(resultToCompareTo.State) &&
                                     City.ToLower() == resultToCompareTo.City.ToLower() && State.ToLower() ==
                                     resultToCompareTo.State.ToLower();

            return addressIsEqual && (cityAndStateEqual || zipCodeIsEqual);
        }
    }
}

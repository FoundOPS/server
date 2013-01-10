using System;
using System.Collections.Generic;
using FoundOps.Api.Tools;

namespace FoundOps.Api.Models
{
    public class Location : ITrackable, IImportable
    {
        /// <summary>
        /// The Id
        /// </summary>
        public Guid Id { get; set; }

        public DateTime CreatedDate { get; private set; }
        public DateTime? LastModified { get; private set; }
        public Guid? LastModifyingUserId { get; private set; }

        /// <summary>
        /// The name given to this Location
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The first line of the Address of this Location
        /// </summary>
        public string AddressLineOne { get; set; }

        /// <summary>
        /// The second line of the Address of this Location
        /// </summary>
        public string AddressLineTwo { get; set; }

        /// <summary>
        /// The City of this Location
        /// </summary>
        public string AdminDistrictTwo { get; set; }

        /// <summary>
        /// The latitude of this Location
        /// </summary>
        public string Latitude { get; set; }

        /// <summary>
        /// The longitude of this Location
        /// </summary>
        public string Longitude { get; set; }

        /// <summary>
        /// The State of this location
        /// </summary>
        public string AdminDistrictOne { get; set; }

        /// <summary>
        /// The Zipcode of this location
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        /// The Country code of this location
        /// </summary>
        public string CountryCode { get; set; }

        /// <summary>
        /// The list of ContactInfo associated with this Location 
        /// </summary>
        public List<ContactInfo> ContactInfoSet { get; set; }

        public Region Region { get; set; }

        public Guid? ClientId { get; set; }

        public int? StatusInt { get; set; }

        /// <summary>
        /// Needed for the Geocoder
        /// </summary>
        public bool IsNew { get; set; }

        public Location()
        {
            ContactInfoSet = new List<ContactInfo>();
            CreatedDate = DateTime.UtcNow;
        }

        public static Core.Models.CoreEntities.Location ConvertBack(Location location)
        {
            Decimal? latitude;
            Decimal? longitude;
            Decimal tempLatLong = 0;

            Decimal.TryParse(location.Latitude, out tempLatLong);
            latitude = tempLatLong == 0 ? (decimal?) null : tempLatLong;

            Decimal.TryParse(location.Longitude, out tempLatLong);
            longitude = tempLatLong == 0 ? (decimal?) null : tempLatLong;

            var newLocation = new Core.Models.CoreEntities.Location
            {
                Id = location.Id,
                AddressLineOne = location.AddressLineOne,
                AddressLineTwo = location.AddressLineTwo,
                Name = location.Name,
                AdminDistrictOne = location.AdminDistrictOne,
                AdminDistrictTwo = location.AdminDistrictTwo,
                PostalCode = location.PostalCode,
                CountryCode = location.CountryCode,
                Latitude = latitude,
                Longitude = longitude,
                CreatedDate = location.CreatedDate,
                LastModified = location.LastModified,
                LastModifyingUserId = location.LastModifyingUserId,
                ClientId = location.ClientId
            };

            return newLocation;
        }

        public static Location ConvertModel(Core.Models.CoreEntities.Location locationModel)
        {
            var location = new Location
            {
                Id = locationModel.Id,
                CreatedDate = locationModel.CreatedDate,
                Name = locationModel.Name,
                AddressLineOne = locationModel.AddressLineOne,
                AddressLineTwo = locationModel.AddressLineTwo,
                Longitude = locationModel.Longitude.ToString(),
                Latitude = locationModel.Latitude.ToString(),
                AdminDistrictTwo = locationModel.AdminDistrictTwo,
                AdminDistrictOne = locationModel.AdminDistrictOne,
                CountryCode = locationModel.CountryCode,
                PostalCode = locationModel.PostalCode,
                ClientId = locationModel.ClientId
            };

            location.SetLastModified(locationModel.LastModified, locationModel.LastModifyingUserId);

            foreach (var contactInfo in locationModel.ContactInfoSet)
                location.ContactInfoSet.Add(ContactInfo.Convert(contactInfo));

            return location;
        }

        public static Location ConvertGeocode(FoundOps.Common.NET.GeocoderResult geocoderResult)
        {
            Decimal tempLatLong = 0;

            Decimal.TryParse(geocoderResult.Latitude, out tempLatLong);
            var latitude = tempLatLong == 0 ? (decimal?)null : tempLatLong;

            Decimal.TryParse(geocoderResult.Longitude, out tempLatLong);
            var longitude = tempLatLong == 0 ? (decimal?)null : tempLatLong;

            var location = new Location
            {
                Id = Guid.NewGuid(),
                AddressLineOne = geocoderResult.AddressLineOne,
                AddressLineTwo = geocoderResult.AddressLineTwo,
                AdminDistrictTwo = geocoderResult.City,
                AdminDistrictOne = geocoderResult.State,
                CountryCode = geocoderResult.CountryCode,
                PostalCode = geocoderResult.ZipCode,
                Latitude = latitude != null ? Decimal.Round((decimal) latitude, 8).ToString() : String.Empty,
                Longitude = longitude != null ? Decimal.Round((decimal) longitude, 8).ToString() : String.Empty,
                IsNew = true
            };

            if (location.CountryCode == "United States")
                location.CountryCode = "US";

            return location;
        }

        public void SetLastModified(DateTime? lastModified, Guid? userId)
        {
            LastModified = lastModified;
            LastModifyingUserId = userId;
        }
    }
}
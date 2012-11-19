using System;
using System.Collections.Generic;

namespace FoundOps.Api.Models
{
    public class Location : ITrackable
    {
        /// <summary>
        /// The Id
        /// </summary>
        public Guid Id { get; set; }

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
        public string ZipCode { get; set; }

        /// <summary>
        /// The Country code of this location
        /// </summary>
        public string CountryCode { get; set; }

        /// <summary>
        /// The list of ContactInfo associated with this Location 
        /// </summary>
        public List<ContactInfo> ContactInfoSet { get; set; }

        public DateTime CreatedDate { get; private set; }
        public DateTime? LastModified { get; private set; }
        public Guid? LastModifyingUserId { get; private set; }
        
        public void SetLastModified(DateTime? lastModified, Guid? userId)
        {
            LastModified = lastModified;
            LastModifyingUserId = userId;
        }

        public Location(DateTime createdDate)
        {
            ContactInfoSet = new List<ContactInfo>();
            CreatedDate = createdDate;
        }

        public static Core.Models.CoreEntities.Location ConvertBack(Location location)
        {
            var newLocation = new Core.Models.CoreEntities.Location
            {
                Id = location.Id,
                AddressLineOne = location.AddressLineOne,
                AddressLineTwo = location.AddressLineTwo,
                Name = location.Name,
                AdminDistrictOne = location.AdminDistrictOne,
                AdminDistrictTwo = location.AdminDistrictTwo,
                PostalCode = location.ZipCode,
                CountryCode = location.CountryCode,
                Latitude = Convert.ToDecimal(location.Latitude),
                Longitude = Convert.ToDecimal(location.Longitude),
                CreatedDate = location.CreatedDate,
                LastModified = location.LastModified,
                LastModifyingUserId = location.LastModifyingUserId
            };

            return newLocation;
        }

        public static Location ConvertModel(Core.Models.CoreEntities.Location locationModel)
        {
            var location = new Location(locationModel.CreatedDate)
            {
                Id = locationModel.Id,
                Name = locationModel.Name,
                AddressLineOne = locationModel.AddressLineOne,
                AddressLineTwo = locationModel.AddressLineTwo,
                Longitude = locationModel.Longitude.ToString(),
                Latitude = locationModel.Latitude.ToString(),
                AdminDistrictTwo = locationModel.AdminDistrictTwo,
                AdminDistrictOne = locationModel.AdminDistrictOne,
                CountryCode = locationModel.CountryCode,
                ZipCode = locationModel.PostalCode
            };

            location.SetLastModified(locationModel.LastModified, locationModel.LastModifyingUserId);

            foreach (var contactInfo in locationModel.ContactInfoSet)
                location.ContactInfoSet.Add(ContactInfo.Convert(contactInfo));

            return location;
        }

        public static Location ConvertGeocode(FoundOps.Common.NET.GeocoderResult geocoderResult)
        {
            var location = new Location(DateTime.UtcNow)
            {
                AddressLineOne = geocoderResult.AddressLineOne,
                AddressLineTwo = geocoderResult.AddressLineTwo,
                AdminDistrictTwo = geocoderResult.City,
                AdminDistrictOne = geocoderResult.State,
                CountryCode = geocoderResult.CountryCode,
                ZipCode = geocoderResult.ZipCode,
                Latitude = Decimal.Round(Convert.ToDecimal(geocoderResult.Latitude), 8).ToString(),
                Longitude = Decimal.Round(Convert.ToDecimal(geocoderResult.Longitude), 8).ToString()
            };

            if (location.CountryCode == "United States")
                location.CountryCode = "US";

            return location;
        }
    }
}
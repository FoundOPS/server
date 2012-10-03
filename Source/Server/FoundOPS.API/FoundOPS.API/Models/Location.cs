﻿using System;
using System.Collections.Generic;

namespace FoundOPS.API.Models
{
    public class Location
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
        public string PostalCode { get; set; }

        /// <summary>
        /// The Country code of this location
        /// </summary>
        public string CountryCode { get; set; }

        /// <summary>
        /// The list of ContactInfo associated with this Location 
        /// </summary>
        public List<ContactInfo> ContactInfoSet { get; set; }

        public Location()
        {
            ContactInfoSet = new List<ContactInfo>();
        }

        public static Location ConvertModel(FoundOps.Core.Models.CoreEntities.Location locationModel)
        {
            var location = new Location
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
                PostalCode = locationModel.PostalCode
            };

            foreach (var contactInfo in locationModel.ContactInfoSet)
                location.ContactInfoSet.Add(ContactInfo.Convert(contactInfo));

            return location;
        }
    }
}

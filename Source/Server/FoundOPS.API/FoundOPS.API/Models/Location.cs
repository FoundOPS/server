using System.Collections.Generic;

namespace FoundOPS.API.Models
{
    public class Location
    {
        public string Name { get; set; }
        public string AddressLineOne { get; set; }
        public string AddressLineTwo { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }

        public List<ContactInfo> ContactInfoSet { get; set; }

        public Location()
        {
            ContactInfoSet = new List<ContactInfo>();
        }

        public static Location ConvertModel(FoundOps.Core.Models.CoreEntities.Location locationModel)
        {
            var location = new Location
            {
                Name = locationModel.Name,
                AddressLineOne = locationModel.AddressLineOne,
                AddressLineTwo = locationModel.AddressLineTwo,
                City = locationModel.City,
                State = locationModel.State,
                ZipCode = locationModel.ZipCode
            };

            foreach (var contactInfo in locationModel.ContactInfoSet)
                location.ContactInfoSet.Add(ContactInfo.Convert(contactInfo));

            return location;
        }
    }
}

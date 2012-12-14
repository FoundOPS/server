using System;
using System.Linq;
using System.Collections.Generic;

namespace FoundOps.Core.Models.CoreEntities.DesignData
{
    /// <summary>
    /// Sets up design locations
    /// </summary>
    public class LocationsDesignData
    {
        private readonly List<Location> _designLocations = new List<Location>();
        /// <summary>
        /// A list of design locations.
        /// </summary>
        public IEnumerable<Location> DesignLocations { get { return _designLocations; } }

        public LocationsDesignData(Client client)
            : this(client, new RegionsDesignData().DesignRegions, 0, 5)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocationsDesignData"/> class.
        /// </summary>
        /// <param name="client">The client that resides at the locations.</param>
        /// <param name="regions">The regions for the locations.</param>
        /// <param name="startIndex">The first location to add</param>
        /// <param name="numberItems">The number of locations to add</param>
        public LocationsDesignData(Client client, IEnumerable<Region> regions, int startIndex, int numberItems)
        {
            InitializeLocations(client.BusinessAccountId.Value);

            var random = new Random();

            var locationsToConsider = DesignLocations.Skip(startIndex).Take(numberItems).ToArray();

            //Go through each location in locationsToConsider and assign it to the Client and Add SubLocations
            foreach (var location in locationsToConsider)
            {
                //Assign location to client
                client.Locations.Add(location);

                //Add SubLocations
                var subLocationsDesignData = new SubLocationsDesignData();
                foreach (var subLocation in subLocationsDesignData.DesignSubLocations)
                    location.SubLocations.Add(subLocation);

                //Setup the Region on the location to be a random Rsegion
                location.Region = regions.ElementAt(random.Next(0, 8));
            }
        }

        private void InitializeLocations(Guid businessAccountId)
        {
            _designLocations.Add(new Location
            {
                Name = "Adelino's Old World Kitchen",
                AddressLineOne = "112 N 3rd St",
                AdminDistrictTwo = "Lafayette",
                AdminDistrictOne = "IN",
                PostalCode = "47901",
                CountryCode = "US",
                Latitude = (decimal?)40.41853713,
                Longitude = (decimal?)-86.89459228,
                BusinessAccountId = businessAccountId,
                CreatedDate = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            });

            _designLocations.Add(new Location
            {
                Name = "Apollo Lounge",
                AddressLineOne = "1758 N Lancer St",
                AdminDistrictTwo = "Peru",
                AdminDistrictOne = "IN",
                PostalCode = "46970-3668",
                CountryCode = "US",
                Latitude = (decimal?)40.67115783,
                Longitude = (decimal?)-86.14487457,
                BusinessAccountId = businessAccountId,
                CreatedDate = DateTime.UtcNow,
                LastModified = DateTime.UtcNow

            });

            _designLocations.Add(new Location
            {
                Name = "Arctic White Soft Serve Ice",
                AddressLineOne = "1108 E Markland Ave",
                AdminDistrictTwo = "Kokomo",
                AdminDistrictOne = "IN",
                PostalCode = "46901-6220",
                CountryCode = "US",
                Latitude = (decimal?)40.47665297,
                Longitude = (decimal?)-86.11924856,
                BusinessAccountId = businessAccountId,
                CreatedDate = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            });

            _designLocations.Add(new Location
            {
                Name = "Akropolis Restaraunt",
                AddressLineOne = "3311 South St",
                AdminDistrictTwo = "Lafayette",
                AdminDistrictOne = "IN",
                PostalCode = "47904-3158",
                CountryCode = "US",
                Latitude = (decimal?)40.41729354,
                Longitude = (decimal?)-86.85967254,
                BusinessAccountId = businessAccountId,
                CreatedDate = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            });

            _designLocations.Add(new Location
            {
                Name = "Arby's",
                AddressLineOne = "2219 Sagamore Pky S",
                AdminDistrictTwo = "Lafayette",
                AdminDistrictOne = "IN",
                PostalCode = "47905-5111",
                CountryCode = "US",
                Latitude = (decimal?)40.39482879,
                Longitude = (decimal?)-86.85511779,
                BusinessAccountId = businessAccountId,
                CreatedDate = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            });

            _designLocations.Add(new Location
            {
                Name = "Applebee's Neighborhood Grill",
                AddressLineOne = "2432 E Wabash St",
                AdminDistrictTwo = "Frankfort",
                AdminDistrictOne = "IN",
                PostalCode = "46041-9429",
                CountryCode = "US",
                Latitude = (decimal?)40.27932357,
                Longitude = (decimal?)-86.47940826,
                BusinessAccountId = businessAccountId,
                CreatedDate = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            });

            _designLocations.Add(new Location
            {
                Name = "Black Sparrow",
                AddressLineOne = "223 Main St",
                AdminDistrictTwo = "Lafayette",
                AdminDistrictOne = "IN",
                PostalCode = "47901-1261",
                CountryCode = "US",
                Latitude = (decimal?)40.41908204,
                Longitude = (decimal?)-86.89462892,
                BusinessAccountId = businessAccountId,
                CreatedDate = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            });

            _designLocations.Add(new Location
            {
                Name = "Artie's Tenderloin",
                AddressLineOne = "922 S Main St",
                AdminDistrictTwo = "Kokomo",
                AdminDistrictOne = "IN",
                PostalCode = "46901-5457",
                CountryCode = "US",
                Latitude = (decimal?)40.47738119,
                Longitude = (decimal?)-86.13095507,
                BusinessAccountId = businessAccountId,
                CreatedDate = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            });

            _designLocations.Add(new Location
            {
                Name = "Bistro 501",
                AddressLineOne = "501 Main St",
                AdminDistrictTwo = "Lafayette",
                AdminDistrictOne = "IN",
                PostalCode = "47901-1446",
                CountryCode = "US",
                Latitude = (decimal?)40.41910484,
                Longitude = (decimal?)-86.89167983,
                BusinessAccountId = businessAccountId,
                CreatedDate = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            });

            _designLocations.Add(new Location
            {
                Name = "Blue Fin Bistro Sushi Lafayette",
                AddressLineOne = "2 S 4th St",
                AdminDistrictTwo = "Lafayette",
                AdminDistrictOne = "IN",
                PostalCode = "47901-1603",
                CountryCode = "US",
                Latitude = (decimal?)40.41706848,
                Longitude = (decimal?)-86.89337158,
                BusinessAccountId = businessAccountId,
                CreatedDate = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            });

            _designLocations.Add(new Location
            {
                Name = "Arthur's",
                AddressLineOne = "111 E Main St",
                AdminDistrictTwo = "Crawfordsville",
                AdminDistrictOne = "IN",
                PostalCode = "47933-1710",
                CountryCode = "US",
                Latitude = (decimal?)40.04165902,
                Longitude = (decimal?)-86.90108232,
                BusinessAccountId = businessAccountId,
                CreatedDate = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            });

            _designLocations.Add(new Location
            {
                Name = "Barcelona Tapas",
                AddressLineOne = "201 N Delaware St",
                AdminDistrictTwo = "Indianapolis",
                AdminDistrictOne = "IN",
                PostalCode = "46204-2127",
                CountryCode = "US",
                Latitude = (decimal?)39.77000808,
                Longitude = (decimal?)-86.15395355,
                BusinessAccountId = businessAccountId,
                CreatedDate = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            });

            _designLocations.Add(new Location
            {
                Name = "Bruno's Pizza and Big O's Sports Room",
                AddressLineOne = "212 Brown St",
                AdminDistrictTwo = "West Lafayette",
                AdminDistrictOne = "IN",
                PostalCode = "47906-3210",
                CountryCode = "US",
                Latitude = (decimal?)40.42228698,
                Longitude = (decimal?)-86.90281677,
                BusinessAccountId = businessAccountId,
                CreatedDate = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            });

            _designLocations.Add(new Location
            {
                Name = "Bob Evans Restaurant",
                AddressLineOne = "4300 State Road 26 E",
                AdminDistrictTwo = "Lafayette",
                AdminDistrictOne = "IN",
                PostalCode = "47905-4819",
                CountryCode = "US",
                Latitude = (decimal?)40.41823196,
                Longitude = (decimal?)-86.82394409,
                BusinessAccountId = businessAccountId,
                CreatedDate = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            });

            _designLocations.Add(new Location
            {
                Name = "Burger King",
                AddressLineOne = "2338 Teal Rd",
                AdminDistrictTwo = "Lafayette",
                AdminDistrictOne = "IN",
                PostalCode = "47905-2219",
                CountryCode = "US",
                Latitude = (decimal?)40.39592361,
                Longitude = (decimal?)-86.86879730,
                BusinessAccountId = businessAccountId,
                CreatedDate = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            });

            _designLocations.Add(new Location
            {
                Name = "Buffalo Wild Wings Grill & Bar",
                AddressLineOne = "2715 S Creasy Ln",
                AdminDistrictTwo = "Lafayette",
                AdminDistrictOne = "IN",
                PostalCode = "47905-5201",
                CountryCode = "US",
                Latitude = (decimal?)40.38864970,
                Longitude = (decimal?)-86.83890201,
                BusinessAccountId = businessAccountId,
                CreatedDate = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            });

            _designLocations.Add(new Location
            {
                Name = "Campbell's On Main Street",
                AddressLineOne = "101 E Main St",
                AdminDistrictTwo = "Crawfordsville",
                AdminDistrictOne = "IN",
                PostalCode = "47933-1710",
                CountryCode = "US",
                Latitude = (decimal?)40.04165768,
                Longitude = (decimal?)-86.90119966,
                BusinessAccountId = businessAccountId,
                CreatedDate = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            });

            _designLocations.Add(new Location
            {
                Name = "Buca di Beppo - Downtown Indianapolis",
                AddressLineOne = "35 N Illinois St",
                AdminDistrictTwo = "Indianapolis",
                AdminDistrictOne = "IN",
                PostalCode = "46204-2803",
                CountryCode = "US",
                Latitude = (decimal?)39.76818466,
                Longitude = (decimal?)-86.15950012,
                BusinessAccountId = businessAccountId,
                CreatedDate = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            });

            _designLocations.Add(new Location
            {
                Name = "Covered Bridge Restaurant",
                AddressLineOne = "5787 N Main St",
                AdminDistrictTwo = "Cayuga",
                AdminDistrictOne = "IN",
                PostalCode = "47928-8024",
                CountryCode = "US",
                Latitude = (decimal?)39.96863454,
                Longitude = (decimal?)-87.47246928,
                BusinessAccountId = businessAccountId,
                CreatedDate = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            });

            _designLocations.Add(new Location
            {
                Name = "El Rodeo",
                AddressLineOne = "140 Howard Ave",
                AdminDistrictTwo = "West Lafayette",
                AdminDistrictOne = "IN",
                PostalCode = "47906-3218",
                CountryCode = "US",
                Latitude = (decimal?)40.42298889,
                Longitude = (decimal?)-86.90251922,
                BusinessAccountId = businessAccountId,
                CreatedDate = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            });

            _designLocations.Add(new Location
            {
                Name = "Dairy Queen",
                AddressLineOne = "3949 State Road 38 E",
                AdminDistrictTwo = "Lafayette",
                AdminDistrictOne = "IN",
                PostalCode = "47905-9464",
                CountryCode = "US",
                Latitude = (decimal?)40.38812600,
                Longitude = (decimal?)-86.83719545,
                BusinessAccountId = businessAccountId,
                CreatedDate = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            });

            _designLocations.Add(new Location
            {
                Name = "Diamond Coffee Company",
                AddressLineOne = "3107 Builder Dr",
                AdminDistrictTwo = "Lafayette",
                AdminDistrictOne = "IN",
                PostalCode = "47909",
                CountryCode = "US",
                Latitude = (decimal?)40.36786651,
                Longitude = (decimal?)-86.86218261,
                BusinessAccountId = businessAccountId,
                CreatedDate = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            });

            _designLocations.Add(new Location
            {
                Name = "Fiesta Mexican Grill",
                AddressLineOne = "102 North Chauncey Avenue",
                AdminDistrictTwo = "West Lafayette",
                AdminDistrictOne = "IN",
                PostalCode = "47906",
                CountryCode = "US",
                Latitude = (decimal?)40.42407028,
                Longitude = (decimal?)-86.90685108,
                BusinessAccountId = businessAccountId,
                CreatedDate = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            });

            _designLocations.Add(new Location
            {
                Name = "Culver's",
                AddressLineOne = "1855 S Us-231",
                AdminDistrictTwo = "Crawfordsville",
                AdminDistrictOne = "IN",
                PostalCode = "47933-9424",
                CountryCode = "US",
                Latitude = (decimal?)40.01213394,
                Longitude = (decimal?)-86.90387651,
                BusinessAccountId = businessAccountId,
                CreatedDate = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            });

            foreach (var location in DesignLocations)
            {
                //Add contact info to Location
                var designContactInfoData = new ContactInfoDesignData();
                foreach (var contactInfo in designContactInfoData.DesignBusinessContactInfoList)
                    location.ContactInfoSet.Add(contactInfo);
            }
        }
    }
}
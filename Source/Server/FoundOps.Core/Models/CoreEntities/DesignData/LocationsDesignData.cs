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
                Latitude = (decimal?)40.418414,
                Longitude = (decimal?)-86.894219,
                BusinessAccountId = businessAccountId
            });

            _designLocations.Add(new Location
            {
                Name = "Apollo Lounge",
                AddressLineOne = "1758 N Lancer St",
                AdminDistrictTwo = "Peru",
                AdminDistrictOne = "IN",
                PostalCode = "46970-3668",
                CountryCode = "US",
                Latitude = (decimal?)40.671159,
                Longitude = (decimal?)-86.145577,
                BusinessAccountId = businessAccountId

            });

            _designLocations.Add(new Location
            {
                Name = "Arctic White Soft Serve Ice",
                AddressLineOne = "1108 E Markland Ave",
                AdminDistrictTwo = "Kokomo",
                AdminDistrictOne = "IN",
                PostalCode = "46901-6220",
                CountryCode = "US",
                Latitude = (decimal?)40.476616,
                Longitude = (decimal?)-86.119251,
                BusinessAccountId = businessAccountId
            });

            _designLocations.Add(new Location
            {
                Name = "Akropolis Restaraunt",
                AddressLineOne = "3311 South St",
                AdminDistrictTwo = "Lafayette",
                AdminDistrictOne = "IN",
                PostalCode = "47904-3158",
                CountryCode = "US",
                Latitude = (decimal?)40.417552,
                Longitude = (decimal?)-86.859526,
                BusinessAccountId = businessAccountId
            });

            _designLocations.Add(new Location
            {
                Name = "Arby's",
                AddressLineOne = "2219 Sagamore Pky S",
                AdminDistrictTwo = "Lafayette",
                AdminDistrictOne = "IN",
                PostalCode = "47905-5111",
                CountryCode = "US",
                Latitude = (decimal?)40.3955,
                Longitude = (decimal?)-86.855914,
                BusinessAccountId = businessAccountId
            });

            _designLocations.Add(new Location
            {
                Name = "Applebee's Neighborhood Grill",
                AddressLineOne = "2432 E Wabash St",
                AdminDistrictTwo = "Frankfort",
                AdminDistrictOne = "IN",
                PostalCode = "46041-9429",
                CountryCode = "US",
                Latitude = (decimal?)40.278846,
                Longitude = (decimal?)-86.479537,
                BusinessAccountId = businessAccountId
            });

            _designLocations.Add(new Location
            {
                Name = "Black Sparrow",
                AddressLineOne = "223 Main St",
                AdminDistrictTwo = "Lafayette",
                AdminDistrictOne = "IN",
                PostalCode = "47901-1261",
                CountryCode = "US",
                Latitude = (decimal?)40.419119,
                Longitude = (decimal?)-86.894604,
                BusinessAccountId = businessAccountId
            });

            _designLocations.Add(new Location
            {
                Name = "Artie's Tenderloin",
                AddressLineOne = "922 S Main St",
                AdminDistrictTwo = "Kokomo",
                AdminDistrictOne = "IN",
                PostalCode = "46901-5457",
                CountryCode = "US",
                Latitude = (decimal?)40.477384,
                Longitude = (decimal?)-86.131005,
                BusinessAccountId = businessAccountId
            });

            _designLocations.Add(new Location
            {
                Name = "Bistro 501",
                AddressLineOne = "501 Main St",
                AdminDistrictTwo = "Lafayette",
                AdminDistrictOne = "IN",
                PostalCode = "47901-1446",
                CountryCode = "US",
                Latitude = (decimal?)40.419142,
                Longitude = (decimal?)-86.891727,
                BusinessAccountId = businessAccountId
            });

            _designLocations.Add(new Location
            {
                Name = "Blue Fin Bistro Sushi Lafayette",
                AddressLineOne = "2 S 4th St",
                AdminDistrictTwo = "Lafayette",
                AdminDistrictOne = "IN",
                PostalCode = "47901-1603",
                CountryCode = "US",
                Latitude = (decimal?)40.417224,
                Longitude = (decimal?)-86.893046,
                BusinessAccountId = businessAccountId
            });

            _designLocations.Add(new Location
            {
                Name = "Arthur's",
                AddressLineOne = "111 E Main St",
                AddressLineTwo = "Crawfordsville",
                AdminDistrictTwo = "Lafayette",
                AdminDistrictOne = "IN",
                PostalCode = "47933-1710",
                CountryCode = "US",
                Latitude = (decimal?)40.041696,
                Longitude = (decimal?)-86.901095,
                BusinessAccountId = businessAccountId
            });

            _designLocations.Add(new Location
            {
                Name = "Barcelona Tapas",
                AddressLineOne = "201 N Delaware St",
                AdminDistrictTwo = "Indianapolis",
                AdminDistrictOne = "IN",
                PostalCode = "46204-2127",
                CountryCode = "US",
                Latitude = (decimal?)39.769887,
                Longitude = (decimal?)-86.154359,
                BusinessAccountId = businessAccountId
            });

            _designLocations.Add(new Location
            {
                Name = "Bruno's Pizza and Big O's Sports Room",
                AddressLineOne = "212 Brown St",
                AdminDistrictTwo = "West Lafayette",
                AdminDistrictOne = "IN",
                PostalCode = "47906-3210",
                CountryCode = "US",
                Latitude = (decimal?)40.422081,
                Longitude = (decimal?)-86.903569,
                BusinessAccountId = businessAccountId
            });

            _designLocations.Add(new Location
            {
                Name = "Bob Evans Restaurant",
                AddressLineOne = "4300 State Road 26 E",
                AdminDistrictTwo = "Lafayette",
                AdminDistrictOne = "IN",
                PostalCode = "47905-4819",
                CountryCode = "US",
                Latitude = (decimal?)40.417733,
                Longitude = (decimal?)-86.824187,
                BusinessAccountId = businessAccountId
            });

            _designLocations.Add(new Location
            {
                Name = "Burger King",
                AddressLineOne = "2338 Teal Rd",
                AdminDistrictTwo = "Lafayette",
                AdminDistrictOne = "IN",
                PostalCode = "47905-2219",
                CountryCode = "US",
                Latitude = (decimal?)40.395504,
                Longitude = (decimal?)-86.868527,
                BusinessAccountId = businessAccountId
            });

            _designLocations.Add(new Location
            {
                Name = "Buffalo Wild Wings Grill & Bar",
                AddressLineOne = "2715 S Creasy Ln",
                AdminDistrictTwo = "Lafayette",
                AdminDistrictOne = "IN",
                PostalCode = "47905-5201",
                CountryCode = "US",
                Latitude = (decimal?)40.388618,
                Longitude = (decimal?)-86.838981,
                BusinessAccountId = businessAccountId
            });

            _designLocations.Add(new Location
            {
                Name = "Campbell's On Main Street",
                AddressLineOne = "101 E Main St",
                AdminDistrictTwo = "Crawfordsville",
                AdminDistrictOne = "IN",
                PostalCode = "47933-1710",
                CountryCode = "US",
                Latitude = (decimal?)40.041694,
                Longitude = (decimal?)-86.901216,
                BusinessAccountId = businessAccountId
            });

            _designLocations.Add(new Location
            {
                Name = "Buca di Beppo - Downtown Indianapolis",
                AddressLineOne = "35 N Illinois St",
                AdminDistrictTwo = "Indianapolis",
                AdminDistrictOne = "IN",
                PostalCode = "46204-2803",
                CountryCode = "US",
                Latitude = (decimal?)39.768098,
                Longitude = (decimal?)-86.159864,
                BusinessAccountId = businessAccountId
            });

            _designLocations.Add(new Location
            {
                Name = "Covered Bridge Restaurant",
                AddressLineOne = "5787 N Main St",
                AdminDistrictTwo = "Cayuga",
                AdminDistrictOne = "IN",
                PostalCode = "47928-8024",
                CountryCode = "US",
                Latitude = (decimal?)39.968641,
                Longitude = (decimal?)-87.472417,
                BusinessAccountId = businessAccountId
            });

            _designLocations.Add(new Location
            {
                Name = "El Rodeo",
                AddressLineOne = "140 Howard Ave",
                AdminDistrictTwo = "West Lafayette",
                AdminDistrictOne = "IN",
                PostalCode = "47906-3218",
                CountryCode = "US",
                Latitude = (decimal?)40.422733,
                Longitude = (decimal?)-86.902758,
                BusinessAccountId = businessAccountId
            });

            _designLocations.Add(new Location
            {
                Name = "Dairy Queen",
                AddressLineOne = "3949 State Road 38 E",
                AdminDistrictTwo = "Lafayette",
                AdminDistrictOne = "IN",
                PostalCode = "47905-9464",
                CountryCode = "US",
                Latitude = (decimal?)40.388141,
                Longitude = (decimal?)-86.837137,
                BusinessAccountId = businessAccountId
            });

            _designLocations.Add(new Location
            {
                Name = "Diamond Coffee Company",
                AddressLineOne = "3107 Builder Dr",
                AdminDistrictTwo = "Lafayette",
                AdminDistrictOne = "IN",
                PostalCode = "47909",
                CountryCode = "US",
                Latitude = (decimal?)40.367866516113281,
                Longitude = (decimal?)-86.8621826171875,
                BusinessAccountId = businessAccountId
            });

            _designLocations.Add(new Location
            {
                Name = "Crawfordsville Forum Family",
                AddressLineOne = "1410 Darlington Ave",
                AdminDistrictTwo = "Crawfordsville",
                AdminDistrictOne = "IN",
                PostalCode = "47933-2007",
                CountryCode = "US",
                Latitude = (decimal?)40.046804,
                Longitude = (decimal?)-86.879933,
                BusinessAccountId = businessAccountId
            });

            _designLocations.Add(new Location
            {
                Name = "Culver's",
                AddressLineOne = "1855 S Us-231",
                AdminDistrictTwo = "Crawfordsville",
                AdminDistrictOne = "IN",
                PostalCode = "47933-9424",
                CountryCode = "US",
                Latitude = (decimal?)40.012145,
                Longitude = (decimal?)-86.903825,
                BusinessAccountId = businessAccountId
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
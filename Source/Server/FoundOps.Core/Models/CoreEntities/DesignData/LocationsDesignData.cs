using System.Linq;
using System.Collections.Generic;

namespace FoundOps.Core.Models.CoreEntities.DesignData
{
    public class LocationsDesignData
    {
        public Location DesignLocation { get; private set; }
        public Location DesignLocationTwo { get; private set; }
        public Location DesignLocationThree { get; private set; }
        public Location DesignLocationFour { get; private set; }
        public Location DesignLocationFive { get; private set; }
        public Location DesignLocationSix { get; private set; }

        public IEnumerable<Location> DesignLocations { get; private set; }

        public LocationsDesignData()
            : this(new PartyDesignData().DesignBusinessAccount, new ClientsDesignData().DesignClient, new RegionsDesignData().DesignRegions, 0, 5)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocationsDesignData"/> class.
        /// </summary>
        /// <param name="ownerParty">The party that created the locations.</param>
        /// <param name="client">The client that resides at the locations.</param>
        /// <param name="regions">The regions for the locations.</param>
        /// <param name="startIndex">The first location to add</param>
        /// <param name="numberItems">The number of locations to add</param>
        public LocationsDesignData(Party ownerParty, Client client, IEnumerable<Region> regions, int startIndex, int numberItems)
        {
            InitializeLocations();

            var locationsToConsider = DesignLocations.Skip(startIndex).Take(numberItems).ToArray();

            //Go through each location in locationsToConsider and assign it to the Client and Add SubLocations
            foreach (var location in locationsToConsider)
            {
                //Assign location to client
                client.OwnedParty.Locations.Add(location);

                //Add SubLocations
                var subLocationsDesignData = new SubLocationsDesignData();
                foreach (var subLocation in subLocationsDesignData.DesignSubLocations)
                    location.SubLocations.Add(subLocation);
            }

            var regionIndex = 0;
            //Go through each location and assign it's OwnerParty and a Region
            foreach (var location in locationsToConsider)
            {
                location.OwnerParty = ownerParty;

                //If the regionIndex is greater than the number of Regions, reset
                if (regionIndex >= regions.Count())
                    regionIndex = 0;

                //Setup the Region on the location
                location.Region = regions.ElementAt(regionIndex);

                //Iterate to the next region
                regionIndex++;
            }
        }

        private void InitializeLocations()
        {
            DesignLocation = new Location
                                 {
                                     Name = "AEPi Purdue",
                                     AddressLineOne = "1051 David Ross Road",
                                     AddressLineTwo = "Room 17",
                                     City = "West Lafayette",
                                     State = "IN",
                                     ZipCode = "47906",
                                     Latitude = (decimal?) 40.434993,
                                     Longitude = (decimal?) -86.925282
                                 };

            DesignLocationTwo = new Location
                                    {
                                        Name = "Current Home",
                                        AddressLineOne = "12414 English Garden Court",
                                        AddressLineTwo = "Room Jon",
                                        City = "Herndon",
                                        State = "VA",
                                        ZipCode = "20171",
                                        Latitude = (decimal?) 38.898934,
                                        Longitude = (decimal?) -77.376901
                                    };

            DesignLocationThree = new Location
                                      {
                                          Name = "Old Home",
                                          AddressLineOne = "1401 Park Lake Drive",
                                          City = "Reston",
                                          State = "VA",
                                          ZipCode = "20190",
                                          Latitude = (decimal?) 38.969607,
                                          Longitude = (decimal?) -75.316368
                                      };

            DesignLocationFour = new Location
                                     {
                                         Name = "Burger King",
                                         AddressLineOne = "1401 Park Lake Drive",
                                         City = "Reston",
                                         State = "VA",
                                         ZipCode = "20190",
                                         Latitude = (decimal?) 45.969607,
                                         Longitude = (decimal?) -77.316368
                                     };

            DesignLocationFive = new Location
                                     {
                                         Name = "Lowes",
                                         AddressLineOne = "1401 Park Lake Drive",
                                         City = "Reston",
                                         State = "VA",
                                         ZipCode = "20190",
                                         Latitude = (decimal?) 43.969607,
                                         Longitude = (decimal?) -88.316368
                                     };

            DesignLocationSix = new Location
                                    {
                                        Name = "Warehouse",
                                        AddressLineOne = "1401 Park Lake Drive",
                                        City = "Reston",
                                        State = "VA",
                                        ZipCode = "20190",
                                        Latitude = (decimal?) 40.969607,
                                        Longitude = (decimal?) -84.316368
                                    };

            DesignLocations = new List<Location>
                                  {
                                      DesignLocation,
                                      DesignLocationTwo,
                                      DesignLocationThree,
                                      DesignLocationFour,
                                      DesignLocationFive,
                                      DesignLocationSix
                                  };

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
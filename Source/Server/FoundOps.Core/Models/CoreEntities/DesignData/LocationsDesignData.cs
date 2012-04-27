using System;
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
        public Location DesignLocationSeven { get; private set; }
        public Location DesignLocationEight { get; private set; }
        public Location DesignLocationNine { get; private set; }
        public Location DesignLocationTen { get; private set; }
        public Location DesignLocationEleven { get; private set; }
        public Location DesignLocationTwelve { get; private set; }
        public Location DesignLocationThirteen { get; private set; }
        public Location DesignLocationFourteen { get; private set; }
        public Location DesignLocationFifteen { get; private set; }
        public Location DesignLocationSixteen { get; private set; }
        public Location DesignLocationSeventeen { get; private set; }
        public Location DesignLocationEighteen { get; private set; }
        public Location DesignLocationNineteen { get; private set; }
        public Location DesignLocationTwenty { get; private set; }
        public Location DesignLocationTwentyOne { get; private set; }
        public Location DesignLocationTwentyTwo { get; private set; }
        public Location DesignLocationTwentyThree { get; private set; }
        public Location DesignLocationTwentyFour { get; private set; }

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

            var random = new Random();

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

            //Go through each location and assign it's OwnerParty and a Region
            foreach (var location in locationsToConsider)
            {
                location.OwnerParty = ownerParty;

                //Setup the Region on the location to be a random Rsegion
                location.Region = regions.ElementAt(random.Next(0, 8));
            }
        }

        private void InitializeLocations()
        {
            DesignLocation = new Location
                                 {
                                     Name = "Adelino's Old World Kitchen",
                                     AddressLineOne = "112 N 3rd St",
                                     City = "Lafayette",
                                     State = "IN",
                                     ZipCode = "47901",
                                     Latitude = (decimal?)40.418414,
                                     Longitude = (decimal?)-86.894219
                                 };

            DesignLocationTwo = new Location
                                    {
                                        Name = "Apollo Lounge",
                                        AddressLineOne = "1758 N Lancer St",
                                        City = "Peru",
                                        State = "IN",
                                        ZipCode = "46970-3668",
                                        Latitude = (decimal?)40.671159,
                                        Longitude = (decimal?)-86.145577

                                    };

            DesignLocationThree = new Location
                                      {
                                          Name = "Arctic White Soft Serve Ice",
                                          AddressLineOne = "1108 E Markland Ave",
                                          City = "Kokomo",
                                          State = "IN",
                                          ZipCode = "46901-6220",
                                          Latitude = (decimal?)40.476616,
                                          Longitude = (decimal?)-86.119251
                                      };

            DesignLocationFour = new Location
                                     {
                                         Name = "Akropolis Restaraunt",
                                         AddressLineOne = "3311 South St",
                                         City = "Lafayette",
                                         State = "IN",
                                         ZipCode = "47904-3158",
                                         Latitude = (decimal?)40.417552,
                                         Longitude = (decimal?)-86.859526
                                     };

            DesignLocationFive = new Location
                                     {
                                         Name = "Arby's",
                                         AddressLineOne = "2219 Sagamore Pky S",
                                         City = "Lafayette",
                                         State = "IN",
                                         ZipCode = "47905-5111",
                                         Latitude = (decimal?)40.3955,
                                         Longitude = (decimal?)-86.855914
                                     };

            DesignLocationSix = new Location
                                    {
                                        Name = "Applebee's Neighborhood Grill",
                                        AddressLineOne = "2432 E Wabash St",
                                        City = "Frankfort",
                                        State = "IN",
                                        ZipCode = "46041-9429",
                                        Latitude = (decimal?)40.278846,
                                        Longitude = (decimal?)-86.479537
                                    };

            DesignLocationSeven = new Location
                                      {
                                          Name = "Black Sparrow",
                                          AddressLineOne = "223 Main St",
                                          City = "Lafayette",
                                          State = "IN",
                                          ZipCode = "47901-1261",
                                          Latitude = (decimal?)40.419119,
                                          Longitude = (decimal?)-86.894604
                                      };

            DesignLocationEight = new Location
                                      {
                                          Name = "Artie's Tenderloin",
                                          AddressLineOne = "922 S Main St",
                                          City = "Kokomo",
                                          State = "IN",
                                          ZipCode = "46901-5457",
                                          Latitude = (decimal?)40.477384,
                                          Longitude = (decimal?)-86.131005
                                      };

            DesignLocationNine = new Location
                                      {
                                          Name = "Bistro 501",
                                          AddressLineOne = "501 Main St",
                                          City = "Lafayette",
                                          State = "IN",
                                          ZipCode = "47901-1446",
                                          Latitude = (decimal?)40.419142,
                                          Longitude = (decimal?)-86.891727
                                      };

            DesignLocationTen = new Location
                                      {
                                          Name = "Blue Fin Bistro Sushi Lafayette",
                                          AddressLineOne = "2 S 4th St",
                                          City = "Lafayette",
                                          State = "IN",
                                          ZipCode = "47901-1603",
                                          Latitude = (decimal?)40.417224,
                                          Longitude = (decimal?)-86.893046
                                      };

            DesignLocationEleven = new Location
                                      {
                                          Name = "Arthur's",
                                          AddressLineOne = "111 E Main St",
                                          AddressLineTwo = "Crawfordsville",
                                          City = "Lafayette",
                                          State = "IN",
                                          ZipCode = "47933-1710",
                                          Latitude = (decimal?)40.041696,
                                          Longitude = (decimal?)-86.901095
                                      };

            DesignLocationTwelve = new Location
                                      {
                                          Name = "Barcelona Tapas",
                                          AddressLineOne = "201 N Delaware St",
                                          City = "Indianapolis",
                                          State = "IN",
                                          ZipCode = "46204-2127",
                                          Latitude = (decimal?)39.769887,
                                          Longitude = (decimal?)-86.154359
                                      };

            DesignLocationThirteen = new Location
                                      {
                                          Name = "Bruno's Pizza and Big O's Sports Room",
                                          AddressLineOne = "212 Brown St",
                                          City = "West Lafayette",
                                          State = "IN",
                                          ZipCode = "47906-3210",
                                          Latitude = (decimal?)40.422081,
                                          Longitude = (decimal?)-86.903569
                                      };

            DesignLocationFourteen = new Location
                                      {
                                          Name = "Bob Evans Restaurant",
                                          AddressLineOne = "4300 State Road 26 E",
                                          City = "Lafayette",
                                          State = "IN",
                                          ZipCode = "47905-4819",
                                          Latitude = (decimal?)40.417733,
                                          Longitude = (decimal?)-86.824187
                                      };

            DesignLocationFifteen = new Location
                                      {
                                          Name = "Burger King",
                                          AddressLineOne = "2338 Teal Rd",
                                          City = "Lafayette",
                                          State = "IN",
                                          ZipCode = "47905-2219",
                                          Latitude = (decimal?)40.395504,
                                          Longitude = (decimal?)-86.868527
                                      };

            DesignLocationSixteen = new Location
                                      {
                                          Name = "Buffalo Wild Wings Grill & Bar",
                                          AddressLineOne = "2715 S Creasy Ln",
                                          City = "Lafayette",
                                          State = "IN",
                                          ZipCode = "47905-5201",
                                          Latitude = (decimal?)40.388618,
                                          Longitude = (decimal?)-86.838981
                                      };

            DesignLocationSeventeen = new Location
                                      {
                                          Name = "Campbell's On Main Street",
                                          AddressLineOne = "101 E Main St",
                                          City = "Crawfordsville",
                                          State = "IN",
                                          ZipCode = "47933-1710",
                                          Latitude = (decimal?)40.041694,
                                          Longitude = (decimal?)-86.901216
                                      };

            DesignLocationEighteen = new Location
                                      {
                                          Name = "Buca di Beppo - Downtown Indianapolis",
                                          AddressLineOne = "35 N Illinois St",
                                          City = "Indianapolis",
                                          State = "IN",
                                          ZipCode = "46204-2803",
                                          Latitude = (decimal?)39.768098,
                                          Longitude = (decimal?)-86.159864
                                      };

            DesignLocationNineteen = new Location
                                      {
                                          Name = "Covered Bridge Restaurant",
                                          AddressLineOne = "5787 N Main St",
                                          City = "Cayuga",
                                          State = "IN",
                                          ZipCode = "47928-8024",
                                          Latitude = (decimal?)39.968641,
                                          Longitude = (decimal?)-87.472417
                                      };

            DesignLocationTwenty = new Location
                                      {
                                          Name = "El Rodeo",
                                          AddressLineOne = "140 Howard Ave",
                                          City = "West Lafayette",
                                          State = "IN",
                                          ZipCode = "47906-3218",
                                          Latitude = (decimal?)40.422733,
                                          Longitude = (decimal?)-86.902758
                                      };

            DesignLocationTwentyOne = new Location
                                      {
                                          Name = "Dairy Queen",
                                          AddressLineOne = "3949 State Road 38 E",
                                          City = "Lafayette",
                                          State = "IN",
                                          ZipCode = "47905-9464",
                                          Latitude = (decimal?)40.388141,
                                          Longitude = (decimal?)-86.837137
                                      };

            DesignLocationTwentyTwo = new Location
                                      {
                                          Name = "Diamond Coffee Company",
                                          AddressLineOne = "3107 Builder Dr",
                                          City = "Lafayette",
                                          State = "IN",
                                          ZipCode = "47909",
                                          Latitude = (decimal?)40.368191,
                                          Longitude = (decimal?)-86.860466
                                      };

            DesignLocationTwentyThree = new Location
                                      {
                                          Name = "Crawfordsville Forum Family",
                                          AddressLineOne = "1410 Darlington Ave",
                                          City = "Crawfordsville",
                                          State = "IN",
                                          ZipCode = "47933-2007",
                                          Latitude = (decimal?)40.046804,
                                          Longitude = (decimal?)-86.879933
                                      };

            DesignLocationTwentyFour = new Location
                                      {
                                          Name = "Culver's",
                                          AddressLineOne = "1855 S Us-231",
                                          City = "Crawfordsville",
                                          State = "IN",
                                          ZipCode = "47933-9424",
                                          Latitude = (decimal?)40.012145,
                                          Longitude = (decimal?)-86.903825
                                      };

            DesignLocations = new List<Location>
                                  {
                                      DesignLocation,
                                      DesignLocationTwo,
                                      DesignLocationThree,
                                      DesignLocationFour,
                                      DesignLocationFive,
                                      DesignLocationSix,
                                      DesignLocationSeven,
                                      DesignLocationEight,
                                      DesignLocationNine,
                                      DesignLocationTen,
                                      DesignLocationEleven,
                                      DesignLocationTwelve,
                                      DesignLocationThirteen,
                                      DesignLocationFourteen,
                                      DesignLocationFifteen,
                                      DesignLocationSixteen,
                                      DesignLocationSeventeen,
                                      DesignLocationEighteen,
                                      DesignLocationNineteen,
                                      DesignLocationTwenty,
                                      DesignLocationTwentyOne,
                                      DesignLocationTwentyTwo,
                                      DesignLocationTwentyThree,
                                      DesignLocationTwentyFour
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
using System.Collections.Generic;

namespace FoundOps.Core.Models.CoreEntities.DesignData
{
    public class SubLocationsDesignData
    {

        public SubLocation DesignSubLocation { get; private set; }
        public SubLocation DesignSubLocationTwo { get; private set; }
        public SubLocation DesignSubLocationThree { get; private set; }
        public SubLocation DesignSubLocationFour { get; private set; }
        public SubLocation DesignSubLocationFive { get; private set; }
        public SubLocation DesignSubLocationSix { get; private set; }
        public SubLocation DesignSubLocationSeven { get; private set; }
        public SubLocation DesignSubLocationEight { get; private set; }
        public SubLocation DesignSubLocationNine { get; private set; }

        public IEnumerable<SubLocation> DesignSubLocations { get; private set; }

        public SubLocationsDesignData()
        {
            InitializeLocations();
        }

        private void InitializeLocations()
        {
            DesignSubLocation = new SubLocation
            {
                Name = "Grease Trap",
                Notes = "Here are some notes",
                Latitude = (decimal?) 40.454821,
                Longitude = (decimal?) -86.935696,
                Number = 1
            };

            DesignSubLocationTwo = new SubLocation
            {
                Name = "Laundy",
                Notes = "Many notes!",
                Latitude = (decimal?)38.888934,
                Longitude = (decimal?)-77.386901,
                Number = 2
            };

            DesignSubLocationThree = new SubLocation
            {
                Name = "Parking",
                Notes = "Small amount of notes",
                Latitude = (decimal?)38.979607,
                Longitude = (decimal?)-77.326368,
                Number = 3
            };
            DesignSubLocationFour = new SubLocation
            {
                Name = "Room",
                Notes = "More notes",
                Latitude = (decimal?)37.979607,
                Longitude = (decimal?)-75.326368,
                Number = 4
            };
            DesignSubLocationFive = new SubLocation
            {
                Name = "Another Room",
                Notes = "Important notes",
                Latitude = (decimal?)38.879607,
                Longitude = (decimal?)-77.226368,
                Number = 5
            };
            DesignSubLocationSix = new SubLocation
            {
                Name = "Hidden Place",
                Notes = "Unimportant notes",
                Latitude = (decimal?)39.979607,
                Longitude = (decimal?)-78.326368,
                Number = 6
            };
            DesignSubLocationSeven = new SubLocation
            {
                Name = "Go Here",
                Notes = "No notes here",
                Latitude = (decimal?)38.679607,
                Longitude = (decimal?)-77.626368,
                Number = 7
            };
            DesignSubLocationEight = new SubLocation
            {
                Name = "Don't Go Here",
                Notes = "Lots of notes",
                Latitude = (decimal?)38.379607,
                Longitude = (decimal?)-77.526368,
                Number = 8
            };
            DesignSubLocationNine = new SubLocation
            {
                Name = "Name",
                Notes = "Notes Notes",
                Latitude = (decimal?)37.979607,
                Longitude = (decimal?)-78.126368,
                Number = 9
            };

            DesignSubLocations = new List<SubLocation> { DesignSubLocation, DesignSubLocationTwo, DesignSubLocationThree, DesignSubLocationFour, DesignSubLocationFive, 
                DesignSubLocationSix, DesignSubLocationSeven, DesignSubLocationEight, DesignSubLocationNine};
        }
    }
}
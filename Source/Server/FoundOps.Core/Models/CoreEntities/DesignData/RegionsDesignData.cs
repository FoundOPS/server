using System.Collections.Generic;

namespace FoundOps.Core.Models.CoreEntities.DesignData
{
    public class RegionsDesignData
    {
        public static string[] DesignRegionColors = new[] { "FFFF0000", "FF00FF00", "FF0000FF", "FFFFFF00", "FFFF00FF", "FF00FFFF", "FF990099", "FFFFcc99"};

        public Region DesignRegion { get; private set; }
        public Region DesignRegionTwo { get; private set; }
        public Region DesignRegionThree { get; private set; }
        public Region DesignRegionFour { get; private set; }
        public Region DesignRegionFive { get; private set; }
        public Region DesignRegionSix { get; private set; }
        public Region DesignRegionSeven { get; private set; }
        public Region DesignRegionEight { get; private set; }

        public IEnumerable<Region> DesignRegions { get; private set; }

        public RegionsDesignData()
        {
            InitializeRegions();
        }

        private void InitializeRegions()
        {
            DesignRegion = new Region
            {
                Name = "South of Market",
                Color = DesignRegionColors[0],
                Notes = "Notes for this region"
            };

            DesignRegionTwo = new Region
            {
                Name = "North of Market",
                Color = DesignRegionColors[1],
                Notes = "Notes for this region"
            };

            DesignRegionThree = new Region
            {
                Name = "Richmond/Sunset",
                Color = DesignRegionColors[2],
                Notes = "Notes for this region"
            };
            DesignRegionFour = new Region
            {
                Name = "Mission/Potrero",
                Color = DesignRegionColors[3],
                Notes = "Notes for this region"
            };
            DesignRegionFive = new Region
            {
                Name = "North Beach & Piers",
                Color = DesignRegionColors[4],
                Notes = "Notes for this region"
            };
            DesignRegionSix = new Region
            {
                Name = "Marina/Western ave.",
                Color = DesignRegionColors[5],
                Notes = "Notes for this region"
            };
            DesignRegionSeven = new Region
            {
                Name = "East Bay",
                Color = DesignRegionColors[6],
                Notes = "Notes for this region"
            };
            DesignRegionEight = new Region
            {
                Name = "Peninsula",
                Color = DesignRegionColors[7],
                Notes = "Notes for this region"
            };

            DesignRegions = new List<Region> { DesignRegion, DesignRegionTwo, DesignRegionThree, DesignRegionFour, DesignRegionFive, DesignRegionSix, DesignRegionSeven, DesignRegionEight};
        }
    }
}
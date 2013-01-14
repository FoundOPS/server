using System;

namespace FoundOps.Api.Models
{
    public class Region
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }

        public static Region ConvertModel(Core.Models.CoreEntities.Region regionModel)
        {
            var region = new Region
            {
                Id = regionModel.Id,
                Name = regionModel.Name,
                Color = regionModel.Color
            };

            return region;
        }

        public static Core.Models.CoreEntities.Region ConvertBack(Region region)
        {
            var newRegion = new Core.Models.CoreEntities.Region
            {
                Id =  region.Id,
                Name = region.Name,
                Color = region.Color
            };
            return newRegion;
        }
    }
}
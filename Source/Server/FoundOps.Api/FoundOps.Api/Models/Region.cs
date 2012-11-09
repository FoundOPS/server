using System;

namespace FoundOps.Api.Models
{
    public class Region
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }

        public static Region Convert(Core.Models.CoreEntities.Region regionModel)
        {
            var region = new Region
            {
                Id = regionModel.Id,
                Name = regionModel.Name,
                Color = regionModel.Color
            };

            return region;
        }
    }
}
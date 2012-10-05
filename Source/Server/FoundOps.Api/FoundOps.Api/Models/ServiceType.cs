using System;

namespace FoundOps.Api.Models
{
    public class ServiceTemplate
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// Converts from the FoundOPS model to the API model
        /// </summary>
        public static ServiceTemplate ConvertModel(FoundOps.Core.Models.CoreEntities.ServiceTemplate model)
        {
            var serviceType = new ServiceTemplate
            {
                Id = model.Id,
                Name = model.Name
            };

            return serviceType;
        }
    }
}
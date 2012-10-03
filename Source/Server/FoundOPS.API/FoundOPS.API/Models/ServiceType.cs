using System;
using System.Collections.Generic;
using System.Linq;

namespace FoundOPS.Api.Models
{
    public class ServiceType
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// Converts from the FoundOPS model to the API model
        /// </summary>
        public static ServiceType ConvertModel(FoundOps.Core.Models.CoreEntities.ServiceTemplate model)
        {
            var serviceType = new ServiceType
            {
                Id = model.Id,
                Name = model.Name
            };

            return serviceType;
        }
    }
}
using FoundOps.Core.Models.CoreEntities;
using System;
using System.Collections.Generic;

namespace FoundOPS.API.Models
{
    public class Service
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public DateTime ServiceDate { get; set; }

        //Associations
        public Guid ClientId { get; set; }

        public List<Field> Fields { get; set; }

        public Guid? RecurringServiceId { get; set; }
        public Guid ServiceProviderId { get; set; }

        public Service()
        {
            Fields = new List<Field>();
        }

        /// <summary>
        /// Converts from the FoundOPS model to the API model
        /// </summary>
        /// <param name="serviceModel">The service to convert</param>
        /// <returns></returns>
        public static Service ConvertModel(FoundOps.Core.Models.CoreEntities.Service serviceModel)
        {
            var service = new Service
            {
                Id = serviceModel.Id,
                Name = serviceModel.ServiceTemplate.Name,
                ServiceDate = serviceModel.ServiceDate,
                ClientId = serviceModel.ClientId,
                ServiceProviderId = serviceModel.ServiceProviderId,
                RecurringServiceId = serviceModel.RecurringServiceId
            };

            var fields = serviceModel.ServiceTemplate.Fields;

            //if (serviceModel.Generated)
            //    fields = coreEntitiesContainer.Fields.Where(f => f.ServiceTemplateId == serviceModel.RecurringServiceParent.ServiceTemplate.Id);

            foreach (var field in fields)
            {
                service.Fields.Add(Field.ConvertModel(field));
            }

            return service;
        }

        ///// <summary>
        ///// Converts from the API model to the FoundOPS model
        ///// </summary>
        ///// <param name="modelService">The service to convert</param>
        ///// <returns></returns>
        //public static FoundOps.Core.Models.CoreEntities.Service ConvertToFoundOpsModel(Service modelService)
        //{
        //    var serviceTemplate = new ServiceTemplate { Name = modelService.Name, ServiceTemplateLevel = ServiceTemplateLevel.ServiceDefined };

        //    foreach (var field in modelService.Fields)
        //        serviceTemplate.Fields.Add(field.ConvertToFoundOpsModel);

        //    var service = new FoundOps.Core.Models.CoreEntities.Service
        //    {
        //        Id = modelService.Id,
        //        ClientId = modelService.ClientId,
        //        ServiceProviderId = modelService.ServiceProviderId,
        //        RecurringServiceId = modelService.RecurringServiceId,
        //        ServiceDate = modelService.ServiceDate,
        //        ServiceTemplate = serviceTemplate
        //    };

        //    return service;
        //}
    }


}
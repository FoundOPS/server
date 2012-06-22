using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FoundOps.Core.Models.CoreEntities;

namespace FoundOPS.API.Models
{
    public class Service 
    {
        public Guid Id { get; set; }

        public Guid ClientId { get; set; }

        public Guid ServiceProviderId { get; set; }

        public Guid? RecurringServiceId { get; set; }

        public DateTime ServiceDate { get; set; }

        public List<Field> Fields { get; set; }

        public Service()
        {
            Fields = new List<Field>();
        }


        public static Service ConvertModel(FoundOps.Core.Models.CoreEntities.Service serviceModel)
        {
            var coreEntitiesContainer = new CoreEntitiesContainer();

            var service = new Service { Id = serviceModel.Id, ClientId = serviceModel.ClientId, ServiceProviderId = serviceModel.ServiceProviderId, 
                RecurringServiceId = serviceModel.RecurringServiceId, ServiceDate = serviceModel.ServiceDate};

            var fields = coreEntitiesContainer.Fields.Where(f => f.ServiceTemplateId == serviceModel.ServiceTemplate.Id);

            if (serviceModel.Generated)
                fields = coreEntitiesContainer.Fields.Where(f => f.ServiceTemplateId == serviceModel.RecurringServiceParent.ServiceTemplate.Id);

            foreach (var field in fields)
            {
                service.Fields.Add(Field.ConvertModel(field));
            }

            return service;
        }
    }


}
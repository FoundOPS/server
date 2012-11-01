using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FoundOps.Api.Tools;

namespace FoundOps.Api.Models
{
    public class Service : IImportable
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public DateTime ServiceDate { get; set; }

        //Associations
        public Guid ClientId { get; set; }
        [ReadOnly(true)]
        public Client Client { get; set; }

        public List<Field> Fields { get; set; }

        public Guid? RecurringServiceId { get; set; }
        public Guid ServiceProviderId { get; set; }
        
        public int? StatusInt { get; set; }

        public Service()
        {
            Fields = new List<Field>();
        }

        /// <summary>
        /// Converts from the FoundOPS model to the API model
        /// </summary>
        /// <param name="serviceModel">The service to convert</param>
        /// <returns>A Service that has been converted to it's API model</returns>
        public static Service ConvertModel(FoundOps.Core.Models.CoreEntities.Service serviceModel)
        {
            //Create the new service and set its properties
            var service = new Service
            {
                Id = serviceModel.Id,
                Name = serviceModel.ServiceTemplate.Name,
                ServiceDate = serviceModel.ServiceDate,
                ClientId = serviceModel.ClientId,
                ServiceProviderId = serviceModel.ServiceProviderId,
                RecurringServiceId = serviceModel.RecurringServiceId
            };

            if(serviceModel.Client!=null)
            {
                service.Client = Client.ConvertModel(serviceModel.Client);
            }

            //Convert each field from the FoundOPS model to the API model
            //Add the newly converted field to the newly created service
            foreach (var field in serviceModel.ServiceTemplate.Fields.OrderBy(f => f.Name))
                service.Fields.Add(Field.ConvertModel(field));

            return service;
        }
    }
}
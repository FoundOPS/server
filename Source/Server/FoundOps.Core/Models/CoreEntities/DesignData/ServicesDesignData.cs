using FoundOps.Common.Tools.ExtensionMethods;
using System;
using System.Collections.Generic;

namespace FoundOps.Core.Models.CoreEntities.DesignData
{
    public class ServicesDesignData
    {
        private readonly BusinessAccount _serviceProvider;
        private readonly Client _client;
        private readonly IEnumerable<ServiceTemplate> _serviceTemplates;

        public List<Service> DesignServices;

        public Service OilService;
        public Service SmallGreaseTrapService;

        public ServicesDesignData(BusinessAccount serviceProvider, Client client, IEnumerable<ServiceTemplate> serviceTemplates)
        {
            _serviceTemplates = serviceTemplates;

            _serviceProvider = serviceProvider;
            _client = client;

            InitializeServices();
        }

        private void InitializeServices()
        {
            OilService = new Service
            {
                ServiceDate = DateTime.UtcNow.AddDays(-1),
                ServiceTemplate = _serviceTemplates.RandomItem().MakeChild(ServiceTemplateLevel.ServiceDefined),
                Client = _client,
                RecurringServiceId = new RecurringServicesDesignData(_client).DesignRecurringService.Id,
                ServiceProvider = _serviceProvider,
                CreatedDate = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            };

            SmallGreaseTrapService = new Service
            {
                ServiceDate = DateTime.UtcNow,
                ServiceTemplate = _serviceTemplates.RandomItem().MakeChild(ServiceTemplateLevel.ServiceDefined),
                Client = _client,
                ServiceProvider = _serviceProvider,
                CreatedDate = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            };

            DesignServices = new List<Service> { OilService, SmallGreaseTrapService };
        }
    }
}
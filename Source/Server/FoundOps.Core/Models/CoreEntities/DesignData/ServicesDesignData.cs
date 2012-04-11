using System;
using System.Collections.Generic;
using FoundOps.Common.Composite.Tools;

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

        public ServicesDesignData() : this(null, null, new[] { ServiceTemplatesDesignData.OilServiceTemplate, ServiceTemplatesDesignData.SmallGreaseTrapServiceTemplate }) { }

        public ServicesDesignData(BusinessAccount serviceProvider, Client client, IEnumerable<ServiceTemplate> serviceTemplates)
        {
            _serviceTemplates = serviceTemplates;

            if (serviceProvider == null || client == null)
            {
                var designPartyData = new PartyDesignData();
                if (serviceProvider == null)
                    serviceProvider = designPartyData.DesignBusinessAccount;
                if (client == null)
                {
                    var designClientData = new ClientsDesignData();
                    client = designClientData.DesignClient;
                }
            }

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
                ServiceProvider = _serviceProvider
            };

            SmallGreaseTrapService = new Service
            {
                ServiceDate = DateTime.UtcNow,
                ServiceTemplate = _serviceTemplates.RandomItem().MakeChild(ServiceTemplateLevel.ServiceDefined),
                Client = _client,
                ServiceProvider = _serviceProvider
            };

            DesignServices = new List<Service> { OilService, SmallGreaseTrapService };
        }
    }
}
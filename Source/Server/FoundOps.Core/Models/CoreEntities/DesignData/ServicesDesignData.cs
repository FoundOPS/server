using System;
using System.Collections.Generic;

#if SILVERLIGHT
using FoundOps.Core.Context.Extensions;
#endif

namespace FoundOps.Core.Models.CoreEntities.DesignData
{
    public class ServicesDesignData
    {

        private readonly BusinessAccount _serviceProvider;
        private readonly Client _client;
        public List<Service> DesignServices;

        public Service OilService;
        public Service SmallGreaseTrapService;

        public ServicesDesignData()
            : this(null, null)
        {
        }

        public ServicesDesignData(BusinessAccount serviceProvider, Client client)
        {
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
                ServiceTemplate = ServiceTemplatesDesignData.OilServiceTemplate.MakeChild(ServiceTemplateLevel.ServiceDefined),
                Client = _client,
                ServiceProvider = _serviceProvider
            };

            SmallGreaseTrapService = new Service
            {
                ServiceTemplate = ServiceTemplatesDesignData.SmallGreaseTrapServiceTemplate.MakeChild(ServiceTemplateLevel.ServiceDefined),
                Client = _client,
                ServiceProvider = _serviceProvider
            };

            DesignServices = new List<Service> { OilService, SmallGreaseTrapService };
        }
    }
}
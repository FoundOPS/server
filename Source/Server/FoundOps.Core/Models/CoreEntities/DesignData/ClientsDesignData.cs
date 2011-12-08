using System;
using System.Collections.Generic;
using System.Linq;

namespace FoundOps.Core.Models.CoreEntities.DesignData
{
    public class ClientsDesignData
    {
        public Client DesignClient { get; private set; }
        public Client DesignClientTwo { get; private set; }
        public Client DesignClientThree { get; private set; }

        public IEnumerable<Client> DesignClients { get; private set; }

        public ClientsDesignData()
            : this(new BusinessAccountsDesignData().GotGrease, new RegionsDesignData())
        {
        }

        public ClientsDesignData(BusinessAccount serviceProvider, RegionsDesignData regionsDesignData)
        {
            var availableServiceTemplates = serviceProvider.ServiceTemplates.Where(st => st.ServiceTemplateLevel == ServiceTemplateLevel.ServiceProviderDefined).ToArray();

            InitializeClients();

            int clientIndex = 0;

            foreach (var client in DesignClients)
            {
                client.Vendor = serviceProvider;

                //Add all Service Templates as AvailableServiceTemplates
                foreach (var availableServiceTemplate in availableServiceTemplates)
                {
                    var clientServiceTemplate = availableServiceTemplate.MakeChild(ServiceTemplateLevel.ClientDefined);
                    clientServiceTemplate.OwnerClient = client;
                }

                //Add Locations to the Client. Must do this before setting up the RecurringServices (they require locations)
                //This adds 2 locations per client. NOTE: If there are more than 3 clients, this or LocationsDesignData must be updated to have enough locations
                new LocationsDesignData(serviceProvider, client, regionsDesignData.DesignRegions, clientIndex * 2, 2);

                //Add RecurringServices
                new RecurringServicesDesignData(client);

                clientIndex++;
            }

        }

        private void InitializeClients()
        {
            var partyDesignData = new PartyDesignData();

            DesignClient = new Client
            {
                DateAdded = DateTime.Now,
                OwnedParty = partyDesignData.DesignBusiness
            };

            DesignClientTwo = new Client
            {
                DateAdded = DateTime.Now,
                OwnedParty = partyDesignData.DesignBusinessTwo
            };

            DesignClientThree = new Client
            {
                DateAdded = DateTime.Now,
                OwnedParty = partyDesignData.DesignBusinessThree
            };

            DesignClients = new List<Client> { DesignClient, DesignClientTwo, DesignClientThree };
        }
    }
}
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
        public Client DesignClientFour { get; private set; }
        public Client DesignClientFive { get; private set; }
        public Client DesignClientSix { get; private set; }
        public Client DesignClientSeven { get; private set; }
        public Client DesignClientEight { get; private set; }
        public Client DesignClientNine { get; private set; }
        public Client DesignClientTen { get; private set; }
        public Client DesignClientEleven { get; private set; }
        public Client DesignClientTwelve { get; private set; }
        public Client DesignClientThirteen { get; private set; }
        public Client DesignClientFourteen { get; private set; }
        public Client DesignClientFifteen { get; private set; }
        public Client DesignClientSixteen { get; private set; }
        public Client DesignClientSeventeen { get; private set; }
        public Client DesignClientEighteen { get; private set; }
        public Client DesignClientNineteen { get; private set; }
        public Client DesignClientTwenty { get; private set; }
        public Client DesignClientTwentyOne { get; private set; }
        public Client DesignClientTwentyTwo { get; private set; }
        public Client DesignClientTwentyThree { get; private set; }
        public Client DesignClientTwentyFour { get; private set; }

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
                client.BusinessAccount = serviceProvider;

                //Add all Service Templates as AvailableServiceTemplates
                foreach (var availableServiceTemplate in availableServiceTemplates)
                {
                    var clientServiceTemplate = availableServiceTemplate.MakeChild(ServiceTemplateLevel.ClientDefined);
                    clientServiceTemplate.OwnerClient = client;
                }

                //Add Locations to the Client. Must do this before setting up the RecurringServices (they require locations)
                //This adds 2 locations per client. NOTE: If there are more than 3 clients, this or LocationsDesignData must be updated to have enough locations
                new LocationsDesignData(client, regionsDesignData.DesignRegions, clientIndex, 1);
                
                //Set the default billing location
                client.Locations.First().IsDefaultBillingLocation = true;

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
                Id = Guid.NewGuid(),
                DateAdded = DateTime.UtcNow,
                Name = partyDesignData.DesignBusiness.Name
            };

            DesignClientTwo = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = DateTime.UtcNow,
                Name = partyDesignData.DesignBusinessTwo.Name
            };

            DesignClientThree = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = DateTime.UtcNow,
                Name = partyDesignData.DesignBusinessThree.Name
            };

            DesignClientFour = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = DateTime.UtcNow,
                Name = partyDesignData.DesignBusinessFour.Name
            };

            DesignClientFive = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = DateTime.UtcNow,
                Name = partyDesignData.DesignBusinessFive.Name
            };

            DesignClientSix = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = DateTime.UtcNow,
                Name = partyDesignData.DesignBusinessSix.Name
            };

            DesignClientSeven = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = DateTime.UtcNow,
                Name = partyDesignData.DesignBusinessSeven.Name
            };

            DesignClientEight = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = DateTime.UtcNow,
                Name = partyDesignData.DesignBusinessEight.Name
            };

            DesignClientNine = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = DateTime.UtcNow,
                Name = partyDesignData.DesignBusinessNine.Name
            };

            DesignClientTen = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = DateTime.UtcNow,
                Name = partyDesignData.DesignBusinessTen.Name
            };

            DesignClientEleven = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = DateTime.UtcNow,
                Name = partyDesignData.DesignBusinessEleven.Name
            };

            DesignClientTwelve = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = DateTime.UtcNow,
                Name = partyDesignData.DesignBusinessTwelve.Name
            };

            DesignClientThirteen = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = DateTime.UtcNow,
                Name = partyDesignData.DesignBusinessThirteen.Name
            };

            DesignClientFourteen = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = DateTime.UtcNow,
                Name = partyDesignData.DesignBusinessFourteen.Name
            };

            DesignClientFifteen = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = DateTime.UtcNow,
                Name = partyDesignData.DesignBusinessFifteen.Name
            };

            DesignClientSixteen = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = DateTime.UtcNow,
                Name = partyDesignData.DesignBusinessSixteen.Name
            };

            DesignClientSeventeen = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = DateTime.UtcNow,
                Name = partyDesignData.DesignBusinessSeventeen.Name
            };

            DesignClientEighteen = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = DateTime.UtcNow,
                Name = partyDesignData.DesignBusinessEighteen.Name
            };

            DesignClientNineteen = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = DateTime.UtcNow,
                Name = partyDesignData.DesignBusinessNineteen.Name
            };

            DesignClientTwenty = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = DateTime.UtcNow,
                Name = partyDesignData.DesignBusinessTwenty.Name
            };

            DesignClientTwentyOne = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = DateTime.UtcNow,
                Name = partyDesignData.DesignBusinessTwentyOne.Name
            };

            DesignClientTwentyTwo = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = DateTime.UtcNow,
                Name = partyDesignData.DesignBusinessTwentyTwo.Name
            };

            DesignClientTwentyThree = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = DateTime.UtcNow,
                Name = partyDesignData.DesignBusinessTwentyThree.Name
            };

            DesignClientTwentyFour = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = DateTime.UtcNow,
                Name = partyDesignData.DesignBusinessTwentyFour.Name
            };

            DesignClients = new List<Client> { 
                                                DesignClient, 
                                                DesignClientTwo, 
                                                DesignClientThree,
                                                DesignClientFour,
                                                DesignClientFive,
                                                DesignClientSix,
                                                DesignClientSeven,
                                                DesignClientEight,
                                                DesignClientNine,
                                                DesignClientTen,
                                                DesignClientEleven,
                                                DesignClientTwelve,
                                                DesignClientThirteen,
                                                DesignClientFourteen,
                                                DesignClientFifteen,
                                                DesignClientSixteen,
                                                DesignClientSeventeen,
                                                DesignClientEighteen,
                                                DesignClientNineteen,
                                                DesignClientTwenty,
                                                DesignClientTwentyOne,
                                                DesignClientTwentyTwo,
                                                DesignClientTwentyThree,
                                                DesignClientTwentyFour
                                            };
        }
    }
}
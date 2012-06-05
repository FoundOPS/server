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
                new LocationsDesignData(serviceProvider, client, regionsDesignData.DesignRegions, clientIndex, 1);
                
                //Set the default billing location
                client.DefaultBillingLocation = client.OwnedParty.Locations.First();

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
                DateAdded = DateTime.UtcNow,
                OwnedParty = partyDesignData.DesignBusiness
            };

            DesignClientTwo = new Client
            {
                DateAdded = DateTime.UtcNow,
                OwnedParty = partyDesignData.DesignBusinessTwo
            };

            DesignClientThree = new Client
            {
                DateAdded = DateTime.UtcNow,
                OwnedParty = partyDesignData.DesignBusinessThree
            };

            DesignClientFour = new Client
            {
                DateAdded = DateTime.UtcNow,
                OwnedParty = partyDesignData.DesignBusinessFour
            };

            DesignClientFive = new Client
            {
                DateAdded = DateTime.UtcNow,
                OwnedParty = partyDesignData.DesignBusinessFive
            };

            DesignClientSix = new Client
            {
                DateAdded = DateTime.UtcNow,
                OwnedParty = partyDesignData.DesignBusinessSix
            };

            DesignClientSeven = new Client
            {
                DateAdded = DateTime.UtcNow,
                OwnedParty = partyDesignData.DesignBusinessSeven
            };

            DesignClientEight = new Client
            {
                DateAdded = DateTime.UtcNow,
                OwnedParty = partyDesignData.DesignBusinessEight
            };

            DesignClientNine = new Client
            {
                DateAdded = DateTime.UtcNow,
                OwnedParty = partyDesignData.DesignBusinessNine
            };

            DesignClientTen = new Client
            {
                DateAdded = DateTime.UtcNow,
                OwnedParty = partyDesignData.DesignBusinessTen
            };

            DesignClientEleven = new Client
            {
                DateAdded = DateTime.UtcNow,
                OwnedParty = partyDesignData.DesignBusinessEleven
            };

            DesignClientTwelve = new Client
            {
                DateAdded = DateTime.UtcNow,
                OwnedParty = partyDesignData.DesignBusinessTwelve
            };

            DesignClientThirteen = new Client
            {
                DateAdded = DateTime.UtcNow,
                OwnedParty = partyDesignData.DesignBusinessThirteen
            };

            DesignClientFourteen = new Client
            {
                DateAdded = DateTime.UtcNow,
                OwnedParty = partyDesignData.DesignBusinessFourteen
            };

            DesignClientFifteen = new Client
            {
                DateAdded = DateTime.UtcNow,
                OwnedParty = partyDesignData.DesignBusinessFifteen
            };

            DesignClientSixteen = new Client
            {
                DateAdded = DateTime.UtcNow,
                OwnedParty = partyDesignData.DesignBusinessSixteen
            };

            DesignClientSeventeen = new Client
            {
                DateAdded = DateTime.UtcNow,
                OwnedParty = partyDesignData.DesignBusinessSeventeen
            };

            DesignClientEighteen = new Client
            {
                DateAdded = DateTime.UtcNow,
                OwnedParty = partyDesignData.DesignBusinessEighteen
            };

            DesignClientNineteen = new Client
            {
                DateAdded = DateTime.UtcNow,
                OwnedParty = partyDesignData.DesignBusinessNineteen
            };

            DesignClientTwenty = new Client
            {
                DateAdded = DateTime.UtcNow,
                OwnedParty = partyDesignData.DesignBusinessTwenty
            };

            DesignClientTwentyOne = new Client
            {
                DateAdded = DateTime.UtcNow,
                OwnedParty = partyDesignData.DesignBusinessTwentyOne
            };

            DesignClientTwentyTwo = new Client
            {
                DateAdded = DateTime.UtcNow,
                OwnedParty = partyDesignData.DesignBusinessTwentyTwo
            };

            DesignClientTwentyThree = new Client
            {
                DateAdded = DateTime.UtcNow,
                OwnedParty = partyDesignData.DesignBusinessTwentyThree
            };

            DesignClientTwentyFour = new Client
            {
                DateAdded = DateTime.UtcNow,
                OwnedParty = partyDesignData.DesignBusinessTwentyFour
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
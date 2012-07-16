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
            DesignClient = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = DateTime.UtcNow,
                Name = "Adelino's Old World Kitchen"
            };

            DesignClientTwo = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = DateTime.UtcNow,
                Name = "Akropolis Restaraunt"
            };

            DesignClientThree = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = DateTime.UtcNow,
                Name = "Apollo Lounge"
            };

            DesignClientFour = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = DateTime.UtcNow,
                Name = "Applebee's Neighborhood Grill"
            };

            DesignClientFive = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = DateTime.UtcNow,
                Name = "Arby's"
            };

            DesignClientSix = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = DateTime.UtcNow,
                Name = "Arctic White Soft Serve Ice"
            };

            DesignClientSeven = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = DateTime.UtcNow,
                Name = "Arthur's"
            };

            DesignClientEight = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = DateTime.UtcNow,
                Name = "Artie's Tenderloin"
            };

            DesignClientNine = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = DateTime.UtcNow,
                Name = "Barcelona Tapas"
            };

            DesignClientTen = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = DateTime.UtcNow,
                Name = "Bistro 501"
            };

            DesignClientEleven = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = DateTime.UtcNow,
                Name = "Black Sparrow"
            };

            DesignClientTwelve = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = DateTime.UtcNow,
                Name = "Blue Fin Bistro Sushi Lafayette"
            };

            DesignClientThirteen = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = DateTime.UtcNow,
                Name = "Bob Evans Restaurant"
            };

            DesignClientFourteen = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = DateTime.UtcNow,
                Name = "Bruno's Pizza and Big O's Sports Room"
            };

            DesignClientFifteen = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = DateTime.UtcNow,
                Name = "Buca di Beppo - Downtown Indianapolis"
            };

            DesignClientSixteen = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = DateTime.UtcNow,
                Name = "Buffalo Wild Wings Grill & Bar"
            };

            DesignClientSeventeen = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = DateTime.UtcNow,
                Name = "Burger King"
            };

            DesignClientEighteen = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = DateTime.UtcNow,
                Name = "Campbell's On Main Street"
            };

            DesignClientNineteen = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = DateTime.UtcNow,
                Name = "Covered Bridge Restaurant"
            };

            DesignClientTwenty = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = DateTime.UtcNow,
                Name = "Crawfordsville Forum Family"
            };

            DesignClientTwentyOne = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = DateTime.UtcNow,
                Name = "Dairy Queen"
            };

            DesignClientTwentyTwo = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = DateTime.UtcNow,
                Name = "Diamond Coffee Company"
            };

            DesignClientTwentyThree = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = DateTime.UtcNow,
                Name = "Culver's"
            };

            DesignClientTwentyFour = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = DateTime.UtcNow,
                Name = "El Rodeo"
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
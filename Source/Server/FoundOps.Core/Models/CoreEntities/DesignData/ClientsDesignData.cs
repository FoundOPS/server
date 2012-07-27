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
            var date = DateTime.UtcNow.Date;

            DesignClient = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = date,
                Name = "Adelino's Old World Kitchen"
            };

            DesignClientTwo = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = date,
                Name = "Akropolis Restaraunt"
            };

            DesignClientThree = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = date,
                Name = "Apollo Lounge"
            };

            DesignClientFour = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = date,
                Name = "Applebee's Neighborhood Grill"
            };

            DesignClientFive = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = date,
                Name = "Arby's"
            };

            DesignClientSix = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = date,
                Name = "Arctic White Soft Serve Ice"
            };

            DesignClientSeven = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = date,
                Name = "Arthur's"
            };

            DesignClientEight = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = date,
                Name = "Artie's Tenderloin"
            };

            DesignClientNine = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = date,
                Name = "Barcelona Tapas"
            };

            DesignClientTen = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = date,
                Name = "Bistro 501"
            };

            DesignClientEleven = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = date,
                Name = "Black Sparrow"
            };

            DesignClientTwelve = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = date,
                Name = "Blue Fin Bistro Sushi Lafayette"
            };

            DesignClientThirteen = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = date,
                Name = "Bob Evans Restaurant"
            };

            DesignClientFourteen = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = date,
                Name = "Bruno's Pizza and Big O's Sports Room"
            };

            DesignClientFifteen = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = date,
                Name = "Buca di Beppo - Downtown Indianapolis"
            };

            DesignClientSixteen = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = date,
                Name = "Buffalo Wild Wings Grill & Bar"
            };

            DesignClientSeventeen = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = date,
                Name = "Burger King"
            };

            DesignClientEighteen = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = date,
                Name = "Campbell's On Main Street"
            };

            DesignClientNineteen = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = date,
                Name = "Covered Bridge Restaurant"
            };

            DesignClientTwenty = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = date,
                Name = "Crawfordsville Forum Family"
            };

            DesignClientTwentyOne = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = date,
                Name = "Dairy Queen"
            };

            DesignClientTwentyTwo = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = date,
                Name = "Diamond Coffee Company"
            };

            DesignClientTwentyThree = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = date,
                Name = "Culver's"
            };

            DesignClientTwentyFour = new Client
            {
                Id = Guid.NewGuid(),
                DateAdded = date,
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
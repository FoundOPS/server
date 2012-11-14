using System;
using System.Collections.Generic;
using System.Linq;

namespace FoundOps.Core.Models.CoreEntities.DesignData
{
    public class ClientsDesignData
    {
        public List<Client> DesignClients { get; private set; }

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
                //This adds 2 locations per client. NOTE: If there are more than 24 clients, this or LocationsDesignData must be updated to have enough locations
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
            DesignClients = new List<Client>
            {
            new Client
            {
                Id = Guid.NewGuid(),
                Name = "Adelino's Old World Kitchen",
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow
            },
            new Client
            {
                Id = Guid.NewGuid(),
                Name = "Akropolis Restaraunt",
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow
            },
            new Client
            {
                Id = Guid.NewGuid(),
                Name = "Apollo Lounge",
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow
            },
            new Client
            {
                Id = Guid.NewGuid(),
                Name = "Applebee's Neighborhood Grill",
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow
            },
            new Client
            {
                Id = Guid.NewGuid(),
                Name = "Arby's",
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow
            },
            new Client
            {
                Id = Guid.NewGuid(),
                Name = "Arctic White Soft Serve Ice",
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow
            },
            new Client
            {
                Id = Guid.NewGuid(),
                Name = "Arthur's",
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow
            },
            new Client
            {
                Id = Guid.NewGuid(),
                Name = "Artie's Tenderloin",
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow
            },
            new Client
            {
                Id = Guid.NewGuid(),
                Name = "Barcelona Tapas",
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow
            },
            new Client
            {
                Id = Guid.NewGuid(),
                Name = "Bistro 501",
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow
            },
            new Client
            {
                Id = Guid.NewGuid(),
                Name = "Black Sparrow",
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow
            },
            new Client
            {
                Id = Guid.NewGuid(),
                Name = "Blue Fin Bistro Sushi Lafayette",
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow
            },
            new Client
            {
                Id = Guid.NewGuid(),
                Name = "Bob Evans Restaurant",
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow
            },
            new Client
            {
                Id = Guid.NewGuid(),
                Name = "Bruno's Pizza and Big O's Sports Room",
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow
            },
            new Client
            {
                Id = Guid.NewGuid(),
                Name = "Buca di Beppo",
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow
            },
            new Client
            {
                Id = Guid.NewGuid(),
                Name = "Buffalo Wild Wings Grill & Bar",
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow
            },
            new Client
            {
                Id = Guid.NewGuid(),
                Name = "Burger King",
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow
            },
            new Client
            {
                Id = Guid.NewGuid(),
                Name = "Campbell's On Main Street",
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow
            },
            new Client
            {
                Id = Guid.NewGuid(),
                Name = "Covered Bridge Restaurant",
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow
            },
            new Client
            {
                Id = Guid.NewGuid(),
                Name = "Crawfordsville Forum Family",
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow
            },
            new Client
            {
                Id = Guid.NewGuid(),
                Name = "Dairy Queen",
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow
            },
            new Client
            {
                Id = Guid.NewGuid(),
                Name = "Diamond Coffee Company",
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow
            },
            new Client
            {
                Id = Guid.NewGuid(),
                Name = "Culver's",
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow
            },
            new Client
            {
                Id = Guid.NewGuid(),
                Name = "El Rodeo",
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow
            }
            };
        }
    }
}
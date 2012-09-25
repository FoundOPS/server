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
            var date = DateTime.UtcNow.Date;

            DesignClients = new List<Client>
                {
                    new Client
                        {
                            Id = Guid.NewGuid(),
                            DateAdded = date,
                            Name = "Adelino's Old World Kitchen"
                        },
                    new Client
                        {
                            Id = Guid.NewGuid(),
                            DateAdded = date,
                            Name = "Akropolis Restaraunt"
                        },
                    new Client
                        {
                            Id = Guid.NewGuid(),
                            DateAdded = date,
                            Name = "Apollo Lounge"
                        },
                    new Client
                        {
                            Id = Guid.NewGuid(),
                            DateAdded = date,
                            Name = "Applebee's Neighborhood Grill"
                        },
                    new Client
                        {
                            Id = Guid.NewGuid(),
                            DateAdded = date,
                            Name = "Arby's"
                        },
                    new Client
                        {
                            Id = Guid.NewGuid(),
                            DateAdded = date,
                            Name = "Arctic White Soft Serve Ice"
                        },
                    new Client
                        {
                            Id = Guid.NewGuid(),
                            DateAdded = date,
                            Name = "Arthur's"
                        },
                    new Client
                        {
                            Id = Guid.NewGuid(),
                            DateAdded = date,
                            Name = "Artie's Tenderloin"
                        },
                    new Client
                        {
                            Id = Guid.NewGuid(),
                            DateAdded = date,
                            Name = "Barcelona Tapas"
                        },
                    new Client
                        {
                            Id = Guid.NewGuid(),
                            DateAdded = date,
                            Name = "Bistro 501"
                        },
                    new Client
                        {
                            Id = Guid.NewGuid(),
                            DateAdded = date,
                            Name = "Black Sparrow"
                        },
                    new Client
                        {
                            Id = Guid.NewGuid(),
                            DateAdded = date,
                            Name = "Blue Fin Bistro Sushi Lafayette"
                        },
                    new Client
                        {
                            Id = Guid.NewGuid(),
                            DateAdded = date,
                            Name = "Bob Evans Restaurant"
                        },
                    new Client
                        {
                            Id = Guid.NewGuid(),
                            DateAdded = date,
                            Name = "Bruno's Pizza and Big O's Sports Room"
                        },
                    new Client
                        {
                            Id = Guid.NewGuid(),
                            DateAdded = date,
                            Name = "Buca di Beppo"
                        },
                    new Client
                        {
                            Id = Guid.NewGuid(),
                            DateAdded = date,
                            Name = "Buffalo Wild Wings Grill & Bar"
                        },
                    new Client
                        {
                            Id = Guid.NewGuid(),
                            DateAdded = date,
                            Name = "Burger King"
                        },
                    new Client
                        {
                            Id = Guid.NewGuid(),
                            DateAdded = date,
                            Name = "Campbell's On Main Street"
                        },
                    new Client
                        {
                            Id = Guid.NewGuid(),
                            DateAdded = date,
                            Name = "Covered Bridge Restaurant"
                        },
                    new Client
                        {
                            Id = Guid.NewGuid(),
                            DateAdded = date,
                            Name = "Crawfordsville Forum Family"
                        },
                    new Client
                        {
                            Id = Guid.NewGuid(),
                            DateAdded = date,
                            Name = "Dairy Queen"
                        },
                    new Client
                        {
                            Id = Guid.NewGuid(),
                            DateAdded = date,
                            Name = "Diamond Coffee Company"
                        },
                    new Client
                        {
                            Id = Guid.NewGuid(),
                            DateAdded = date,
                            Name = "Culver's"
                        },
                    new Client
                        {
                            Id = Guid.NewGuid(),
                            DateAdded = date,
                            Name = "El Rodeo"
                        }
                };
        }
    }
}
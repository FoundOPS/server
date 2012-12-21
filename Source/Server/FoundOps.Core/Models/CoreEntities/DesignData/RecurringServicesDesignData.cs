using System;
using System.Linq;
using System.Collections.ObjectModel;
using FoundOps.Core.Models.CoreEntities.Extensions.Services;

namespace FoundOps.Core.Models.CoreEntities.DesignData
{
    public class RecurringServicesDesignData
    {
        private readonly Client _client;

        public ObservableCollection<RecurringService> DesignRecurringServices { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecurringServicesDesignData"/> class.
        /// </summary>
        /// <param name="client">The client. The client must have locations.</param>
        public RecurringServicesDesignData(Client client)
        {
            _client = client;
            InitializeRecurringServices();

            int locationIndex = 0;

            //Setup the RecurringService's client and Destination, and generate one Service offspring
            foreach (var recurringService in DesignRecurringServices)
            {
                recurringService.Client = client;

                //Set the desination to the next location of the client
                recurringService.ServiceTemplate.SetDestination(client.Locations.ElementAtOrDefault(locationIndex));

                locationIndex++;

                if (locationIndex >= client.Locations.Count)
                    locationIndex = 0;

                //Add a service child for each recurring service on today
                new Service
                {
                    ServiceDate = DateTime.UtcNow.Date,
                    ServiceProviderId = client.BusinessAccountId.Value,
                    RecurringServiceParent = recurringService,
                    Client = recurringService.Client,
                    ServiceTemplate = recurringService.ServiceTemplate.MakeChild(ServiceTemplateLevel.ServiceDefined)
                };
            }
        }

        private void InitializeRecurringServices()
        {
            var firstServiceTemplate = _client.ServiceTemplates.ElementAt(0);
            var secondServiceTemplate = _client.ServiceTemplates.ElementAt(1);
            var thirdServiceTemplate = _client.ServiceTemplates.ElementAt(2);

            var recurringServiceOne = new RecurringService { ServiceTemplate = firstServiceTemplate.MakeChild(ServiceTemplateLevel.RecurringServiceDefined) };

            var repeat = (new RepeatDesignData()).DesignWeeklyRepeat;
            repeat.Id = recurringServiceOne.Id; //Fix Referential Contraint
            recurringServiceOne.Repeat = repeat;
            recurringServiceOne.CreatedDate = DateTime.UtcNow;
            recurringServiceOne.LastModified = DateTime.UtcNow;

            var recurringServiceTwo = new RecurringService { ServiceTemplate = secondServiceTemplate.MakeChild(ServiceTemplateLevel.RecurringServiceDefined) };

            repeat = (new RepeatDesignData()).DesignMonthlyRepeat;
            repeat.Id = recurringServiceTwo.Id; //Fix Referential Contraint
            recurringServiceTwo.Repeat = repeat;
            recurringServiceTwo.CreatedDate = DateTime.UtcNow;
            recurringServiceTwo.LastModified = DateTime.UtcNow;

            var recurringServiceThree = new RecurringService { ServiceTemplate = thirdServiceTemplate.MakeChild(ServiceTemplateLevel.RecurringServiceDefined) };

            repeat = (new RepeatDesignData()).DesignNeverEndingWeeklyRepeat;
            repeat.Id = recurringServiceThree.Id; //Fix Referential Contraint
            recurringServiceThree.Repeat = repeat;
            recurringServiceThree.CreatedDate = DateTime.UtcNow;
            recurringServiceThree.LastModified = DateTime.UtcNow;

            DesignRecurringServices = new ObservableCollection<RecurringService>
            {
               recurringServiceOne,
               recurringServiceTwo,
               recurringServiceThree
            };
        }
    }
}
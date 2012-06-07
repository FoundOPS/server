using System;
using System.Linq;
using System.Collections.ObjectModel;
using FoundOps.Core.Models.CoreEntities.Extensions.Services;

namespace FoundOps.Core.Models.CoreEntities.DesignData
{
    public class RecurringServicesDesignData
    {
        private readonly Client _client;
        public RecurringService DesignRecurringService { get; private set; }
        public RecurringService DesignRecurringServiceTwo { get; private set; }
        public RecurringService DesignRecurringServiceThree { get; private set; }

        public ObservableCollection<RecurringService> DesignRecurringServices { get; private set; }

        public RecurringServicesDesignData()
            : this(new ClientsDesignData().DesignClient)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecurringServicesDesignData"/> class.
        /// </summary>
        /// <param name="client">The client. The client must have locations.</param>
        public RecurringServicesDesignData(Client client)
        {
            _client = client;
            InitializeRecurringServices();

            int locationIndex = 0;

            //Setup the RecurringService's client and Destination
            foreach (var recurringService in DesignRecurringServices)
            {
                recurringService.Client = client;

                //Set the desination to the next location of the client
                recurringService.ServiceTemplate.SetDestination(client.Locations.ElementAtOrDefault(locationIndex));

                locationIndex++;

                if (locationIndex >= client.Locations.Count)
                    locationIndex = 0;
            }
        }

        private void InitializeRecurringServices()
        {
            var firstServiceTemplate = _client.ServiceTemplates.ElementAt(0);
            var secondServiceTemplate = _client.ServiceTemplates.ElementAt(1);
            var thirdServiceTemplate = _client.ServiceTemplates.ElementAt(2);

            DesignRecurringService = new RecurringService { ServiceTemplate = firstServiceTemplate.MakeChild(ServiceTemplateLevel.RecurringServiceDefined) };

            var repeat = (new RepeatDesignData()).DesignWeeklyRepeat;
            repeat.Id = DesignRecurringService.Id; //Fix Referential Contraint
            DesignRecurringService.Repeat = repeat;

            DesignRecurringServiceTwo = new RecurringService { ServiceTemplate = secondServiceTemplate.MakeChild(ServiceTemplateLevel.RecurringServiceDefined) };

            repeat = (new RepeatDesignData()).DesignMonthlyRepeat;
            repeat.Id = DesignRecurringServiceTwo.Id; //Fix Referential Contraint
            DesignRecurringServiceTwo.Repeat = repeat;

            DesignRecurringServiceThree = new RecurringService { ServiceTemplate = thirdServiceTemplate.MakeChild(ServiceTemplateLevel.RecurringServiceDefined) };

            repeat = (new RepeatDesignData()).DesignNeverEndingWeeklyRepeat;
            repeat.Id = DesignRecurringServiceThree.Id; //Fix Referential Contraint
            DesignRecurringServiceThree.Repeat = repeat;
            DesignRecurringServices = new ObservableCollection<RecurringService>
            {
               DesignRecurringService,
               DesignRecurringServiceTwo,
               DesignRecurringServiceThree
            };
        }
    }
}
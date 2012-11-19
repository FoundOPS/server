using System;
using System.Collections.Generic;

namespace FoundOps.Core.Models.CoreEntities.DesignData
{
    /// <summary>
    /// BusinessAccount constants to be used outside of design data.
    /// </summary>
    public static class BusinessAccountsConstants
    {
        /// <summary>
        /// The FoundOPS's BusinessAccount Id
        /// </summary>
        public static Guid FoundOpsId = new Guid("5606A728-B99F-4AA1-B0CD-0AB38A649000");
    }

    /// <summary>
    /// Setup Business Accounts (and ServiceTemplates)
    /// </summary>
    public class BusinessAccountsDesignData
    {
        public static readonly BusinessAccount FoundOps = new BusinessAccount
        {
            Id = BusinessAccountsConstants.FoundOpsId,
            Name = "FoundOPS",
            CreatedDate = DateTime.UtcNow,
            LastModified = DateTime.UtcNow
        };

        public BusinessAccount GotGrease { get; private set; }
        public BusinessAccount ABCouriers { get; private set; }
        public BusinessAccount GenericOilCollector { get; private set; }
        public BusinessAccount GenericBiodiesel { get; private set; }

        public IEnumerable<BusinessAccount> DesignServiceProviders { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessAccountsDesignData"/> class.
        /// </summary>
        public BusinessAccountsDesignData()
        {
            InitializeBusinessAccounts();
        }

        private void InitializeBusinessAccounts()
        {
            GotGrease = new BusinessAccount
            {
                Id = new Guid("8528E50D-E2B9-4779-9B29-759DBEA53B61"),
                Name = "GotGrease?",
                QuickBooksEnabled = true,
                MaxRoutes = 10,
                CreatedDate = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            };

            ABCouriers = new BusinessAccount
            {
                Id = new Guid("3281B373-101F-415A-B708-14B9BA95A618"),
                Name = "AB Couriers",
                QuickBooksEnabled = true,
                MaxRoutes = 10,
                CreatedDate = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            };

            GenericOilCollector = new BusinessAccount
            {
                Id = new Guid("62047896-B2A1-49E4-BA10-72F0667B1DB0"),
                Name = "Generic Oil Collector",
                QuickBooksEnabled = true,
                MaxRoutes = 10,
                CreatedDate = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            };

            GenericBiodiesel = new BusinessAccount
            {
                Id = new Guid("BEB79E47-9B39-4730-BFEB-D15986438DAA"),
                Name = "Generic Biodiesel",
                QuickBooksEnabled = true,
                MaxRoutes = 10,
                CreatedDate = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            };

            DesignServiceProviders = new List<BusinessAccount> { GotGrease, ABCouriers, GenericOilCollector, GenericBiodiesel };


            //Add ServiceTemplates
            foreach (var serviceProvider in new[] { GotGrease, ABCouriers, GenericOilCollector, GenericBiodiesel })
            {
                //Add task statuses
                var defaultStatuses = TaskStatuses.CreateDefaultTaskStatuses();
                foreach (var taskStatus in defaultStatuses)
                    serviceProvider.TaskStatuses.Add(taskStatus);

                //Add depot
                serviceProvider.Depots.Add(new Location
                {
                    Name = "Depot",
                    AddressLineOne = "1305 Cumberland Ave",
                    AdminDistrictTwo = "West Lafayette",
                    AdminDistrictOne = "IN",
                    PostalCode = "47906",
                    CountryCode = "US",
                    BusinessAccount = serviceProvider,
                    Latitude = (decimal?)40.460335,
                    Longitude = (decimal?)(-86.929840),
                    CreatedDate = DateTime.UtcNow,
                    LastModified = DateTime.UtcNow
                });

                //Choose the right set of ServiceTemplates
                var serviceTemplates = serviceProvider == ABCouriers ? ServiceTemplatesDesignData.SameDayDeliveryCompanyServiceTemplates :
                                                                      ServiceTemplatesDesignData.OilGreaseCompanyServiceTemplates;

                foreach (var serviceTemplate in serviceTemplates)
                {
                    var serviceTemplateChild = serviceTemplate.MakeChild(ServiceTemplateLevel.ServiceProviderDefined);
                    serviceTemplateChild.OwnerServiceProvider = serviceProvider;
                }
            }
        }
    }
}

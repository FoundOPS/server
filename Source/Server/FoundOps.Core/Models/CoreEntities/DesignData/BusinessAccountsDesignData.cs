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
        public static readonly BusinessAccount FoundOps = new BusinessAccount { Id = BusinessAccountsConstants.FoundOpsId, Name = "FoundOPS" };

        public BusinessAccount GotGrease { get; private set; }
        public BusinessAccount ABCouriers { get; private set; }
        public BusinessAccount OrenKosherSteakhouse { get; private set; }

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
                MaxRoutes = 10
            };

            ABCouriers = new BusinessAccount
            {
                Id = new Guid("3281B373-101F-415A-B708-14B9BA95A618"),
                Name = "AB Couriers",
                QuickBooksEnabled = true,
                MaxRoutes = 10
            };

            OrenKosherSteakhouse = new BusinessAccount
            {
                Id = new Guid("62047896-B2A1-49E4-BA10-72F0667B1DB0"),
                Name = "Oren's Kosher Steakhouse",
                QuickBooksEnabled = true,
                MaxRoutes = 10
            };

            DesignServiceProviders = new List<BusinessAccount> { GotGrease, ABCouriers, OrenKosherSteakhouse };

            //Add Depots
            foreach(var serviceProvider in DesignServiceProviders)
            {
                serviceProvider.Depots.Add(new Location { Name = "Depot", AddressLineOne = "1305 Cumberland Ave", City = "West Lafayette", State = "IN", ZipCode = "47906",
                    BusinessAccount = serviceProvider, Latitude = (decimal?) 40.460335, Longitude = (decimal?) (-86.929840)});
            }

            //Add ServiceTemplates
            foreach (var businessAccount in new[] { GotGrease, ABCouriers, OrenKosherSteakhouse })
            {
                //Choose the right set of ServiceTemplates
                var serviceTemplates = businessAccount == GotGrease ? ServiceTemplatesDesignData.OilGreaseCompanyServiceTemplates :
                                                                      ServiceTemplatesDesignData.SameDayDeliveryCompanyServiceTemplates;

                foreach (var serviceTemplate in serviceTemplates)
                {
                    var serviceTemplateChild = serviceTemplate.MakeChild(ServiceTemplateLevel.ServiceProviderDefined);
                    serviceTemplateChild.OwnerServiceProvider = businessAccount;
                }
            }
        }
    }
}

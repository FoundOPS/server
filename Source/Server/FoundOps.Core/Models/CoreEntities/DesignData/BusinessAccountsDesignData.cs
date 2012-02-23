using System;
using System.Collections.Generic;

namespace FoundOps.Core.Models.CoreEntities.DesignData
{
    public static class BusinessAccountsConstants
    {
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

        public BusinessAccountsDesignData()
        {
            InitializeBusinessAccounts();
        }

        private void InitializeBusinessAccounts()
        {
            GotGrease = new BusinessAccount
            {
                Name = "GotGrease?",
                QuickBooksEnabled = true
            };

            ABCouriers = new BusinessAccount
            {
                Name = "AB Couriers",
                QuickBooksEnabled = true
            };

            OrenKosherSteakhouse = new BusinessAccount
            {
                Name = "Oren's Kosher Steakhouse",
                QuickBooksEnabled = true
            };

            DesignServiceProviders = new List<BusinessAccount> { GotGrease, ABCouriers, OrenKosherSteakhouse };


            //var setupOG = ServiceTemplatesDesignData.OilGreaseCompanyServiceTemplates;
            //var setupSD = ServiceTemplatesDesignData.SameDayDeliveryCompanyServiceTemplates;

            //return;
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

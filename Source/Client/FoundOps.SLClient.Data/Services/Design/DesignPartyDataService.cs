using System;
using FoundOps.Common.Silverlight.MVVM.Services;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Models.CoreEntities.DesignData;
using FoundOps.Server.Services.CoreDomainService;
using MEFedMVVM.ViewModelLocator;

namespace FoundOps.Core.Context.Services.Design
{
    [ExportService(ServiceType.DesignTime, typeof(IPartyDataService))]
    public class DesignPartyDataService : DesignDataService, IPartyDataService
    {
        private readonly PartyDesignData _partyDesignData = new PartyDesignData();
        private readonly ClientsDesignData _clientsDesignData = new ClientsDesignData();

        public Type PartyType { get; set; }

        public CoreDomainContext Context
        {
            get { return null; }
        }

        public void GetCurrentParty(Guid roleId, Action<Party> getCurrentPartyCallback)
        {
            getCurrentPartyCallback(_partyDesignData.DesignBusinessAccount);
        }

        public void GetCurrentUserAccount(Action<UserAccount> getCurrentUserAccountCallback)
        {
            getCurrentUserAccountCallback(_partyDesignData.DesignUserAccount);
        }

        public void GetPartyToAdministerForRole(Guid roleId, Action<Party> getPartyToAdministerCallback)
        {
            if (PartyType != null)
            {
                if (PartyType == typeof(UserAccount))
                {
                    getPartyToAdministerCallback(_partyDesignData.DesignUserAccount);
                    return;
                }
                if (PartyType == typeof(BusinessAccount))
                {
                    getPartyToAdministerCallback(_partyDesignData.DesignBusinessAccount);
                    return;
                }
            }

            var random = new Random((int)DateTime.Now.Ticks);
            if (random.Next(2) == 0)
                getPartyToAdministerCallback(_partyDesignData.DesignUserAccount);
            else
                getPartyToAdministerCallback(_partyDesignData.DesignBusinessAccount);
        }

        public void GetBusinessAccountWithClientsForRole(Guid roleId, Action<BusinessAccount> getBusinessAccountWithClientsForRoleCallback)
        {
            PartyType = typeof(BusinessAccount);

            GetPartyToAdministerForRole(roleId, (account) =>
                                                      {
                                                          var businessAccount = (BusinessAccount)account;

                                                          businessAccount.Clients.Add(_clientsDesignData.DesignClient);
                                                          businessAccount.Clients.Add(_clientsDesignData.DesignClientTwo);

                                                          getBusinessAccountWithClientsForRoleCallback(businessAccount);
                                                      });
        }

        public void GetBusinessForRole(Guid roleId, Action<Business> getBusinessForRoleCallback)
        {
            PartyType = typeof(BusinessAccount);

            GetPartyToAdministerForRole(roleId, (account) =>
            {
                var businessAccount = (BusinessAccount)account;

                getBusinessForRoleCallback(businessAccount);
            });
        }
    }
}

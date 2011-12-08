using System;
using System.ComponentModel.Composition;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Server.Services.CoreDomainService;
using MEFedMVVM.ViewModelLocator;
using ScalableCourier.Client.CommonResources.MVVM.Services;

namespace FoundOps.Core.Context.Services
{
    [ExportService(ServiceType.Runtime, typeof(IPartyDataService))]
    public class PartyDataService : DomainContextDataService<CoreDomainContext>, IPartyDataService
    {
        [ImportingConstructor]
        public PartyDataService(CoreDomainContext context)
            : base(context)
        {
        }

        public Type PartyType { get; set; }

        public void GetCurrentParty(Guid roleId, Action<Party> getCurrentPartyCallback)
        {
            LoadSingle(Context.PartyForRoleQuery(roleId), getCurrentPartyCallback);
        }

        public void GetCurrentUserAccount(Action<UserAccount> getCurrentUserAccountCallback)
        {
            LoadSingle(Context.CurrentUserAccountQuery(), getCurrentUserAccountCallback);
        }

        public void GetPartyToAdministerForRole(Guid roleId, Action<Party> getPartyToAdministerCallback)
        {
            LoadSingle(Context.PartyToAdministerForRoleQuery(roleId), getPartyToAdministerCallback);
        }

        public void GetBusinessAccountWithClientsForRole(Guid roleId, Action<BusinessAccount> getBusinessAccountWithClientsForRoleCallback)
        {
            LoadSingle(Context.BusinessAccountWithClientsForRoleQuery(roleId), getBusinessAccountWithClientsForRoleCallback);
        }

        public void GetBusinessForRole(Guid roleId, Action<Business> getBusinessForRoleCallback)
        {
            LoadSingle(Context.BusinessForRoleQuery(roleId), getBusinessForRoleCallback);
        }
    }
}

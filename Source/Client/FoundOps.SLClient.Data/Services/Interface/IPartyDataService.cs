using System;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Server.Services.CoreDomainService;
using FoundOps.Common.Silverlight.MVVM.Services.Interfaces;

namespace FoundOps.Core.Context.Services
{
    public interface IPartyDataService : IDataService
    {
        Type PartyType { get; set; }
        CoreDomainContext Context { get; }

        void GetCurrentParty(Guid roleId, Action<Party> getCurrentPartyCallback);
        void GetCurrentUserAccount(Action<UserAccount> getCurrentUserAccountCallback);

        void GetPartyToAdministerForRole(Guid roleId, Action<Party> getPartyToAdministerCallback);
        void GetBusinessAccountWithClientsForRole(Guid roleId, Action<BusinessAccount> getBusinessAccountWithClientsForRoleCallback);
        void GetBusinessForRole(Guid roleId, Action<Business> getBusinessForRoleCallback);
    }
}

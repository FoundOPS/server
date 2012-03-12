using System;
using System.Reactive.Linq;
using FoundOps.SLClient.Data.ViewModels;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Server.Services.CoreDomainService;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// Contains the logic for displaying PartySettings
    /// </summary>
    public abstract class PartySettingsVM : CoreEntityVM
    {
        //Public Properties
        /// <summary>
        /// Gets the party VM.
        /// </summary>
        public PartyVM PartyVM { get; private set; }
        /// <summary>
        /// Gets the contact info VM.
        /// </summary>
        public ContactInfoVM ContactInfoVM { get; private set; }

        #region Protected (Extendable) Features

        protected abstract void OnGetPartyToAdminister(Party party);

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="PartySettingsVM"/> class.
        /// </summary>
        /// <param name="partyVM">The party VM.</param>
        protected PartySettingsVM(PartyVM partyVM)
        {
            PartyVM = partyVM;

            DataManager.ContextManager.OwnerAccountObservable.Where(ownerAccount => ownerAccount != null)
                .Subscribe(ownerAccount =>
                 {
                     OnGetPartyToAdminister(ownerAccount);
                     ContactInfoVM = new ContactInfoVM(ContactInfoType.OwnedParties, ownerAccount.ContactInfoSet);
                 });
        }
    }
}

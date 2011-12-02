using System;
using System.Reactive.Linq;
using FoundOps.SLClient.Data.Services;
using FoundOps.SLClient.Data.ViewModels;
using FoundOps.Core.Models.CoreEntities;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// Contains the logic for displaying PartySettings
    /// </summary>
    public abstract class PartySettingsVM : CoreEntityVM
    {
        //Public Properties
        public PartyVM PartyVM { get; private set; }
        public ContactInfoVM ContactInfoVM { get; private set; }

        #region Protected (Extendable) Features

        protected abstract void OnGetPartyToAdminister(Party party);

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="PartySettingsVM"/> class.
        /// </summary>
        /// <param name="dataManager">The data manager.</param>
        /// <param name="partyVM">The party VM.</param>
        protected PartySettingsVM(DataManager dataManager, PartyVM partyVM)
            : base(dataManager)
        {
            PartyVM = partyVM;

            dataManager.ContextManager.OwnerAccountObservable.Where(ownerAccount => ownerAccount != null)
                .Subscribe(ownerAccount =>
                 {
                     OnGetPartyToAdminister(ownerAccount);
                     ContactInfoVM = new ContactInfoVM(dataManager, ContactInfoType.OwnedParties, ownerAccount.ContactInfoSet);
                 });
        }
    }
}

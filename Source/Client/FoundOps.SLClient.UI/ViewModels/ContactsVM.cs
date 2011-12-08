using System.Linq;
using MEFedMVVM.ViewModelLocator;
using FoundOps.Core.Context.Services;
using FoundOps.SLClient.Data.Services;
using FoundOps.SLClient.Data.ViewModels;
using FoundOps.Core.Models.CoreEntities;
using System.ComponentModel.Composition;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// Contains the logic for displaying Contacts
    /// </summary>
    [ExportViewModel("ContactsVM")]
    public class ContactsVM : CoreEntityCollectionInfiniteAccordionVM<Contact>
    {
        //Public Properties
        private PersonVM _selectedContactPersonVM;
        /// <summary>
        /// Gets the selected contact person VM.
        /// </summary>
        public PersonVM SelectedContactPersonVM
        {
            get { return _selectedContactPersonVM; }
            private set
            {
                _selectedContactPersonVM = value;
                this.RaisePropertyChanged("SelectedContactPersonVM");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContactsVM"/> class.
        /// </summary>
        /// <param name="partyDataService">The party data service.</param>
        /// <param name="dataManager">The data manager.</param>
        [ImportingConstructor]
        public ContactsVM(IPartyDataService partyDataService, DataManager dataManager)
            : base(dataManager)
        {
            SelectedContactPersonVM = new PersonVM(dataManager, partyDataService);

            this.SetupMainQuery(DataManager.Query.Contacts, null, "DisplayName");
        }

        #region Logic

        protected override bool BeforeSaveCommand()
        {
            return true;
        }

        protected override bool EntityIsPartOfView(Contact entity, bool isNew)
        {
            if (isNew)
                return true;

            var entityIsPartOfView = true;

            //Setup filters

            var clientContext = ContextManager.GetContext<Client>();
            if (clientContext != null)
                entityIsPartOfView = clientContext.ClientTitles.Any(ct => entity.Id == ct.ContactId && ct.ClientId == clientContext.Id);

            var businessAccountContext = ContextManager.GetContext<BusinessAccount>();

            if (businessAccountContext != null)
                entityIsPartOfView = entityIsPartOfView && entity.Id == businessAccountContext.Id;

            return entityIsPartOfView;
        }


        protected override void OnAddEntity(Contact newEntity)
        {
            //Create an OwnedPerson
            newEntity.OwnedPerson = new Person();

            //Set the OwnerParty
            newEntity.OwnerParty = this.ContextManager.OwnerAccount;

            base.OnAddEntity(newEntity);
        }

        protected override void OnSelectedEntityChanged(Contact oldValue, Contact newValue)
        {
            SelectedContactPersonVM.SelectedPerson = newValue != null ? newValue.OwnedPerson : null;

            base.OnSelectedEntityChanged(oldValue, newValue);
        }

        #endregion
    }
}

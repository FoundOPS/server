using ReactiveUI;
using System.Linq;
using System.Reactive.Linq;
using MEFedMVVM.ViewModelLocator;
using FoundOps.SLClient.Data.Services;
using FoundOps.Core.Models.CoreEntities;
using System.ComponentModel.Composition;
using FoundOps.SLClient.Data.ViewModels;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// Contains the logic for displaying Contacts
    /// </summary>
    [ExportViewModel("ContactsVM")]
    public class ContactsVM : InfiniteAccordionVM<Contact>
    {
        private readonly ObservableAsPropertyHelper<PartyVM> _selectedContactPersonVM;
        /// <summary>
        /// Gets the selected Contact's PartyVM
        /// </summary>
        public PartyVM SelectedContactPersonVM { get { return _selectedContactPersonVM.Value; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContactsVM"/> class.
        /// </summary>
        [ImportingConstructor]
        public ContactsVM()
        {
            this.SetupMainQuery(DataManager.Query.Contacts, null, "DisplayName");

            //Setup the selected contact's OwnedPerson PartyVM whenever the selected contact changes
            _selectedContactPersonVM =
                SelectedEntityObservable.Where(se => se != null && se.OwnedPerson != null).Select(se => new PartyVM(se.OwnedPerson))
                .ToProperty(this, x => x.SelectedContactPersonVM);
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

        #endregion
    }
}

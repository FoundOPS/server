using FoundOps.Core.Models.CoreEntities;
using FoundOps.SLClient.Data.ViewModels;
using MEFedMVVM.ViewModelLocator;
using ReactiveUI;
using System;
using System.ComponentModel.Composition;
using System.Reactive.Linq;
using System.ServiceModel.DomainServices.Client;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// Contains the logic for displaying Contacts
    /// </summary>
    [ExportViewModel("ContactsVM")]
    public class ContactsVM : InfiniteAccordionVM<Contact, Contact>
    {
        #region Public

        private readonly ObservableAsPropertyHelper<PartyVM> _selectedContactPersonVM;
        /// <summary>
        /// Gets the selected Contact's PartyVM
        /// </summary>
        public PartyVM SelectedContactPersonVM { get { return _selectedContactPersonVM.Value; } }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ContactsVM"/> class.
        /// </summary>
        [ImportingConstructor]
        public ContactsVM()
            : base(new[] { typeof(Client) })
        {
            SetupDataLoading();

            //Setup the selected contact's OwnedPerson PartyVM whenever the selected contact changes
            _selectedContactPersonVM =
                SelectedEntityObservable.Where(se => se != null && se.OwnedPerson != null).Select(se => new PartyVM(se.OwnedPerson))
                .ToProperty(this, x => x.SelectedContactPersonVM);
        }

        /// <summary>
        /// Used in the constructor to setup data loading.
        /// </summary>
        private void SetupDataLoading()
        {
            SetupContextDataLoading(roleId => DomainContext.GetContactsForRoleQuery(roleId),
                                    new[] { new ContextRelationshipFilter("OwnerPartyId",  typeof(Client), v => ((Client)v).Id) }, false, ContextLoadingType.NoRequiredContext);

            //Whenever the contact changes load the contact info
            SelectedEntityObservable.Where(se => se != null && se.OwnedPerson != null).Subscribe(selectedContact =>
                DomainContext.Load(DomainContext.GetContactInfoSetQuery().Where(c => c.PartyId == selectedContact.Id)));
        }

        #region Logic

        /// <summary>
        /// Called when [add entity].
        /// </summary>
        /// <param name="newEntity">The new entity.</param>
        protected override void OnAddEntity(Contact newEntity)
        {
            //Create an OwnedPerson
            newEntity.OwnedPerson = new Person();

            //Set the OwnerParty
            newEntity.OwnerParty = this.ContextManager.OwnerAccount;
        }

        #endregion
    }
}
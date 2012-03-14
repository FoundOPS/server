using System;
using System.Reactive.Subjects;
using System.ServiceModel.DomainServices.Client;
using ReactiveUI;
using System.Linq;
using System.Reactive.Linq;
using MEFedMVVM.ViewModelLocator;
using FoundOps.SLClient.Data.Services;
using FoundOps.Core.Models.CoreEntities;
using System.ComponentModel.Composition;
using FoundOps.SLClient.Data.ViewModels;
using Telerik.Windows.Data;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// Contains the logic for displaying Contacts
    /// </summary>
    [ExportViewModel("ContactsVM")]
    public class ContactsVM : InfiniteAccordionVM<Contact>
    {
        #region Public

        private QueryableCollectionView _queryableCollectionView;
        /// <summary>
        /// The collection of Contacts.
        /// </summary>
        public QueryableCollectionView QueryableCollectionView
        {
            get { return _queryableCollectionView; }
            private set
            {
                _queryableCollectionView = value;
                this.RaisePropertyChanged("QueryableCollectionView");
            }
        }

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
        {
            SetupDataLoading();

            //Setup the selected contact's OwnedPerson PartyVM whenever the selected contact changes
            _selectedContactPersonVM =
                SelectedEntityObservable.Where(se => se != null && se.OwnedPerson != null).Select(se => new PartyVM(se.OwnedPerson))
                .ToProperty(this, x => x.SelectedContactPersonVM);
        }

        private void SetupDataLoading()
        {
            var relatedTypes = new[] { typeof(Client) };

            var filterDescriptorsObservable = new[]
            {
                ContextManager.GetContextObservable<Client>().DistinctUntilChanged().ObserveOnDispatcher().Select(clientContext=> clientContext==null ? null :
                    new FilterDescriptor("OwnerPartyId", FilterOperator.IsEqualTo, clientContext.Id))
            };

            var disposeObservable = new Subject<bool>();

            //Whenever the RoleId updates, update the VirtualQueryableCollectionView
            ContextManager.RoleIdObservable.ObserveOnDispatcher().Subscribe(roleId =>
            {
                //Dispose the last VQCV subscriptions
                disposeObservable.OnNext(true);

                var initialQuery = Context.GetContactsForRoleQuery(ContextManager.RoleId);

                var result = DataManager.CreateContextBasedVQCV(initialQuery, disposeObservable, relatedTypes, filterDescriptorsObservable, false,
                    loadedEntities => { SelectedEntity = loadedEntities.FirstOrDefault(); });

                QueryableCollectionView = result.VQCV;

                //Subscribe the loading subject to the LoadingAfterFilterChange observable
                result.LoadingAfterFilterChange.Subscribe(IsLoadingSubject);
            });

            //Whenever the contact changes load the contact info
            SelectedEntityObservable.Where(se => se != null && se.OwnedPerson != null).Subscribe(selectedContact =>
                Context.Load(Context.GetContactInfoSetQuery().Where(c => c.PartyId == selectedContact.Id)));
        }

        #region Logic

        #region TODO Replace base with QueryableCollectionView and this logic

        /// <summary>
        /// The method called for the AddCommand to create an entity. Defaults to DomainCollectionView.AddNew()
        /// </summary>
        /// <param name="commandParameter">The command parameter.</param>
        /// <returns>The entity to add.</returns>
        protected override Contact AddNewEntity(object commandParameter)
        {
            var newContact = (Contact)this.QueryableCollectionView.AddNew();
            Context.Contacts.Add(newContact);
            return newContact;
        }

        /// <summary>
        /// The logic to delete an entity. Returns true if it can delete.
        /// </summary>
        /// <param name="entityToDelete">The entity to delete.</param>
        public override void DeleteEntity(Contact entityToDelete)
        {
            Context.Contacts.Remove(entityToDelete);
            this.QueryableCollectionView.Remove(entityToDelete);
            QueryableCollectionView.Refresh();
        }

        #endregion

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

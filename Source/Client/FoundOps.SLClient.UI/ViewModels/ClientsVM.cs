using System.Linq;
using System.Threading.Tasks;
using FoundOps.Common.Silverlight.Tools.ExtensionMethods;
using FoundOps.Common.Silverlight.UI.Controls;
using FoundOps.Common.Silverlight.UI.Controls.AddEditDelete;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.SLClient.Data.Services;
using FoundOps.SLClient.UI.Tools;
using FoundOps.SLClient.Data.ViewModels;
using FoundOps.Server.Services.CoreDomainService;
using MEFedMVVM.ViewModelLocator;
using ReactiveUI;
using System;
using System.Collections;
using System.ComponentModel.Composition;
using System.Reactive.Linq;
using System.ServiceModel.DomainServices.Client;
using System.Windows.Controls;
using System.Collections.Generic;

namespace FoundOps.SLClient.UI.ViewModels
{
    ///<summary>
    /// Contains the logic for displaying Clients
    ///</summary>
    [ExportViewModel("ClientsVM")]
    public class ClientsVM : InfiniteAccordionVM<Client, Client>,
        IAddToDeleteFromDestination<Location>, IAddNewExisting<Location>, IRemoveDelete<Location>
    {
        #region Public

        private readonly ObservableAsPropertyHelper<ContactInfoVM> _selectedClientContactInfoVM;
        /// <summary>
        /// Gets the selected Client's ContactInfoVM
        /// </summary>
        public ContactInfoVM SelectedClientContactInfoVM { get { return _selectedClientContactInfoVM.Value; } }

        /// <summary>
        /// A method to update the AddToDeleteFrom's AutoCompleteBox with suggestions remotely loaded.
        /// </summary>
        public Action<AutoCompleteBox> ManuallyUpdateSuggestions { get; private set; }

        #region Implementation of IAddToDeleteFromDestination

        private readonly List<IDisposable> _addToDeleteFromSubscriptions = new List<IDisposable>();
        /// <summary>
        /// Subscribes to the add delete from control observables.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="sourceType">Type of the source.</param>
        public void LinkToAddToDeleteFrom(AddToDeleteFrom control, Type sourceType)
        {
            if (sourceType == typeof(Location))
            {
                _addToDeleteFromSubscriptions.Add(control.AddExistingItem.Subscribe(existingItem => AddExistingItemLocation(existingItem)));
                _addToDeleteFromSubscriptions.Add(control.AddNewItem.Subscribe(text => AddNewItemLocation(text)));
                _addToDeleteFromSubscriptions.Add(control.RemoveItem.Subscribe(_ => RemoveItemLocation()));
                _addToDeleteFromSubscriptions.Add(control.DeleteItem.Subscribe(_ => DeleteItemLocation()));
            }
        }

        /// <summary>
        /// Disposes the subscriptions to the add delete from control observables.
        /// </summary>
        /// <param name="c">The control.</param>
        /// <param name="type">Type of the source.</param>
        public void UnlinkAddToDeleteFrom(AddToDeleteFrom c, Type type)
        {
            foreach (var subscription in _addToDeleteFromSubscriptions)
                subscription.Dispose();

            _addToDeleteFromSubscriptions.Clear();
        }

        /// <summary>
        /// Gets the locations destination items source.
        /// </summary>
        public IEnumerable LocationsDestinationItemsSource
        {
            get { return SelectedEntity == null ? null : SelectedEntity.Locations; }
        }

        #endregion

        #region Implementation of IAddNewExisting<Location> & IRemoveDelete<Location>

        /// <summary>
        /// An action to add an existing Location to the current BusinessAccount.
        /// </summary>
        public Action<object> AddExistingItemLocation { get; private set; }
        Action<object> IAddNewExisting<Location>.AddExistingItem { get { return AddExistingItemLocation; } }

        /// <summary>
        /// An action to add a new Location to the current BusinessAccount.
        /// </summary>
        public Func<string, Location> AddNewItemLocation { get; private set; }
        Func<string, Location> IAddNew<Location>.AddNewItem { get { return AddNewItemLocation; } }

        /// <summary>
        /// An action to remove a Location from the current BusinessAccount.
        /// </summary>
        public Func<Location> RemoveItemLocation { get; private set; }
        Func<Location> IRemove<Location>.RemoveItem { get { return RemoveItemLocation; } }

        /// <summary>
        /// An action to remove a Location from the current BusinessAccount and delete it.
        /// </summary>
        public Func<Location> DeleteItemLocation { get; private set; }
        Func<Location> IRemoveDelete<Location>.DeleteItem { get { return DeleteItemLocation; } }

        #endregion

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientsVM"/> class.
        /// </summary> 
        [ImportingConstructor]
        public ClientsVM()
        {
            SetupDataLoading();

            //Setup the selected client's ContactInfoVM whenever the selected client changes
            _selectedClientContactInfoVM =
                SelectedEntityObservable.Where(se => se != null && se.BusinessAccount != null).Select(se => new ContactInfoVM(ContactInfoType.Clients, se.ContactInfoSet))
                .ToProperty(this, x => x.SelectedClientContactInfoVM);

            ManuallyUpdateSuggestions = autoCompleteBox =>
                SearchSuggestionsHelper(autoCompleteBox, () => Manager.Data.DomainContext.SearchClientsForRoleQuery(Manager.Context.RoleId, autoCompleteBox.SearchText));

            #region Implementation of IAddToDeleteFromDestination<Location>

            //Whenever the SelectedEntity changes, notify the DestinationItemsSource changed
            this.SelectedEntityObservable.ObserveOnDispatcher().Subscribe(_ => this.RaisePropertyChanged("LocationsDestinationItemsSource"));

            #endregion

            #region Implementation of IAddNewExisting<Location> & IRemoveDelete<Location>

            AddNewItemLocation = name =>
            {
                var newLocation = VM.Locations.CreateNewItem(name);
                VM.Locations.MoveToDetailsView.Execute(null);
                return newLocation;
            };

            AddExistingItemLocation = existingItem =>
            {
                SelectedEntity.Locations.Add((Location)existingItem);
                VM.Locations.MoveToDetailsView.Execute(null);

                VM.Locations.SelectedEntity = (Location)existingItem;
            };

            RemoveItemLocation = () =>
            {
                var selectedLocation = VM.Locations.SelectedEntity;
                if (selectedLocation != null)
                    this.SelectedEntity.Locations.Remove(selectedLocation);

                return selectedLocation;
            };

            DeleteItemLocation = () =>
            {
                var selectedLocation = VM.Locations.SelectedEntity;
                if (selectedLocation != null)
                    VM.Locations.ConfirmDelete(selectedLocation);
                
                return selectedLocation;
            };

            #endregion
        }

        /// <summary>
        /// Used in the constructor to setup data loading.
        /// </summary>
        private void SetupDataLoading()
        {
            SetupTopEntityDataLoading(roleId => DomainContext.GetClientsForRoleQuery(ContextManager.RoleId));

            //Whenever the client changes load the ContactInfo, Available Services (ServiceTemplates),
            //RecurringServices, RecurringServices.Repeat, RecurringServices.ServiceTemplate (without fields)
            //This is all done in one query, otherwise it would take a long time
            SetupDetailsLoading(selectedEntity => DomainContext.GetClientDetailsForRoleQuery(ContextManager.RoleId, selectedEntity.Id));

            //Service templates are required for adding. So disable CanAdd until they are loaded.
            ContextManager.ServiceTemplatesLoading.Select(loading => !loading).Subscribe(CanAddSubject);
        }

        #region Logic

        protected override void OnAddEntity(Client newClient)
        {
            newClient.Name = "";
            newClient.BusinessAccount = (BusinessAccount)this.ContextManager.OwnerAccount;
            newClient.DateAdded = Manager.Context.UserAccount.Now().Date;

            //Add a default Location
            //Set the OwnerParty to the current OwnerAccount
            var defaultLocation = new Location
            {
                BusinessAccount = ContextManager.ServiceProvider,
                Region = ContextManager.GetContext<Region>()
            };
            newClient.Locations.Add(defaultLocation);

            defaultLocation.IsDefaultBillingLocation = true;

            //Add every available service to the client by default
            foreach (var serviceTemplate in ContextManager.CurrentServiceTemplates)
            {
                var availableServiceTemplate = serviceTemplate.MakeChild(ServiceTemplateLevel.ClientDefined);
                newClient.ServiceTemplates.Add(availableServiceTemplate);
            }
        }

        protected override void OnDeleteEntity(Client entityToDelete)
        {
            //Detach the entity graph related to the client 
            //they will be deleted in Client's delete method of the DomainService
            DataManager.DetachEntities(entityToDelete.EntityGraphToRemove);
        }

        #endregion
    }
}
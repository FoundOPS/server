using FoundOps.Common.Silverlight.UI.Controls.AddEditDelete;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.SLClient.Data.Services;
using FoundOps.SLClient.UI.Tools;
using FoundOps.SLClient.Data.ViewModels;
using MEFedMVVM.ViewModelLocator;
using ReactiveUI;
using System;
using System.Collections;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.ServiceModel.DomainServices.Client;
using System.Windows.Controls;

namespace FoundOps.SLClient.UI.ViewModels
{
    ///<summary>
    /// Contains the logic for displaying Clients
    ///</summary>
    [ExportViewModel("ClientsVM")]
    public class ClientsVM : InfiniteAccordionVM<Client>,
        IAddToDeleteFromDestination<Location>, IAddNewExisting<Location>, IRemoveDelete<Location>
    {
        #region Public

        private readonly ObservableAsPropertyHelper<PartyVM> _selectedClientOwnedBusinessVM;
        /// <summary>
        /// Gets the selected Client's OwnedParty's PartyVM. (The OwnedPary is a Business)
        /// </summary>
        public PartyVM SelectedClientOwnedBusinessVM { get { return _selectedClientOwnedBusinessVM.Value; } }

        /// <summary>
        /// A method to update the AddToDeleteFrom's AutoCompleteBox with suggestions remotely loaded.
        /// </summary>
        public Action<string, AutoCompleteBox> ManuallyUpdateSuggestions { get; private set; }

        #region Implementation of IAddToDeleteFromDestination

        /// <summary>
        /// Links to the LinkToAddToDeleteFromControl events.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="sourceType">Type of the source.</param>
        public void LinkToAddToDeleteFromEvents(AddToDeleteFrom control, Type sourceType)
        {
            if (sourceType == typeof(Location))
            {
                control.AddExistingItem += (s, existingItem) => this.AddExistingItemLocation((Location)existingItem);
                control.AddNewItem += (s, newItemText) => this.AddNewItemLocation(newItemText);
                control.RemoveItem += (s, e) => this.RemoveItemLocation();
                control.DeleteItem += (s, e) => this.DeleteItemLocation();
            }
        }

        /// <summary>
        /// Gets the locations destination items source.
        /// </summary>
        public IEnumerable LocationsDestinationItemsSource
        {
            get { return SelectedEntity == null || SelectedEntity.OwnedParty == null ? null : SelectedEntity.OwnedParty.Locations; }
        }

        #endregion

        #region Implementation of IAddNewExisting<Location> & IRemoveDelete<Location>

        /// <summary>
        /// An action to add a new Location to the current BusinessAccount.
        /// </summary>
        public Func<string, Location> AddNewItemLocation { get; private set; }
        Func<string, Location> IAddNew<Location>.AddNewItem { get { return AddNewItemLocation; } }

        /// <summary>
        /// An action to add an existing Location to the current BusinessAccount.
        /// </summary>
        public Action<Location> AddExistingItemLocation { get; private set; }
        Action<Location> IAddNewExisting<Location>.AddExistingItem { get { return AddExistingItemLocation; } }

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

            //Setup the selected client's OwnedParty PartyVM whenever the selected client changes
            _selectedClientOwnedBusinessVM =
                SelectedEntityObservable.Where(se => se != null && se.OwnedParty != null).Select(se => new PartyVM(se.OwnedParty))
                .ToProperty(this, x => x.SelectedClientOwnedBusinessVM);

            var serialDisposable = new SerialDisposable();

            //Whenever the client name changes update the default location name
            //There is a default location as long as there is only one location
            SelectedEntityObservable.Where(se => se != null && se.OwnedParty != null).Subscribe(selectedClient =>
            {
                serialDisposable.Disposable = Observable2.FromPropertyChangedPattern(selectedClient, x => x.DisplayName)
                    .Subscribe(displayName =>
                    {
                        var defaultLocation = selectedClient.OwnedParty.Locations.Count == 1
                                                  ? selectedClient.OwnedParty.Locations.First()
                                                  : null;

                        if (defaultLocation == null) return;
                        defaultLocation.Name = displayName;
                    });
            });

            ManuallyUpdateSuggestions = (searchText, autoCompleteBox) =>
                SearchSuggestionsHelper(autoCompleteBox, () => Manager.Data.DomainContext.SearchClientsForRoleQuery(Manager.Context.RoleId, searchText));

            #region Implementation of IAddToDeleteFromDestination<Location>

            //Whenever the SelectedEntity changes, notify the DestinationItemsSource changed
            this.SelectedEntityObservable.ObserveOnDispatcher().Subscribe(_ => this.RaisePropertyChanged("LocationsDestinationItemsSource"));

            #endregion

            #region Implementation of IAddNewExisting<Location> & IRemoveDelete<Location>

            AddNewItemLocation = name =>
            {
                var newLocation = VM.Locations.CreateNewItem(name);
                this.SelectedEntity.OwnedParty.Locations.Add(newLocation);
                VM.Locations.MoveToDetailsView.Execute(null);
                return newLocation;
            };

            AddExistingItemLocation = existingItem =>
            {
                SelectedEntity.OwnedParty.Locations.Add(existingItem);
                VM.Locations.MoveToDetailsView.Execute(null);
            };

            RemoveItemLocation = () =>
            {
                var selectedLocation = VM.Locations.SelectedEntity;
                if (selectedLocation != null)
                    this.SelectedEntity.OwnedParty.Locations.Remove(selectedLocation);

                return selectedLocation;
            };

            DeleteItemLocation = () =>
            {
                var selectedLocation = VM.Locations.SelectedEntity;
                if (selectedLocation != null)
                    VM.Locations.DeleteEntity(selectedLocation);

                return selectedLocation;
            };

            #endregion
        }

        /// <summary>
        /// Used in the constructor to setup data loading.
        /// </summary>
        private void SetupDataLoading()
        {
            SetupTopEntityDataLoading(roleId => Context.GetClientsForRoleQuery(ContextManager.RoleId));

            //Whenever the client changes load the contact info
            SelectedEntityObservable.Where(se => se != null && se.OwnedParty != null).Subscribe(selectedClient =>
                Context.Load(Context.GetContactInfoSetQuery().Where(c => c.PartyId == selectedClient.Id)));

            //Service templates are required for adding. So disable CanAdd until they are loaded.
            ContextManager.ServiceTemplatesLoading.Select(loading => !loading).Subscribe(CanAddSubject);
        }

        #region Logic

        #region TODO Replace base with QueryableCollectionView and this logic

        /// <summary>
        /// The method called for the AddCommand to create an entity. Defaults to DomainCollectionView.AddNew()
        /// </summary>
        /// <param name="commandParameter">The command parameter.</param>
        /// <returns>The entity to add.</returns>
        protected override Client AddNewEntity(object commandParameter)
        {
            var newClient = (Client)this.QueryableCollectionView.AddNew();
            Context.Clients.Add(newClient);
            return newClient;
        }

        #endregion

        protected override void OnAddEntity(Client newClient)
        {
            newClient.OwnedParty = new Business { Name = "" };
            newClient.Vendor = (BusinessAccount)this.ContextManager.OwnerAccount;

            //Add a default Location
            //Set the OwnerParty to the current OwnerAccount
            var defaultLocation = new Location
            {
                OwnerParty = ContextManager.OwnerAccount,
                Region = ContextManager.GetContext<Region>()
            };
            newClient.OwnedParty.Locations.Add(defaultLocation);

            //Add every available service to the client by default
            foreach (var serviceTemplate in ContextManager.CurrentServiceTemplates)
            {
                var availableServiceTemplate = serviceTemplate.MakeChild(ServiceTemplateLevel.ClientDefined);
                newClient.ServiceTemplates.Add(availableServiceTemplate);
            }

            //If there is a location context add it to the new client
            var currentLocation = ContextManager.GetContext<Location>();
            if (currentLocation != null)
                newClient.OwnedParty.OwnedLocations.Add(currentLocation);
        }

        protected override void OnDeleteEntity(Client entityToDelete)
        {
            //TODO remove Client's EntityGraphToRemove when DBOptimization is merged
            var clientEntitiesToRemove = entityToDelete.EntityGraphToRemove;
            DataManager.RemoveEntities(clientEntitiesToRemove);

            QueryableCollectionView.Remove(entityToDelete);
        }

        #endregion
    }
}
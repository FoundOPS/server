using System;
using ReactiveUI;
using System.Linq;
using System.Collections;
using Telerik.Windows.Data;
using System.Reactive.Linq;
using System.Collections.Generic;
using FoundOps.SLClient.UI.Tools;
using MEFedMVVM.ViewModelLocator;
using System.Reactive.Disposables;
using FoundOps.SLClient.Data.Services;
using FoundOps.SLClient.Data.ViewModels;
using System.ComponentModel.Composition;
using FoundOps.Core.Models.CoreEntities;
using Telerik.Windows.Controls.DomainServices;
using System.ServiceModel.DomainServices.Client;
using FoundOps.Common.Silverlight.Tools.ExtensionMethods;
using FoundOps.Common.Silverlight.UI.Controls.AddEditDelete;

namespace FoundOps.SLClient.UI.ViewModels
{
    ///<summary>
    /// Contains the logic for displaying Clients
    ///</summary>
    [ExportViewModel("ClientsVM")]
    public class ClientsVM : CoreEntityCollectionInfiniteAccordionVM<Client>,
        IAddToDeleteFromDestination<Location>, IAddNewExisting<Location>, IRemoveDelete<Location>
    {
        #region Public

        private QueryableCollectionView _queryableCollectionView;
        /// <summary>
        /// The collection of Clients.
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

        public string SearchText { get; set; }

        private readonly ObservableAsPropertyHelper<PartyVM> _selectedClientOwnedBusinessVM;
        /// <summary>
        /// Gets the selected Client's OwnedParty's PartyVM. (The OwnedPary is a Business)
        /// </summary>
        public PartyVM SelectedClientOwnedBusinessVM { get { return _selectedClientOwnedBusinessVM.Value; } }

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

        //private

        private IEnumerable<ServiceTemplate> _loadedServiceTemplates;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientsVM"/> class.
        /// </summary>
        /// <param name="dataManager">The data manager.</param>
        [ImportingConstructor]
        public ClientsVM(DataManager dataManager)
            : base(dataManager)
        {
            SetupDataLoading();

            //Setup the selected client's OwnedParty PartyVM whenever the selected client changes
            _selectedClientOwnedBusinessVM =
                SelectedEntityObservable.Where(se => se != null && se.OwnedParty != null).Select(se => new PartyVM(se.OwnedParty, this.DataManager))
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

        private void SetupDataLoading()
        {
            ////Wait until this is visible
            //ObservationState.Where(os=>os == Common.Models.ObservationState.Active).Take(1)
            //.Subscribe()

            //Whenever the RoleId updates, update the VirtualQueryableCollectionView
            ContextManager.RoleIdObservable.ObserveOnDispatcher().Subscribe(async roleId =>
            {
                //Service templates are required for adding
                CanAddSubject.OnNext(false);
                //Load the service templates
                var serviceTemplatesQuery = Context.GetServiceTemplatesForServiceProviderQuery(ContextManager.RoleId)
                    .Where(st => st.LevelInt == (int)ServiceTemplateLevel.ServiceProviderDefined);
                
                Context.Load(serviceTemplatesQuery, lo =>
                {
                    _loadedServiceTemplates = lo.Entities;
                    CanAddSubject.OnNext(true);
                }, null);

                #region Setup Client Loading

                var query = Context.GetClientsForRoleQuery(ContextManager.RoleId);

                var view = new VirtualQueryableCollectionView<Client>
                {
                    LoadSize = 100,
                    VirtualItemCount = await Context.CountAsync(query)
                };

                view.FilterDescriptors.CollectionChanged += async (s, e) =>
                {
                    view.VirtualItemCount = await Context.CountAsync(query.Where(view.FilterDescriptors));
                };

                view.FilterDescriptors.ItemChanged += async (s, e) =>
                {
                    view.VirtualItemCount = await Context.CountAsync(query.Where(view.FilterDescriptors));
                };

                view.ItemsLoading += async (s, e) =>
                {
                    var queryToLoad = query.Sort(view.SortDescriptors).Where(view.FilterDescriptors)
                                           .Skip(e.StartIndex).Take(e.ItemCount);

                    view.Load(e.StartIndex, await Context.LoadAsync(queryToLoad));
                };

                QueryableCollectionView = view;

                #endregion
            });

            //Whenever the client changes load the contact info
            SelectedEntityObservable.Where(se => se != null && se.OwnedParty != null).Subscribe(selectedClient =>
                Context.Load(Context.GetContactInfoSetQuery().Where(c => c.PartyId == selectedClient.Id)));
        }

        #region Logic

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

            this.RaisePropertyChanged("ClientsView");

            var availableServicesForServiceProvider = _loadedServiceTemplates.Where(st =>
                    st.ServiceTemplateLevel == ServiceTemplateLevel.ServiceProviderDefined &&
                    st.OwnerServiceProviderId == ContextManager.OwnerAccount.Id).ToArray();

            //Add every available service to the client by default
            foreach (var serviceTemplate in availableServicesForServiceProvider)
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
            //Remove Client's EntityGraphToRemove
            var clientEntitiesToRemove = entityToDelete.EntityGraphToRemove;

            DataManager.RemoveEntities(clientEntitiesToRemove);
        }

        #endregion
    }
}

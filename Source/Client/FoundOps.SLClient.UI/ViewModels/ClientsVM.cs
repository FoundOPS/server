using System;
using System.Reactive.Disposables;
using ReactiveUI;
using System.Linq;
using ReactiveUI.Xaml;
using System.Reactive.Linq;
using MEFedMVVM.ViewModelLocator;
using FoundOps.Core.Context.Services;
using FoundOps.SLClient.Data.Services;
using FoundOps.SLClient.Data.ViewModels;
using System.ComponentModel.Composition;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Common.Silverlight.Controls;
using Microsoft.Windows.Data.DomainServices;
using System.ServiceModel.DomainServices.Client;
using FoundOps.Common.Silverlight.UI.Controls.InfiniteAccordion;

namespace FoundOps.SLClient.UI.ViewModels
{
    ///<summary>
    /// Contains the logic for displaying Clients
    ///</summary>
    [ExportViewModel("ClientsVM")]
    public class ClientsVM : CoreEntityCollectionInfiniteAccordionVM<Client>
    {
        #region Public

        /// <summary>
        /// A command to add a location.
        /// </summary>
        public IReactiveCommand AddLocationCommand { get; private set; }

        /// <summary>
        /// A command to remove a location.
        /// </summary>
        public IReactiveCommand DeleteLocationCommand { get; private set; }

        private readonly ObservableAsPropertyHelper<PartyVM> _selectedClientOwnedBusinessVM;
        /// <summary>
        /// Gets the selected Client's OwnedParty's PartyVM. (The OwnedPary is a Business)
        /// </summary>
        public PartyVM SelectedClientOwnedBusinessVM { get { return _selectedClientOwnedBusinessVM.Value; } }

        #endregion

        //private

        private EntityList<ServiceTemplate> _loadedServiceTemplates;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientsVM"/> class.
        /// </summary>
        /// <param name="partyDataService">The party data service.</param>
        /// <param name="dataManager">The data manager.</param>
        [ImportingConstructor]
        public ClientsVM(IPartyDataService partyDataService, DataManager dataManager)
            : base(dataManager)
        {
            //Setup the MainQuery to load Clients
            SetupMainQuery(DataManager.Query.Clients, null, "DisplayName");

            //ClientsVM requires ServiceTemplate
            DataManager.Subscribe<ServiceTemplate>(DataManager.Query.ServiceTemplates, ObservationState, entities => _loadedServiceTemplates = entities);

            //Can only add whenever ServiceTemplates is loaded
            DataManager.GetIsLoadingObservable(DataManager.Query.ServiceTemplates).Subscribe(isLoading => CanAddSubject.OnNext(!isLoading));

            //Setup the selected client's OwnedParty PartyVM whenever the selected client changes
            _selectedClientOwnedBusinessVM =
                SelectedEntityObservable.Where(se => se != null && se.OwnedParty != null).Select(se => new PartyVM(se.OwnedParty, this.DataManager))
                .ToProperty(this, x => x.SelectedClientOwnedBusinessVM);

            #region Register Commands

            AddLocationCommand = new ReactiveCommand(this.WhenAny(x => x.SelectedEntity, client => client.Value != null));

            AddLocationCommand.Subscribe(param =>
            {
                var locationsVM = param as LocationsVM;
                if (locationsVM == null) return;

                //The location is either the LocationInCreation or an existing location

                var locationToAdd = locationsVM.LocationInCreation;

                //If the selected location.Name == the LocationInCreation.Name, add the existing location
                if (locationsVM.SelectedEntity != null && locationsVM.SelectedEntity.Name == locationsVM.LocationInCreation.Name)
                {
                    locationToAdd = locationsVM.SelectedEntity;
                    locationsVM.DeleteLocationInCreation();
                }

                //add the location to the current Account
                ContextManager.OwnerAccount.OwnedLocations.Add(locationToAdd);

                //add the Location to the selected client
                SelectedEntity.OwnedParty.Locations.Add(locationToAdd);

                //Must Commit and EndEdit to prevent error on Save or DiscardChanges
                this.Commit();
                DataManager.EnqueueSubmitOperation();

                //Move to Locations context only if a new location was added
                if (locationToAdd.EntityState == EntityState.New)
                    MessageBus.Current.SendMessage(new MoveToDetailsViewMessage(typeof(Location), MoveStrategy.AddContextToExisting));

                //Update LocationsVM filter
                locationsVM.UpdateFilter();

                //Set the selected entity to the added location
                locationsVM.SelectedEntity = locationToAdd;
            });

            DeleteLocationCommand = new ReactiveCommand(this.WhenAny(x => x.SelectedEntity, client => client.Value != null));

            DeleteLocationCommand.Subscribe(param =>
            {
                var locationsVM = param as LocationsVM;
                if (locationsVM == null || locationsVM.SelectedEntity == null) return;

                var location = locationsVM.SelectedEntity;
                //Set up the pop up text box to have the client and locations name
                var removeDeleteCancel = new RemoveDeleteCancel
                {
                    LocationNameTextBox = location.Name,
                    ClientNameTextBox = location.Party.DisplayName
                };

                removeDeleteCancel.RemoveButton.Click += (sender, e) =>
                {
                    //Removes the location from the Client
                    this.SelectedEntity.OwnedParty.Locations.Remove(location);
                    DataManager.EnqueueSubmitOperation();
                    removeDeleteCancel.Close();
                };
                removeDeleteCancel.DeleteButton.Click += (sender, e) =>
                {
                    //Delete the location entirely
                    locationsVM.DeleteEntity(location);
                    DataManager.EnqueueSubmitOperation();
                    removeDeleteCancel.Close();
                };
                removeDeleteCancel.CancelButtonRDC.Click += (sender, e) =>
                {
                    //Do nothing and close the window
                    removeDeleteCancel.Close();
                    return;
                };

                removeDeleteCancel.Show();

                return;
            });

            #endregion

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

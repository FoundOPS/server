using System;
using System.Linq;
using System.ServiceModel.DomainServices.Client;
using System.Windows;
using FoundOps.Common.Silverlight.Controls;
using FoundOps.Common.Silverlight.UI.Controls.InfiniteAccordion;
using ReactiveUI;
using ReactiveUI.Xaml;
using System.ComponentModel;
using MEFedMVVM.ViewModelLocator;
using FoundOps.Core.Context.Services;
using FoundOps.SLClient.Data.Services;
using FoundOps.SLClient.Data.ViewModels;
using System.ComponentModel.Composition;
using FoundOps.Core.Models.CoreEntities;
using Microsoft.Windows.Data.DomainServices;

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


        private BusinessVM _selectedClientOwnedBusinessVM;
        /// <summary>
        /// Gets the selected client owned business VM.
        /// </summary>
        public BusinessVM SelectedClientOwnedBusinessVM
        {
            get { return _selectedClientOwnedBusinessVM; }
            private set
            {
                if (SelectedClientOwnedBusinessVM != null)
                    SelectedClientOwnedBusinessVM.PropertyChanged -= SelectedClientOwnedBusinessVMPropertyChanged; _selectedClientOwnedBusinessVM = value;
                if (SelectedClientOwnedBusinessVM != null)
                    SelectedClientOwnedBusinessVM.PropertyChanged += SelectedClientOwnedBusinessVMPropertyChanged;
                this.RaisePropertyChanged("SelectedClientOwnedBusinessVM");
            }
        }

        void SelectedClientOwnedBusinessVMPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "SelectedBusiness") return;

            if (SelectedClientOwnedBusinessVM.SelectedParty == null)
            {
                SelectedEntity = null;
                return;
            }

            this.RaisePropertyChanged("SelectedContext"); //In case the BusinessVM adds a Location

            if (SelectedEntity == SelectedClientOwnedBusinessVM.SelectedParty.ClientOwner) return;

            SelectedEntity = SelectedClientOwnedBusinessVM.SelectedParty.ClientOwner;
        }

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
            SelectedClientOwnedBusinessVM = new BusinessVM(dataManager, partyDataService);

            //Setup the MainQuery to load Clients
            SetupMainQuery(DataManager.Query.Clients, null, "DisplayName");

            DataManager.Subscribe<ServiceTemplate>(DataManager.Query.ServiceTemplates, ObservationState, entities => _loadedServiceTemplates = entities);

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
        }

        #region Logic

        protected override void OnAddEntity(Client newClient)
        {
            newClient.OwnedParty = new Business { Name = "" };
            newClient.Vendor = (BusinessAccount)this.ContextManager.OwnerAccount;
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

        protected override void OnSelectedEntityChanged(Client oldValue, Client newValue)
        {
            if (newValue == null || !(newValue.OwnedParty is Business))
                SelectedClientOwnedBusinessVM.SelectedBusiness = null;
            else
                SelectedClientOwnedBusinessVM.SelectedBusiness = (Business)newValue.OwnedParty;

            base.OnSelectedEntityChanged(oldValue, newValue);
        }

        #endregion
    }
}

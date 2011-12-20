using System;
using System.IO;
using System.Windows.Controls;
using Kent.Boogaart.KBCsv;
using ReactiveUI;
using System.Linq;
using System.Reactive.Linq;
using System.ComponentModel;
using FoundOps.Common.Tools;
using System.Reactive.Subjects;
using System.Collections.Generic;
using MEFedMVVM.ViewModelLocator;
using GalaSoft.MvvmLight.Command;
using FoundOps.SLClient.Data.Services;
using FoundOps.SLClient.Data.ViewModels;
using System.ComponentModel.Composition;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Common.Silverlight.Services;
using Microsoft.Windows.Data.DomainServices;
using FoundOps.Core.Context.Services.Interface;
using ReactiveUI.Xaml;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// Contains the logic for displaying Locations
    /// </summary>
    [ExportViewModel("LocationsVM")]
    public class LocationsVM : CoreEntityCollectionInfiniteAccordionVM<Location>
    {
        #region Public Properties

        private readonly ObservableAsPropertyHelper<bool> _canExportCSV;
        /// <summary>
        /// Gets a value indicating whether this instance can export CSV.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can export CSV; otherwise, <c>false</c>.
        /// </value>
        public bool CanExportCSV { get { return _canExportCSV.Value; } }

        #region Location Properties

        public ObservableAsPropertyHelper<IEnumerable<Location>> _locationsWithoutClient;
        /// <summary>
        /// Gets the Locations without Clients.
        /// </summary>
        public IEnumerable<Location> LocationsWithoutClient { get { return _locationsWithoutClient.Value; } }

        private readonly Subject<LocationVM> _selectedLocationVMObservable = new Subject<LocationVM>();
        /// <summary>
        /// Gets the location VM observable.
        /// </summary>
        public IObservable<LocationVM> SelectedLocationVMObservable { get { return _selectedLocationVMObservable; } }

        private readonly ObservableAsPropertyHelper<LocationVM> _selectedLocationVM;
        /// <summary>
        /// Gets the selected location's LocationVM.
        /// </summary>
        public LocationVM SelectedLocationVM { get { return _selectedLocationVM.Value; } }

        #endregion

        #region SubLocation Properties

        private readonly Subject<SubLocationsVM> _selectedSubLocationsVMObservable = new Subject<SubLocationsVM>();
        /// <summary>
        /// Gets or sets the location VM observable.
        /// </summary>
        /// <value>
        /// The location VM observable.
        /// </value>
        public IObservable<SubLocationsVM> SelectedSubLocationsVMObservable
        {
            get { return _selectedSubLocationsVMObservable; }
        }

        private readonly ObservableAsPropertyHelper<SubLocationsVM> _selectedSubLocationsVM;
        /// <summary>
        /// Gets the selected location's SubLocationsVM.
        /// </summary>
        public SubLocationsVM SelectedSubLocationsVM { get { return _selectedSubLocationsVM.Value; } }

        #endregion

        #endregion

        //Local Properties
        public Location _locationInCreation;
        public Location LocationInCreation
        {
            get { return _locationInCreation; }
            set
            {
                if (LocationInCreation != null)
                    LocationInCreation.PropertyChanged -= LocationInCreationPropertyChanged;

                _locationInCreation = value;

                if (LocationInCreation != null)
                    LocationInCreation.PropertyChanged += LocationInCreationPropertyChanged;

                this.RaisePropertyChanged("LocationInCreation");
            }
        }

        void LocationInCreationPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "HasValidationErrors")
            {
                //Whenever the LocationInCreation's HasValidationErrors property changes update the AddCommand's CanExecute
                ((RelayCommand<object>)this.AddCommand).RaiseCanExecuteChanged();
            }
            else if (e.PropertyName == "Location")
            {
                //Whenever the LocationInCreation's ClientId or Title changed update its validation errors
                LocationInCreation.RaiseValidationErrors();
            }
        }

        //Locals
        private readonly ILocationsDataService _locationsDataService;

        //The loaded locations entity list observable
        private readonly IObservable<EntityList<Location>> _loadedLocations;
        private readonly ObservableAsPropertyHelper<EntityList<Location>> _loadedLocationsProperty;
        private EntityList<Location> LoadedLocations { get { return _loadedLocationsProperty.Value; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocationsVM"/> class.
        /// </summary>
        /// <param name="dataManager">The data manager.</param>
        /// <param name="locationsDataService">The locations data service.</param>
        [ImportingConstructor]
        public LocationsVM(DataManager dataManager, ILocationsDataService locationsDataService)
            : base(dataManager)
        {
            _locationsDataService = locationsDataService;

            //Subscribe to the locations query
            IsLoadingObservable = DataManager.Subscribe<Location>(DataManager.Query.Locations, this.ObservationState, null);

            IsLoadingObservable.Subscribe(l =>
                                              {

                                              });

            _loadedLocations = DataManager.GetEntityListObservable<Location>(DataManager.Query.Locations);
            _loadedLocationsProperty = _loadedLocations.ToProperty(this, x => x.LoadedLocations);

            #region DomainCollectionView

            //Whenever the OwnerAccount, or the Client or Region context changes, and when the loaded locations changes, update the DCV
            this.ContextManager.OwnerAccountObservable.AsGeneric().Merge(ContextManager.GetContextObservable<Client>().AsGeneric()).Merge(this.ContextManager.GetContextObservable<Region>().AsGeneric()).Merge(_loadedLocations.AsGeneric())
                .Throttle(TimeSpan.FromMilliseconds(200))
                .ObserveOnDispatcher().Subscribe(_ =>
            {
                IEnumerable<Location> setOfLocations;
                var closestContext = this.ContextManager.ClosestContext(new[] { typeof(Client), typeof(Region) });

                if (closestContext is Client)
                    setOfLocations = ((Client)closestContext).OwnedParty.Locations;
                else if (closestContext is Region)
                    setOfLocations = ((Region)closestContext).Locations;
                else
                    setOfLocations = this.ContextManager.OwnerAccount == null
                                         ? (IEnumerable<Location>)new List<Location>()
                                         : this.ContextManager.OwnerAccount.OwnedLocations;

                this.DomainCollectionViewObservable.OnNext(DomainCollectionViewFactory<Location>.GetDomainCollectionView(setOfLocations));
            });

            //Whenever the DCV changes, sort by Name and select the first entity
            this.DomainCollectionViewObservable.Throttle(TimeSpan.FromMilliseconds(300)) //wait for UI to load
                .ObserveOnDispatcher().Subscribe(dcv =>
            {
                dcv.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
                this.SelectedEntity = this.DomainCollectionView.FirstOrDefault();
            });

            #endregion

            #region LocationsWithoutClient

            //Whenever loadedLocations changes, select the IObservable<IList<Location>> of Locations without a Client
            var locationsWithoutClient = _loadedLocations.FromCollectionChangedOrSet()
                .SelectMany(loadedLocs => loadedLocs.Select(loadedLoc =>
                    //Whenever the Location.Party.ClientOwner == null
                    loadedLoc.WhenAny(x => x.Party, x => x.Party.ClientOwner, (party, clientOwner) => party.Value == null || clientOwner.Value == null)
                        //Select the Location if there is noClientOwner
                    .Select(noClientOwner => noClientOwner ? loadedLoc : null))
                    //Combine the latest of the Location Observables into a list
                    .CombineLatest())
                //Remove the locations that are null
                    .Select(locations => locations.Where(l => l != null));

            //Setup the LocationsWithoutClient property
            _locationsWithoutClient = locationsWithoutClient.ToProperty(this, x => x.LocationsWithoutClient);

            #endregion

            #region Entity

            //Hookup _selectedLocationVM to SelectedLocationVMObservable
            _selectedLocationVM = SelectedLocationVMObservable.ToProperty(this, x => x.SelectedLocationVM);

            //Hookup _selectedSubLocationsVM to SelectedSubLocationsVMObservable
            _selectedSubLocationsVM =
                SelectedSubLocationsVMObservable.ToProperty(this, x => x.SelectedSubLocationsVM, new SubLocationsVM(dataManager, locationsDataService, SelectedEntity));

            //Whenever the SelectedEntity changes: create a new LocationVM and SubLocationsVM; update the SearchText
            SelectedEntityObservable.ObserveOnDispatcher().Subscribe(selectedLocation =>
            {
                if (selectedLocation == null)
                {
                    _selectedLocationVMObservable.OnNext(null);
                    _selectedSubLocationsVMObservable.OnNext(null);
                    return;
                }

                //Create a new LocationVM
                _selectedLocationVMObservable.OnNext(new LocationVM(selectedLocation, dataManager, locationsDataService));

                //Create a new SubLocationsVM
                _selectedSubLocationsVMObservable.OnNext(new SubLocationsVM(DataManager, _locationsDataService, selectedLocation));
            });

            #endregion

            //Setup CanExportCSV property
            //it can execute when Locations, Clients, and Regions are loaded
            var canExportCSV = IsLoadingObservable.CombineLatest(DataManager.GetIsLoadingObservable(DataManager.Query.Clients), DataManager.GetIsLoadingObservable(DataManager.Query.Regions),
                                                  (locationsLoading, clientsLoading, regionsLoading) => !locationsLoading && !clientsLoading && !regionsLoading);
            _canExportCSV = canExportCSV.ToProperty(this, x => x.CanExportCSV);
        }

        #region Logic

        #region Export to CSV

        /// <summary>
        /// Exports to a CSV file.
        /// NOTE: Must be called directly in a user initiated event handler (like a click) for security purposes for SaveFileDialog. Therefore it cannot be executed from a command.
        /// </summary>
        public void ExportToCSV()
        {
            var fileName = String.Format("LocationsExport {0}.csv", DateTime.Now.ToString("MM'-'dd'-'yyyy"));
            var saveFileDialog = new SaveFileDialog { DefaultFileName = fileName, DefaultExt = ".csv", Filter = "CSV File|*.csv" };

            if(saveFileDialog.ShowDialog() == true)
            {
                using (var fileWriter = new StreamWriter(saveFileDialog.OpenFile()))
                {
                    var csvWriter = new CsvWriter(fileWriter);

                    csvWriter.WriteHeaderRecord("Name", "Region", "Client",
                                             "Address 1", "Address 2", "City", "State", "Zip Code",
                                             "Latitude", "Longitude");

                    foreach (var location in DomainCollectionView)
                    {
                        csvWriter.WriteDataRecord(location.Name,
                                               location.Region != null ? location.Region.Name : "",
                                               location.Party != null && location.Party.ClientOwner != null ? location.Party.DisplayName : "",
                                               location.AddressLineOne, location.AddressLineTwo, location.City, location.State, location.ZipCode,
                                               location.Latitude, location.Longitude);
                    }

                    csvWriter.Close();
                    fileWriter.Close();
                }
            }
        }

        #endregion

        #region Location in creation (for adding a Location to an entity)

        /// <summary>
        /// Deletes the location in creation.
        /// </summary>
        public void DeleteLocationInCreation()
        {
            //Remove the Location from the Locations EntitySet
            this.Context.Locations.Remove(LocationInCreation);

            //Clear LocationInCreation
            LocationInCreation = null;
        }

        /// <summary>
        /// Starts the creation of a location.
        /// </summary>
        /// <returns></returns>
        public Location StartCreationOfLocation()
        {
            //Create a new Location
            LocationInCreation = new Location();

            //Do default add location logic
            OnAddEntity(LocationInCreation);

            return LocationInCreation;
        }

        #endregion

        protected override void OnAddEntity(Location newLocation)
        {
            //Set the OwnerParty to the current OwnerAccount
            newLocation.OwnerParty = ContextManager.OwnerAccount;

            var client = ContextManager.GetContext<Client>();

            //If the client context != null add this to the current Client's Locations
            if (client != null)
                client.OwnedParty.Locations.Add(newLocation);

            var region = ContextManager.GetContext<Region>();

            //If the region context != null add this to the current Regions's Locations
            if (region != null)
                region.Locations.Add(newLocation);

            newLocation.RaiseValidationErrors();
        }

        public override void DeleteEntity(Location locationToDelete)
        {
            //Remove Location and it's EntityGraphToRemove
            //This is not automatically done because the DCV is not backed by an EntityList
            var locationEntitiesToRemove = locationToDelete.EntityGraphToRemove;
            DataManager.RemoveEntities(locationEntitiesToRemove);

            base.DeleteEntity(locationToDelete);
        }

        #endregion
    }
}

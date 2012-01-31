using System;
using System.IO;
using ReactiveUI;
using System.Linq;
using System.Collections;
using Kent.Boogaart.KBCsv;
using System.Reactive.Linq;
using System.ComponentModel;
using FoundOps.Common.Tools;
using System.Windows.Controls;
using System.Reactive.Subjects;
using System.Collections.Generic;
using MEFedMVVM.ViewModelLocator;
using FoundOps.SLClient.Data.Services;
using FoundOps.SLClient.Data.ViewModels;
using System.ComponentModel.Composition;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Common.Silverlight.Services;
using Microsoft.Windows.Data.DomainServices;
using FoundOps.Common.Silverlight.UI.Controls.AddEditDelete;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// Contains the logic for displaying Locations
    /// </summary>
    [ExportViewModel("LocationsVM")]
    public class LocationsVM : CoreEntityCollectionInfiniteAccordionVM<Location>,
          IAddToDeleteFromSource<Location>
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

        private readonly ObservableAsPropertyHelper<IEnumerable<Location>> _locationsWithoutClient;
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

        #region Implementation of IAddToDeleteFromSource<Location>

        public Func<string, Location> CreateNewItem { get; private set; }

        //There is none required
        public IEqualityComparer<object> CustomComparer { get; set; }

        public IEnumerable ExistingItemsSource { get { return _locationsWithoutClient.Value; } }

        public string MemberPath { get; private set; }

        #endregion

        #endregion

        #region Locals

        //The loaded locations entity list observable
        private readonly IObservable<EntityList<Location>> _loadedLocations;
        private readonly ObservableAsPropertyHelper<EntityList<Location>> _loadedLocationsProperty;
        private EntityList<Location> LoadedLocations { get { return _loadedLocationsProperty.Value; } }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="LocationsVM"/> class.
        /// </summary>
        /// <param name="dataManager">The data manager.</param>
        [ImportingConstructor]
        public LocationsVM(DataManager dataManager)
            : base(dataManager)
        {
            //Subscribe to the locations query
            IsLoadingObservable = DataManager.Subscribe<Location>(DataManager.Query.Locations, this.ObservationState, null);

            _loadedLocations = DataManager.GetEntityListObservable<Location>(DataManager.Query.Locations);
            _loadedLocationsProperty = _loadedLocations.ToProperty(this, x => x.LoadedLocations);

            #region LocationsWithoutClient

            //Whenever loadedLocations changes, select the IObservable<IList<Location>> of Locations without a Client
            var locationsWithoutClient = _loadedLocations.FromCollectionChangedOrSet()
                .SelectLatest(loadedLocs => loadedLocs.Select(loadedLoc =>
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

            #region IAddToDeleteFromSource<Location> Implementation

            //Whenever the _locationsWithoutClient changes notify ExistingItemsSource changed
            _locationsWithoutClient.ToProperty(this, x => x.ExistingItemsSource);

            MemberPath = "Name";

            CreateNewItem = name =>
            {
                //Set the Party to the current OwnerAccount
                var newLocation = new Location { Party = ContextManager.OwnerAccount };

                var client = ContextManager.GetContext<Client>();

                //If the client context != null add this to the current Client's Locations
                if (client != null)
                    client.OwnedParty.Locations.Add(newLocation);

                var region = ContextManager.GetContext<Region>();

                //If the region context != null add this to the current Regions's Locations
                if (region != null)
                    region.Locations.Add(newLocation);

                newLocation.RaiseValidationErrors();

                //Add the new entity to the loaded locations EntityList
                LoadedLocations.Add(newLocation);

                return newLocation;
            };

            #endregion

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

            #region Entity

            //Hookup _selectedLocationVM to SelectedLocationVMObservable
            _selectedLocationVM = SelectedLocationVMObservable.ToProperty(this, x => x.SelectedLocationVM);

            //Hookup _selectedSubLocationsVM to SelectedSubLocationsVMObservable
            _selectedSubLocationsVM =
                SelectedSubLocationsVMObservable.ToProperty(this, x => x.SelectedSubLocationsVM, new SubLocationsVM(dataManager, SelectedEntity));

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
                _selectedLocationVMObservable.OnNext(new LocationVM(selectedLocation, dataManager));

                //Create a new SubLocationsVM
                _selectedSubLocationsVMObservable.OnNext(new SubLocationsVM(DataManager, selectedLocation));
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

            if (saveFileDialog.ShowDialog() == true)
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

        protected override Location AddNewEntity(object commandParameter)
        {
            //Reuse the CreateNewItem method
            return CreateNewItem("");
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

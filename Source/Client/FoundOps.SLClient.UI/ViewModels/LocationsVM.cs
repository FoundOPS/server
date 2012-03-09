using System;
using System.IO;
using System.Linq;
using System.ServiceModel.DomainServices.Client;
using ReactiveUI;
using System.Collections;
using Kent.Boogaart.KBCsv;
using Telerik.Windows.Data;
using System.Reactive.Linq;
using System.Windows.Controls;
using System.Reactive.Subjects;
using System.Collections.Generic;
using MEFedMVVM.ViewModelLocator;
using FoundOps.SLClient.Data.Services;
using FoundOps.SLClient.Data.ViewModels;
using System.ComponentModel.Composition;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Common.Silverlight.UI.Controls.AddEditDelete;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// Contains the logic for displaying Locations
    /// </summary>
    [ExportViewModel("LocationsVM")]
    public class LocationsVM : CoreEntityCollectionInfiniteAccordionVM<Location>, IAddToDeleteFromSource<Location>
    {
        #region Public Properties

        #region Entity Data Items

        private QueryableCollectionView _queryableCollectionView;
        /// <summary>
        /// The collection of Locations.
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

        /// <summary>
        /// The search text.
        /// </summary>
        public string SearchText { get; set; }

        /// <summary>
        /// The collection of items being searched.
        /// </summary>
        public IEnumerable<Location> SearchCollectionView
        {
            get;
            set;
        }

        #endregion

        #region Location Properties

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

        public IEnumerable ExistingItemsSource { get { return null; } }

        public string MemberPath { get; private set; }

        #endregion

        #endregion

        #region Locals

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="LocationsVM"/> class.
        /// </summary>
        /// <param name="dataManager">The data manager.</param>
        [ImportingConstructor]
        public LocationsVM(DataManager dataManager)
            : base(dataManager)
        {
            SetupDataLoading();

            #region Setup location/sublocation properties

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

            #region IAddToDeleteFromSource<Location> Implementation

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

                return newLocation;
            };

            #endregion
        }

        private void SetupDataLoading()
        {
            var relatedTypes = new[] { typeof(Region), typeof(Client) };

            var filterDescriptorsObservable = new[]
            {
                ContextManager.GetContextObservable<Region>().DistinctUntilChanged().ObserveOnDispatcher().Select(regionContext=> regionContext==null ? null :
                    new FilterDescriptor("RegionId", FilterOperator.IsEqualTo, regionContext.Id)),
                ContextManager.GetContextObservable<Client>().DistinctUntilChanged().ObserveOnDispatcher().Select(clientContext=> clientContext==null ? null :
                    new FilterDescriptor("PartyId", FilterOperator.IsEqualTo, clientContext.Id))
            };

            var disposeObservable = new Subject<bool>();

            //Whenever the RoleId updates, update the VirtualQueryableCollectionView
            ContextManager.RoleIdObservable.ObserveOnDispatcher().Subscribe(roleId =>
            {
                //Dispose the last VQCV subscriptions
                disposeObservable.OnNext(true);

                var initialQuery = Context.GetLocationsToAdministerForRoleQuery(ContextManager.RoleId);

                //Force load the entities when in a related types view
                //this is because VDCV will only normally load when a virtual item is loaded onto the screen
                //virtual items will not always load because in clients context the gridview does not always show (sometimes it is in single view)
                var result = DataManager.CreateContextBasedVQCV(initialQuery, disposeObservable, relatedTypes, filterDescriptorsObservable, true,
                    loadedEntities => { SelectedEntity = loadedEntities.FirstOrDefault(); });

                QueryableCollectionView = result.VQCV;

                //Subscribe the loading subject to the LoadingAfterFilterChange observable
                result.LoadingAfterFilterChange.Subscribe(IsLoadingSubject);
            });

            LoadOperation<Location> detailsLoadOperation = null;

            //Whenever the location changes load the location details
            SelectedEntityObservable.Where(se => se != null).Subscribe(selectedLocation =>
            {
                if (detailsLoadOperation != null && detailsLoadOperation.CanCancel)
                    detailsLoadOperation.Cancel();

                selectedLocation.DetailsLoading = true;
                detailsLoadOperation = Context.Load(Context.GetLocationDetailsForRoleQuery(ContextManager.RoleId, selectedLocation.Id),
                                 loadOp => selectedLocation.DetailsLoading = false, null);
            });
        }

        #region Logic

        #region Export to CSV

        /// <summary>
        /// Exports to a CSV file.
        /// NOTE: Must be called directly in a user initiated event handler (like a click) for security purposes for SaveFileDialog. Therefore it cannot be executed from a command.
        /// </summary>
        public void ExportToCSV()
        {
            var csvLoadedObservable = new ReplaySubject<byte[]>();

            var clientContext = ContextManager.GetContext<Client>();
            var regionContext = ContextManager.GetContext<Region>();

            //Load the CSV
            Context.GetLocationsCSVForRole(ContextManager.RoleId, clientContext != null ? clientContext.Id : new Guid(), regionContext != null ? regionContext.Id : new Guid(),
                loadedCSV => csvLoadedObservable.OnNext(loadedCSV.Value), null);

            var fileName = String.Format("LocationsExport {0}.csv", DateTime.Now.ToString("MM'-'dd'-'yyyy"));
            var saveFileDialog = new SaveFileDialog { DefaultFileName = fileName, DefaultExt = ".csv", Filter = "CSV File|*.csv" };

            if (saveFileDialog.ShowDialog() != true) return;

            csvLoadedObservable.Take(1).ObserveOnDispatcher().Subscribe(csvByteArray =>
            {
                var fileWriter = new BinaryWriter(saveFileDialog.OpenFile());
                fileWriter.Write(csvByteArray);
                fileWriter.Close();
            });
        }

        #endregion

        #region Add Delete Entities

        protected override Location AddNewEntity(object commandParameter)
        {
            //Reuse the CreateNewItem method
            return CreateNewItem("");
        }

        public override void DeleteEntity(Location locationToDelete)
        {
            //Remove Location (this is not automatically done because the DCV is not backed by an EntityList)
            DataManager.RemoveEntities(new[] { locationToDelete });

            base.DeleteEntity(locationToDelete);
        }

        #endregion

        #endregion
    }
}

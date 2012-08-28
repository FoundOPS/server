using FoundOps.Common.Silverlight.Tools.ExtensionMethods;
using FoundOps.Common.Silverlight.UI.Controls.AddEditDelete;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.SLClient.Data.Services;
using FoundOps.SLClient.Data.ViewModels;
using MEFedMVVM.ViewModelLocator;
using ReactiveUI;
using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.ServiceModel.DomainServices.Client;
using System.Windows.Controls;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// Contains the logic for displaying Locations
    /// </summary>
    [ExportViewModel("LocationsVM")]
    public class LocationsVM : InfiniteAccordionVM<Location, Location>, IAddToDeleteFromSource<Location>
    {
        #region Public Properties

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

        public string MemberPath { get; private set; }

        /// <summary>
        /// A method to update the AddToDeleteFrom's AutoCompleteBox with suggestions remotely loaded.
        /// </summary>
        public Action<AutoCompleteBox> ManuallyUpdateSuggestions { get; private set; }

        #endregion

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="LocationsVM"/> class.
        /// </summary>
        [ImportingConstructor]
        public LocationsVM()
            //WORKAROUND TO BE FIXED: Prevent null selection because unsaved entities will not show up
            : base(new[] { typeof(Client), typeof(Region) }, true, true)
        {
            SetupDataLoading();

            #region Setup location/sublocation properties

            //Hookup _selectedLocationVM to SelectedLocationVMObservable
            _selectedLocationVM = SelectedLocationVMObservable.ToProperty(this, x => x.SelectedLocationVM);

            //Hookup _selectedSubLocationsVM to SelectedSubLocationsVMObservable
            _selectedSubLocationsVM =
                SelectedSubLocationsVMObservable.ToProperty(this, x => x.SelectedSubLocationsVM, new SubLocationsVM(SelectedEntity));

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
                _selectedLocationVMObservable.OnNext(new LocationVM(selectedLocation));

                //Create a new SubLocationsVM
                _selectedSubLocationsVMObservable.OnNext(new SubLocationsVM(selectedLocation));
            });

            #endregion

            #region IAddToDeleteFromSource<Location> Implementation

            MemberPath = "Name";

            CreateNewItem = name =>
            {
                //Set the Party to the current OwnerAccount
                var newLocation = new Location { BusinessAccount = ContextManager.ServiceProvider };

                //Add the entity to the EntitySet so it is tracked by the DomainContext
                DomainContext.Locations.Add(newLocation);

                var client = ContextManager.GetContext<Client>();

                //If the client context != null add this to the current Client's Locations
                if (client != null)
                {
                    client.Locations.Add(newLocation);
                }

                var region = ContextManager.GetContext<Region>();

                //If the region context != null add this to the current Regions's Locations
                if (region != null)
                    region.Locations.Add(newLocation);

                newLocation.RaiseValidationErrors();

                DataManager.EnqueueSubmitOperation(submitOperation =>
                    Rxx3.RunDelayed(TimeSpan.FromSeconds(1), () => SelectedEntity = newLocation));

                return newLocation;
            };

            ManuallyUpdateSuggestions =
                autoCompleteBox => SearchSuggestionsHelper(autoCompleteBox,
                () =>
                {
                    var query = Manager.Data.DomainContext.SearchLocationsForRoleQuery(Manager.Context.RoleId, autoCompleteBox.SearchText);
                    //If the search is happening in a recurring service context, filter by the recurring service's client
                    var recurringServiceContext = ContextManager.GetContext<RecurringService>();
                    if (recurringServiceContext != null && recurringServiceContext.ClientId != Guid.Empty)
                        query = query.Where(l => l.ClientId == recurringServiceContext.ClientId);
                    return query;
                });

            #endregion
        }

        /// <summary>
        /// Used in the constructor to setup data loading.
        /// </summary>
        private void SetupDataLoading()
        {
            var contextRelationshipFilters = new[] { new ContextRelationshipFilter("ClientId", typeof (Client), v => ((Client) v).Id), 
                                                     new ContextRelationshipFilter("RegionId", typeof (Region), v => ((Region) v).Id) };

            //Force load the entities when in a related types view
            //this is because VDCV will only normally load when a virtual item is loaded onto the screen
            //virtual items will not always load because in clients context the gridview does not always show (sometimes it is in single view)
            SetupContextDataLoading(roleId => DomainContext.GetLocationsToAdministerForRoleQuery(roleId), contextRelationshipFilters, ContextLoadingType.NoRequiredContext);

            //Whenever the location changes load the location details
            SetupDetailsLoading(selectedEntity => DomainContext.GetLocationDetailsForRoleQuery(ContextManager.RoleId, selectedEntity.Id));
        }

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
            DomainContext.GetLocationsCSVForRole(ContextManager.RoleId, clientContext != null ? clientContext.Id : new Guid(), regionContext != null ? regionContext.Id : new Guid(),
                loadedCSV => csvLoadedObservable.OnNext(loadedCSV.Value), null);

            var fileName = String.Format("LocationsExport {0}.csv", Manager.Context.UserAccount.AdjustTimeForUserTimeZone(DateTime.UtcNow).ToString("MM'-'dd'-'yyyy"));
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
            var newLocation = CreateNewItem("New Location");
            this.QueryableCollectionView.AddNew(newLocation);

            return newLocation;
        }

        #endregion
    }
}

using FoundOps.Common.Silverlight.UI.Controls.AddEditDelete;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.SLClient.Data.Services;
using FoundOps.SLClient.Data.ViewModels;
using MEFedMVVM.ViewModelLocator;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Controls;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// Contains the logic for displaying Locations
    /// </summary>
    [ExportViewModel("LocationsVM")]
    public class LocationsVM : InfiniteAccordionVM<Location>, IAddToDeleteFromSource<Location>
    {
        #region Public Properties

        #region Entity Data Items

        /// <summary>
        /// The search text.
        /// </summary>
        public string SearchText { get; set; }

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

        public string MemberPath { get; private set; }

        /// <summary>
        /// A method to update the ExistingItemsSource with suggestions remotely loaded.
        /// </summary>
        public Action<string, AutoCompleteBox> ManuallyUpdateSuggestions { get; private set; }

        #endregion

        #endregion

        #region Locals

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="LocationsVM"/> class.
        /// </summary>
        [ImportingConstructor]
        public LocationsVM()
            : base(new[] { typeof(Client), typeof(Region) })
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

                //Add the entity to the EntitySet so it is tracked by the DomainContext
                Context.Locations.Add(newLocation);

                return newLocation;
            };

            ManuallyUpdateSuggestions = (searchText, autoCompleteBox) => 
                SearchSuggestionsHelper(autoCompleteBox, () => Manager.Data.Context.SearchLocationsForRoleQuery(Manager.Context.RoleId, searchText));

            #endregion
        }

        /// <summary>
        /// Used in the constructor to setup data loading.
        /// </summary>
        private void SetupDataLoading()
        {
            //Force load the entities when in a related types view
            //this is because VDCV will only normally load when a virtual item is loaded onto the screen
            //virtual items will not always load because in clients context the gridview does not always show (sometimes it is in single view)
            SetupContextDataLoading(roleId => Context.GetLocationsToAdministerForRoleQuery(roleId),
                                    new[]
                                        {
                                            new ContextRelationshipFilter
                                                {
                                                    EntityMember = "PartyId",
                                                    FilterValueGenerator = v => ((Client) v).Id,
                                                    RelatedContextType = typeof (Client)
                                                },
                                                  new ContextRelationshipFilter
                                                {
                                                    EntityMember = "RegionId",
                                                    FilterValueGenerator = v => ((Region) v).Id,
                                                    RelatedContextType = typeof (Region)
                                                }
                                        }, true);


            //Whenever the location changes load the location details
            SetupDetailsLoading(selectedEntity => Context.GetLocationDetailsForRoleQuery(ContextManager.RoleId, selectedEntity.Id));
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
            var newLocation = CreateNewItem("");
            this.QueryableCollectionView.AddNew(newLocation);
            return newLocation;
        }

        #endregion
    }
}

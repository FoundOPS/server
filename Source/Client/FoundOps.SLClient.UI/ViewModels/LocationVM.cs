using System;
using ReactiveUI; 
using System.Linq; 
using FoundOps.Common.NET;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using GalaSoft.MvvmLight.Command;
using MEFedMVVM.ViewModelLocator;
using System.Collections.Generic; 
using System.Collections.ObjectModel;
using FoundOps.SLClient.Data.Services;
using System.ComponentModel.Composition;
using FoundOps.SLClient.Data.ViewModels;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Context.Services.Interface;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// Contains the logic for searching for and displaying a Location
    /// </summary>
    [ExportViewModel("LocationVM")]
    public class LocationVM : CoreEntityVM
    {
        #region Public Properties

        /// <summary>
        /// Gets the ContactInfoVM for the Location
        /// </summary>
        public ContactInfoVM ContactInfoVM { get; private set; }

        private readonly Location _entity;
        /// <summary>
        /// Gets the location entity this viewmodel is for.
        /// </summary>
        /// <value>
        /// The location entity.
        /// </value>
        public Location Entity { get { return _entity; } }

        #region Geocoder Results

        private IEnumerable<GeocoderResult> _geocoderResults = new ObservableCollection<GeocoderResult>();
        /// <summary>
        /// Gets or sets the geocoder results.
        /// </summary>
        /// <value>
        /// The geocoder results of a search.
        /// </value>
        public IEnumerable<GeocoderResult> GeocoderResults
        {
            get
            {
                //Include ManuallySelectGeocoderResult
                return new List<GeocoderResult>(_geocoderResults) { ManuallySelectGeocoderResult };
            }
            set
            {
                _geocoderResults = value;
                RaisePropertyChanged("GeocoderResults");
                RaisePropertyChanged("GeocoderResultsWithLocations");
            }
        }

        /// <summary>
        /// The manually selected geocoder result.
        /// </summary>
        public GeocoderResult ManuallySelectGeocoderResult { get; set; }

        /// <summary>
        /// Gets the geocoder results that have locations.
        /// </summary>
        public IEnumerable<GeocoderResult> GeocoderResultsWithLocations { get { return GeocoderResults.Where(gr => gr.TelerikLocation != null); } }

        private GeocoderResult _selectedGeocoderResult;
        /// <summary>
        /// Gets or sets the user selected geocoder result.
        /// </summary>
        /// <value>
        /// The user selected geocoder result.
        /// </value>
        public GeocoderResult SelectedGeocoderResult
        {
            get { return _selectedGeocoderResult; }
            set
            {
                _selectedGeocoderResult = value;
                RaisePropertyChanged("SelectedGeocoderResult");
                SetLocationToGeocoderResultCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion

        private string _searchText;
        /// <summary>
        /// Gets or sets the user's search text.
        /// </summary>
        /// <value>
        /// The user's search text.
        /// </value>
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                RaisePropertyChanged("SearchText");
            }
        }

        #region Commands

        /// <summary>
        /// A command for manually setting the Location's latitude and longitude.
        /// </summary>
        public RelayCommand<Tuple<decimal, decimal>> ManuallySetLatitudeLongitude { get; private set; }
        
        /// <summary>
        /// A command to Search for an address.
        /// </summary>
        public RelayCommand SearchCommand { get; private set; }
        
        /// <summary>
        /// A command for setting the Location's information based off a GeocoderResult.
        /// </summary>
        public RelayCommand SetLocationToGeocoderResultCommand { get; private set; }

        #endregion

        #region Observables

        private readonly BehaviorSubject<IEnumerable<GeocoderResult>> _geocodeCompletion = new BehaviorSubject<IEnumerable<GeocoderResult>>(null);
        /// <summary>
        /// An observable that publishes whenever Geocoding has completed.
        /// </summary>
        public IObservable<IEnumerable<GeocoderResult>> GeocodeCompletion { get { return _geocodeCompletion; } }

        /// <summary>
        /// Gets the latitude longitude change.
        /// </summary>
        public IObservable<bool> ValidLatitudeLongitudeState { get; private set; }

        #endregion

        #endregion

        #region Private Properties

        private const string ClickOnTheMapString = "(click on the map)";
        private readonly ILocationsDataService _locationsDataService;
        private const string ManuallySelectLocationString = "Manually select the location";

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="LocationVM"/> class.
        /// </summary>
        /// <param name="locationsDataService">The locations data service.</param>
        /// <param name="entity">The entity this viewmodel is for</param>
        /// <param name="dataManager">The data manager.</param>
        [ImportingConstructor]
        public LocationVM(Location entity, DataManager dataManager, ILocationsDataService locationsDataService)
            : base(dataManager)
        {
            this._locationsDataService = locationsDataService;

            #region Register Commands

            SearchCommand = new RelayCommand(OnSearch);
            SetLocationToGeocoderResultCommand = new RelayCommand(OnSetLocation,
                                                                  () =>
                                                                  SelectedGeocoderResult != null &&
                                                                  !String.IsNullOrEmpty(SelectedGeocoderResult.Latitude) &&
                                                                  !String.IsNullOrEmpty(SelectedGeocoderResult.Longitude));
            ManuallySetLatitudeLongitude = new RelayCommand<Tuple<decimal, decimal>>(OnManuallySetLatitudeLongitude,
                                                                                     (latitudeLongitude) =>
                                                                                     SelectedGeocoderResult != null &&
                                                                                     SelectedGeocoderResult.Name ==
                                                                                     ManuallySelectLocationString);

            #endregion

            #region Entity logic

            _entity = entity;

            //Setup the ContactInfoVM and update the Geocoder properties
            ContactInfoVM = new ContactInfoVM(DataManager, ContactInfoType.Locations,
                                              entity.ContactInfoSet);

            //Set ManuallySelectGeocoderResult to the Entity if it has a Latitude/Longitude

            ManuallySelectGeocoderResult = new GeocoderResult { Name = ManuallySelectLocationString, AddressLineOne = ClickOnTheMapString };

            if (entity.Latitude != null && entity.Longitude != null)
            {
                ManuallySelectGeocoderResult.Latitude = entity.Latitude.Value.ToString();
                ManuallySelectGeocoderResult.Longitude = entity.Longitude.Value.ToString();
                SelectedGeocoderResult = ManuallySelectGeocoderResult;
                if (SelectedGeocoderResult.TelerikLocation != null)
                    MessageBus.Current.SendMessage(new LocationSetMessage(SelectedGeocoderResult.TelerikLocation.Value));
            }

            #endregion

            //Set the default SearchText
            SearchText = string.Format("{0}, {1}, {2}, {3}", entity.AddressLineOne, entity.City, entity.State, entity.ZipCode);



            #region Subscribe to Location state validation
            //Setup ValidLatitudeLongitude observable, it will be valid as long as the Entity's lat & long have a value
            //Start with a value
            ValidLatitudeLongitudeState = new BehaviorSubject<bool>(Entity.Latitude.HasValue && Entity.Longitude.HasValue);
            // Subscribe to changes
            Observable2.FromPropertyChangedPattern(Entity, x => x.Latitude).CombineLatest(
                Observable2.FromPropertyChangedPattern(Entity, x => x.Longitude),(a, b) => a.HasValue && b.HasValue).Throttle(new TimeSpan(0, 0, 0, 0, 250))
                .Subscribe((BehaviorSubject<bool>) ValidLatitudeLongitudeState);

            #endregion

            // Subscribe to Geocoding completion, aka Search completion.
            Observable2.FromPropertyChangedPattern(this, x => x.GeocoderResults).Subscribe(GeocodeCompletion as BehaviorSubject<IEnumerable<GeocoderResult>>);
        }

        #region Logic


        private void OnManuallySetLatitudeLongitude(Tuple<decimal, decimal> latitudeLongitude)
        {
            ManuallySelectGeocoderResult.Latitude = latitudeLongitude.Item1.ToString();
            ManuallySelectGeocoderResult.Longitude = latitudeLongitude.Item2.ToString();
            SetLocationToGeocoderResultCommand.RaiseCanExecuteChanged();
            RaisePropertyChanged("GeocoderResultsWithLocations");
        }

        private void OnSearch()
        {
            _locationsDataService.TryGeocode(SearchText, results =>
                                                             {
                                                                 GeocoderResults = new ObservableCollection<GeocoderResult>(results);
                                                                 //TODO: Signal end of search event
                                                                 GeocodeCompletion.Next();
                                                             });

            ManuallySelectGeocoderResult.Latitude = null;
            ManuallySelectGeocoderResult.Longitude = null;
        }

        private void OnSetLocation()
        {
            if (Entity == null) return;

            Entity.Latitude = Convert.ToDecimal(SelectedGeocoderResult.Latitude);
            Entity.Longitude = Convert.ToDecimal(SelectedGeocoderResult.Longitude);

            if (SelectedGeocoderResult.Name != ManuallySelectLocationString)
            {
                if (!String.IsNullOrEmpty(SelectedGeocoderResult.Name))
                    Entity.Name = SelectedGeocoderResult.Name;

                if (!String.IsNullOrEmpty(SelectedGeocoderResult.AddressLineOne))
                    Entity.AddressLineOne = SelectedGeocoderResult.AddressLineOne;

                if (!String.IsNullOrEmpty(SelectedGeocoderResult.City))
                    Entity.City = SelectedGeocoderResult.City;

                if (!String.IsNullOrEmpty(SelectedGeocoderResult.State))
                    Entity.State = SelectedGeocoderResult.State;

                if (!String.IsNullOrEmpty(SelectedGeocoderResult.ZipCode))
                    Entity.ZipCode = SelectedGeocoderResult.ZipCode;
            }
            MessageBus.Current.SendMessage(new LocationSetMessage(SelectedGeocoderResult.TelerikLocation.Value));
        }

        #endregion
    }

    /// <summary>
    /// A message for force setting the Location
    /// </summary>
    public class LocationSetMessage
    {
        /// <summary>
        /// Gets the location to set.
        /// </summary>
        public Telerik.Windows.Controls.Map.Location SetLocation { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocationSetMessage"/> class.
        /// </summary>
        /// <param name="location">The location to force set.</param>
        public LocationSetMessage(Telerik.Windows.Controls.Map.Location location)
        {
            SetLocation = location;
        }
    }
}

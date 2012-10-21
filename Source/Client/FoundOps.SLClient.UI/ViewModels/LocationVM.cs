using System.Windows;
using FoundOps.Common.NET;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Server.Services.CoreDomainService;
using FoundOps.SLClient.Data.ViewModels;
using GalaSoft.MvvmLight.Command;
using MEFedMVVM.ViewModelLocator;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;

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

        private readonly Subject<IEnumerable<GeocoderResult>> _geocodeCompletion = new Subject<IEnumerable<GeocoderResult>>();
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
        private const string ManuallySelectLocationString = "Manually select the location";

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="LocationVM"/> class.
        /// </summary>
        /// <param name="entity">The entity this viewmodel is for</param>
        [ImportingConstructor]
        public LocationVM(Location entity)
        {
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

            //Setup the ContactInfoVM
            ContactInfoVM = new ContactInfoVM(ContactInfoType.Locations, entity != null ? entity.ContactInfoSet : null);

            ManuallySelectGeocoderResult = new GeocoderResult { Name = ManuallySelectLocationString, AddressLineOne = ClickOnTheMapString };

            var depot = ContextManager.ServiceProvider.Depots.FirstOrDefault();
            var ipInformation = ((string)Application.Current.Resources["IPInformation"]);

            //Set ManuallySelectGeocoderResult to the Entity if it has a Latitude/Longitude
            if (entity != null && entity.Latitude != null && entity.Longitude != null)
            {
                ManuallySelectGeocoderResult.Latitude = entity.Latitude.Value.ToString();
                ManuallySelectGeocoderResult.Longitude = entity.Longitude.Value.ToString();
                SelectedGeocoderResult = ManuallySelectGeocoderResult;
                if (SelectedGeocoderResult.TelerikLocation != null)
                    MessageBus.Current.SendMessage(new LocationSetMessage(SelectedGeocoderResult.TelerikLocation.Value));
            }
            //Otherwise set it to the depot if there is one
            else if (depot != null && depot.Latitude.HasValue && depot.Longitude.HasValue)
            {
                ManuallySelectGeocoderResult.Latitude = depot.Latitude.Value.ToString();
                ManuallySelectGeocoderResult.Longitude = depot.Longitude.Value.ToString();
            }
            //Otherwise use the IPInfo if there is any
            else if (ipInformation != null)
            {
                var ipInformationDelimited = ipInformation.Split(';');
                var latitude = ipInformationDelimited[8];
                var longitude = ipInformationDelimited[9];

                ManuallySelectGeocoderResult.Latitude = latitude;
                ManuallySelectGeocoderResult.Longitude = longitude;
            }

            #endregion

            if (entity == null)
                return;

            //Set the default SearchText
            SearchText = string.Format("{0}, {1}, {2}, {3}", entity.AddressLineOne, entity.AdminDistrictTwo, entity.AdminDistrictOne, entity.PostalCode);

            #region Subscribe to Location state validation
            //Setup ValidLatitudeLongitude observable, it will be valid as long as the Entity's lat & long have a value
            //Start with a value
            ValidLatitudeLongitudeState = new BehaviorSubject<bool>(Entity.Latitude.HasValue && Entity.Longitude.HasValue);
            // Subscribe to changes
            Observable2.FromPropertyChangedPattern(Entity, x => x.Latitude).CombineLatest(
                Observable2.FromPropertyChangedPattern(Entity, x => x.Longitude), (a, b) => a.HasValue && b.HasValue).Throttle(new TimeSpan(0, 0, 0, 0, 250))
                .Subscribe((BehaviorSubject<bool>)ValidLatitudeLongitudeState);

            #endregion
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
            DataManager.TryGeocode(SearchText, (results, us) =>
            {
                GeocoderResults = new ObservableCollection<GeocoderResult>(results);
                //Signal end of search event
                _geocodeCompletion.OnNext(GeocoderResults);
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
                if (!String.IsNullOrEmpty(SelectedGeocoderResult.AddressLineOne))
                    Entity.AddressLineOne = SelectedGeocoderResult.AddressLineOne;

                if (!String.IsNullOrEmpty(SelectedGeocoderResult.City))
                    Entity.AdminDistrictTwo = SelectedGeocoderResult.City;

                if (!String.IsNullOrEmpty(SelectedGeocoderResult.State))
                    Entity.AdminDistrictOne = SelectedGeocoderResult.State;

                if (!String.IsNullOrEmpty(SelectedGeocoderResult.ZipCode))
                    Entity.PostalCode = SelectedGeocoderResult.ZipCode;
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

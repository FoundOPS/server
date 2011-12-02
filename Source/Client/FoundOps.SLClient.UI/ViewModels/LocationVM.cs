using System;
using System.Linq;
using System.ComponentModel;
using FoundOps.Common.NET;
using GalaSoft.MvvmLight.Command;
using MEFedMVVM.ViewModelLocator;
using System.Collections.Generic;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.ObjectModel;
using FoundOps.SLClient.Data.Services;
using System.ComponentModel.Composition;
using FoundOps.SLClient.Data.ViewModels;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Context.Services.Interface;
using ReactiveUI;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// Contains the logic for searching for and displaying a Location
    /// </summary>
    [ExportViewModel("LocationVM")]
    public class LocationVM : CoreEntityVM, IAddDeleteSelectedService
    {
        #region Public Properties

        private readonly Location _selectedLocation;
        /// <summary>
        /// Gets the selected location this viewmodel is for.
        /// </summary>
        /// <value>
        /// The selected location.
        /// </value>
        public Location SelectedLocation { get { return _selectedLocation; } }

        /// <summary>
        /// Gets the ContactInfoVM for the Location
        /// </summary>
        public ContactInfoVM ContactInfoVM { get; private set; }

        // IAddDeleteSelectedService
        public RelayCommand<Service> AddSelectedServiceCommand { get; set; } //Not intended to be used
        public RelayCommand<Service> DeleteSelectedServiceCommand { get; set; }

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
        /// Gets the geocoder results that have locations.
        /// </summary>
        public IEnumerable<GeocoderResult> GeocoderResultsWithLocations
        {
            get
            {
                return GeocoderResults.Where(gr => gr.TelerikLocation != null);
            }
        }

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

        private GeocoderResult _manuallySelectGeocoderResult = new GeocoderResult { Name = ManuallySelectLocationString, AddressLineOne = ClickOnTheMapString };
        /// <summary>
        /// Gets or sets the manually selected geocoder result.
        /// </summary>
        /// <value>
        /// The manually selected geocoder result.
        /// </value>
        public GeocoderResult ManuallySelectGeocoderResult
        {
            get { return _manuallySelectGeocoderResult; }
            set
            {
                if (ManuallySelectGeocoderResult != null)
                    ManuallySelectGeocoderResult.PropertyChanged -= ManuallySelectGeocoderResultPropertyChanged;
                _manuallySelectGeocoderResult = value;
                if (ManuallySelectGeocoderResult != null)
                    ManuallySelectGeocoderResult.PropertyChanged += ManuallySelectGeocoderResultPropertyChanged;
            }
        }
        void ManuallySelectGeocoderResultPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "TelerikLocation")
                RaisePropertyChanged("GeocoderResultsWithLocations");
        }

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

        /// <summary>
        /// A command to Search for an address.
        /// </summary>
        public RelayCommand SearchCommand { get; private set; }
        /// <summary>
        /// A command for setting the Location's information based off a GeocoderResult.
        /// </summary>
        public RelayCommand SetLocationToGeocoderResultCommand { get; private set; }
        /// <summary>
        /// A command for manually setting the Location's latitude and longitude.
        /// </summary>
        public RelayCommand<Tuple<decimal, decimal>> ManuallySetLatitudeLongitude { get; private set; }

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
        /// <param name="selectedLocation">The selectedLocation this viewmodel is for</param>
        /// <param name="dataManager">The data manager.</param>
        [ImportingConstructor]
        public LocationVM(Location selectedLocation, DataManager dataManager, ILocationsDataService locationsDataService)
            : base(dataManager)
        {
            this._locationsDataService = locationsDataService;

            #region Register Commands

            DeleteSelectedServiceCommand = new RelayCommand<Service>(OnDeleteSelectedServiceCommand);
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

            #region SelectedLocation logic

            _selectedLocation = selectedLocation;

            if (_selectedLocation == null)
            {
                MessageBus.Current.SendMessage(new ResetMapMessage());
            }
            else
            {
                //Setup the ContactInfoVM and update the Geocoder properties
                ContactInfoVM = new ContactInfoVM(DataManager, ContactInfoType.Locations,
                                                  selectedLocation.ContactInfoSet);

                //Set ManuallySelectGeocoderResult to the SelectedLocation if it has a Latitude/Longitude
                if (selectedLocation.Latitude != null && selectedLocation.Longitude != null)
                {
                    ManuallySelectGeocoderResult.Latitude = selectedLocation.Latitude.Value.ToString();
                    ManuallySelectGeocoderResult.Longitude = selectedLocation.Longitude.Value.ToString();
                    SelectedGeocoderResult = ManuallySelectGeocoderResult;
                    if (SelectedGeocoderResult.TelerikLocation != null)
                        MessageBus.Current.SendMessage(new LocationSetMessage(SelectedGeocoderResult.TelerikLocation.Value));
                }
            }

            #endregion
        }

        #region Logic

        private void OnDeleteSelectedServiceCommand(Service service)
        {
            //TODO: Set this up
            //var locationValue = this.SelectedLocation.LocationValues.FirstOrDefault(locValue => locValue.ServiceTemplateId == service.ServiceTemplate.Id);
            //if (locationValue != null)
            //{
            //    RaisePropertyChanged("SelectedLocation"); //This updates CurrentContext on LocationsVM
            //}
        }

        private void OnGeocodingComplete(IEnumerable<GeocoderResult> geocoderResults)
        {
            GeocoderResults = new ObservableCollection<GeocoderResult>(geocoderResults);
        }

        private void OnManuallySetLatitudeLongitude(Tuple<decimal, decimal> latitudeLongitude)
        {
            ManuallySelectGeocoderResult.Latitude = latitudeLongitude.Item1.ToString();
            ManuallySelectGeocoderResult.Longitude = latitudeLongitude.Item2.ToString();
            SetLocationToGeocoderResultCommand.RaiseCanExecuteChanged();
            RaisePropertyChanged("GeocoderResultsWithLocations");
        }

        private void OnSearch()
        {
            _locationsDataService.TryGeocode(SearchText, OnGeocodingComplete);
            ManuallySelectGeocoderResult.Latitude = null;
            ManuallySelectGeocoderResult.Longitude = null;
        }

        private void OnSetLocation()
        {
            if (SelectedLocation == null) return;

            SelectedLocation.Latitude = Convert.ToDecimal(SelectedGeocoderResult.Latitude);
            SelectedLocation.Longitude = Convert.ToDecimal(SelectedGeocoderResult.Longitude);

            if (SelectedGeocoderResult.Name != ManuallySelectLocationString)
            {
                if (!String.IsNullOrEmpty(SelectedGeocoderResult.Name))
                    SelectedLocation.Name = SelectedGeocoderResult.Name;

                if (!String.IsNullOrEmpty(SelectedGeocoderResult.AddressLineOne))
                    SelectedLocation.AddressLineOne = SelectedGeocoderResult.AddressLineOne;

                if (!String.IsNullOrEmpty(SelectedGeocoderResult.City))
                    SelectedLocation.City = SelectedGeocoderResult.City;

                if (!String.IsNullOrEmpty(SelectedGeocoderResult.State))
                    SelectedLocation.State = SelectedGeocoderResult.State;

                if (!String.IsNullOrEmpty(SelectedGeocoderResult.ZipCode))
                    SelectedLocation.ZipCode = SelectedGeocoderResult.ZipCode;
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

    /// <summary>
    /// A message for Resetting the map
    /// </summary>
    public class ResetMapMessage { }
}

using FoundOps.Common.NET;
using FoundOps.Common.Silverlight.Services;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.SLClient.Data.ViewModels;
using GalaSoft.MvvmLight.Command;
using MEFedMVVM.ViewModelLocator;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;

namespace FoundOps.SLClient.UI.ViewModels
{
    //TODO: Figure a way to merge with LocationVM
    /// <summary>
    /// A view model for all of the SubLocations
    /// </summary>
    [ExportViewModel("SubLocationsVM")]
    public class SubLocationsVM : CoreEntityCollectionVM<SubLocation>
    {
        #region Public Properties

        private readonly Location _selectedLocation;

        public RelayCommand SearchCommand { get; private set; }
        public RelayCommand SetSubLocationToGeocoderResultCommand { get; private set; }
        public RelayCommand<Tuple<decimal, decimal>> ManuallySetLatitudeLongitude { get; private set; }

        private IEnumerable<GeocoderResult> _geocoderResults;
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

        public IEnumerable<GeocoderResult> GeocoderResultsWithSubLocations
        {
            get
            {

                return GeocoderResults.Where(gr => gr.TelerikLocation != null);
            }
        }

        private GeocoderResult _selectedGeocoderResult;
        public GeocoderResult SelectedGeocoderResult
        {
            get { return _selectedGeocoderResult; }
            set
            {
                _selectedGeocoderResult = value;
                RaisePropertyChanged("SelectedGeocoderResult");
                SetSubLocationToGeocoderResultCommand.RaiseCanExecuteChanged();
            }
        }

        private GeocoderResult _manuallySelectGeocoderResult;
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
                RaisePropertyChanged("GeocoderResultsWithSubLocations");
        }

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                RaisePropertyChanged("SearchText");
            }
        }

        #endregion

        #region Private Properties

        private const string ManuallySelectLocationString = "Manually select the location";
        private const string ClickOnTheMapString = "(click on the map)";

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="SubLocationsVM"/> class.
        /// </summary>
        /// <param name="selectedLocation">The selected location.</param>
        [ImportingConstructor]
        public SubLocationsVM(Location selectedLocation) : base(false)
        {
            GeocoderResults = new ObservableCollection<GeocoderResult>();
            ManuallySelectGeocoderResult = new GeocoderResult
                {
                    Name = ManuallySelectLocationString,
                    AddressLineOne = ClickOnTheMapString
                };
            _selectedLocation = selectedLocation;

            if (_selectedLocation != null)
            {
                ViewObservable.OnNext(new DomainCollectionViewFactory<SubLocation>(_selectedLocation.SubLocationsListWrapper).View);
                CollectionView.SortDescriptions.Add(new SortDescription("Number", ListSortDirection.Ascending));
            }

            #region Register Commands

            SearchCommand = new RelayCommand(OnSearch);
            SetSubLocationToGeocoderResultCommand = new RelayCommand(OnSetSubLocation,
                                                                  () =>
                                                                  SelectedGeocoderResult != null &&
                                                                  !String.IsNullOrEmpty(SelectedGeocoderResult.Latitude) &&
                                                                  !String.IsNullOrEmpty(SelectedGeocoderResult.Longitude));
            ManuallySetLatitudeLongitude = new RelayCommand<Tuple<decimal, decimal>>(OnManuallySetLatitudeLongitude, latitudeLongitude => true);

            #endregion
        }

        #region Logic

        //Override adding a new sublocation to use SubLocationsListWrapper
        protected override SubLocation AddNewEntity(object commandParameter)
        {
            var newSubLocation = new SubLocation();
            this._selectedLocation.SubLocationsListWrapper.Add(newSubLocation);
            return newSubLocation;
        }

        //Override deleting a new sublocation so it is actually deleted 
        //instead of just being removed from the _selectedLocation.SubLocationsListWrapper
        public override void DeleteEntity(SubLocation entityToDelete)
        {
            this.DomainContext.SubLocations.Remove(entityToDelete);
        }

        protected override void OnAddEntity(SubLocation newSubLocation)
        {
            RaisePropertyChanged("DomainCollectionView");
        }

        protected override void OnDeleteEntity(SubLocation entity)
        {
            RaisePropertyChanged("DomainCollectionView");
        }

        private void OnGeocodingComplete(IEnumerable<GeocoderResult> geocoderResults, object userState)
        {
            GeocoderResults = new ObservableCollection<GeocoderResult>(geocoderResults);
        }

        private void OnManuallySetLatitudeLongitude(Tuple<decimal, decimal> latitudeLongitude)
        {
            if (SelectedEntity == null) return;

            SelectedEntity.Latitude = Convert.ToDecimal(latitudeLongitude.Item1);
            SelectedEntity.Longitude = Convert.ToDecimal(latitudeLongitude.Item2);
            SetSubLocationToGeocoderResultCommand.RaiseCanExecuteChanged();
            RaisePropertyChanged("DomainCollectionView");
            RaisePropertyChanged("GeocoderResultsWithSubLocations");

            if (SelectedEntity.TelerikLocation != null)
                MessageBus.Current.SendMessage(new SubLocationSetMessage(SelectedEntity.TelerikLocation.Value));
        }

        private void OnSearch()
        {
            DataManager.TryGeocode(SearchText, OnGeocodingComplete);
            ManuallySelectGeocoderResult.Latitude = null;
            ManuallySelectGeocoderResult.Longitude = null;
        }

        private void OnSetSubLocation()
        {
            if (SelectedEntity == null) return;

            SelectedEntity.Latitude = Convert.ToDecimal(SelectedGeocoderResult.Latitude);
            SelectedEntity.Longitude = Convert.ToDecimal(SelectedGeocoderResult.Longitude);

            if (SelectedGeocoderResult.Name != ManuallySelectLocationString)
            {
                if (!String.IsNullOrEmpty(SelectedGeocoderResult.Name))
                    SelectedEntity.Name = SelectedGeocoderResult.Name;
            }
            if (SelectedGeocoderResult.TelerikLocation != null)
                MessageBus.Current.SendMessage(new LocationSetMessage(SelectedGeocoderResult.TelerikLocation.Value));
        }

        protected override void OnSelectedEntityChanged(SubLocation oldValue, SubLocation newValue)
        {
            if (newValue == null) return;

            if (newValue.Latitude != null && newValue.Longitude != null)
            {
                if (newValue.TelerikLocation != null)
                    MessageBus.Current.SendMessage(new SubLocationSetMessage(newValue.TelerikLocation.Value));
            }
        }

        #endregion

        public class SubLocationSetMessage
        {
            public Telerik.Windows.Controls.Map.Location SetSubLocation { get; private set; }

            public SubLocationSetMessage(Telerik.Windows.Controls.Map.Location subLocation)
            {
                SetSubLocation = subLocation;
            }
        }

        public class ResetMapMessage { }
    }
}
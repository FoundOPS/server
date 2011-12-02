using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows;
using FoundOps.Core.Context.Services;
using FoundOps.SLClient.Data.Services;
using FoundOps.Core.Models.CoreEntities;
using GalaSoft.MvvmLight.Command;
using MEFedMVVM.ViewModelLocator;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// Contains the logic for displaying Busineses
    /// </summary>
    [ExportViewModel("BusinessAccountVM")]
    public class BusinessVM : PartyVM, IAddDeleteSelectedLocation, IAddDeleteSelectedService
    {
        #region Public Properties

        private Business _selectedBusiness;
        /// <summary>
        /// Gets or sets the selected business.
        /// </summary>
        /// <value>
        /// The selected business.
        /// </value>
        public Business SelectedBusiness
        {
            get
            {
                return _selectedBusiness;
            }
            set
            {
                if (SelectedBusiness == value) return;

                _selectedBusiness = value;
                this.SelectedParty = value;
                RaisePropertyChanged("SelectedBusiness");
                RaisePropertyChanged("RootIndustries"); //Triger view update
            }
        }

        #region Implementation of IAddDeleteSelectedLocation

        public RelayCommand<Location> AddSelectedLocationCommand { get; set; }
        public RelayCommand<Location> DeleteSelectedLocationCommand { get; set; }

        #endregion

        #region Implementation of IAddDeleteSelectedService

        public RelayCommand<Service> AddSelectedServiceCommand { get; set; } //Not intended to be used
        public RelayCommand<Service> DeleteSelectedServiceCommand { get; set; }

        #endregion

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessVM"/> class.
        /// </summary>
        /// <param name="dataManager">The data manager.</param>
        /// <param name="partyDataService">The party data service.</param>
        [ImportingConstructor]
        public BusinessVM(DataManager dataManager, IPartyDataService partyDataService)
            : base(dataManager)
        {
            this.PropertyChanged += (sender, args) =>
                                        {
                                            if (args.PropertyName != "SelectedParty" ||
                                                _selectedBusiness == SelectedParty) return;
                                            SelectedBusiness = (Business) SelectedParty;
                                        };

            #region Register Commands

            AddSelectedLocationCommand = new RelayCommand<Location>(OnAddSelectedLocation);
            DeleteSelectedLocationCommand = new RelayCommand<Location>(OnDeleteSelectedLocation);

            DeleteSelectedServiceCommand = new RelayCommand<Service>(OnDeleteSelectedService);

            #endregion

            if (DesignerProperties.IsInDesignTool) //For Design Mode
            {
                partyDataService.PartyType = typeof (BusinessAccount);
                partyDataService.GetPartyToAdministerForRole(new Guid(), account => SelectedBusiness = (BusinessAccount) account);
            }
        }

        #region Logic

        private void OnAddSelectedLocation(Location locationToAdd)
        {
            if (locationToAdd == null)
                MessageBox.Show("Please select a Location to add");
            else
            {
                this.SelectedBusiness.Locations.Add(locationToAdd);
                RaisePropertyChanged("SelectedBusiness"); //This updates CurrentContext on ClientsVM
            }
        }

        private void OnDeleteSelectedLocation(Location locationToRemove)
        {
            this.SelectedBusiness.Locations.Remove(locationToRemove);
            RaisePropertyChanged("SelectedBusiness"); //This updates CurrentContext on ClientsVM
        }

        private void OnDeleteSelectedService(Service service)
        {
            //this.SelectedBusinessAccount.OwnedLocations.Remove(service);
            //RaisePropertyChanged("SelectedBusiness"); //This updates CurrentContext on ClientsVM
        }

        #endregion
    }
}

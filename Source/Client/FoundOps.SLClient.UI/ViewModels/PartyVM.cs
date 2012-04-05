using System.Windows;
using GalaSoft.MvvmLight.Command;
using FoundOps.SLClient.Data.Services;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.SLClient.Data.ViewModels;
using FoundOps.Server.Services.CoreDomainService;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// Contains the logic for displaying Party
    /// </summary>
    public class PartyVM : CoreEntityVM, IAddDeleteSelectedLocation
    {
        #region Public Properties

        private readonly Party _entity;
        /// <summary>
        /// Gets the Party entity this viewmodel is for.
        /// </summary>
        public Party Entity { get { return _entity; } }

        /// <summary>
        /// Gets the ContactInfoVM for the Party
        /// </summary>
        public ContactInfoVM ContactInfoVM { get; private set; }

        #region Implementation of IAddDeleteSelectedLocation

        public RelayCommand<Location> AddSelectedLocationCommand { get; set; }
        public RelayCommand<Location> DeleteSelectedLocationCommand { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="PartyVM"/> class.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public PartyVM(Party entity)
        {
            _entity = entity;

            //Setup the ContactInfoVM
            ContactInfoVM = new ContactInfoVM(ContactInfoType.OwnedParties, entity != null ? entity.ContactInfoSet : null);

            #region Register Commands

            AddSelectedLocationCommand = new RelayCommand<Location>(OnAddSelectedLocation);
            DeleteSelectedLocationCommand = new RelayCommand<Location>(OnDeleteSelectedLocation);

            #endregion
        }

        #endregion

        #region Logic

        private void OnAddSelectedLocation(Location locationToAdd)
        {
            if (locationToAdd == null)
                MessageBox.Show("Please select a Location to add");
            else
            {
                this.Entity.Locations.Add(locationToAdd);
                RaisePropertyChanged("SelectedBusiness"); //This updates CurrentContext on ClientsVM
            }
        }

        private void OnDeleteSelectedLocation(Location locationToRemove)
        {
            this.Entity.Locations.Remove(locationToRemove);
            RaisePropertyChanged("SelectedBusiness"); //This updates CurrentContext on ClientsVM
        }

        #endregion
    }
}

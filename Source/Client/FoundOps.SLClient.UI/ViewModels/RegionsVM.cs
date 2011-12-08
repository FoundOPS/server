using System.Windows;
using System.ComponentModel;
using GalaSoft.MvvmLight.Command;
using MEFedMVVM.ViewModelLocator;
using GalaSoft.MvvmLight.Messaging;
using FoundOps.SLClient.Data.Services;
using FoundOps.SLClient.Data.ViewModels;
using System.ComponentModel.Composition;
using FoundOps.Core.Models.CoreEntities;
using ReactiveUI;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// Contains the logic for displaying Regions
    /// </summary>
    [ExportViewModel("RegionsVM")]
    public class RegionsVM : CoreEntityCollectionInfiniteAccordionVM<Region>, IAddDeleteSelectedLocation
    {
        #region Public Properties

        public RelayCommand<Location> AddSelectedLocationCommand { get; set; }
        public RelayCommand<Location> DeleteSelectedLocationCommand { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="RegionsVM"/> class.
        /// </summary>
        /// <param name="dataManager">The data manager.</param>
        [ImportingConstructor]
        public RegionsVM(DataManager dataManager)
            : base(dataManager)
        {
            //Setup the main query to Regions
            this.SetupMainQuery(DataManager.Query.Regions);

            #region Register Commands

            AddSelectedLocationCommand = new RelayCommand<Location>(OnAddSelectedLocation);
            DeleteSelectedLocationCommand = new RelayCommand<Location>(OnDeleteSelectedLocation);

            #endregion
        }

        #region Logic

        private void OnAddSelectedLocation(Location locationToAdd)
        {
            if (locationToAdd == null)
                MessageBox.Show("Please select a Location to add");
            else
            {
                this.SelectedEntity.Locations.Add(locationToAdd);
                //Refresh the Context to assure proper filtering
                RaisePropertyChanged("SelectedContext");
            }
        }

        private void OnDeleteSelectedLocation(Location locationToRemove)
        {
            this.SelectedEntity.Locations.Remove(locationToRemove);
            //Refresh the Context to assure proper filtering
            RaisePropertyChanged("SelectedContext");
        }

        protected void OnSelectedEntityChanged(Location oldValue, Location newValue)
        {
            if (newValue == null) return;

            if (newValue.Latitude != null && newValue.Longitude != null)
            {
                if (newValue.TelerikLocation != null)
                    MessageBus.Current.SendMessage(new LocationSetMessage(newValue.TelerikLocation.Value));
            }
        }

        protected override void OnAddEntity(Region newEntity)
        {
            //Set the proper owner BusinessAccount
            newEntity.BusinessAccount = ContextManager.OwnerAccount as BusinessAccount;
        }

        #endregion

    }
}

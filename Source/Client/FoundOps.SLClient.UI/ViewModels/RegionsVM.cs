using FoundOps.Core.Models.CoreEntities;
using FoundOps.SLClient.Data.ViewModels;
using MEFedMVVM.ViewModelLocator;
using ReactiveUI;
using System.ComponentModel.Composition;
using Telerik.Windows.Data;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// Contains the logic for displaying Regions
    /// </summary>
    [ExportViewModel("RegionsVM")]
    public class RegionsVM : InfiniteAccordionVM<Region>
    {
        #region Public Properties

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

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="RegionsVM"/> class.
        /// </summary>
        [ImportingConstructor]
        public RegionsVM()
        {
            //Setup data loading
            SetupTopEntityDataLoading(roleId => Context.GetRegionsForServiceProviderQuery(ContextManager.RoleId));
        }

        #region Logic

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

using System;
using ReactiveUI;
using Telerik.Windows.Data;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using MEFedMVVM.ViewModelLocator;
using FoundOps.SLClient.Data.Services;
using FoundOps.SLClient.Data.ViewModels;
using System.ComponentModel.Composition;
using FoundOps.Core.Models.CoreEntities;

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
            var disposeObservable = new Subject<bool>();

            //Whenever the RoleId updates, update the VirtualQueryableCollectionView
            ContextManager.RoleIdObservable.ObserveOnDispatcher().Subscribe(roleId =>
            {
                //Dispose the last VQCV subscriptions
                disposeObservable.OnNext(true);

                var initialQuery = Context.GetRegionsForServiceProviderQuery(ContextManager.RoleId);

                var result = DataManager.CreateContextBasedVQCV(initialQuery, disposeObservable);
                QueryableCollectionView = result.VQCV;

                //Subscribe the loading subject to the LoadingAfterFilterChange observable
                result.LoadingAfterFilterChange.Subscribe(IsLoadingSubject);
            });

            //Whenever the location changes load the location details
            SelectedEntityObservable.Where(se => se != null).Subscribe(selectedLocation =>
                Context.Load(Context.GetLocationDetailsForRoleQuery(ContextManager.RoleId, selectedLocation.Id)));
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

using FoundOps.Common.Silverlight.Tools.ExtensionMethods;
using FoundOps.Common.Tools;
using Microsoft.Windows.Data.DomainServices;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.ServiceModel.DomainServices.Client;
using System.Threading.Tasks;
using Telerik.Windows.Controls.DomainServices;
using Telerik.Windows.Data;

namespace FoundOps.Common.Silverlight.UI.Tools
{
    /// <summary>
    /// An extension for the VirtualQueryableCollectionView to automate data loading with an EntityQuery.
    /// The data loading incorporates any CollectionView imposed sort and filter conditions.
    /// TODO It stays up to date with added and removed entities, see http://www.telerik.com/community/forums/silverlight/gridview/problem-with-paging-and-queryablecollectionview.aspx.
    /// </summary>
    public class ExtendedVirtualQueryableCollectionView<TEntity> : VirtualQueryableCollectionView<TEntity> where TEntity : Entity
    {
        #region Public Events and Properties

        private bool _loadingVirtualItems;
        /// <summary>
        /// Set to true when loading virtual items.
        /// </summary>
        public bool LoadingVirtualItems
        {
            get { return _loadingVirtualItems; }
            set
            {
                _loadingVirtualItems = value;
                OnPropertyChanged("LoadingVirtualItems");
            }
        }

        private bool _loadingVirtualItemCount;
        /// <summary>
        /// Set to true when loading the virtual item count.
        /// </summary>
        public bool LoadingVirtualItemCount
        {
            get { return _loadingVirtualItemCount; }
            set
            {
                _loadingVirtualItemCount = value;
                OnPropertyChanged("LoadingVirtualItemCount");
            }
        }

        private readonly Subject<int> _virtualItemCountLoaded = new Subject<int>();
        /// <summary>
        /// Pushes the updated virtual item count whenever it is reloaded.
        /// </summary>
        public IObservable<int> VirtualItemCountLoaded { get { return _virtualItemCountLoaded.AsObservable(); } }

        private readonly Subject<IEnumerable<TEntity>> _firstItemsLoadedAfterUpdated = new Subject<IEnumerable<TEntity>>();
        /// <summary>
        /// Pushes the first loaded items after the virtual item count is loaded/reloaded.
        /// </summary>
        public IObservable<IEnumerable<TEntity>> FirstItemsLoadedAfterUpdated { get { return _firstItemsLoadedAfterUpdated.AsObservable(); } }

        #endregion

        #region Local Variables

        //Keeps track of added and removed items from the DomainContext
        private readonly EntityList<TEntity> _backingSource;

        //The DomainContext to load the entities with
        private readonly DomainContext _context;

        //When a value is pushed, it will cancel any previous loads
        private readonly Subject<bool> _cancelAllLoads = new Subject<bool>();

        //A function which returns the EntityQuery to use to load the entities
        private readonly Func<EntityQuery<TEntity>> _loadItemsQuery;

        //When a value is pushed it will load/reload the virtual item count. This will cancel all previous loads.
        private readonly Subject<bool> _loadVirtualItemCount = new Subject<bool>();

        #region Subscriptions to track for disposal

        private IDisposable _filterSubscription;
        private IDisposable _loadVirtualItemCountSubscription;
        private IDisposable _loadVirtualItemsSubscription;
        private IDisposable _forceLoadFirstItemsSubscription;

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedVirtualQueryableCollectionView&lt;TEntity&gt;"/> class.
        /// </summary>
        /// <param name="context">The DomainContext to load the entities with.</param>
        /// <param name="loadItemsQuery">A function which returns the EntityQuery to use to load the entities.</param>
        /// <param name="loadVirtualItemCount">An observable when pushed will load/reload the virtual item count. You must push at least once. It will cancel previous loads.</param>
        /// <param name="forceFirstLoad">Force load the first items after the VirtualItemCount is loaded. Used for controls that do not automatically support virtual loading.</param>
        public ExtendedVirtualQueryableCollectionView(DomainContext context, Func<EntityQuery<TEntity>> loadItemsQuery, IObservable<bool> loadVirtualItemCount, bool forceFirstLoad = false)
        {
            _backingSource = new EntityList<TEntity>((EntitySet<TEntity>)context.EntityContainer.GetEntitySet(typeof(TEntity)));
            _context = context;
            _loadItemsQuery = loadItemsQuery;

            //TODO allow this to be variable based off an observable or just do that externally with the existing load size property?
            this.LoadSize = 50;

            //Whenever the loadVirtualItemCount pushes: load/reload the virtual item count
            _loadVirtualItemsSubscription = _loadVirtualItemCount.Throttle(TimeSpan.FromMilliseconds(200)).ObserveOnDispatcher()
                .Subscribe(_ => UpdateVirtualItemCount());

            //Reload the virtual item count when
            //a) there are filter changes
            _filterSubscription = SubscribeToFilterChanges(_loadVirtualItemCount);
            //b) a value is pushed to the user passed loadVirtualItemCount
            _loadVirtualItemCountSubscription = loadVirtualItemCount.Subscribe(_loadVirtualItemCount);

            //Handle the ItemsLoading event by loading the items
            this.ItemsLoading += ExtendedVirtualQueryableCollectionViewItemsLoading;

            //Use the backingSource to keep the VQCV updated
            _backingSource.CollectionChanged += BackingSourceCollectionChanged;

            //Whenever the VirtualItemCount is loaded force load the first items
            if (forceFirstLoad)
                _forceLoadFirstItemsSubscription = VirtualItemCountLoaded.Subscribe(itemCount => LoadItems(0, LoadSize));
        }

        /// <summary>
        /// Will subscribe the observer to any individual or collection filter changes.
        /// </summary>
        /// <param name="observer">The observer to push when there are changes.</param>
        private IDisposable SubscribeToFilterChanges(IObserver<bool> observer)
        {
            var individualFilterChanged = Observable.FromEventPattern<EventHandler<ItemChangedEventArgs<IFilterDescriptor>>, ItemChangedEventArgs<IFilterDescriptor>>
                (h => this.FilterDescriptors.ItemChanged += h, h => this.FilterDescriptors.ItemChanged -= h).AsGeneric();

            var filterCollectionChanged = this.FilterDescriptors.FromCollectionChanged().AsGeneric();

            return individualFilterChanged.Merge(filterCollectionChanged).Subscribe(observer);
        }

        #endregion

        #region Logic

        #region Public

        /// <summary>
        /// This will cancel all cancellable loads for both item count and items.
        /// </summary>
        public void CancelLoads()
        {
            _cancelAllLoads.OnNext(true);
        }

        #endregion

        #region Protected

        #region Overriden

        protected override void Dispose(bool disposing)
        {
            //Cancel loads before finishing dispose
            CancelLoads();

            //Dispose all subscriptions
            if (_filterSubscription != null)
            {
                _filterSubscription.Dispose();
                _filterSubscription = null;
            }
            if (_loadVirtualItemCountSubscription != null)
            {
                _loadVirtualItemCountSubscription.Dispose();
                _loadVirtualItemCountSubscription = null;
            }
            if (_loadVirtualItemsSubscription != null)
            {
                _loadVirtualItemsSubscription.Dispose();
                _loadVirtualItemsSubscription = null;
            }
            if (_forceLoadFirstItemsSubscription != null)
            {
                _forceLoadFirstItemsSubscription.Dispose();
                _forceLoadFirstItemsSubscription = null;
            }

            base.Dispose(disposing);
        }

        #endregion

        #endregion

        #region Private

        /// <summary>
        /// TODO Use the backingSource to keep the VQCV updated
        /// </summary>
        private void BackingSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            //Keep track of removed added items
        }

        #region Private

        //Used for tracking the first load after the virtual item count is updated
        private bool _firstVirtualLoadAfterVirtualItemCountLoadedFlag;
        /// <summary>
        /// Update the VirtualItemCount
        /// </summary>
        private void UpdateVirtualItemCount()
        {
            //Cancel any previous loads when reloading the VirtualItemCount
            _cancelAllLoads.OnNext(true);

            //Filter the query with the FilterDescriptors and SortDescriptors
            var filteredQuery = _loadItemsQuery().Where(this.FilterDescriptors).Sort(this.SortDescriptors);

            _context.CountAsync(filteredQuery, _cancelAllLoads)
                 .ContinueWith(task =>
                 {
                     if (task.IsCanceled)
                         return;

                     //TODO add count of new items to the entity list. subtract removed items. on save update those counts.
                     this.VirtualItemCount = task.Result;

                     _virtualItemCountLoaded.OnNext(VirtualItemCount);

                     _firstVirtualLoadAfterVirtualItemCountLoadedFlag = true;
                 }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// Loads the virtual items using the loadItemsQuery into the VQCV.
        /// </summary>
        /// <param name="startIndex">The start index to the load.</param>
        /// <param name="numberOfItemsToLoad">The number of items to load.</param>
        private void LoadItems(int startIndex, int numberOfItemsToLoad)
        {
            LoadingVirtualItems = true;

            //Filter the query with the FilterDescriptors and SortDescriptors
            //Limit the query based on the startIndex and the numberOfItemsToLoad
            var filteredLimitedQuery = _loadItemsQuery().Where(this.FilterDescriptors).Sort(this.SortDescriptors)
                .Skip(startIndex).Take(numberOfItemsToLoad);

            Debug.WriteLine(typeof(TEntity) + " virtual load " + startIndex + " to " + (startIndex + numberOfItemsToLoad));

            //Cancel whenever the _cancelAllLoads is pushed
            _context.LoadAsync(filteredLimitedQuery, _cancelAllLoads)
                .ContinueWith(task =>
                {
                    //TODO insert new item into this.Load
                    //TODO ?? they might already be saved, removed deleted items
                    if (!task.IsCanceled)
                        this.Load(startIndex, task.Result);
                    else
                        Debug.WriteLine(typeof(TEntity) + " cancelled virtual load " + startIndex + " to " + (startIndex + numberOfItemsToLoad));

                    LoadingVirtualItems = false;

                    //If this is the first virtual load since the virtual item count was loaded
                    //push the result and reset the flag
                    if (!task.IsCanceled && _firstVirtualLoadAfterVirtualItemCountLoadedFlag)
                    {
                        _firstVirtualLoadAfterVirtualItemCountLoadedFlag = false;
                        _firstItemsLoadedAfterUpdated.OnNext(task.Result);
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// When the VirtualQueryableCollection requests, (asynchronously) load the items from the DomainContext.
        /// </summary>
        private void ExtendedVirtualQueryableCollectionViewItemsLoading(object sender, VirtualQueryableCollectionViewItemsLoadingEventArgs e)
        {
            //Do not load if ther VQCV has no load size (the system is not in details view or a related type's details view)

            //TODO (look into) Do not load if loadingAfterFilterChange (to prevent double loading if force first load is flagged)
            if (this.LoadSize == 0)
            {
                e.Cancel = true;
                return;
            }

            LoadItems(e.StartIndex, e.ItemCount);
        }

        #endregion

        #endregion

        #endregion
    }
}

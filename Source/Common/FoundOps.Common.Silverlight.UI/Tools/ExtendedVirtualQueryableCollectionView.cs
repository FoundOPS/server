using System.Collections.ObjectModel;
using FoundOps.Common.Models;
using FoundOps.Common.Silverlight.Tools.ExtensionMethods;
using FoundOps.Common.Tools;
using System;
using System.Collections.Generic;
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
    public class ExtendedVirtualQueryableCollectionView<TBase, TEntity> : VirtualQueryableCollectionView<TEntity>
        where TBase : Entity
        where TEntity : TBase
    {
        #region Public Events and Properties

        #region IsLoading/Loaded

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

        private readonly Subject<bool> _loadingVirtualItemCountSubject = new Subject<bool>();
        /// <summary>
        /// An observable that pushes when the virtual item count is loading or not.
        /// </summary>
        public IObservable<bool> LoadingVirtualItemCountObservable { get { return _loadingVirtualItemCountSubject; } }

        private bool _loadingVirtualItemCount;
        /// <summary>
        /// Set to true when loading the virtual item count.
        /// </summary>
        public bool LoadingVirtualItemCount
        {
            get { return _loadingVirtualItemCount; }
            private set
            {
                _loadingVirtualItemCount = value;
                _loadingVirtualItemCountSubject.OnNext(value);
                OnPropertyChanged("LoadingVirtualItemCount");
            }
        }

        private readonly Subject<int> _virtualItemCountLoaded = new Subject<int>();
        /// <summary>
        /// Pushes the updated virtual item count whenever it is reloaded.
        /// </summary>
        public IObservable<int> VirtualItemCountLoaded { get { return _virtualItemCountLoaded.AsObservable(); } }

        private readonly Subject<IEnumerable<TBase>> _firstItemsLoadedAfterUpdated = new Subject<IEnumerable<TBase>>();
        /// <summary>
        /// Pushes the first loaded items after the virtual item count is loaded/reloaded.
        /// </summary>
        public IObservable<IEnumerable<TBase>> FirstItemsLoadedAfterUpdated { get { return _firstItemsLoadedAfterUpdated.AsObservable(); } }

        #endregion

        #endregion

        #region Local Variables

        //The DomainContext to load the entities with
        private readonly DomainContext _context;

        //When a value is pushed, it will cancel any previous loads
        private readonly Subject<bool> _cancelAllLoads = new Subject<bool>();

        //The EntitySet of the entity to load.
        private readonly EntitySet<TBase> _entitySet;

        //A function which returns the EntityQuery to use to load the entities
        private readonly Func<EntityQuery<TBase>> _loadItemsQuery;

        //When a value is pushed it will load/reload the virtual item count. This will cancel all previous loads.
        private readonly Subject<bool> _loadVirtualItemCount = new Subject<bool>();

        #region Subscriptions to track for disposal

        private IDisposable _filterSubscription;
        private IDisposable _loadVirtualItemCountObservableSubscription;
        private IDisposable _loadVirtualItemCountSubscription;

        #endregion

        #region ObservationState

        private readonly ObservableCollection<object> _controlsThatCurrentlyRequireThisData = new ObservableCollection<object>();
        ///<summary>
        /// A list of controls that require this data.
        /// It will only load if this is > 0
        ///</summary>
        protected ObservableCollection<object> ControlsThatCurrentlyRequireThisData { get { return _controlsThatCurrentlyRequireThisData; } }

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedVirtualQueryableCollectionView&lt;TBase,TBase&gt;"/> class.
        /// </summary>
        /// <typeparam name="TBase">The entity type of the query and the EntitySet. Either the same as TEntity or it is the base class.</typeparam>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <param name="context">The DomainContext to load the entities with.</param>
        /// <param name="loadItemsQuery">A function which returns the EntityQuery to use to load the entities.</param>
        /// <param name="loadVirtualItemCount">An observable when pushed will load/reload the virtual item count. You must push at least once. It will cancel previous loads.</param>
        public ExtendedVirtualQueryableCollectionView(DomainContext context, Func<EntityQuery<TBase>> loadItemsQuery, IObservable<bool> loadVirtualItemCount)
        {
            //Keep this updated when entities are deleted from the entity set
            _entitySet = ((EntitySet<TBase>)context.EntityContainer.GetEntitySet(typeof(TBase)));
            _entitySet.EntityRemoved += ExtendedVirtualQueryableCollectionViewEntityRemoved;

            _context = context;
            _loadItemsQuery = loadItemsQuery;

            //TODO allow this to be variable based off an observable or just do that externally with the existing load size property?
            this.LoadSize = 50;

            //Whenever the loadVirtualItemCount pushes: load/reload the virtual item count
            _loadVirtualItemCountSubscription = _loadVirtualItemCount.ObserveOnDispatcher()
                .Subscribe(_ => UpdateVirtualItemCount());

            //Reload the virtual item count when
            //a) there are filter changes
            _filterSubscription = SubscribeToFilterChanges(_loadVirtualItemCount);
            //b) a value is pushed to the user passed loadVirtualItemCount
            _loadVirtualItemCountObservableSubscription = loadVirtualItemCount.Subscribe(_loadVirtualItemCount);

            //Handle the ItemsLoading event by loading the items
            this.ItemsLoading += ExtendedVirtualQueryableCollectionViewItemsLoading;
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
            if (_loadVirtualItemCountObservableSubscription != null)
            {
                _loadVirtualItemCountObservableSubscription.Dispose();
                _loadVirtualItemCountObservableSubscription = null;
            }

            _entitySet.EntityRemoved -= ExtendedVirtualQueryableCollectionViewEntityRemoved;
            this.ItemsLoading -= ExtendedVirtualQueryableCollectionViewItemsLoading;

            base.Dispose(disposing);
        }

        #endregion

        #endregion

        #region Private

        //Used for tracking the first load after the virtual item count is updated
        private bool _firstVirtualLoadAfterVirtualItemCountLoadedFlag;
        /// <summary>
        /// Update the VirtualItemCount
        /// </summary>
        public void UpdateVirtualItemCount()
        {
            var query = _loadItemsQuery();
            if (query == null) return;

            LoadingVirtualItemCount = true;

            //Cancel any previous loads when reloading the VirtualItemCount
            _cancelAllLoads.OnNext(true);

            //Filter the query with the FilterDescriptors and SortDescriptors
            var filteredQuery = query.Where(this.FilterDescriptors).Sort(this.SortDescriptors);

            Debug.WriteLine(typeof(TBase) + " load count");

            _context.CountAsync(filteredQuery, _cancelAllLoads)
                 .ContinueWith(task =>
                 {
                     if (task.IsCanceled)
                     {
                         Debug.WriteLine(typeof(TBase) + " cancelled load count");
                         return;
                     }

                     //TODO add count of new items to the entity list. subtract removed items. on save update those counts.
                     this.VirtualItemCount = task.Result;

                     _virtualItemCountLoaded.OnNext(VirtualItemCount);

                     _firstVirtualLoadAfterVirtualItemCountLoadedFlag = true;

                     LoadingVirtualItemCount = false;

                     //Force load first items
                     LoadItems(0, 50);
                 }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// Loads the virtual items using the loadItemsQuery into the VQCV.
        /// Skips new items.
        /// </summary>
        /// <param name="startIndex">The start index to the load.</param>
        /// <param name="numberOfItemsToLoad">The number of items to load.</param>
        private void LoadItems(int startIndex, int numberOfItemsToLoad)
        {
            var query = _loadItemsQuery();
            if (query == null) return;

            if (LoadingVirtualItemCount) return;

            LoadingVirtualItems = true;

            //Filter the query with the FilterDescriptors and SortDescriptors
            //Limit the query based on the startIndex and the numberOfItemsToLoad
            var filteredLimitedQuery = query.Where(this.FilterDescriptors).Sort(this.SortDescriptors)
                .Skip(startIndex).Take(numberOfItemsToLoad);

            Debug.WriteLine(typeof(TBase) + " virtual load " + startIndex + " to " + (startIndex + numberOfItemsToLoad));

            //Cancel whenever the _cancelAllLoads is pushed
            _context.LoadAsync(filteredLimitedQuery, _cancelAllLoads)
            .ContinueWith(task =>
            {
                if (!task.IsCanceled)
                {
                    //Load the items into the VQCV
                    this.Load(startIndex, task.Result);
                }
                else
                    Debug.WriteLine(typeof(TBase) + " cancelled virtual load " + startIndex + " to " + (startIndex + numberOfItemsToLoad));

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
            LoadItems(e.StartIndex, e.ItemCount);
        }

        /// <summary>
        /// Follows when entities are removed from the EntitySet and keeps the source collection updated.
        /// </summary>
        void ExtendedVirtualQueryableCollectionViewEntityRemoved(object sender, EntityCollectionChangedEventArgs<TBase> e)
        {
            //Ignore other base class changes
            //if (e.Entity as TEntity == null)
            //    return;

            //var sourceCollection = (RadObservableCollection<object>)this.SourceCollection;
            //if (sourceCollection.Contains(e.Entity))
            //{
            //    sourceCollection.Remove(e.Entity);
            //    this.VirtualItemCount = sourceCollection.Count;
            //}

            Rxx3.RunDelayed(TimeSpan.FromSeconds(1), UpdateVirtualItemCount);
        }

        #endregion

        #endregion
    }
}

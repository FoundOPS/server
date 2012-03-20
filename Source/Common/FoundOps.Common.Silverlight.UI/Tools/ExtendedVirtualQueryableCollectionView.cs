using System.Collections.Specialized;
using System.ComponentModel;
using System.Reactive.Linq;
using FoundOps.Common.Silverlight.Tools.ExtensionMethods;
using FoundOps.Common.Tools;
using Microsoft.Windows.Data.DomainServices;
using System;
using System.Diagnostics;
using System.Reactive.Subjects;
using System.ServiceModel.DomainServices.Client;
using System.Threading.Tasks;
using Telerik.Windows.Controls.DomainServices;
using Telerik.Windows.Data;

namespace FoundOps.Common.Silverlight.UI.Tools
{
    /// <summary>
    /// An extension on the VirtualQueryableCollectionView to automate data loading with an EntityQuery.
    /// It also extends the VQCV to stay up to date with added and removed entities (see http://www.telerik.com/community/forums/silverlight/gridview/problem-with-paging-and-queryablecollectionview.aspx)
    /// </summary>
    public class ExtendedVirtualQueryableCollectionView<TEntity> : VirtualQueryableCollectionView<TEntity>, INotifyPropertyChanged where TEntity : Entity
    {
        #region Public Events and Properties

        //Implementation of INotifyPropertyChanged
        public new event PropertyChangedEventHandler PropertyChanged;

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
                this.RaisePropertyChanged("LoadingVirtualItems");
            }
        }

        private bool _loadingVirtualItemsCount;
        /// <summary>
        /// Set to true when loading the virtual items count.
        /// </summary>
        public bool LoadingVirtualItemsCount
        {
            get { return _loadingVirtualItemsCount; }
            set
            {
                _loadingVirtualItemsCount = value;
                this.RaisePropertyChanged("LoadingVirtualItemsCount");
            }
        }

        #endregion

        #region Local Variables

        //Keeps track of added and removed items from the DomainContext
        private readonly EntityList<TEntity> _backingSource;

        //The DomainContext to load the entities with
        private readonly DomainContext _context;

        //A function which returns the EntityQuery to use to load the entities
        private readonly Func<EntityQuery<TEntity>> _loadItemsQuery;

        //An observable when pushed will load/reload the virtual item count. You must push at least once. This will cancel all previous loads.
        private readonly Subject<bool> _loadVirtualItemCount = new Subject<bool>();

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedVirtualQueryableCollectionView&lt;TEntity&gt;"/> class.
        /// </summary>
        /// <param name="context">The DomainContext to load the entities with.</param>
        /// <param name="loadItemsQuery">A function which returns the EntityQuery to use to load the entities.</param>
        /// <param name="loadVirtualItemCount">An observable when pushed will load/reload the virtual item count. You must push at least once. It will cancel previous loads.</param>
        public ExtendedVirtualQueryableCollectionView(DomainContext context, Func<EntityQuery<TEntity>> loadItemsQuery, IObservable<bool> loadVirtualItemCount)
        {
            _backingSource = new EntityList<TEntity>((EntitySet<TEntity>)context.EntityContainer.GetEntitySet(typeof(TEntity)));
            _context = context;
            _loadItemsQuery = loadItemsQuery;

            //Reload the virtual item count when
            //a) there are filter changes
            SubscribeToFilterChanges(_loadVirtualItemCount);
            //b) a value is pushed to the user passed loadVirtualItemCount
            loadVirtualItemCount.Subscribe(_loadVirtualItemCount);

            //Whenever the loadVirtualItemCount pushes: load/reload the virtual item count
            _loadVirtualItemCount.Subscribe(_ => UpdateVirtualItemCount());

            //Handle the ItemsLoading event by loading the items
            this.ItemsLoading += ExtendedVirtualQueryableCollectionViewItemsLoading;

            //Use the backingSource to keep the VQCV updated
            _backingSource.CollectionChanged += BackingSourceCollectionChanged;

            //Need to override the implemented INotifyPropertyChanged since the base class does not expose RaisePropertyChanged
            //Push the base classes property changes to the new PropertyChanged event
            base.PropertyChanged += (s, e) => this.RaisePropertyChanged(e.PropertyName);
        }

        /// <summary>
        /// Will subscribe the observer to any individual or collection filter changes.
        /// </summary>
        /// <param name="observer">The observer to push when there are changes.</param>
        private void SubscribeToFilterChanges(IObserver<bool> observer)
        {
            var individualFilterChanged = Observable.FromEventPattern<EventHandler<ItemChangedEventArgs<IFilterDescriptor>>, ItemChangedEventArgs<IFilterDescriptor>>
                (h => this.FilterDescriptors.ItemChanged += h, h => this.FilterDescriptors.ItemChanged -= h).AsGeneric();

            var filterCollectionChanged = this.FilterDescriptors.FromCollectionChanged().AsGeneric();

            individualFilterChanged.Merge(filterCollectionChanged).Subscribe(observer);
        }

        #endregion

        #region Logic

        #region Protected

        /// <summary>
        /// Raises a PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected virtual void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Private

        /// <summary>
        /// TODO Use the backingSource to keep the VQCV updated
        /// </summary>
        private void BackingSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
        }

        #region Data Loading

        private readonly Subject<bool> _cancelLastCountLoad = new Subject<bool>();
        /// <summary>
        /// Update the VirtualItemCount
        /// </summary>
        private async void UpdateVirtualItemCount()
        {
            //Cancel the last VirtualItemCount
            _cancelLastCountLoad.OnNext(true);
            this.VirtualItemCount = await _context.CountAsync(_loadItemsQuery(), _cancelLastCountLoad.Merge(_loadVirtualItemCount));
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

            Debug.WriteLine(typeof(TEntity) + " Virtual load " + startIndex + " to " + (startIndex + numberOfItemsToLoad));

            //Cancel whenever the _loadVirtualItemCount is pushed
            _context.LoadAsync(filteredLimitedQuery, _loadVirtualItemCount)
                .ContinueWith(task =>
                {
                    if (!task.IsCanceled)
                        this.Load(startIndex, task.Result);
                    else
                        Debug.WriteLine(typeof(TEntity) + " Cancelled virtual load " + startIndex + " to " + (startIndex + numberOfItemsToLoad));

                    LoadingVirtualItems = false;
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

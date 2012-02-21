using System;
using System.Linq;
using System.Diagnostics;
using Telerik.Windows.Data;
using System.Reactive.Linq;
using FoundOps.Common.Tools;
using System.Reactive.Subjects;
using System.Collections.Generic;
using Telerik.Windows.Controls.DomainServices;
using System.ServiceModel.DomainServices.Client;
using FoundOps.Common.Silverlight.Tools.ExtensionMethods;

namespace FoundOps.SLClient.Data.Services
{
    /// <summary>
    /// The new query system.
    /// </summary>
    public partial class DataManager
    {
        /// <summary>
        /// This method sets up the VQSV for infinite accordion entities.
        /// It takes into consideration any UI imposed sort conditions and filter conditions.
        /// It also filters based on the context filters.
        /// If the entity is in details view: it will load 100 at a time.
        /// If a related type of the entity is in details view: it will load 20 at a time.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity</typeparam>
        /// <param name="initialQuery">The initial query.</param>
        /// <param name="dispose">Will dispose all the subscriptions when any value is pushed.</param>
        /// <param name="relatedTypes">The related types when this is in details view and should load.</param>
        /// <param name="filterDescriptorObservables">An array of IObservable'FilterDescriptors to filter by.</param>
        /// <param name="forceLoadRelatedTypesContext">if set to <c>true</c> [force load when in a related types context].</param>
        /// <param name="onFirstLoadAfterFilter">An action to call after the first load after a filter.</param>
        /// <returns>
        /// a VirtualQueryableCollectionView (VQCV)
        /// </returns>
        public CreateVQCVResult<TEntity> SetupMainVQCV<TEntity>(EntityQuery<TEntity> initialQuery,
            IObservable<bool> dispose, IEnumerable<Type> relatedTypes = null, IObservable<FilterDescriptor>[] filterDescriptorObservables = null,
            bool forceLoadRelatedTypesContext = false, Action<IEnumerable<TEntity>> onFirstLoadAfterFilter = null) where TEntity : Entity
        {
            var view = new VirtualQueryableCollectionView<TEntity>();

            //Keep all the subscriptions at the top so they can be disposed in one place
            IDisposable filterDescriptorObservablesSubscription = null;
            IDisposable filterChangeLoadSubscription = null;
            IDisposable loadVirtualItemsSubscription = null;

            IDisposable disposeSubscription = null;
            disposeSubscription = dispose.Subscribe(_ =>
            {
                if (filterDescriptorObservablesSubscription != null)
                    filterDescriptorObservablesSubscription.Dispose();
                filterChangeLoadSubscription.Dispose();
                loadVirtualItemsSubscription.Dispose();
                disposeSubscription.Dispose();
            });

            //Update the filter descriptors on the view whenever the filterDescriptorObservables changes
            if (filterDescriptorObservables != null)
            {
                filterDescriptorObservablesSubscription = filterDescriptorObservables.CombineLatest().ObserveOnDispatcher().Subscribe(filterDescriptors =>
                {
                    //Clear the existing filter descriptors
                    view.FilterDescriptors.Clear();

                    //Add the current filter descriptors
                    view.FilterDescriptors.AddRange(filterDescriptors.Where(fd => fd != null));
                });
            }

            //Loading behavior subjects
            var countLoading = new BehaviorSubject<bool>(false);
            var loadingAfterFilterChange = new BehaviorSubject<bool>(false);
            var loadingVirtualItems = new BehaviorSubject<bool>(false);

            var contextOrFiltersChanged = new Subject<bool>();
            //Setup the load size and set the count whenever
            //a) details view changes
            //b) FilterDescriptors collection changes
            //c) a FilterDescriptor item changes
            filterChangeLoadSubscription = ContextManager.CurrentContextProviderObservable.Where(cp => cp != null).Select(cp => cp.ObjectTypeProvided).DistinctUntilChanged().AsGeneric()
            .Merge(view.FilterDescriptors.FromCollectionChanged().AsGeneric())
            .Merge(Observable.FromEventPattern<EventHandler<ItemChangedEventArgs<IFilterDescriptor>>, ItemChangedEventArgs<IFilterDescriptor>>(h => view.FilterDescriptors.ItemChanged += h, h => view.FilterDescriptors.ItemChanged -= h).AsGeneric())
                //Only allow filter changes every 200 milliseconds
            .Throttle(TimeSpan.FromMilliseconds(200))
            .ObserveOnDispatcher().Subscribe(_ =>
            {
                //push to contextOrFiltersChanged observable to cancel previous queries
                contextOrFiltersChanged.OnNext(true);

                var detailsType = ContextManager.CurrentContextProvider != null
                                      ? ContextManager.CurrentContextProvider.ObjectTypeProvided
                                      : null;

                //In details view load 100
                if (detailsType == typeof(TEntity))
                    view.LoadSize = 100;
                //In a related type view load 20
                else if (relatedTypes != null && relatedTypes.Any(t => t == detailsType))
                    view.LoadSize = 20;
                else //Otherwise do not load anything
                    view.LoadSize = 0;

                var filteredQuery = initialQuery.Where(view.FilterDescriptors);

                //update the item count if in details view or a related type view
                //cancel whenever the context or filters changed
                if (view.LoadSize > 0)
                {
                    countLoading.OnNext(true);
                    Debug.WriteLine(typeof(TEntity) + " Count filter change");
                    Context.CountAsync(filteredQuery, contextOrFiltersChanged)
                        .ContinueWith(task =>
                        {
                            if (!task.IsCanceled)
                                view.VirtualItemCount = task.Result;

                            countLoading.OnNext(false);
                        });
                }
                //If in a related type and the forceLoadRelatedTypesContext option is true than force load the first items
                if (forceLoadRelatedTypesContext && relatedTypes != null && relatedTypes.Any(t => t == detailsType))
                {
                    //Load the first items after a filter change
                    loadingAfterFilterChange.OnNext(true);
                    Context.LoadAsync(filteredQuery.Sort(view.SortDescriptors).Take(view.LoadSize), contextOrFiltersChanged)
                        .ContinueWith(task =>
                        {
                            if (!task.IsCanceled)
                            {
                                view.Load(0, task.Result);
                                if (onFirstLoadAfterFilter != null)
                                    onFirstLoadAfterFilter(task.Result);
                            }

                            loadingAfterFilterChange.OnNext(false);
                        });

                    Debug.WriteLine(typeof(TEntity) + " Load after filter change");
                }
            });

            //When the user scrolls to an area, load those entities
            loadVirtualItemsSubscription = Observable.FromEventPattern<EventHandler<VirtualQueryableCollectionViewItemsLoadingEventArgs>, VirtualQueryableCollectionViewItemsLoadingEventArgs>(h => view.ItemsLoading += h, h => view.ItemsLoading -= h)
                //Throttle by 300 milliseconds to prevent loading throughout every scroll and to allow the filter to resolve
            .Throttle(TimeSpan.FromMilliseconds(300))
            .ObserveOnDispatcher().Subscribe(e =>
            {
                //Do not load if ther VQCV has no load size (the system is not in details view or a related type's details view)
                if (view.LoadSize == 0) return;

                //Do not load if loadingAfterFilterChange (to prevent double loading because first load is always forced)
                if (loadingAfterFilterChange.First()) return;

                var itemsToLoad = initialQuery.Where(view.FilterDescriptors).Sort(view.SortDescriptors)
                    .Skip(e.EventArgs.StartIndex).Take(e.EventArgs.ItemCount);

                //Cancel whenever the context or filters changed
                loadingVirtualItems.OnNext(true);
                Context.LoadAsync(itemsToLoad, contextOrFiltersChanged)
                    .ContinueWith(task =>
                    {
                        if (!task.IsCanceled)
                            view.Load(e.EventArgs.StartIndex, task.Result);

                        loadingVirtualItems.OnNext(false);
                    });

                Debug.WriteLine(typeof(TEntity) + " Virtual load " + e.EventArgs.StartIndex + " to " + e.EventArgs.ItemCount);
            });

            return new CreateVQCVResult<TEntity>
            {
                CountLoading = countLoading,
                LoadingAfterFilterChange = loadingAfterFilterChange,
                LoadingVirtualItems = loadingVirtualItems,
                VQCV = view
            };
        }
    }

    /// <summary>
    /// The result of calling the CreateVQCV method.
    /// </summary>
    public class CreateVQCVResult<TEntity> where TEntity : Entity
    {
        /// <summary>
        /// A behavior subject which pushes whether or not the item count of the VQCV is loading.
        /// </summary>
        public IObservable<bool> CountLoading { get; set; }
        /// <summary>
        /// A behavior subject which pushes whether the first load after a filter change is loading.
        /// </summary>
        public IObservable<bool> LoadingAfterFilterChange { get; set; }
        /// <summary>
        /// A behavior subject which pushes whether the VQCV is loading virtual items.
        /// </summary>
        public IObservable<bool> LoadingVirtualItems { get; set; }

        /// <summary>
        /// The created VirtualQueryableCollectionView.
        /// </summary>
        public VirtualQueryableCollectionView<TEntity> VQCV { get; set; }
    }
}
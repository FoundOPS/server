using FoundOps.Common.Silverlight.Tools.ExtensionMethods;
using FoundOps.Common.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.ServiceModel.DomainServices.Client;
using System.Threading.Tasks;
using Telerik.Windows.Controls.DomainServices;
using Telerik.Windows.Data;

namespace FoundOps.SLClient.Data.Services
{
    /// <summary>
    /// The new query system.
    /// </summary>
    public partial class DataManager
    {
        /// <summary>
        /// This method sets up the VQCV for infinite accordion entities.
        /// It takes into consideration any UI imposed sort conditions and filter conditions.
        /// It also filters based on the context filters.
        /// If the entity is in details view: it will load 100 at a time.
        /// If a related type of the entity is in details view: it will load 20 at a time.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity</typeparam>
        /// <param name="queryFunction">A function which returns the query to execute.</param>
        /// <param name="dispose">Will dispose all the subscriptions when any value is pushed.</param>
        /// <param name="relatedTypes">The related types when this is in details view and should load.</param>
        /// <param name="contextRelationshipFilters">The related types and their property values on the current type. Ex. Vehicle, VehicleId, Vehicle.Id</param>
        /// <param name="forceLoadInRelatedTypesContext">if set to <c>true</c> [force load when in a related types context].</param>
        /// <param name="updateCountWhenRelatedTypesContextChanges">if set to <c>true</c> [update count when a related types context changes].</param>
        /// <param name="onForcedFirstLoadAfterFilter">An action to call after the first load after a filter is forced.</param>
        /// <returns>
        /// a VirtualQueryableCollectionView (VQCV)
        /// </returns>
        public CreateVQCVResult<TEntity> CreateContextBasedVQCV<TEntity>(Func<EntityQuery<TEntity>> queryFunction,
            IObservable<bool> dispose, IEnumerable<Type> relatedTypes = null, ContextRelationshipFilter[] contextRelationshipFilters = null,
            bool forceLoadInRelatedTypesContext = false, bool updateCountWhenRelatedTypesContextChanges = false, Action<IEnumerable<TEntity>> onForcedFirstLoadAfterFilter = null) where TEntity : Entity
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

            #region Setup FilterDescriptor Observables from the ContextRelationshipFilters

            if (contextRelationshipFilters == null)
                contextRelationshipFilters = new ContextRelationshipFilter[] {};

            //Build an array of FilterDescriptorObservables from the related types and from the GetContextObservable
            IObservable<FilterDescriptor>[] filterDescriptorObservables =
                                               (from relatedType in contextRelationshipFilters
                                                let method = typeof(ContextManager).GetMethod("GetContextObservable")
                                                let generic = method.MakeGenericMethod(new[] { relatedType.RelatedContextType })
                                                let contextObservable = (IObservable<object>)generic.Invoke(ContextManager, null)
                                                select contextObservable.DistinctUntilChanged().ObserveOnDispatcher().Select(context => context == null ? null :
                                                new FilterDescriptor(relatedType.EntityMember, FilterOperator.IsEqualTo, relatedType.FilterValueGenerator(context)))).ToArray();

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

            #endregion

            //Loading behavior subjects
            var countLoading = new BehaviorSubject<bool>(false);
            var loadingAfterFilterChange = new BehaviorSubject<bool>(false);
            var loadingVirtualItems = new BehaviorSubject<bool>(false);

            #region Setup Initial Count & Load (after a Filter changes, or the DetailsView changes)

            var contextOrFiltersChanged = new Subject<bool>();
            //Setup the load size and set the count whenever
            //a) details view changes
            //b) FilterDescriptors collection changes
            //c) a FilterDescriptor item changes
            //d) updateCountWhenRelatedTypesContextChanges is true and a related type context changes
            var updateLoadSizeCount = ContextManager.CurrentContextProviderObservable.Where(cp => cp != null).Select(cp => cp.ObjectTypeProvided).DistinctUntilChanged().AsGeneric()
             .Merge(view.FilterDescriptors.FromCollectionChanged().AsGeneric())
             .Merge(Observable.FromEventPattern<EventHandler<ItemChangedEventArgs<IFilterDescriptor>>, ItemChangedEventArgs<IFilterDescriptor>>(h => view.FilterDescriptors.ItemChanged += h, h => view.FilterDescriptors.ItemChanged -= h).AsGeneric());

            if (updateCountWhenRelatedTypesContextChanges && relatedTypes != null)
            {
                //Setup an observable that pushes whenever a related types context changes
                var relatedTypeContextChanged = (from relatedType in relatedTypes
                                                 let method = typeof(ContextManager).GetMethod("GetContextObservable")
                                                 let generic = method.MakeGenericMethod(new[] { relatedType })
                                                 let contextObservable = (IObservable<object>)generic.Invoke(ContextManager, null)
                                                 select contextObservable.DistinctUntilChanged().ObserveOnDispatcher()).Merge().AsGeneric();

                updateLoadSizeCount = updateLoadSizeCount.Merge(relatedTypeContextChanged);
            }

            //Only allow filter changes every 200 milliseconds
            filterChangeLoadSubscription = updateLoadSizeCount.Throttle(TimeSpan.FromMilliseconds(200))
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

                var query = queryFunction();
                var filteredQuery = query.Where(view.FilterDescriptors);

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
                        }, TaskScheduler.FromCurrentSynchronizationContext());
                }

                //Force load the first items if the forceLoadRelatedTypesContext option is true
                //and the details view is a related type
                if (forceLoadInRelatedTypesContext && relatedTypes != null && relatedTypes.Any(t => t == detailsType))
                {
                    //Load the first items after a filter change
                    loadingAfterFilterChange.OnNext(true);

                    Context.LoadAsync(filteredQuery.Sort(view.SortDescriptors).Take(view.LoadSize), contextOrFiltersChanged)
                        .ContinueWith(task =>
                        {
                            if (!task.IsCanceled)
                            {
                                view.Load(0, task.Result);
                                if (onForcedFirstLoadAfterFilter != null)
                                    onForcedFirstLoadAfterFilter(task.Result);
                            }

                            loadingAfterFilterChange.OnNext(false);
                        }, TaskScheduler.FromCurrentSynchronizationContext());

                    Debug.WriteLine(typeof(TEntity) + " Force load after filter change in related type");
                }
            });

            #endregion

            #region Setup Virtual Load (when a user scrolls or the collection is first shown in a loaded GridView)

            //When the user scrolls to an area, load those entities
            loadVirtualItemsSubscription = Observable.FromEventPattern<EventHandler<VirtualQueryableCollectionViewItemsLoadingEventArgs>, VirtualQueryableCollectionViewItemsLoadingEventArgs>(h => view.ItemsLoading += h, h => view.ItemsLoading -= h)
                .ObserveOnDispatcher().Subscribe(e =>
            {
                //Do not load if ther VQCV has no load size (the system is not in details view or a related type's details view)
                if (view.LoadSize == 0)
                {
                    e.EventArgs.Cancel = true;
                    return;
                }

                //Do not load if loadingAfterFilterChange (to prevent double loading if force first load is flagged)
                if (loadingAfterFilterChange.First())
                {
                    e.EventArgs.Cancel = true;
                    return;
                }

                var query = queryFunction();
                var itemsToLoad = query.Where(view.FilterDescriptors).Sort(view.SortDescriptors)
                    .Skip(e.EventArgs.StartIndex).Take(e.EventArgs.ItemCount);

                loadingVirtualItems.OnNext(true);

                //Cancel whenever the context or filters changed
                Context.LoadAsync(itemsToLoad, contextOrFiltersChanged)
                    .ContinueWith(task =>
                    {
                        if (!task.IsCanceled)
                            view.Load(e.EventArgs.StartIndex, task.Result);
                        else
                            Debug.WriteLine(typeof(TEntity) + " Cancelled virtual load " + e.EventArgs.StartIndex + " to " + (e.EventArgs.StartIndex + e.EventArgs.ItemCount));

                        loadingVirtualItems.OnNext(false);
                    }, TaskScheduler.FromCurrentSynchronizationContext());

                Debug.WriteLine(typeof(TEntity) + " Virtual load " + e.EventArgs.StartIndex + " to " + (e.EventArgs.StartIndex + e.EventArgs.ItemCount));
            });

            #endregion

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
    /// Defines the relationship between an entity and its context/
    /// </summary>
    public class ContextRelationshipFilter
    {
        /// <summary>
        /// The type of related entity context. Ex. Vehicle
        /// </summary>
        public Type RelatedContextType { get; set; }

        /// <summary>
        /// The Member of the current entity related to the context. Ex. VehicleMaintenance's "VehicleId"
        /// </summary>
        public String EntityMember { get; set; }

        /// <summary>
        /// A function when given the current related entity context, return the value of the context.
        /// Ex. FilterValueGenerator(someVehicle) = abcd-1324-asfa-fegeg (someVehicle.Id)
        /// </summary>
        public Func<object, object> FilterValueGenerator { get; set; }
    }

    /// <summary>
    /// The result of creating a VirtualQueryableCollectionView.
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
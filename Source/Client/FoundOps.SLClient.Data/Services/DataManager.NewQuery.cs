using System;
using System.Linq;
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
        #region Public

        #endregion

        /// <summary>
        /// This method sets up the VQSV for infinite accordion entities.
        /// It takes into consideration any UI imposed sort conditions and filter conditions.
        /// It also filters based on the context filters.
        /// If the entity is in details view: it will load 100 at a time
        /// If a related type of the entity is in details view: it will load 20 at a time
        /// It will always force load the first set
        /// </summary>
        /// <typeparam name="TEntity">The type of entity</typeparam>
        /// <param name="initialQuery">The initial query.</param>
        /// <param name="isLoading">A subject to push when loading.</param>
        /// <param name="relatedTypes">The related types when this is in details view and should load.</param>
        /// <param name="filterDescriptorObservables">An array of IObservable'FilterDescriptors to filter by.</param>
        /// <returns>
        /// a VirtualQueryableCollectionView (VQCV)
        /// </returns>
        public VirtualQueryableCollectionView<TEntity> SetupMainVQCV<TEntity>(EntityQuery<TEntity> initialQuery, Subject<bool> isLoading,
            IEnumerable<Type> relatedTypes = null, IObservable<FilterDescriptor>[] filterDescriptorObservables = null) where TEntity : Entity
        {
            var view = new VirtualQueryableCollectionView<TEntity>();

            #region Setup loadQuery method

            //An observable to push whenever canceling the last count and load
            var cancelLastCountLoad = new Subject<bool>();

            //An action to load the query
            //it will cancel the previous load if the filterDescriptors changed
            //Parameters: 1) the startIndex, 2) the amount to take, 3) if the query FilterDescriptors have changed
            var loadQuery = new Action<int, int, bool>(async (startIndex, take, filterDescriptorsChanged) =>
            {
                ////TODO: Only load if in details view or if a related type is in details view. Allow force load, add detailsview/ forceloadobservable to SetupMainVQCV parameters
                //var detailsViewType = this.ContextManager.CurrentContextProvider.ObjectTypeProvided;
                //if (detailsViewType != typeof(TEntity) && relatedTypes.All(t => t != detailsViewType))
                //    //TODO: If the load query needs to load but this is not and this changes before loadquery called again load
                //    return;

                //Cancel the previous load
                if (filterDescriptorsChanged)
                    cancelLastCountLoad.OnNext(true);

                //Signal that this is loading
                isLoading.OnNext(true);

                //update the VirtualItemCount
                if (filterDescriptorsChanged)
                    view.VirtualItemCount = await Context.CountAsync(initialQuery);

                //Sorts and filters the query based on the QCVs Sort and FilterDescriptors
                var queryToLoad = initialQuery.Sort(view.SortDescriptors).Where(view.FilterDescriptors)
                    .Skip(startIndex).Take(take);

                var load = Context.LoadAsync(queryToLoad, cancelLastCountLoad);
                load.ContinueWith(action =>
                {
                    if (!action.IsCanceled)
                        view.Load(startIndex, action.Result);

                    //load is complete, set is loading to false
                    isLoading.OnNext(false);
                });
            });

            #endregion

            //Load the first query
            loadQuery(0, 50, true);

            #region Setup the VirtualLoading

            //Change the virtual load size depending on if details view or not
            ContextManager.CurrentContextProviderObservable.Select(cp => cp.ObjectTypeProvided)
            .DistinctUntilChanged().Subscribe(detailsType =>
            {
                if (detailsType == typeof(TEntity))
                    view.LoadSize = 100;
                else if (relatedTypes != null && relatedTypes.Any(t => t == detailsType))
                    view.LoadSize = 20;
            });

            view.ItemsLoading += (s, e) => loadQuery(e.StartIndex, e.ItemCount, false);

            #endregion

            #region Setup Filters

            if (filterDescriptorObservables != null)
                //Update the filter descriptors on the view whenever the filterDescriptorObservables changes
                filterDescriptorObservables.CombineLatest().Throttle(TimeSpan.FromMilliseconds(200)).ObserveOnDispatcher().Subscribe(filterDescriptors =>
                {
                    //Clear the existing filter descriptors
                    view.FilterDescriptors.Clear();

                    //Add the current filter descriptors
                    foreach (var filterDescriptor in filterDescriptors.Where(fd => fd != null))
                        view.FilterDescriptors.Add(filterDescriptor);
                });

            //Load data when
            //a) the FilterDescriptors collection changes
            view.FilterDescriptors.FromCollectionChanged().AsGeneric()
                //b) an FilterDescriptor item changes
            .Merge(Observable.FromEventPattern<EventHandler<ItemChangedEventArgs<IFilterDescriptor>>, ItemChangedEventArgs<IFilterDescriptor>>(h => view.FilterDescriptors.ItemChanged += h, h => view.FilterDescriptors.ItemChanged -= h).AsGeneric())
                .Throttle(TimeSpan.FromMilliseconds(200))
                .Subscribe(_ => loadQuery(0, 20, true));

            #endregion

            return view;
        }
    }
}
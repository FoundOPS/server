using System.Collections.Generic;
using System.Linq;
using FoundOps.Common.Silverlight.UI.Interfaces;
using FoundOps.Common.Silverlight.UI.Controls.InfiniteAccordion;
using FoundOps.Common.Silverlight.UI.Tools;
using FoundOps.Common.Tools;
using FoundOps.SLClient.Data.Services;
using GalaSoft.MvvmLight.Command;
using ReactiveUI;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.ServiceModel.DomainServices.Client;
using Telerik.Windows.Data;

namespace FoundOps.SLClient.Data.ViewModels
{
    /// <summary>
    /// Contains logic for displaying things in the infinite accordion.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public abstract class InfiniteAccordionVM<TEntity> : CoreEntityCollectionVM<TEntity>, IProvideContext where TEntity : Entity
    {
        #region Public Properties

        private QueryableCollectionView _queryableCollectionView;
        /// <summary>
        /// The collection of Entities.
        /// </summary>
        public QueryableCollectionView QueryableCollectionView
        {
            get { return _queryableCollectionView; }
            protected set
            {
                _queryableCollectionView = value;
                CollectionViewObservable.OnNext(value);

                //Make sure the SelectedEntity is selected
                if (value != null)
                    value.MoveCurrentTo(SelectedEntity);

                this.RaisePropertyChanged("QueryableCollectionView");
            }
        }

        #region Implementation of IProvideContext

        private readonly Subject<object> _selectedContextSubject = new Subject<object>();
        /// <summary>
        /// Gets the selected context observable. Updates whenever the SelectedEntity changes.
        /// </summary>
        public IObservable<Object> SelectedContextObservable { get { return _selectedContextSubject.AsObservable(); } }

        private readonly ObservableAsPropertyHelper<object> _selectedContext;
        /// <summary>
        /// Gets the currently selected context.
        /// </summary>
        public object SelectedContext { get { return _selectedContext.Value; } }

        /// <summary>
        /// A command to move into this viewmodel's DetailsView.
        /// </summary>
        public RelayCommand MoveToDetailsView { get; private set; }

        /// <summary>
        /// Gets the object type provided for the InfiniteAccordion.
        /// </summary>
        public virtual Type ObjectTypeProvided
        {
            get { return typeof(TEntity); }
        }

        /// <summary>
        /// Gets a value indicating whether this view model's ObjectTypeProvided is currently the main entity displayed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is currently the main details view; otherwise, <c>false</c>.
        /// </value>
        public bool IsInDetailsView
        {
            get { return this.ContextManager.CurrentContextProvider == this; }
        }

        #endregion

        /// <summary>
        /// The relationships where this is the * end of a 1 to * relationship. Ex. LocationVM  = { Client, Region }
        /// </summary>
        public IEnumerable<Type> ManyRelationships { get; private set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="InfiniteAccordionVM&lt;TEntity&gt;"/> class.
        /// </summary>
        /// <param name="manyRelationships">(Optional) The relationships where this is the * end of a 1 to * relationship. Ex. LocationVM  = { Client, Region }</param>
        /// <param name="preventChangingSelectionWhenChanges">Whether or not to prevent changing the selected entity when the DomainContext has changes.</param>
        protected InfiniteAccordionVM(IEnumerable<Type> manyRelationships = null, bool preventChangingSelectionWhenChanges = true)
            : base(preventChangingSelectionWhenChanges)
        {
            ManyRelationships = manyRelationships;

            _selectedContext = this._selectedContextSubject.ToProperty(this, x => x.SelectedContext);

            //Set the SelectedContext to the SelectedEntity
            this.SelectedEntityObservable.Subscribe(_selectedContextSubject);

            MoveToDetailsView = new RelayCommand(NavigateToThis);

            //Whenever not in details view, disable SaveDiscardCancel
            this.ContextManager.CurrentContextProviderObservable.Select(contextProvider => contextProvider != this).SubscribeOnDispatcher()
                .Subscribe(disableSaveDiscardCancel =>
                               {
                                   DisableSaveDiscardCancel = disableSaveDiscardCancel;
                               });
        }

        #region CoreEntityCollectionInfiniteAccordionVM's Logic

        #region Data Loading

        //Keep these so they can be disposed whenever replacing a QCV
        private IDisposable _contextRelationshipFiltersSubscription;
        private IDisposable _selectFirstEntitySubscription;
        /// <summary>
        /// Sets up data loading for context based entities.
        /// </summary>
        /// <param name="entityQuery">The entity query.</param>
        /// <param name="contextRelationshipFilters">The related types and their property values on the current type. Ex. Vehicle, VehicleId, Vehicle.Id</param>
        /// <param name="forceFirstLoad">Force load the first items after the VirtualItemCount is loaded. Used for controls that do not automatically support virtual loading.</param>
        /// <param name="virtualItemCountLoadBehavior">sets the VirtualItemCountLoadBehavior</param>
        protected void SetupContextDataLoading(Func<Guid, EntityQuery<TEntity>> entityQuery, ContextRelationshipFilter[] contextRelationshipFilters,
            bool forceFirstLoad = false, VirtualItemCountLoadBehavior virtualItemCountLoadBehavior = VirtualItemCountLoadBehavior.LoadAfterCreation)
        {
            //Whenever the RoleId updates, update the VirtualQueryableCollectionView
            ContextManager.RoleIdObservable.ObserveOnDispatcher().Subscribe(roleId =>
            {
                #region Dispose last QCV

                //Dispose the last QueryableCollectionView
                if (QueryableCollectionView != null)
                    QueryableCollectionView.Dispose();

                //Dispose the previous QueryableCollectionView's _contextRelationshipFiltersSubscription
                if (_contextRelationshipFiltersSubscription != null)
                {
                    _contextRelationshipFiltersSubscription.Dispose();
                    _contextRelationshipFiltersSubscription = null;
                }

                //Dispose the previous _selectFirstEntitySubscription
                if (_selectFirstEntitySubscription != null)
                {
                    _selectFirstEntitySubscription.Dispose();
                    _selectFirstEntitySubscription = null;
                }

                #endregion

                IObservable<bool> loadVirtualItemCount;
                //Loads once immediately after creation (and automatically whenever a filter or sort changes).
                if (virtualItemCountLoadBehavior == VirtualItemCountLoadBehavior.LoadAfterCreation)
                {
                    loadVirtualItemCount = new[] { true }.ToObservable();
                }
                else
                {
                    //Loads any time a many relation context changes (and automatically whenever a filter or sort changes).
                    loadVirtualItemCount = (from manyRelation in ManyRelationships
                                            let method = typeof(ContextManager).GetMethod("GetContextObservable")
                                            let generic = method.MakeGenericMethod(new[] { manyRelation })
                                            let contextObservable = (IObservable<object>)generic.Invoke(ContextManager, null)
                                            select contextObservable.DistinctUntilChanged().ObserveOnDispatcher()).Merge().AsGeneric();
                }

                QueryableCollectionView = new ExtendedVirtualQueryableCollectionView<TEntity>(Context, () => entityQuery(roleId), loadVirtualItemCount, forceFirstLoad);

                _contextRelationshipFiltersSubscription = AddContextRelationshipFiltersHelper(contextRelationshipFilters, QueryableCollectionView, ContextManager);

                //Select the first loaded entity after the virtual item count is loaded
                _selectFirstEntitySubscription = ((ExtendedVirtualQueryableCollectionView<TEntity>)QueryableCollectionView).
                        FirstItemsLoadedAfterUpdated.Subscribe(loadedEntities => SelectedEntity = loadedEntities.FirstOrDefault());

                //TODO Subscribe the loading subject to when the count is loading
                //result.CountLoading.Subscribe(IsLoadingSubject);
            });
        }

        /// <summary>
        /// Adds and maintains context relationship filters for a QueryableCollectionView
        /// </summary>
        /// <param name="contextRelationshipFilters">The context relationship filters to add and maintain.</param>
        /// <param name="queryableCollectionView">The QueryableCollectionView to add and maintain the filters on.</param>
        /// <returns></returns>
        private static IDisposable AddContextRelationshipFiltersHelper(IEnumerable<ContextRelationshipFilter> contextRelationshipFilters, QueryableCollectionView queryableCollectionView, ContextManager contextManager)
        {
            //Build an array of FilterDescriptorObservables from the related types whenever the GetContextObservable pushes a new context
            var filterDescriptorObservableChanged =
                                           (from contextRelationshipFilter in contextRelationshipFilters
                                            let method = typeof(ContextManager).GetMethod("GetContextObservable")
                                            let generic = method.MakeGenericMethod(new[] { contextRelationshipFilter.RelatedContextType })
                                            let contextObservable = (IObservable<object>)generic.Invoke(contextManager, null)
                                            select contextObservable.DistinctUntilChanged().ObserveOnDispatcher()
                                            .Select(context =>
                                            {
                                                var filterDescriptor = context == null ? null :
                                                    new FilterDescriptor(contextRelationshipFilter.EntityMember, FilterOperator.IsEqualTo, contextRelationshipFilter.FilterValueGenerator(context));

                                                return new Tuple<ContextRelationshipFilter, FilterDescriptor>(contextRelationshipFilter, filterDescriptor);
                                            })).Merge();

            //Add, remove, or update the context relationship filters whenever their contexts change
            return filterDescriptorObservableChanged.Subscribe(tple =>
             {
                 //Try to find an existing related filter descriptor to the ContextRelationshipFilter.EntityMember for use below
                 var relatedFilterDescriptor = queryableCollectionView.FilterDescriptors.OfType<FilterDescriptor>().FirstOrDefault(fd => fd.Member == tple.Item1.EntityMember);

                 //If there is not a filter descriptor in the tuple (there is no longer a context)
                 //remove the existing filter descriptor if there is one
                 if (tple.Item2 == null && relatedFilterDescriptor != null)
                     queryableCollectionView.FilterDescriptors.Remove(relatedFilterDescriptor);
                 //If there is a filter descriptor in the tuple
                 //a filter should be added or updated on the QCV
                 else if (tple.Item2 != null)
                 {
                     //If no relatedFilterDescriptor exists, add the filter descriptor
                     if (relatedFilterDescriptor == null)
                         queryableCollectionView.FilterDescriptors.Add(tple.Item2);
                     //If a relatedFilterDescriptor exists, update the existing related filter descriptor
                     else
                         relatedFilterDescriptor.Value = tple.Item2.Value;
                 }
             });
        }

        /// <summary>
        /// Sets up data loading for entities without any context.
        /// </summary>
        /// <param name="entityQuery">An action which is passed the role id, and should return the entity query to load data with.</param>
        protected void SetupTopEntityDataLoading(Func<Guid, EntityQuery<TEntity>> entityQuery)
        {
            //Whenever the RoleId updates
            //update the VirtualQueryableCollectionView
            ContextManager.RoleIdObservable.ObserveOnDispatcher().Subscribe(roleId =>
            {
                //Dispose the last QueryableCollectionView
                if (QueryableCollectionView != null)
                    QueryableCollectionView.Dispose();

                //Load the VirtualItemCount immediately after creation (and automatically whenever a filter or sort changes)
                var loadVirtualItemCount = new[] { true }.ToObservable();

                QueryableCollectionView = new ExtendedVirtualQueryableCollectionView<TEntity>(Context, () => entityQuery(roleId), loadVirtualItemCount);

                //TODO Subscribe the loading subject to when the count is loading
                //result.CountLoading.Subscribe(IsLoadingSubject);
            });
        }

        /// <summary>
        /// Sets up data loading of an entities details when it is selected.
        /// TEntity must implement ILoadDetails.
        /// </summary>
        /// <param name="entityQuery">An action which is passed the selected entity and should return the entity query to load data with.</param>
        protected void SetupDetailsLoading(Func<TEntity, EntityQuery<TEntity>> entityQuery)
        {
            LoadOperation<TEntity> detailsLoadOperation = null;
            //Whenever the selected entity changes load the details
            SelectedEntityObservable.Where(se => se != null).Subscribe(selectedEntity =>
            {
                //Cancel the last load
                if (detailsLoadOperation != null && detailsLoadOperation.CanCancel)
                    detailsLoadOperation.Cancel();

                ((ILoadDetails)selectedEntity).DetailsLoading = true;
                detailsLoadOperation = Context.Load(entityQuery(selectedEntity), loadOp => ((ILoadDetails)selectedEntity).DetailsLoading = false, null);
            });
        }

        #endregion

        /// <summary>
        /// Navigates to this ViewModel.
        /// </summary>
        public void NavigateToThis()
        {
            //Check if its possible to navigate from the CurrentContextProvider 
            ((IPreventNavigationFrom)ContextManager.CurrentContextProvider).CanNavigateFrom(() =>
                //If it is possible send the MoveToDetailsViewMessage
                MessageBus.Current.SendMessage(new MoveToDetailsViewMessage(ObjectTypeProvided, MoveStrategy.AddContextToExisting)));
        }

        #endregion
    }

    /// <summary>
    /// Describes the QueryableCollectionView's virtual item count load behavior
    /// </summary>
    public enum VirtualItemCountLoadBehavior
    {
        /// <summary>
        /// Loads once immediately after creation (and automatically whenever a filter or sort changes).
        /// </summary>
        LoadAfterCreation,
        /// <summary>
        /// Loads any time a many relation context changes (and automatically whenever a filter or sort changes).
        /// </summary>
        LoadAfterManyRelationContextChanges
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
}

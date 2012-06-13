using System.Threading.Tasks;
using FoundOps.Common.Silverlight.Tools.ExtensionMethods;
using FoundOps.Common.Silverlight.UI.Controls.InfiniteAccordion;
using FoundOps.Common.Silverlight.UI.Interfaces;
using FoundOps.Common.Silverlight.UI.Tools;
using FoundOps.Common.Tools;
using FoundOps.SLClient.Data.Services;
using GalaSoft.MvvmLight.Command;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.ServiceModel.DomainServices.Client;
using System.Windows.Controls;
using Telerik.Windows.Data;

namespace FoundOps.SLClient.Data.ViewModels
{
    /// <summary>
    /// Contains logic for displaying things in the infinite accordion.
    /// </summary>
    /// <typeparam name="TBase">The entity type of the query and the EntitySet. Either the same as TEntity or it is the base class.</typeparam>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    public abstract class InfiniteAccordionVM<TBase, TEntity> : CoreEntityCollectionVM<TBase>, IProvideContext
        where TBase : Entity
        where TEntity : TBase
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
                ViewObservable.OnNext(value);

                //Make sure the SelectedEntity is selected
                if (value != null)
                    value.MoveCurrentTo(SelectedEntity);

                this.RaisePropertyChanged("QueryableCollectionView");
            }
        }

        /// <summary>
        /// Returns the ExtendedVirtualQueryableCollectionView 
        /// </summary>
        public ExtendedVirtualQueryableCollectionView<TBase, TEntity> ExtendedVirtualQueryableCollectionView
        {
            get { return QueryableCollectionView as ExtendedVirtualQueryableCollectionView<TBase, TEntity>; }
        }

        #region Implementation of IProvideContext

        private readonly Subject<object> _selectedContextSubject = new Subject<object>();

        /// <summary>
        /// Gets the selected context observable. Updates whenever the SelectedEntity changes.
        /// </summary>
        public IObservable<Object> SelectedContextObservable
        {
            get { return _selectedContextSubject.AsObservable(); }
        }

        private readonly ObservableAsPropertyHelper<object> _selectedContext;

        /// <summary>
        /// Gets the currently selected context.
        /// </summary>
        public object SelectedContext
        {
            get { return _selectedContext.Value; }
        }

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
        /// Initializes a new instance of the <see cref="InfiniteAccordionVM&lt;TBase, TEntity&gt;"/> class.
        /// </summary>
        /// <param name="manyRelationships">(Optional) The relationships where this is the * end of a 1 to * relationship. Ex. LocationVM  = { Client, Region }</param>
        /// <param name="preventChangingSelectionWhenChanges">Whether or not to prevent changing the selected entity when the DomainContext has changes.</param>
        /// <param name="preventNullSelection">Prevents SelectedEntity from being set to null</param>
        /// <param name="initializeDefaultCollectionView">Initialize a default QueryableCollectionView for the CollectionView property.</param>
        protected InfiniteAccordionVM(IEnumerable<Type> manyRelationships = null,
                                      bool preventChangingSelectionWhenChanges = true, bool preventNullSelection = false,
                                      bool initializeDefaultCollectionView = true)
            : base(preventChangingSelectionWhenChanges, preventNullSelection, initializeDefaultCollectionView)
        {
            ManyRelationships = manyRelationships;

            _selectedContext = this._selectedContextSubject.ToProperty(this, x => x.SelectedContext);

            //Set the SelectedContext to the SelectedEntity
            this.SelectedEntityObservable.Subscribe(_selectedContextSubject);

            MoveToDetailsView = new RelayCommand(NavigateToThis);

            //Whenever not in details view, disable SaveDiscardCancel
            this.ContextManager.CurrentContextProviderObservable.Select(contextProvider => contextProvider != this)
                .ObserveOnDispatcher().Subscribe(disableSaveDiscardCancel =>
                {
                    DisableSelectedEntitySaveDiscardCancel = disableSaveDiscardCancel;
                });

            //When rejecting changes, if the entity is detached. Clear the SelectedEntity
            this.DomainContext.ChangesRejectedObservable.ObserveOnDispatcher().Subscribe(args =>
            {
                if (SelectedEntity != null && args.RejectedAddedEntities.Contains(SelectedEntity))
                    SelectedEntity = null;
            });

            //TODO FIX ADDING NEW ENTITIES then remove workaround
            this.DataManager.ChangesSaved += submitOperation =>
            {
                //Add the added entities to the VirtualItemCount
                if (ExtendedVirtualQueryableCollectionView != null)
                {
                    var addedEntities = submitOperation.ChangeSet.AddedEntities.OfType<TEntity>();
                    ExtendedVirtualQueryableCollectionView.VirtualItemCount += addedEntities.Count();
                }
            };
        }

        #region InfiniteAccordionVM's Logic

        #region Data Loading

        //Keep these so they can be disposed whenever replacing a QCV
        private IDisposable _contextRelationshipFiltersSubscription;
        private IDisposable _selectFirstEntitySubscription;

        /// <summary>
        /// Sets up data loading for context based entities.
        /// </summary>
        /// <param name="entityQuery">The entity query.</param>
        /// <param name="contextRelationshipFilters">The related types and their property values on the current type. Ex. Vehicle, VehicleId, Vehicle.Id</param>
        /// <param name="contextType">Whether of not a many relation context is required to load.</param>
        protected void SetupContextDataLoading(Func<Guid, EntityQuery<TBase>> entityQuery, ContextRelationshipFilter[] contextRelationshipFilters, ContextLoadingType contextType = ContextLoadingType.RequiredContext)
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

                //Loads the VirtualItemCount any time a many relation context changes
                var loadVirtualItemCount = (from manyRelation in ManyRelationships
                                            let method = typeof(ContextManager).GetMethod("GetContextObservable")
                                            let generic = method.MakeGenericMethod(new[] { manyRelation })
                                            let contextObservable = (IObservable<object>)generic.Invoke(ContextManager, null)
                                            select contextObservable.DistinctUntilChanged())
                                            .Merge()
                    //TODO replace Merge with commented out below to prevent loads when there is no context
                    //.CombineLatest()
                    ////If there is no required context, always load
                    ////Otherwise make sure one of the contexts is not null
                    //.Where(contexts => contextType == ContextType.NoRequiredContext || contexts.Any(c => c != null))
                                            .Throttle(TimeSpan.FromMilliseconds(100)).ObserveOnDispatcher().AsGeneric();

                QueryableCollectionView = new ExtendedVirtualQueryableCollectionView<TBase, TEntity>(DomainContext,
                    () =>
                    {
                        //If a context is required for loading, check there is one
                        if (contextType == ContextLoadingType.RequiredContext)
                        {
                            var contexts = (from manyRelation in ManyRelationships
                                            let method = typeof(ContextManager).GetMethod("GetContext")
                                            let generic = method.MakeGenericMethod(new[] { manyRelation })
                                            let context = generic.Invoke(ContextManager, null)
                                            select context);

                            if (contexts.All(c => c == null))
                                return null;
                        }
                        return entityQuery(roleId);
                    }, loadVirtualItemCount);

                _contextRelationshipFiltersSubscription = AddContextRelationshipFiltersHelper(contextRelationshipFilters, QueryableCollectionView, ContextManager);

                //Select the first loaded entity after the virtual item count is loaded
                _selectFirstEntitySubscription = ExtendedVirtualQueryableCollectionView.FirstItemsLoadedAfterUpdated
                    .ObserveOnDispatcher().Subscribe(loadedEntities => SelectedEntity = loadedEntities.FirstOrDefault());

                //Subscribe the loading subject to when the count is loading
                ExtendedVirtualQueryableCollectionView.LoadingVirtualItemCountObservable.Subscribe(IsLoadingSubject);
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
            if (contextRelationshipFilters == null)
                return null;

            //Build an array of FilterDescriptorObservables from the related types whenever the GetContextObservable pushes a new context
            var filterDescriptorObservableChanged =
                (from contextRelationshipFilter in contextRelationshipFilters
                 let method = typeof(ContextManager).GetMethod("GetContextObservable")
                 let generic = method.MakeGenericMethod(new[] { contextRelationshipFilter.RelatedContextType })
                 let contextObservable = (IObservable<object>)generic.Invoke(contextManager, null)
                 select contextObservable.DistinctUntilChanged().ObserveOnDispatcher()
                     .Select(context =>
                     {
                         var filterDescriptor = context == null
                                                    ? null
                                                    : new FilterDescriptor(contextRelationshipFilter.EntityMember,
                                                          FilterOperator.IsEqualTo,
                                                          contextRelationshipFilter.FilterValueGenerator(context));

                         return new Tuple<ContextRelationshipFilter, FilterDescriptor>(contextRelationshipFilter, filterDescriptor);
                     })).Merge();

            //Add, remove, or update the context relationship filters whenever their contexts change
            return filterDescriptorObservableChanged.ObserveOnDispatcher().Subscribe(tple =>
            {
                //Try to find an existing related filter descriptor to the ContextRelationshipFilter.EntityMember for use below
                var relatedFilterDescriptor = queryableCollectionView.FilterDescriptors.
                    OfType<FilterDescriptor>().FirstOrDefault(fd => fd.Member == tple.Item1.EntityMember);

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
        protected void SetupTopEntityDataLoading(Func<Guid, EntityQuery<TBase>> entityQuery)
        {
            //Whenever the RoleId updates
            //update the VirtualQueryableCollectionView
            ContextManager.RoleIdObservable.ObserveOnDispatcher().Subscribe(roleId =>
            {
                //Dispose the last QueryableCollectionView
                if (QueryableCollectionView != null)
                    QueryableCollectionView.Dispose();

                //Load the VirtualItemCount immediately after creation (and automatically whenever a filter or sort changes)
                var loadVirtualItemCount = new BehaviorSubject<bool>(true);

                QueryableCollectionView = new ExtendedVirtualQueryableCollectionView<TBase, TEntity>(DomainContext, () => entityQuery(roleId), loadVirtualItemCount);

                //Subscribe the loading subject to when the count is loading
                ExtendedVirtualQueryableCollectionView.LoadingVirtualItemCountObservable.Subscribe(IsLoadingSubject);
            });
        }

        private readonly Subject<bool> _cancelLastSearchSubject = new Subject<bool>();
        /// <summary>
        /// Sets up the search suggestions for an AutoCompleteBox.
        /// </summary>
        /// <param name="autoCompleteBox">The autocomplete box.</param>
        /// <param name="searchQueryFunction">A function that returns the search EntityQuery.</param>
        protected void SearchSuggestionsHelper(AutoCompleteBox autoCompleteBox, Func<EntityQuery<TBase>> searchQueryFunction)
        {
            Manager.CoreDomainContext.LoadAsync(searchQueryFunction().Take(10), _cancelLastSearchSubject)
                .ContinueWith(task =>
                {
                    _cancelLastSearchSubject.OnNext(true);

                    if (task.IsCanceled)
                        return;

                    //TODO: Filter the Entities by the CustomComparer and the Destination ItemSource

                    autoCompleteBox.ItemsSource = task.Result;
                    autoCompleteBox.PopulateComplete();
                }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        #endregion

        #region Overriden Properties

        /// <summary>
        /// The method called for the AddCommand to create an entity. Defaults to DomainCollectionView.AddNew()
        /// </summary>
        /// <param name="commandParameter">The command parameter.</param>
        /// <returns>The entity to add.</returns>
        protected override TBase AddNewEntity(object commandParameter)
        {
            var newEntity = (TBase)this.QueryableCollectionView.AddNew();

            //Add the entity to the EntitySet so it is tracked by the DomainContext
            DomainContext.EntityContainer.GetEntitySet(typeof(TBase)).Add(newEntity);

            //Must call this or else adds will not show up in RadGridView
            this.RaisePropertyChanged("QueryableCollectionView");

            DataManager.EnqueueSubmitOperation(submitOperation =>
                  Rxx3.RunDelayed(TimeSpan.FromSeconds(1), () => SelectedEntity = newEntity));

            return newEntity;
        }

        /// <summary>
        /// The logic to delete an entity.
        /// </summary>
        /// <param name="entityToDelete">The entity to delete.</param>
        public override void DeleteEntity(TBase entityToDelete)
        {
            //Remove the entity from the QueryableCollectionView
            if (QueryableCollectionView != null)
                QueryableCollectionView.Remove(entityToDelete);

            //Remove the entity from the EntitySet (so it is deleted by the DomainContext)
            DomainContext.EntityContainer.GetEntitySet(typeof(TBase)).Remove(entityToDelete);

            base.DeleteEntity(entityToDelete);
        }

        protected override void OnSave(SubmitOperation submitOperation)
        {
            base.OnSave(submitOperation);

            //TODO Undo when fixing non-saved added entities
            if (ExtendedVirtualQueryableCollectionView != null)
                ExtendedVirtualQueryableCollectionView.UpdateVirtualItemCount();
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
    /// Describes the QueryableCollectionView's load behavior
    /// </summary>
    public enum ContextLoadingType
    {
        /// <summary>
        /// Load only when part of a Context
        /// </summary>
        RequiredContext,
        /// <summary>
        /// Load even when there is no Context
        /// </summary>
        NoRequiredContext
    }

    /// <summary>
    /// Defines the relationship between an entity and its context/
    /// </summary>
    public class ContextRelationshipFilter
    {
        /// <summary>
        /// The type of related entity context. Ex. Vehicle
        /// </summary>
        public Type RelatedContextType { get; private set; }

        /// <summary>
        /// The Member of the current entity related to the context. Ex. VehicleMaintenance's "VehicleId"
        /// </summary>
        public String EntityMember { get; private set; }

        /// <summary>
        /// A function when given the current related entity context, returns the value of the context.
        /// Ex. FilterValueGenerator(someVehicle) = abcd-1324-asfa-fegeg (someVehicle.Id)
        /// </summary>
        public Func<object, object> FilterValueGenerator { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextRelationshipFilter"/> class.
        /// </summary>
        /// <param name="entityMember">The Member of the current entity related to the context. Ex. VehicleMaintenance's "VehicleId"</param>
        /// <param name="relatedContextType">The type of related entity context. Ex. Vehicle</param>
        /// <param name="filterValueGenerator">A function when given the current related entity context, returns the value of the context</param>
        public ContextRelationshipFilter(String entityMember, Type relatedContextType, Func<object, object> filterValueGenerator)
        {
            EntityMember = entityMember;
            RelatedContextType = relatedContextType;
            FilterValueGenerator = filterValueGenerator;
        }
    }
}

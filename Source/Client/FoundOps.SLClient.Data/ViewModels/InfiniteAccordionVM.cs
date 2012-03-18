using System.Collections.Generic;
using System.Linq;
using FoundOps.Common.Silverlight.UI.Interfaces;
using FoundOps.Common.Silverlight.UI.Controls.InfiniteAccordion;
using FoundOps.SLClient.Data.Services;
using GalaSoft.MvvmLight.Command;
using ReactiveUI;
using System;
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

        /// <summary>
        /// Sets up data loading for context based entities.
        /// </summary>
        /// <param name="entityQuery">The entity query.</param>
        /// <param name="contextRelationshipFilters">The related types and their property values on the current type. Ex. Vehicle, VehicleId, Vehicle.Id</param>
        /// <param name="forceLoadRelatedTypesContext">if set to <c>true</c> [force load when in a related types context.</param>
        protected void SetupContextDataLoading(Func<Guid, EntityQuery<TEntity>> entityQuery, ContextRelationshipFilter[] contextRelationshipFilters, bool forceLoadRelatedTypesContext = false)
        {
           var disposeObservable = new Subject<bool>();

            //Whenever the RoleId updates, update the VirtualQueryableCollectionView
            ContextManager.RoleIdObservable.ObserveOnDispatcher().Subscribe(roleId =>
            {
                //Dispose the last VQCV subscriptions
                disposeObservable.OnNext(true);

                var result = DataManager.CreateContextBasedVQCV(() => entityQuery(roleId), disposeObservable, contextRelationshipFilters.Select(crf => crf.RelatedContextType), contextRelationshipFilters, forceLoadRelatedTypesContext,
                    loadedEntities => { SelectedEntity = loadedEntities.FirstOrDefault(); });

                QueryableCollectionView = result.VQCV;

                //Subscribe the loading subject to when the count is loading
                result.CountLoading.Subscribe(IsLoadingSubject);
            });
        }

        /// <summary>
        /// Sets up data loading for entities without any context.
        /// </summary>
        /// <param name="entityQuery">An action which is passed the role id, and should return the entity query to load data with.</param>
        protected void SetupTopEntityDataLoading(Func<Guid, EntityQuery<TEntity>> entityQuery)
        {
            var disposeObservable = new Subject<bool>();
            //Whenever the RoleId updates
            //update the VirtualQueryableCollectionView
            ContextManager.RoleIdObservable.ObserveOnDispatcher().Subscribe(roleId =>
            {
                //Dispose the last VQCV subscriptions
                disposeObservable.OnNext(true);

                var result = DataManager.CreateContextBasedVQCV(() => entityQuery(roleId), disposeObservable);

                //Subscribe the loading subject to when the count is loading
                result.CountLoading.Subscribe(IsLoadingSubject);

                QueryableCollectionView = result.VQCV;
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
}

using FoundOps.Common.Silverlight.UI.Interfaces;
using FoundOps.Common.Tools;
using FoundOps.Common.Silverlight.UI.Controls.InfiniteAccordion;
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

        #endregion

        protected InfiniteAccordionVM(bool preventChangingSelectionWhenChanges = true)
            : base(preventChangingSelectionWhenChanges)
        {
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

                var initialQuery = entityQuery(roleId);
                var result = DataManager.CreateContextBasedVQCV(initialQuery, disposeObservable);

                //Subscribe the loading subject to the LoadingAfterFilterChange observable
                result.LoadingAfterFilterChange.Subscribe(IsLoadingSubject);
                QueryableCollectionView = result.VQCV;
            });
        }

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

        #region Implementation of IContextAware

        /// <summary>
        /// Used by the Filter to determine if the current entity should be part of the view.
        /// This is generally relying on Context.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="isNew">if set to <c>true</c> [is new].</param>
        /// <returns></returns>
        protected virtual bool EntityIsPartOfView(TEntity entity, bool isNew) { return true; }

        #endregion

        #endregion
    }
}

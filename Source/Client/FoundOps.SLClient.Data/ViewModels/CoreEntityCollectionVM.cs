using System.Collections.Generic;
using FoundOps.Common.Silverlight.MVVM.VMs;
using FoundOps.Common.Silverlight.Services;
using FoundOps.Common.Silverlight.UI.Interfaces;
using Microsoft.Windows.Data.DomainServices;
using ReactiveUI;
using ReactiveUI.Xaml;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.ServiceModel.DomainServices.Client;
using System.Windows;
using Telerik.Windows.Data;

namespace FoundOps.SLClient.Data.ViewModels
{
    /// <summary>
    /// A view model which encapsulates some standard collection logic/properties for an entity type.
    /// Properties: DomainCollectionView, SelectedEntity
    /// Commands: Add, Delete, Save, Discard Changes
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public abstract class CoreEntityCollectionVM<TEntity> : CoreEntityVM, IPreventNavigationFrom, IAddDeleteCommands
        where TEntity : Entity
    {
        #region Public

        #region Add Delete

        private readonly ObservableAsPropertyHelper<bool> _canAdd;
        /// <summary>
        /// Gets a value indicating whether this instance can add.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance can add; otherwise, <c>false</c>.
        /// </value>
        public bool CanAdd { get { return _canAdd.Value; } }

        /// <summary>
        /// A command for adding an entity
        /// </summary>
        public IReactiveCommand AddCommand { get; protected set; }

        private readonly ObservableAsPropertyHelper<bool> _canDelete;
        /// <summary>
        /// Gets a value indicating whether this instance can delete.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can delete; otherwise, <c>false</c>.
        /// </value>
        public bool CanDelete { get { return _canDelete.Value; } }

        /// <summary>
        /// A command for deleting an entity
        /// </summary>
        public IReactiveCommand DeleteCommand { get; protected set; }

        #endregion

        /// <summary>
        /// The CollectionView observable.
        /// </summary>
        protected readonly Subject<IEditableCollectionView> CollectionViewObservable = new Subject<IEditableCollectionView>();

        private readonly ObservableAsPropertyHelper<IEditableCollectionView> _collectionView;
        /// <summary>
        /// Gets the editable collection view.
        /// </summary>
        public IEditableCollectionView EditableCollectionView { get { return _collectionView.Value; } }

        /// <summary>
        /// Gets the collection view.
        /// </summary>
        public ICollectionView CollectionView { get { return (ICollectionView) _collectionView.Value; } }

        /// <summary>
        /// Gets the casted collection.
        /// </summary>
        public IEnumerable<TEntity> Collection { get { return ((ICollectionView)_collectionView.Value).Cast<TEntity>(); } }

        #region Selected Entity

        readonly Subject<TEntity> _selectedEntityObservable = new Subject<TEntity>(); //The backing SelectedEntityObservable
        /// <summary>
        /// Gets the selected entity observable.
        /// </summary>
        public IObservable<TEntity> SelectedEntityObservable { get { return _selectedEntityObservable.DistinctUntilChanged(); } }

        //Used to disable setting SelectedEntity to prevent bindings from unwanted changes
        private bool _disableSelectedEntity;

        /// <summary>
        /// Used to disable SaveDiscardCancel
        /// </summary>
        public bool DisableSaveDiscardCancel;

        private TEntity _oldValue;
        readonly ObservableAsPropertyHelper<TEntity> _selectedEntity;
        /// <summary>
        /// Gets or sets the selected entity.
        /// </summary>
        public TEntity SelectedEntity
        {
            get { return _selectedEntity.Value; }
            set
            {
                if (_disableSelectedEntity) return;

                _oldValue = SelectedEntity;

                if (SelectedEntity == value) return;

                if (!BeforeSelectedEntityChanges(_selectedEntity.Value, value))
                    return;

                if (!DisableSaveDiscardCancel && _preventChangingSelectionWhenChanges)
                {
                    //If there are changes
                    if (value != null && value != _oldValue && SelectedEntity != null && DomainContextHasRelatedChanges())
                    {
                        //Ask the user to: Save or Discard their changes before moving on, or Cancel to stay on the changed entity

                        //set the new value
                        Action selectNewValue = () =>
                        {
                            DisableSaveDiscardCancel = true;
                            SelectedEntity = value;
                            DisableSaveDiscardCancel = false;
                        };

                        Action customDiscardAction = () => this.DiscardCommand.Execute(null);
                        //Move back to the old value
                        Action moveToOldValue = () => ((ICollectionView)EditableCollectionView).MoveCurrentTo(SelectedEntity);

                        //Call SaveDiscardCancelPrompt
                        DataManager.SaveDiscardCancelPrompt(afterSave: selectNewValue, customDiscardAction: customDiscardAction, afterDiscard: selectNewValue, afterCancel: moveToOldValue);

                        return;
                    }
                }

                _selectedEntityObservable.OnNext(value);
            }
        }

        #endregion

        #endregion

        #region Protected

        /// <summary>
        /// An entry to the CanAddSubject to be updated by extended classes. NOTE: Make sure to merge with existing when extending.
        /// </summary>
        protected readonly BehaviorSubject<bool> CanAddSubject = new BehaviorSubject<bool>(true);

        /// <summary>
        /// An entry to the CanDeleteSubject to be updated by extended classes. NOTE: Make sure to merge with existing when extending.
        /// </summary>
        protected readonly BehaviorSubject<bool> CanDeleteSubject = new BehaviorSubject<bool>(true);

        protected bool IsAddingNew; //For filter

        /// <summary>
        /// An option to keep track of the last selected entity.
        /// Usually this prevents double code execution because bindings trigger SelectedEntity twice sometimes.
        /// In certain scenarios this will need to be set to false.
        /// </summary>
        protected bool KeepTrackLastSelectedEntity = true;

        #endregion

        //Locals

        private readonly bool _preventChangingSelectionWhenChanges;

        /// <summary>
        /// A base class for a type of Entity that has a collection and one selected item.
        /// Contains standard logic for saving, discarding changes, and adding and removing items.
        /// </summary>
        /// <param name="preventChangingSelectionWhenChanges">Whether or not to prevent changing the selected entity when the DomainContext has changes.</param>
        protected CoreEntityCollectionVM(bool preventChangingSelectionWhenChanges)
        {
            //Setup SelectedEntity property
            _selectedEntity = SelectedEntityObservable.ToProperty(this, x => x.SelectedEntity);

            SelectedEntityObservable.ObserveOnDispatcher().Subscribe(
                newSelectedEntity =>
                {
                    OnSelectedEntityChanged(_oldValue, newSelectedEntity);

                    if (EditableCollectionView != null)
                        ((ICollectionView)EditableCollectionView).MoveCurrentTo(newSelectedEntity);

                    if (newSelectedEntity == null) return;
                    IsAddingNew = false;
                });

            //Setup CollectionView property, set an initial ObservableCollection (many VMs expect the CollectionView source to be an ObservableCollection)
            _collectionView = CollectionViewObservable.ToProperty(this, x => x.EditableCollectionView, new QueryableCollectionView(new ObservableCollection<TEntity>()));

            _preventChangingSelectionWhenChanges = preventChangingSelectionWhenChanges;

            #region Register Commands

            //Setup CanAdd property
            _canAdd = CanAddSubject.ToProperty(this, x => x.CanAdd);

            //can add when: not loading && OwnerAccount != null && DomainCollectionView != null && CanAdd
            var canAddCommand = this.WhenAny(x => x.IsLoading, x => x.ContextManager.OwnerAccount, x => x.EditableCollectionView, x => x.CanAdd,
                                             (isLoading, ownerAccount, domainCollectionView, canAdd) =>
                                                 !isLoading.Value && ownerAccount.Value != null && domainCollectionView.Value != null && canAdd.Value);

            AddCommand = new ReactiveCommand(canAddCommand);

            AddCommand.Throttle(TimeSpan.FromMilliseconds(500)).ObserveOnDispatcher()
                .Subscribe(x =>
            {
                var canAdd = BeforeAdd();
                if (!canAdd)
                    return;
                _disableSelectedEntity = true;
                var newEntity = AddNewEntity(x);
                if (newEntity == null)
                    return;
                IsAddingNew = true;
                OnAddEntity(newEntity);
                EntityAdded();
                _disableSelectedEntity = false;
                SelectedEntity = newEntity;
            });

            //Setup CanDelete property
            _canDelete = CanDeleteSubject.ToProperty(this, x => x.CanDelete);

            //can delete when: not loading && SelectedEntity != null && DomainCollectionView != null && CanDelete
            var canDeleteCommand = this.WhenAny(x => x.IsLoading, x => x.SelectedEntity, x => x.EditableCollectionView, x => x.CanDelete,
                                                (isLoading, selectedEntity, domainCollectionView, canDelete) =>
                                               !isLoading.Value && selectedEntity.Value != null && domainCollectionView.Value != null && canDelete.Value);

            DeleteCommand = new ReactiveCommand(canDeleteCommand);

            DeleteCommand.Throttle(TimeSpan.FromMilliseconds(500)).ObserveOnDispatcher().Subscribe(x =>
                CheckDelete(canDelete =>
                {
                    if (!canDelete) return;

                    //Disable SaveDiscardCancel until saved
                    DisableSaveDiscardCancel = true;

                    var selectedEntity = this.SelectedEntity;
                    this.SelectedEntity = null;

                    DeleteEntity(selectedEntity);
                    OnDeleteEntity(selectedEntity);

                    SaveCommand.Execute(null);
                }));

            #endregion
        }

        #region Logic

        #region Protected

        /// <summary>
        /// Sets the SelectedEntity directly without executing the SelectedEntity Logic (SelectionChanged, etc).
        /// </summary>
        /// <param name="entity">The entity to set as selected.</param>
        protected void DirectSetSelectedEntity(TEntity entity)
        {
            _selectedEntityObservable.OnNext(entity);
        }

        /// <summary>
        /// Sets up the main query for loading Data. Will setup the IsLoadingObservable and the DomainCollectionView.
        /// </summary>
        /// <param name="queryKey">The query key.</param>
        /// <param name="action">Optional: An action to perform after loading the data.</param>
        /// <param name="sortBy">Optional: a SortDescription to add</param>
        /// <param name="selectFirstEntity">Optional: Whether or not to select the first entity. Defaults to true.</param>
        /// <returns>The EntityList Observable</returns>
        protected IObservable<EntityList<TEntity>> SetupMainQuery(object queryKey, Action<EntityList<TEntity>> action = null, string sortBy = null, bool selectFirstEntity = true)
        {
            IsLoadingObservable = DataManager.Subscribe<TEntity>(queryKey, ObservationState, entities =>
            {
                //Setup the DomainCollectionView
                var domainCollectionView = DomainCollectionViewFactory<TEntity>.GetDomainCollectionView(entities);
                if (sortBy != null)
                    domainCollectionView.SortDescriptions.Add(new SortDescription(sortBy, ListSortDirection.Ascending));

                CollectionViewObservable.OnNext(domainCollectionView);

                if (action != null)
                    action(entities);
            });

            //Set the SelectedEntity to the first entity (or null if there are none)
            if (selectFirstEntity)
                DataManager.GetEntityListObservable<TEntity>(queryKey).Throttle(TimeSpan.FromMilliseconds(300)) //Delay .3 second to allow UI to catchup
                    .ObserveOnDispatcher().Subscribe(_ =>
                    {
                        SelectedEntity = ((ICollectionView)EditableCollectionView).Cast<TEntity>().FirstOrDefault();
                    });

            return DataManager.GetEntityListObservable<TEntity>(queryKey);
        }

        #region Methods to Override

        #region Methods w Initial Logic

        /// <summary>
        /// The method called for the AddCommand to create an entity. Defaults to DomainCollectionView.AddNew()
        /// </summary>
        /// <param name="commandParameter">The command parameter.</param>
        /// <returns>The entity to add.</returns>
        protected virtual TEntity AddNewEntity(object commandParameter)
        {
            return (TEntity)this.EditableCollectionView.AddNew();
        }

        /// <summary>
        /// Checks if you can delete.
        /// </summary>
        /// <param name="checkCompleted">Call this action with the result when the check is completed.</param>
        protected virtual void CheckDelete(Action<bool> checkCompleted)
        {
            checkCompleted(MessageBox.Show("Are you sure you want to delete?", "Delete?", MessageBoxButton.OKCancel) == MessageBoxResult.OK);
        }

        /// <summary>
        /// The logic to delete an entity. Returns true if it can delete.
        /// </summary>
        public virtual void DeleteEntity(TEntity entityToDelete)
        {
            this.EditableCollectionView.Remove(entityToDelete);
        }

        /// <summary>
        /// Whether or not the DomainContext has related changes to this view model.
        /// This is used to prevent navigation.
        /// </summary>
        protected virtual bool DomainContextHasRelatedChanges()
        {
            return
                (Context.EntityContainer.GetChanges().AddedEntities.Contains(SelectedEntity) ||
                 Context.EntityContainer.GetChanges().ModifiedEntities.Count > 0 ||
                 Context.EntityContainer.GetChanges().RemovedEntities.Count > 0);
        }

        // IPreventNavigationFrom

        /// <summary>
        /// Determines whether this instance [can be navigated from]. If it can, then execute <param name="navigate"/> 
        /// </summary>
        /// <param name="navigate">The navigate action to execute, if this CanNavigateFrom.</param>
        /// <returns>
        ///   <c>true</c> if this instance [can navigate from] the specified navigate; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool CanNavigateFrom(Action navigate)
        {
            Action navigateAction = () =>
            {
                navigate();
                OnNavigateFrom();
            };

            if (DomainContextHasRelatedChanges())
            {
                //Before saving, navigate

                Action customDiscardAction = () => this.DiscardCommand.Execute(null);

                //Call SaveDiscardCancelPrompt
                //Saving: before saving navigate,
                //Discarding: replace discard with DiscardCommand execution, after discard navigate
                DataManager.SaveDiscardCancelPrompt(navigateAction, customDiscardAction: customDiscardAction, afterDiscard: navigateAction);

                return false;
            }

            navigateAction();
            return true;
        }

        protected override void OnSave(SubmitOperation submitOperation)
        {
            DisableSaveDiscardCancel = false;
        }

        #endregion //w Initial Logic

        #region Empty methods

        #region Before something happened

        /// <summary>
        /// Before the selected entity changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        /// <returns></returns>
        protected virtual bool BeforeSelectedEntityChanges(TEntity oldValue, TEntity newValue) { return true; }

        /// <summary>
        /// Called before an entity is added.
        /// </summary>
        /// <returns>Whether to continue adding a new entity.</returns>
        protected virtual bool BeforeAdd()
        {
            return true;
        }

        #endregion

        #region After something happened

        /// <summary>
        /// Called after an Entity is added
        /// </summary>
        protected virtual void EntityAdded() { }
        /// <summary>
        /// Called when an Entity is being added
        /// </summary>
        /// <param name="newEntity">The new entity.</param>
        protected virtual void OnAddEntity(TEntity newEntity) { }

        /// <summary>
        /// Called when an Entity is being deleted
        /// </summary>
        /// <param name="entityToDelete">The entity to delete.</param>
        protected virtual void OnDeleteEntity(TEntity entityToDelete) { }

        /// <summary>
        /// Logic to execute after navigating from this ViewModel
        /// </summary>
        public virtual void OnNavigateFrom() { }

        /// <summary>
        /// Called when the selected entity changed.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected virtual void OnSelectedEntityChanged(TEntity oldValue, TEntity newValue) { }

        #endregion //After something happened

        #endregion //Empty methods

        #endregion //Methods to Override

        #endregion //Protected

        #endregion //Logic
    }
}

using FoundOps.Common.Silverlight.MVVM.VMs;
using FoundOps.Common.Silverlight.Tools.ExtensionMethods;
using FoundOps.Common.Silverlight.UI.Interfaces;
using FoundOps.Common.Tools;
using ReactiveUI;
using ReactiveUI.Xaml;
using System;
using System.Collections.Generic;
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
        /// The View observable.
        /// </summary>
        protected readonly Subject<IEditableCollectionView> ViewObservable = new Subject<IEditableCollectionView>();
        private readonly ObservableAsPropertyHelper<IEditableCollectionView> _collectionView;
        /// <summary>
        /// Gets the editable collection view.
        /// </summary>
        public IEditableCollectionView EditableCollectionView { get { return _collectionView.Value; } }

        /// <summary>
        /// The CollectionView observable.
        /// </summary>
        public IObservable<ICollectionView> CollectionViewObservable
        {
            get
            {
                return ViewObservable.Cast<ICollectionView>();
            }
        }

        /// <summary>
        /// Gets the collection view.
        /// </summary>
        public ICollectionView CollectionView { get { return (ICollectionView)_collectionView.Value; } }

        /// <summary>
        /// Gets the casted collection.
        /// </summary>
        public IEnumerable<TEntity> Collection { get { return ((ICollectionView)_collectionView.Value).Cast<TEntity>(); } }

        /// <summary>
        /// Gets the casted source collection.
        /// </summary>
        public IEnumerable<TEntity> SourceCollection
        {
            get
            {
                if (_collectionView.Value == null) return null;
                return ((ICollectionView)_collectionView.Value).SourceCollection as IEnumerable<TEntity>;
            }
        }

        /// <summary>
        /// An observable that pushes whenever the CollectionView is changed or set.
        /// NOTE: This will only work if the SourceCollection is an ObservableCollection.
        /// </summary>
        public IObservable<bool> SourceCollectionChangedOrSet
        {
            get
            {
                return ViewObservable.DistinctUntilChanged().WhereNotNull()
                    .Select(cv => ((ICollectionView)cv).SourceCollection as ObservableCollection<TEntity>).WhereNotNull()
                    .FromCollectionChangedOrSet().AsGeneric();
            }
        }

        #region Selected Entity

        readonly Subject<TEntity> _selectedEntityObservable = new Subject<TEntity>(); //The backing SelectedEntityObservable
        /// <summary>
        /// Gets the selected entity observable.
        /// </summary>
        public IObservable<TEntity> SelectedEntityObservable { get { return _selectedEntityObservable.DistinctUntilChanged(); } }

        //Used to disable setting SelectedEntity to prevent bindings from unwanted changes
        private bool _disableSelectedEntity;

        /// <summary>
        /// Used to disable the selected entity changed SaveDiscardCancel prompt.
        /// </summary>
        public bool DisableSelectedEntitySaveDiscardCancel;

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
                if (_preventNullSelection && value == null)
                    return;

                if (_disableSelectedEntity) return;

                _oldValue = SelectedEntity;

                if (SelectedEntity == value) return;

                if (!BeforeSelectedEntityChanges(_selectedEntity.Value, value))
                    return;

                if (!DisableSelectedEntitySaveDiscardCancel && _preventChangingSelectionWhenChanges)
                {
                    //If there are changes
                    if (value != null && value != _oldValue && SelectedEntity != null && DomainContextHasRelatedChanges())
                    {
                        //Ask the user to: Save or Discard their changes before moving on, or Cancel to stay on the changed entity

                        //set the new value
                        Action selectNewValue = () =>
                        {
                            DisableSelectedEntitySaveDiscardCancel = true;
                            SelectedEntity = value;
                            DisableSelectedEntitySaveDiscardCancel = false;
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
        private readonly bool _preventNullSelection;

        /// <summary>
        /// A base class for a type of Entity that has a collection and one selected item.
        /// Contains standard logic for saving, discarding changes, and adding and removing items.
        /// </summary>
        /// <param name="preventChangingSelectionWhenChanges">Whether or not to prevent changing the selected entity when the DomainContext has changes.</param>
        /// <param name="preventNullSelection">Do not allow the SelectedEntity to become null</param>
        /// <param name="initializeDefaultCollectionView">Initialize a default QueryableCollectionView for the CollectionView property.</param>
        protected CoreEntityCollectionVM(bool preventChangingSelectionWhenChanges = true, bool preventNullSelection = false, bool initializeDefaultCollectionView = true)
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
                    Commit();
                    IsAddingNew = false;
                });

            //Setup CollectionView property, set an initial ObservableCollection (many VMs expect the CollectionView source to be an ObservableCollection)
            _collectionView = initializeDefaultCollectionView
                                  ? ViewObservable.ToProperty(this, x => x.EditableCollectionView, new QueryableCollectionView(new ObservableCollection<TEntity>()))
                                  : ViewObservable.ToProperty(this, x => x.EditableCollectionView);

            ViewObservable.Cast<ICollectionView>().ToProperty(this, x => x.CollectionView);

            _preventChangingSelectionWhenChanges = preventChangingSelectionWhenChanges;
            _preventNullSelection = preventNullSelection;

            #region Register Commands

            //Setup CanAdd property
            _canAdd = CanAddSubject.ToProperty(this, x => x.CanAdd);

            //can add when: not loading && OwnerAccount != null && DomainCollectionView != null && CanAdd
            var canAddCommand = this.WhenAny(x => x.IsLoading, x => x.ContextManager.OwnerAccount, x => x.EditableCollectionView, x => x.CanAdd,
                                             (isLoading, ownerAccount, collectionView, canAdd) =>
                                                 !isLoading.Value && ownerAccount.Value != null && collectionView.Value != null && canAdd.Value);

            AddCommand = new ReactiveCommand(canAddCommand);

            AddCommand.Throttle(TimeSpan.FromMilliseconds(500)).ObserveOnDispatcher().Subscribe(x =>
                CheckAdd(canAdd =>
                {
                    if (!canAdd) return;

                    _disableSelectedEntity = true;
                    var newEntity = AddNewEntity(x);
                    if (newEntity == null)
                        return;
                    IsAddingNew = true;
                    OnAddEntity(newEntity);
                    Commit();
                    EntityAdded();
                    _disableSelectedEntity = false;
                    SelectedEntity = newEntity;
                }));

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
                    DisableSelectedEntitySaveDiscardCancel = true;

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
        /// Sets up data loading of the SelectedEntity's details.
        /// TEntity must implement ILoadDetails.
        /// </summary>
        /// <param name="entityQuery">An action which is passed the selected entity and should return the entity query to load data with.</param>
        protected void SetupDetailsLoading(Func<TEntity, EntityQuery<TEntity>> entityQuery)
        {
            SetupDetailsLoading(entityQuery, SelectedEntityObservable);
        }

        /// <summary>
        /// Sets up data loading of an entities details when it is pushed. It will cancel the last details load when a new item is pushed.
        /// T must implement ILoadDetails.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="entityQuery">An action which is passed the selected entity and should return the entity query to load data with.</param>
        /// <param name="entitiesObservable">An observable of T entities to load details from.</param>
        protected void SetupDetailsLoading<T>(Func<T, EntityQuery<T>> entityQuery, IObservable<T> entitiesObservable) where T : Entity
        {
            LoadOperation<T> detailsLoadOperation = null;
            //Whenever the entity changes
            //a) cancel the last load
            //b) load the details
            entitiesObservable.Where(se => se != null).Subscribe(selectedEntity =>
            {
                //a) cancel the last load
                if (detailsLoadOperation != null && detailsLoadOperation.CanCancel)
                    detailsLoadOperation.Cancel();

                //Do not try to load details for an entity that does not exist yet.
                if (selectedEntity.EntityState == EntityState.New)
                {
                    ((ILoadDetails)selectedEntity).DetailsLoaded = true;
                    return;
                }

                var query = entityQuery(selectedEntity);
                if (query == null)
                    return;

                //b) load the details
                ((ILoadDetails)selectedEntity).DetailsLoaded = false;
                detailsLoadOperation = DomainContext.Load(query, loadOp => ((ILoadDetails)selectedEntity).DetailsLoaded = true, null);
            });
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
        /// <param name="deleteItem">Call this action with the result when the check is completed to delete the item.</param>
        protected virtual void CheckDelete(Action<bool> deleteItem)
        {
            deleteItem(MessageBox.Show("Are you sure you want to delete?", "Delete?", MessageBoxButton.OKCancel) == MessageBoxResult.OK);
        }

        /// <summary>
        /// The logic to delete an entity.
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
                (DomainContext.EntityContainer.GetChanges().AddedEntities.Contains(SelectedEntity) ||
                 DomainContext.EntityContainer.GetChanges().ModifiedEntities.Count > 0 ||
                 DomainContext.EntityContainer.GetChanges().RemovedEntities.Count > 0);
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
            DisableSelectedEntitySaveDiscardCancel = false;
        }

        #endregion //w Initial Logic

        #region Empty methods

        #region Before something happened

        /// <summary>
        /// Checks if you can add.
        /// </summary>
        /// <param name="addItem">Call this action with the result when the check is completed to add the item.</param>
        protected virtual void CheckAdd(Action<bool> addItem)
        {
            addItem(true);
        }

        /// <summary>
        /// Before the selected entity changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        /// <returns></returns>
        protected virtual bool BeforeSelectedEntityChanges(TEntity oldValue, TEntity newValue) { return true; }

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

        #region Private Methods

        protected void Commit()
        {
            //Must Commit and EndEdit to prevent error on Save or DiscardChanges
            if (this.EditableCollectionView == null)
                return;
            if (this.EditableCollectionView.IsAddingNew)
                IsAddingNew = true;

            this.EditableCollectionView.Commit();
            this.EditableCollectionView.CommitEdit();
            this.EditableCollectionView.CommitNew();

            if (SelectedEntity != null)
                ((IEditableObject)SelectedEntity).EndEdit();
        }

        #endregion //Private

        #endregion //Logic
    }
}

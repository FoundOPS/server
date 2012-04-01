using System.ComponentModel;
using FoundOps.Common.Silverlight.Services;
using FoundOps.Common.Silverlight.Tools.ExtensionMethods;
using FoundOps.Common.Tools;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.SLClient.Data.Services;
using FoundOps.SLClient.Data.ViewModels;
using MEFedMVVM.ViewModelLocator;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.ServiceModel.DomainServices.Client;
using System.Threading.Tasks;
using Telerik.Windows.Controls;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// Loads, displays (and generates) services based on context. Allows adding/deleting of services.
    /// </summary>
    [ExportViewModel("ServicesVM")]
    public class ServicesVM : InfiniteAccordionVM<ServiceHolder>
    {
        #region Properties

        #region Public

        /// <summary>
        /// The last loaded time.
        /// </summary>
        public DateTime LastLoad { get; private set; }

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ServicesVM"/> class.
        /// </summary>
        [ImportingConstructor]
        public ServicesVM()
            : base(null, true, false, false)
        {
            SetupDataLoading();
            //TODO Regenerate the Services when the current RecurringServiceContext.Repeat changes
            //recurringServiceContextObservable.WhereNotNull().SelectLatest(rs => rs.RepeatChangedObservable())
            //    .Subscribe(_ => LoadData);

            //Remove any non added new Services from the collection when RejectChanges is called
            DomainContext.ChangesRejected += (s, ra, rm) =>
            {
                if (SourceCollection as ObservableCollection<ServiceHolder> == null)
                    return;

                ((ObservableCollection<ServiceHolder>)SourceCollection).RemoveAll(sh => sh.ServiceIsNew && sh.Service.EntityState == EntityState.Detached);
            };

            //Can only delete if the Details are loaded (because it might need the RecurringService parent)
            SelectedEntityObservable.WhereNotNull().SelectLatest(se => se.FromPropertyChanged("DetailsLoaded").Select(_ => se.DetailsLoaded)).Subscribe(CanDeleteSubject);
        }

        #region Data Loading

        /// <summary>
        /// Whenever this is pushed, the last details load should cancel
        /// </summary>
        readonly Subject<bool> _cancelLastDetailsLoad = new Subject<bool>();
        private void SetupDataLoading()
        {
            //Load the ServiceHolders whenever
            //a) the RoleId changes and this is visible 
            //b) the ClosestContext changes
            //c) whenever this becomes the DetailsView (so there are enough)
            SetupExecuteObservable().AsGeneric()
                //Skip the first closest context because it is null
            .Merge(ContextManager.ClosestContextContextObservable(new[] { typeof(RecurringService), typeof(Client) }).Skip(1).AsGeneric())
            .Merge(this.ContextManager.CurrentContextProviderObservable.DistinctUntilChanged().Where(contextProvider => contextProvider == this).AsGeneric())
            .ObserveOnDispatcher()
            .Subscribe(roleId => LoadServiceHolders(ServiceHoldersLoad.InitialLoad));

            SelectedEntityObservable.Where(se => se != null).Subscribe(selectedEntity => selectedEntity.LoadDetails(_cancelLastDetailsLoad));
        }

        /// <summary>
        /// Whenever this is pushed, the last ServiceHolders load should cancel
        /// </summary>
        readonly Subject<bool> _cancelLastServiceHoldersLoad = new Subject<bool>();

        /// <summary>
        /// This is the first available service date for the current context.
        /// It will be set after an initial load or when moving backwards if the number of serviceholders 
        /// before the seed date is less than the minimumOccurrencesToLoad.
        /// </summary>
        private DateTime? _firstAvailableServiceDate;

        /// <summary>
        /// This is the last available service date for the current context.
        /// It will be set after an initial load or when moving forwards if the number of serviceholders 
        /// after the first date on or after the seedDate is less than the minimumOccurrencesToLoad.
        /// </summary>
        private DateTime? _lastAvailableServiceDate;

        //A switch for whether this can move backward or not. 
        //This is based off the last seed date and the _firstAvailableServiceDate.
        //Default to false until the initial load
        private bool CanMoveBackward
        {
            get
            {
                return
                    //If the collection has not been loaded or the _firstAvailableServiceDate is null
                    //the beginning is still unknown so we can move backwards
                   Collection == null || !_firstAvailableServiceDate.HasValue ||
                    //otherwise make sure the first loaded element > _firstAvailableServiceDate
                    (Collection.Any() && Collection.First().OccurDate > _firstAvailableServiceDate.Value);
            }
        }

        //A switch for whether this can move forward or not
        //Default to false until the initial load
        private bool CanMoveForward
        {
            get
            {
                return
                    //If the collection has not been loaded or the _lastAvailableServiceDate is null
                    //the beginning is still unknown so we can move backwards
                     Collection == null || !_lastAvailableServiceDate.HasValue ||
                    //otherwise make sure the last loaded element < _lastAvailableServiceDate
                      (Collection.Any() && Collection.Last().OccurDate < _lastAvailableServiceDate.Value);
            }
        }

        /// <summary>
        /// Loads the ServiceHolders based on the current context.
        /// </summary>
        /// <param name="loadType">The type of load to perform</param>
        /// <param name="completedCallback">Called when the load is completed.</param>
        private void LoadServiceHolders(ServiceHoldersLoad loadType, Action<IEnumerable<ServiceHolder>> completedCallback = null)
        {
            //Cancel the last service holders load if it is still running
            _cancelLastServiceHoldersLoad.OnNext(true);

            //Notify this is loading
            IsLoadingSubject.OnNext(true);

            #region Setup the query

            #region Setup seed date

            var seedDate = DateTime.Now.Date;
            switch (loadType)
            {
                //If this is moving backwards set the seed date to the earliest occur date of the collection
                case ServiceHoldersLoad.Backwards:
                    {
                        var earliestServiceHolder = Collection.FirstOrDefault();

                        if (earliestServiceHolder != null)
                            seedDate = earliestServiceHolder.OccurDate;
                    }
                    break;
                //If this is moving forwards set the seed date to the latest occur date of the collection
                case ServiceHoldersLoad.Forwards:
                    {
                        var latestServiceHolder = Collection.LastOrDefault();

                        if (latestServiceHolder != null)
                            seedDate = latestServiceHolder.OccurDate;
                    }
                    break;
                //If this is the initial load leave the seedDate as today
            }

            #endregion

            #region Setup contexts

            var recurringServiceContext = ContextManager.GetContext<RecurringService>();
            var clientContext = ContextManager.GetContext<Client>();

            //Setup the query's contexts
            Guid? recurringServiceContextId = null;
            Guid? clientContextId = null;

            //Try to get a recurring service context first because it is the most specific
            if (recurringServiceContext != null)
                recurringServiceContextId = recurringServiceContext.Id;
            else if (clientContext != null) //If there is none try to get a client context
                clientContextId = clientContext.Id;

            #endregion

            //This is not in details view load at least 100 service holders on the front/back
            //Otherwise load at least 10 on the front/back
            var minimumOccurrencesToLoad = IsInDetailsView ? 100 : 10;

            var query = this.DomainContext.GetServiceHoldersQuery(ContextManager.RoleId, clientContextId, recurringServiceContextId,
                seedDate, minimumOccurrencesToLoad, true, true);

            #endregion

            #region Load and handle the result

            Manager.CoreDomainContext.LoadAsync(query, _cancelLastServiceHoldersLoad)
            .ContinueWith(task =>
            {
                if (task.IsCanceled || !task.Result.Any())
                {
                    ViewObservable.OnNext(new DomainCollectionViewFactory<ServiceHolder>(new List<ServiceHolder>()).View);
                    IsLoadingSubject.OnNext(false);
                    return;
                }

                var earliest = task.Result.FirstOrDefault();
                var latest = task.Result.LastOrDefault();
                var closestBeforeSeed = task.Result.LastOrDefault(sh => sh.OccurDate < seedDate);
                var closestOnOrAfterSeed = task.Result.FirstOrDefault(sh => sh.OccurDate >= seedDate);

                #region Select the closest ServiceHolder for the query

                switch (loadType)
                {
                    //If the load was backwards
                    //Try to select the closest ServiceHolder before the seed date
                    //Otherwise try to select the closest ServiceHolder on or after the seed date
                    case ServiceHoldersLoad.Backwards:
                        SelectedEntity = closestBeforeSeed ?? closestOnOrAfterSeed;
                        break;
                    //If the load was an initial load
                    //Try to select the closest ServiceHolder on or after the seed date
                    //Otherwise try to select the closest ServiceHolder before the seed date 
                    case ServiceHoldersLoad.InitialLoad:
                        SelectedEntity = closestOnOrAfterSeed ?? closestBeforeSeed;
                        break;
                    //If the load was forwards
                    //Try to select the closest ServiceHolder after the seed date
                    //Otherwise try to select the closest ServiceHolder on or after the seed date
                    //Otherwise try to select the closest ServiceHolder before the seed date
                    case ServiceHoldersLoad.Forwards:
                        SelectedEntity = closestOnOrAfterSeed ?? closestBeforeSeed;
                        break;
                }

                #endregion

                var itemsBeforeSeed = task.Result.Count(sh => sh.OccurDate < seedDate);
                var itemsAfterClosestOnOrAfterSeed = task.Result.Count(sh => sh.OccurDate > seedDate);

                //Check the results against the minimumOccurrencesToLoad 
                //to determine if this can push further back or forward
                switch (loadType)
                {
                    //If the load was backwards check the number of services 
                    //before the seed date >= minimumOccurrencesToLoad
                    case ServiceHoldersLoad.Backwards:
                        if (itemsBeforeSeed < minimumOccurrencesToLoad && earliest != null)
                            _firstAvailableServiceDate = earliest.OccurDate;
                        else
                            _firstAvailableServiceDate = null;
                        break;
                    //If the load was an initial load
                    //Check the number of services before the seed date are >= minimumOccurrencesToLoad
                    //and check the number of services after the closestOnOrAfterSeed date >= minimumOccurrencesToLoad
                    case ServiceHoldersLoad.InitialLoad:
                        if (itemsBeforeSeed < minimumOccurrencesToLoad && earliest != null)
                            _firstAvailableServiceDate = earliest.OccurDate;
                        else
                            _firstAvailableServiceDate = null;
                        if (itemsAfterClosestOnOrAfterSeed < minimumOccurrencesToLoad && latest != null)
                            _lastAvailableServiceDate = latest.OccurDate;
                        else
                            _lastAvailableServiceDate = null;
                        break;
                    //If the load was forwards check the number of services 
                    //after the closestOnOrAfterSeed date >= minimumOccurrencesToLoad
                    case ServiceHoldersLoad.Forwards:
                        if (itemsAfterClosestOnOrAfterSeed < minimumOccurrencesToLoad && latest != null)
                            _lastAvailableServiceDate = latest.OccurDate;
                        else
                            _lastAvailableServiceDate = null;
                        break;
                }

                //Detach the ServiceHolders so they can be edited
                DataManager.DetachEntities(task.Result);

                #region Update the CollectionViewObservable with the new loaded ServiceHolders

                var loadedServiceHolders = task.Result.ToObservableCollection();

                ViewObservable.OnNext(new DomainCollectionViewFactory<ServiceHolder>(loadedServiceHolders).View);

                #endregion

                LastLoad = DateTime.Now;

                //Notify this has completed
                IsLoadingSubject.OnNext(false);
                if (completedCallback != null)
                    completedCallback(task.Result);
            }, TaskScheduler.FromCurrentSynchronizationContext());

            #endregion
        }

        /// <summary>
        /// A method to load the previous day of services.
        /// </summary>
        internal void PushBackServices()
        {
            //Return if this cannot move backwards or if this is loading ServiceHolders
            if (!CanMoveBackward || IsLoading)
                return;

            //Do not move back or forward if the SelectedEntity has changes 
            if (SelectedEntity != null && SelectedEntity.HasChanges)
                return;

            LoadServiceHolders(ServiceHoldersLoad.Backwards);
        }

        /// <summary>
        /// A method to load the next day of services.
        /// </summary>
        internal void PushForwardServices()
        {
            //Return if this cannot move forwards or if this is loading ServiceHolders
            if (!CanMoveForward || IsLoading)
                return;

            //Do not move back or forward if the SelectedEntity has changes 
            if (SelectedEntity != null && SelectedEntity.HasChanges)
                return;

            LoadServiceHolders(ServiceHoldersLoad.Forwards);
        }

        #endregion

        #endregion

        #region Logic

        #region Overriden Methods

        #region Add Delete Entities

        /// <summary>
        /// Cannot add if this has unsaved changes.
        /// </summary>
        protected override void CheckAdd(Action<bool> addNewItem)
        {
            if (!DomainContextHasRelatedChanges())
            {
                addNewItem(true);
                return;
            }
            
            //Disable selected entity save discard prompts
            DisableSelectedEntitySaveDiscardCancel = true;

            //After save or discard: checkCompleted(true);
            Action afterSaveDiscard = () =>
            {
                addNewItem(true);
                DisableSelectedEntitySaveDiscardCancel = false;
            };

            //Move back to the old value
            Action moveToOldValue = () =>
            {
                ((ICollectionView)EditableCollectionView).MoveCurrentTo(SelectedEntity);
                addNewItem(false);
            };

            //Call SaveDiscardCancelPrompt
            DataManager.SaveDiscardCancelPrompt(afterSave: afterSaveDiscard, afterDiscard: afterSaveDiscard, afterCancel: moveToOldValue);
        }

        protected override ServiceHolder AddNewEntity(object commandParameter)
        {
            if (Collection == null)
                return null;

            var newService = new Service { Id = Guid.NewGuid(), ServiceDate = DateTime.Now };

            //The RecurringServices Add Button will pass a ServiceProviderLevel or ClientLevel ServiceTemplate (Available Service)
            var parentServiceTemplate = (ServiceTemplate)commandParameter;

            //Make a ServiceDefined ServiceTemplate child from the parentServiceTemplate
            var childServiceTemplate = parentServiceTemplate.MakeChild(ServiceTemplateLevel.ServiceDefined);

            //Set the new service's service template
            newService.ServiceTemplate = childServiceTemplate;

            newService.ServiceProvider = (BusinessAccount)this.ContextManager.OwnerAccount;

            var clientContext = ContextManager.GetContext<Client>();
            newService.Client = clientContext;

            //Add the new service to a service holder
            var serviceHolder = new ServiceHolder(newService);
            ((ObservableCollection<ServiceHolder>)SourceCollection).Add(serviceHolder);

            return serviceHolder;
        }

        protected override void EntityAdded()
        {
            base.EntityAdded();

            //If this is not in DetailsView, move it into DetailsView
            if (!IsInDetailsView)
                this.MoveToDetailsView.Execute(null);
        }

        public override void DeleteEntity(ServiceHolder entityToDelete)
        {
            //If the entityToDelete has a ParentRecurringService add it's ServiceDate 
            //to the ParentRecurringService's ExcludedDates so that it does not get regenerated
            var recurringServiceParent = entityToDelete.Service.RecurringServiceParent;
            if (recurringServiceParent != null)
            {
                var newExcludedDates = recurringServiceParent.ExcludedDates.ToList();
                newExcludedDates.Add(entityToDelete.OccurDate);
                recurringServiceParent.ExcludedDates = newExcludedDates;
            }

            //If the Service is part of the DomainContext, remove it
            if (DomainContext.Services.Contains(entityToDelete.Service))
                DomainContext.Services.Remove(entityToDelete.Service);

            //Remove the ServiceHolder from the CollectionView
            ((ObservableCollection<ServiceHolder>)SourceCollection).Remove(entityToDelete);
        }

        #endregion

        /// <summary>
        /// Must update IsSelected on the ServiceHolders to prevent unnecessary data from loading.
        /// </summary>
        protected override bool BeforeSelectedEntityChanges(ServiceHolder oldValue, ServiceHolder newValue)
        {
            if (oldValue != null)
                oldValue.IsSelected = false;

            if (newValue != null)
                newValue.IsSelected = true;

            return true;
        }

        /// <summary>
        /// Override DomainContextHasRelatedChanges so it considers the updated HasChanges of the ServiceHolder.
        /// </summary>
        protected override bool DomainContextHasRelatedChanges()
        {
            return SelectedEntity != null && SelectedEntity.HasChanges;
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// The type of load to perform.
    /// </summary>
    enum ServiceHoldersLoad
    {
        /// <summary>
        /// Load the seed date previous and next
        /// </summary>
        InitialLoad,
        /// <summary>
        /// Load the seed date and previous
        /// </summary>
        Backwards,
        /// <summary>
        /// Load the seed date and next
        /// </summary>
        Forwards
    }
}

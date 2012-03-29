using System.ServiceModel.DomainServices.Client;
using FoundOps.Common.Silverlight.Services;
using FoundOps.Common.Silverlight.Tools.ExtensionMethods;
using FoundOps.Common.Tools;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.SLClient.Data.Services;
using FoundOps.SLClient.Data.ViewModels;
using MEFedMVVM.ViewModelLocator;
using ReactiveUI;
using ReactiveUI.Xaml;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// Loads, displays (and generates) services based on context. Allows adding/deleting of services.
    /// </summary>
    [ExportViewModel("ServicesVM")]
    public class ServicesVM : InfiniteAccordionVM<ServiceHolder>
    {
        #region Public Properties

        /// <summary>
        /// The last loaded time.
        /// </summary>
        public DateTime LastLoad { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ServicesVM"/> class.
        /// </summary>
        [ImportingConstructor]
        public ServicesVM()
        {
            SetupDataLoading();

            //TODO Regenerate the Services when the current RecurringServiceContext.Repeat changes
            //recurringServiceContextObservable.WhereNotNull().SelectLatest(rs => rs.RepeatChangedObservable())
            //    .Subscribe(_ => LoadData);

            #region Override the SaveCommand and DiscardCommand

            var canSaveDiscard = this.WhenAny(x => x.SelectedEntity, x => x.SelectedEntity.HasChanges, (selectedEntity, hasChanges) =>
                                                   selectedEntity.Value != null && hasChanges.Value);

            SaveCommand = new ReactiveCommand(canSaveDiscard);
            SaveCommand.Subscribe(param =>
            {
                if (!BeforeSaveCommand()) return;

                DataManager.EnqueueSubmitOperation(OnSave);
            });

            DiscardCommand = new ReactiveCommand(canSaveDiscard);
            DiscardCommand.Subscribe(param =>
            {
                if (!BeforeDiscardCommand()) return;
                DomainContext.RejectChanges();
                AfterDiscard();
                DiscardSubject.OnNext(true);
            });

            #endregion
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

                //Update the CollectionViewObservable with the new loaded ServiceHolders
                ViewObservable.OnNext(new DomainCollectionViewFactory<ServiceHolder>(task.Result).View);

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

            LoadServiceHolders(ServiceHoldersLoad.Forwards);
        }

        #endregion

        #endregion

        #region Logic

        #region Overriden Methods

        #region Add Delete Entities

        //protected override Service AddNewEntity(object commandParameter)
        //{
        //    var newService = base.AddNewEntity(commandParameter);

        //    //The RecurringServices Add Button will pass a ServiceProviderLevel or ClientLevel ServiceTemplate (Available Service)
        //    var parentServiceTemplate = (ServiceTemplate)commandParameter;

        //    //Make a ServiceDefined ServiceTemplate child from the parentServiceTemplate
        //    var childServiceTemplate = parentServiceTemplate.MakeChild(ServiceTemplateLevel.ServiceDefined);

        //    //Set the new service's service template
        //    newService.ServiceTemplate = childServiceTemplate;

        //    newService.ServiceProvider = (BusinessAccount)this.ContextManager.OwnerAccount;

        //    return newService;
        //}

        //protected override void OnAddEntity(Service newEntity)
        //{
        //    newEntity.ServiceIsNew = true;

        //    var clientContext = ContextManager.GetContext<Client>();
        //    if (clientContext != null)
        //        newEntity.Client = clientContext;

        //    newEntity.TrackChanges();
        //}

        protected override void EntityAdded()
        {
            base.EntityAdded();

            //If this is not in DetailsView, move it into DetailsView
            if (!IsInDetailsView)
                this.MoveToDetailsView.Execute(null);
        }

        //protected override void OnDeleteEntity(Service entityToDelete)
        //{
        //    //Must do this manually because even though it is removed fromt the DCV, the DCV is not backed by a EntityList
        //    if (!entityToDelete.Generated)
        //        DomainContext.Services.Remove(entityToDelete);

        //    //If the entityToDelete has a ParentRecurringService
        //    //add it's ServiceDate to the ParentRecurringService's ExcludedDates so that it does not get regenerated)
        //    if (entityToDelete.RecurringServiceParent != null)
        //    {
        //        var newExcludedDates = entityToDelete.RecurringServiceParent.ExcludedDates.ToList();
        //        newExcludedDates.Add(entityToDelete.ServiceDate);
        //        entityToDelete.RecurringServiceParent.ExcludedDates = newExcludedDates;
        //    }
        //}

        #endregion

        protected override bool BeforeDiscardCommand()
        {
            if (SelectedEntity == null) return true;

            //If it is a generated Service, reload the details
            //Regenerating the ServiceTemplate should discard the changes
            if (SelectedEntity.ServiceIsGenerated)
            {
                _cancelLastDetailsLoad.OnNext(true);
                SelectedEntity.LoadDetails(_cancelLastDetailsLoad);
            }
            
            //The changes for generated services are not tracked
            //so no need to perform a DomainContext Discard
            return !SelectedEntity.ServiceIsGenerated;
        }

        protected override void AfterDiscard()
        {
            //Existing services no longer use standard change tracking
            //so update the HasChanges to false after a discard
            if (SelectedEntity != null)
                SelectedEntity.HasChanges = false;
        }

        protected override bool BeforeSaveCommand()
        {
            //Add the detached generated service to the context before saving
            if (SelectedEntity.ServiceIsGenerated)
                DomainContext.Services.Add(SelectedEntity.Service);

            return base.BeforeAdd();
        }

        protected override bool DomainContextHasRelatedChanges()
        {
            //Return whether or not the current serviceholder has changes
            return SelectedEntity != null && SelectedEntity.HasChanges;
        }

        protected override void OnSave(SubmitOperation submitOperation)
        {
            //If there was an issue or if this was cancelled
            //a) detach any GeneratedServices
            //b) reload the selected entity's details
            if (submitOperation.HasError || submitOperation.IsCanceled)
            {
                //a) detach any GeneratedServices
                this.DataManager.DetachEntities(submitOperation.ChangeSet.AddedEntities.OfType<Service>().SelectMany(s => s.EntityGraph()));

                //b) reload the selected entity's details
                if (SelectedEntity != null)
                    SelectedEntity.LoadDetails(_cancelLastDetailsLoad);
            }
            else
            {
                //If a generated service was added succesfully update the ServiceHolder
                //No need to detach it because it is now an existing service
                foreach (var addedGeneratedService in submitOperation.ChangeSet.AddedEntities.OfType<Service>())
                    addedGeneratedService.ParentServiceHolder.ConvertToExistingService();
            }

            base.OnSave(submitOperation);
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

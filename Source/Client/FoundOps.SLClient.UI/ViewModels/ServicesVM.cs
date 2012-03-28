using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using FoundOps.Common.Silverlight.Services;
using FoundOps.Common.Silverlight.Tools.ExtensionMethods;
using FoundOps.Common.Tools;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.SLClient.Data.Services;
using FoundOps.SLClient.Data.ViewModels;
using MEFedMVVM.ViewModelLocator;
using System;
using System.ComponentModel.Composition;
using System.Linq;

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
        //private bool _contextChanged;
        //private bool ContextChanged
        //{
        //    get { return _contextChanged; }
        //    set
        //    {
        //        _contextChanged = value;
        //        if (!_contextChanged) return;

        //        //Reset conditions
        //        _canMoveBackward = true;
        //        _canMoveForward = true;

        //        //This started loading
        //        IsLoadingSubject.OnNext(true);
        //    }
        //}

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ServicesVM"/> class.
        /// </summary>
        [ImportingConstructor]
        public ServicesVM()
        {
            //It doesn't seem to need it on this VM
            //KeepTrackLastSelectedEntity = false;
            SetupDataLoading();

            //TrackContext();
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

            ////Whenever this becomes DetailsView:
            ////a) update the generated services so there are enough 
            ////b) track the SelectedEntity
            //this.ContextManager.CurrentContextProviderObservable.DistinctUntilChanged()
            //    .Where(contextProvider => contextProvider == this).SubscribeOnDispatcher().Subscribe(_ =>
            //    {
            //        var selectedEntity = SelectedEntity;
            //        //a) update the generated services so there are enough
            //        //Initially load ServiceHolders for today
            //        IsLoadingSubject.OnNext(true);
            //        this.DataManager.LoadCollection(ServiceHolderQueryForContext(DateTime.Now, true, true), loadedServiceHolders =>
            //        {
            //            IsLoadingSubject.OnNext(false);
            //            CollectionViewObservable.OnNext(new DomainCollectionViewFactory<ServiceHolder>(loadedServiceHolders).View);
            //            SelectedEntity = Collection.FirstOrDefault(sh => sh.Equals(selectedEntity));
            //        });

            //        //b) track the SelectedEntity
            //        //TrackSelectedEntity();
            //    });

            ////Regenerate the Services when the current RecurringServiceContext.Repeat changes
            //recurringServiceContextObservable.WhereNotNull().SelectLatest(rs => rs.RepeatChangedObservable())
            //    .Subscribe(_ => ContextChanged = true);

            SelectedEntityObservable.Where(se => se != null).Subscribe(selectedEntity => selectedEntity.LoadDetails(_cancelLastDetailsLoad));

            //var canSaveDiscard = this.WhenAny(x => x.SelectedEntity, x => x.SelectedEntity.ServiceIsNew,
            //                                   x => x.SelectedEntity.ServiceHasChanges, (selectedEntity, serviceIsNew, serviceHasChanges) =>
            //                                       selectedEntity.Value != null && (serviceIsNew.Value || serviceHasChanges.Value));

            //CanSaveObservable = canSaveDiscard;
            //CanDiscardObservable = canSaveDiscard;
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
                CollectionViewObservable.OnNext(new DomainCollectionViewFactory<ServiceHolder>(task.Result).View);

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

        //private void TrackContext()
        //{
        //    ////Whenever this becomes DetailsView:
        //    ////a) update the generated services so there are enough 
        //    ////b) track the SelectedEntity
        //    //this.ContextManager.CurrentContextProviderObservable.DistinctUntilChanged()
        //    //    .Where(contextProvider => contextProvider == this).SubscribeOnDispatcher().Subscribe(_ =>
        //    //    {
        //    //        var selectedEntity = SelectedEntity;
        //    //        //a) update the generated services so there are enough
        //    //        //Initially load ServiceHolders for today
        //    //        IsLoadingSubject.OnNext(true);
        //    //        this.DataManager.LoadCollection(ServiceHolderQueryForContext(DateTime.Now, true, true), loadedServiceHolders =>
        //    //        {
        //    //            IsLoadingSubject.OnNext(false);
        //    //            CollectionViewObservable.OnNext(new DomainCollectionViewFactory<ServiceHolder>(loadedServiceHolders).View);
        //    //            SelectedEntity = Collection.FirstOrDefault(sh => sh.Equals(selectedEntity));
        //    //        });

        //    //        //b) track the SelectedEntity
        //    //        //TrackSelectedEntity();
        //    //    });
        //}



        #endregion

        #region Logic

        #region Helper Methods



        #endregion

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
            {
                this.MoveToDetailsView.Execute(null);
            }
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

        protected override bool BeforeSaveCommand()
        {
            foreach (var service in
                this.DomainContext.EntityContainer.GetChanges().OfType<Service>().Where(service => service.ServiceHasChanges))
            {
                //After saving an entity is no longer new, generated, nor has changes. So change these properties, then save
                service.ServiceIsNew = false;
                service.Generated = false;
                service.ServiceHasChanges = false;
            }

            return true;
        }

        //protected override bool DomainContextHasRelatedChanges()
        //{
        //    //If the current service has changes or is new then there are related changes
        //    return SelectedEntity != null && (SelectedEntity.ServiceIsNew || SelectedEntity.ServiceHasChanges);
        //}

        #region Generate Services

        private void DetachGeneratedServices()
        {
            //Detach all generated services' graph's entities
            var entitiesToDetach = DomainContext.Services.Where(s => s.Generated).SelectMany(s => s.EntityGraph);

            DataManager.DetachEntities(entitiesToDetach);
        }

        //Keeps track of the last service generation
        private DateTime _lastServiceGeneration = DateTime.Now;

        private void UpdateVisibleServices()
        {
            ////Check if the current context should show services
            //if (DataManager.ContextManager.CurrentContextProvider == null)
            //    return;
            //var mainViewObjectType = DataManager.ContextManager.CurrentContextProvider.ObjectTypeProvided;
            //if (mainViewObjectType != typeof(Service) && mainViewObjectType != typeof(Client) && mainViewObjectType != typeof(RecurringService)
            //    && mainViewObjectType != typeof(Region) && mainViewObjectType != typeof(Location))
            //    return; //Should not generate services if not necessary

            //var startTime = DateTime.Now;

            ////Clear the selection before updating the services
            //DirectSetSelectedEntity(null);

            //Service selectedEntity = null;

            ////Generate Services depending on a couple conditions:
            ////Condition A: There are no services or the context has changed and we need to regenerate all the services (around today)
            ////Condition B: There are services and we need to generate services in the past (from the first generated service)
            ////Condition C: There are services and we need to generate services in the future (from the last generated service)

            ////Condition A: There are no services or the context has changed and we need to regenerate all the services (around today)
            //if (!_visibleServices.Any() || ContextChanged) { } //Keep selectedEntity null
            ////Condition B: There are services and we need to generate services in the past (from the first generated service)
            //else if (_pushBackwardSwitch)
            //    selectedEntity = CollectionView.Cast<Service>().First();
            ////Condition C: There are services and we need to generate services in the future (from the last generated service))
            //else if (_pushForwardSwitch)
            //    selectedEntity = CollectionView.Cast<Service>().Last();

            //int pageSize;

            //if (IsInDetailsView)
            //{
            //    //If Services is in DetailsView and
            //    //a) has no Context: generate 150
            //    //b) has a Context: generate 75
            //    pageSize = ContextManager.CurrentContext.Count == 0 ? 150 : 75;
            //}
            //else
            //    pageSize = 10; //If it is not in DetailsView, generate 10

            ////Clear the services
            //_visibleServices.Clear();

            ////Generate services from results

            //var serviceGenerationResult = ServiceGenerator.GenerateServiceTuples(selectedEntity, _existingServices,
            //                                                           _recurringServices.Where(rs => rs.Repeat != null), ContextManager.GetContext<Client>(),
            //                                                           ContextManager.GetContext<Location>(), ContextManager.GetContext<RecurringService>(),
            //                                                           pageSize);

            //_canMoveBackward = serviceGenerationResult.CanMoveBackward;
            //_canMoveForward = serviceGenerationResult.CanMoveForward;

            ////Add the proper services
            //foreach (var serviceTuple in serviceGenerationResult.ServicesTuples)
            //{
            //    serviceTuple.GenerateService();
            //    _visibleServices.Add(serviceTuple.Service);
            //}

            //DetachGeneratedServices();

            ////Select the corresponding selectedEntity
            //if (selectedEntity == null)
            //    selectedEntity = CollectionView.Cast<Service>().FirstOrDefault(s => s != null && s.ServiceDate >= DateTime.Now.Date);
            //else if (selectedEntity.Generated)
            //    selectedEntity = CollectionView.Cast<Service>().FirstOrDefault(s => s != null && s.ServiceDate == selectedEntity.ServiceDate && s.RecurringServiceId == selectedEntity.RecurringServiceId);

            //SelectedEntity = selectedEntity;

            ////Clear the context change after generating services
            //ContextChanged = false;

            //_lastServiceGeneration = DateTime.Now;

            ////Clear the switches
            //_pushBackwardSwitch = false;
            //_pushForwardSwitch = false;

            ////This is no longer loading, if everything is loaded and this just finished generating
            //if (!ServicesOrRecurringServicesLoading)
            //    IsLoadingSubject.OnNext(false);
        }


        #endregion

        //public override void OnNavigateFrom()
        //{
        //    //If the current service does not have changes and is not new and there are no related changes: discard changes
        //    if (!DomainContextHasRelatedChanges())
        //    {
        //        //This will allow it to be removed
        //        if (SelectedEntity != null)
        //            SelectedEntity.Generated = false;

        //        this.DiscardCommand.Execute(null);
        //    }
        //}

        #region Save Discard

        //protected override bool BeforeDiscardCommand()
        //{
        //    if (SelectedEntity == null) return true;

        //    //If it is a generated Service: regenerating the ServiceTemplate should discard the changes
        //    if (SelectedEntity.Generated)
        //        RegenerateSelectedEntityServiceTemplate();

        //    return !SelectedEntity.Generated;
        //}

        ///// <summary>
        ///// Regenerates the selected entity's service template.
        ///// </summary>
        //private void RegenerateSelectedEntityServiceTemplate()
        //{
        //    var selectedEntity = SelectedEntity;

        //    //Detach entities
        //    DetachGeneratedServices();

        //    //Regenerate the SelectedEntity's ServiceTemplate (to clear changes)
        //    selectedEntity.ServiceTemplate = selectedEntity.RecurringServiceParent.ServiceTemplate.MakeChild(ServiceTemplateLevel.ServiceDefined);

        //    //Reselect the SelectedEntity
        //    SelectedEntity = selectedEntity;

        //    //Clear the tracked property's changes
        //    SelectedEntity.ServiceHasChanges = false;
        //}

        protected override void OnDiscard()
        {
            //if (SelectedEntity != null)
            //{
            //    //Clear the tracked property's changes
            //    SelectedEntity.ServiceHasChanges = false;

            //    if (SelectedEntity.ServiceIsNew)
            //    {
            //        _visibleServices.Remove(SelectedEntity);
            //        DirectSetSelectedEntity(null);
            //        CollectionView.MoveCurrentTo(null);
            //    }
            //}

            //base.OnDiscard();
        }

        #endregion

        #region Selection

        //protected override bool BeforeSelectedEntityChanges(Service oldValue, Service newValue)
        //{
        //    if (!IsInDetailsView)
        //        return true;

        //    //Discard changes if the old selected service does not have any changes
        //    if (oldValue != null && !oldValue.ServiceHasChanges)
        //        this.DiscardCommand.Execute(null);

        //    return true;
        //}

        //protected override void OnSelectedEntityChanged(Service oldValue, Service newValue)
        //{
        //    if (!IsInDetailsView) return;

        //    //Only track changes/add the selected generated service to the Context if the ServicesVM is in the DetailsView
        //    //This will prevent unwanted changes being made

        //    DetachGeneratedServices();
        //    TrackSelectedEntity();
        //}

        ////Track the selected service's changes/add it to the DomainContext if it is generated
        //private void TrackSelectedEntity()
        //{
        //    if (SelectedEntity == null) return;

        //    if (SelectedEntity.Generated)
        //        this.DomainContext.Services.Add(SelectedEntity);

        //    //If the SelectedEntity is Generated and does not have changes, regenerate the ServiceTemplate
        //    if (SelectedEntity.Generated && !SelectedEntity.HasChanges)
        //        RegenerateSelectedEntityServiceTemplate();

        //    SelectedEntity.TrackChanges();
        //    SelectedEntity.ServiceHasChanges = false;
        //}

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

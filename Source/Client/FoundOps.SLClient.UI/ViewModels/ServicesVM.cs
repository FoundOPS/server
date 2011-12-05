using System;
using FoundOps.Common.Silverlight.Tools.ExtensionMethods;
using ReactiveUI;
using System.Linq;
using System.Threading;
using System.Reactive.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using FoundOps.SLClient.UI.Tools;
using MEFedMVVM.ViewModelLocator;
using System.Collections.ObjectModel;
using FoundOps.SLClient.Data.Services;
using System.ComponentModel.Composition;
using FoundOps.SLClient.Data.ViewModels;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Common.Silverlight.Services;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// Loads, displays (and generates) services based on context. Allows adding/deleting of services.
    /// </summary>
    [ExportViewModel("ServicesVM")]
    public class ServicesVM : CoreEntityCollectionInfiniteAccordionVM<Service>
    {
        #region Locals

        private bool _contextChanged;
        private bool ContextChanged
        {
            get { return _contextChanged; }
            set
            {
                _contextChanged = value;
                if (!_contextChanged) return;

                //Reset conditions
                _canMoveBackward = true;
                _canMoveForward = true;

                //Update the visible services
                _updateVisibleServicesTimer.Change(1000, Timeout.Infinite);

                //This started loading
                IsLoadingSubject.OnNext(true);
            }
        }

        private readonly ObservableCollection<Service> _visibleServices = new ObservableCollection<Service>();
        private readonly Timer _updateVisibleServicesTimer;
        private IEnumerable<Service> _existingServices;
        private IEnumerable<RecurringService> _recurringServices;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ServicesVM"/> class.
        /// </summary>
        /// <param name="dataManager">The data manager</param>
        [ImportingConstructor]
        public ServicesVM(DataManager dataManager)
            : base(dataManager, true)
        {
            //It doesn't seem to need it on this VM
            KeepTrackLastSelectedEntity = false;

            //Instantiate the DomainCollectionView wrapping _visibleServices
            DomainCollectionViewObservable.OnNext(new DomainCollectionViewFactory<Service>(_visibleServices).View);

            //Setup Filters
            DomainCollectionView.SortDescriptions.Add(new SortDescription("ServiceDate", ListSortDirection.Ascending));
            DomainCollectionView.SortDescriptions.Add(new SortDescription("ServiceType", ListSortDirection.Ascending));
            DomainCollectionView.SortDescriptions.Add(new SortDescription("Client.DisplayName", ListSortDirection.Ascending));

            _updateVisibleServicesTimer = new Timer(cbk =>
            {
                //If this is still loading data wait to update
                if (_existingServices == null || _recurringServices == null)
                    _updateVisibleServicesTimer.Change(1000, Timeout.Infinite);
                else
                    CurrentDispatcher.BeginInvoke(UpdateVisibleServices);
            }, null, Timeout.Infinite, Timeout.Infinite);

            SetupDataLoading();

            TrackContext();
        }

        private void SetupDataLoading()
        {
            //Load services
            var servicesLoading = DataManager.Subscribe<Service>(DataManager.Query.Services, ObservationState, entities => _existingServices = entities);

            //Load recurring services
            var recurringServicesLoading = DataManager.Subscribe<RecurringService>(DataManager.Query.RecurringServices, ObservationState, entities => _recurringServices = entities);

            var loadingData = servicesLoading.CombineLatest(recurringServicesLoading).Select(loadingTuple => loadingTuple.Item1 || loadingTuple.Item2);

            //If not everything is loaded, set IsLoading to true
            if (_existingServices == null || _recurringServices == null)
                IsLoadingSubject.OnNext(true);

            //Whenever loadingData publishes update IsLoadingSubject
            loadingData.Subscribe(IsLoadingSubject);

            //When everything is loaded update the visible services
            loadingData.Where(isLoading => !isLoading)
                .Throttle(new TimeSpan(0, 0, 0, 0, 300)) //Throttle to allow associations to settle
                .ObserveOnDispatcher().Subscribe(isLoading =>
                {
                    if (_existingServices != null && _recurringServices != null)
                        UpdateVisibleServices();
                });


            var canSaveDiscard = this.WhenAny(x => x.SelectedEntity, x => x.SelectedEntity.ServiceIsNew,
                                               x => x.SelectedEntity.ServiceHasChanges,
                                               (selectedEntity, serviceIsNew, serviceHasChanges) => selectedEntity.Value != null && (serviceIsNew.Value || serviceHasChanges.Value));

            CanSaveObservable = canSaveDiscard;
            CanDiscardObservable = canSaveDiscard;
        }

        private void TrackContext()
        {
            //Whenever this becomes DetailsView: update the generated services (so there are enough)
            //and track the SelectedEntity
            this.ContextManager.CurrentContextProviderObservable
                .Where(contextProvider => contextProvider == this) //This is the DetailsView
                .SubscribeOnDispatcher().Subscribe(_ =>
            {
                //update the generated services
                ContextChanged = true;
                TrackSelectedEntity();
            });

            var clientContextSubject = ContextManager.GetContextObservable<Client>();
            var locationContextObservable = ContextManager.GetContextObservable<Location>();
            var recurringServiceContextObservable = ContextManager.GetContextObservable<RecurringService>();

            //Whenever one of the specific context changes, update ContextChanged
            clientContextSubject.Select(_ => true)
            .Merge(locationContextObservable.Select(_ => true))
            .Merge(recurringServiceContextObservable.Select(_ => true))
            .SubscribeOnDispatcher().Subscribe(_ => ContextChanged = true);

            //Regenerate the Services when:
            //a) the RecurringServiceContext.Repeat changes
            //b) any ClientContext.RecurringServices' Repeats change
            var caseA = recurringServiceContextObservable.Where(rs => rs != null).SelectMany(rs => rs.RepeatChangedObservable());

            //Any time the clientContext changes
            var caseB = clientContextSubject.Where(c => c != null).SelectMany(clientContext =>
                //Select the clientContext.RecurringServices changes
                          clientContext.RecurringServices.Select(rs => rs.RepeatChangedObservable()).Merge()
                              //whenever the clientContext.RecurringServices Collection changes
                      .Merge(clientContext.RecurringServices.FromCollectionChanged()
                              //Delay to allow Repeat association to be set
                      .Delay(new TimeSpan(0, 0, 0, 0, 250))
                              //Also choose the clientContext.RecurringServices changes from
                      .SelectMany(rss => rss.Select(rs => rs.RepeatChangedObservable()).Merge())));

            //Regenerate the Services
            caseA.Merge(caseB).SubscribeOnDispatcher().Subscribe(_ => ContextChanged = true);
        }

        #endregion

        #region Logic

        #region Add Delete Entities

        protected override Service AddNewEntity(object commandParameter)
        {
            var newService = base.AddNewEntity(commandParameter);

            //The RecurringServices Add Button will pass a ServiceProviderLevel or ClientLevel ServiceTemplate (Available Service)
            var parentServiceTemplate = (ServiceTemplate)commandParameter;

            //Make a ServiceDefined ServiceTemplate child from the parentServiceTemplate
            var childServiceTemplate = parentServiceTemplate.MakeChild(ServiceTemplateLevel.ServiceDefined);

            //Set the new service's service template
            newService.ServiceTemplate = childServiceTemplate;

            newService.ServiceProvider = (BusinessAccount)this.ContextManager.OwnerAccount;

            return newService;
        }

        protected override void OnAddEntity(Service newEntity)
        {
            newEntity.ServiceIsNew = true;

            var clientContext = ContextManager.GetContext<Client>();
            if (clientContext != null)
                newEntity.Client = clientContext;

            newEntity.TrackChanges();
        }

        protected override void EntityAdded()
        {
            base.EntityAdded();

            //If this is not in DetailsView, move it into DetailsView
            if (!IsInDetailsView)
            {
                this.MoveToDetailsView.Execute(null);
            }
        }

        protected override void OnDeleteEntity(Service entityToDelete)
        {
            //Must do this manually because even though it is removed fromt the DCV, the DCV is not backed by a EntityList
            if (!entityToDelete.Generated)
                Context.Services.Remove(entityToDelete);

            //If the entityToDelete has a ParentRecurringService
            //add it's ServiceDate to the ParentRecurringService's ExcludedDates so that it does not get regenerated)
            if (entityToDelete.RecurringServiceParent != null)
            {
                var newExcludedDates = entityToDelete.RecurringServiceParent.ExcludedDates.ToList();
                newExcludedDates.Add(entityToDelete.ServiceDate);
                entityToDelete.RecurringServiceParent.ExcludedDates = newExcludedDates;
            }
        }

        #endregion

        protected override bool BeforeSaveCommand()
        {
            foreach (Service service in
                this.Context.EntityContainer.GetChanges().OfType<Service>().Where(service => service.ServiceHasChanges))
            {
                //After saving an entity is no longer new, generated, nor has changes. So change these properties, then save
                service.ServiceIsNew = false;
                service.Generated = false;
                service.ServiceHasChanges = false;
            }

            return true;
        }

        protected override bool DomainContextHasRelatedChanges()
        {
            //If the current service has changes or is new then there are related changes
            return SelectedEntity != null && (SelectedEntity.ServiceIsNew || SelectedEntity.ServiceHasChanges);
        }

        #region Generate Services

        private void DetachGeneratedServices()
        {
            //Detach all generated services' graph's entities
            var entitiesToDetach = Context.Services.Where(s => s.Generated).SelectMany(s => s.EntityGraph);

            DataManager.DetachEntities(entitiesToDetach);
        }

        private void UpdateVisibleServices()
        {
            //Check if the current context should show services
            if (DataManager.ContextManager.CurrentContextProvider == null)
                return;
            var mainViewObjectType = DataManager.ContextManager.CurrentContextProvider.ObjectTypeProvided;
            if (mainViewObjectType != typeof(Service) && mainViewObjectType != typeof(Client) && mainViewObjectType != typeof(RecurringService)
                && mainViewObjectType != typeof(Region) && mainViewObjectType != typeof(Location))
                return; //Should not generate services if not necessary

            var startTime = DateTime.Now;

            //Clear the selection before updating the services
            DirectSetSelectedEntity(null);

            Service selectedEntity = null;

            //Generate Services depending on a couple conditions:
            //Condition A: There are no services or the context has changed and we need to regenerate all the services (around today)
            //Condition B: There are services and we need to generate services in the past (from the first generated service)
            //Condition C: There are services and we need to generate services in the future (from the last generated service)

            //Condition A: There are no services or the context has changed and we need to regenerate all the services (around today)
            if (_visibleServices.Count() <= 0 || ContextChanged) { } //Keep selectedEntity null
            //Condition B: There are services and we need to generate services in the past (from the first generated service)
            else if (_pushBackwardSwitch)
                selectedEntity = DomainCollectionView.First();
            //Condition C: There are services and we need to generate services in the future (from the last generated service))
            else if (_pushForwardSwitch)
                selectedEntity = DomainCollectionView.Last();

            int pageSize;

            if (IsInDetailsView)
            {
                //If Services is in DetailsView and
                //a) has no Context: generate 150
                //b) has a Context: generate 75
                pageSize = ContextManager.CurrentContext.Count == 0 ? 150 : 75;
            }
            else
                pageSize = 10; //If it is not in DetailsView, generate 10

            //Clear the services
            _visibleServices.Clear();

            //Generate services from results

            var serviceGenerationResult = ServiceGenerator.GenerateServiceTuples(selectedEntity, _existingServices,
                                                                       _recurringServices.Where(rs => rs.Repeat != null), ContextManager.GetContext<Client>(),
                                                                       ContextManager.GetContext<Location>(), ContextManager.GetContext<RecurringService>(),
                                                                       pageSize);

            _canMoveBackward = serviceGenerationResult.CanMoveBackward;
            _canMoveForward = serviceGenerationResult.CanMoveForward;

            //Add the proper services
            foreach (var serviceTuple in serviceGenerationResult.ServicesTuples)
            {
                serviceTuple.GenerateService();
                _visibleServices.Add(serviceTuple.Service);
            }

            DetachGeneratedServices();

            //Select the corresponding selectedEntity
            if (selectedEntity == null)
                selectedEntity = DomainCollectionView.FirstOrDefault(s => s != null && s.ServiceDate >= DateTime.Now.Date);
            else if (selectedEntity.Generated)
                selectedEntity = DomainCollectionView.FirstOrDefault(s => s != null && s.ServiceDate == selectedEntity.ServiceDate && s.RecurringServiceId == selectedEntity.RecurringServiceId);

            SelectedEntity = selectedEntity;

            //Clear the context change after generating services
            ContextChanged = false;

            //Clear the switches
            _pushBackwardSwitch = false;
            _pushForwardSwitch = false;

            //This is no longer loading, if everything is loaded and this just finished generating
            if (_recurringServices != null && _existingServices != null)
                IsLoadingSubject.OnNext(false);
        }

        private bool _canMoveBackward;
        private bool _pushBackwardSwitch;
        internal bool PushBackGeneratedServices()
        {
            if (!_canMoveBackward)
                return false;

            _canMoveBackward = false; //Prevent double tripping
            _pushBackwardSwitch = true;

            //This started loading
            IsLoadingSubject.OnNext(true);

            _updateVisibleServicesTimer.Change(250, Timeout.Infinite);

            return true;
        }

        private bool _canMoveForward;
        private bool _pushForwardSwitch;
        internal bool PushForwardGeneratedServices()
        {
            if (!_canMoveForward)
                return false;

            _canMoveForward = false; //Prevent double tripping
            _pushForwardSwitch = true;

            //This started loading
            IsLoadingSubject.OnNext(true);
            _updateVisibleServicesTimer.Change(250, Timeout.Infinite);

            return true;
        }

        #endregion

        public override void OnNavigateFrom()
        {
            //If the current service does not have changes and is not new and there are no related changes: discard changes
            if (!DomainContextHasRelatedChanges())
            {
                //This will allow it to be removed
                if (SelectedEntity != null)
                    SelectedEntity.Generated = false;

                this.DiscardCommand.Execute(null);
            }
        }

        #region Save Discard

        protected override bool BeforeDiscardCommand()
        {
            if (SelectedEntity == null) return true;

            //If it is a generated Service: regenerating the ServiceTemplate should discard the changes
            if (SelectedEntity.Generated)
                RegenerateSelectedEntityServiceTemplate();

            return !SelectedEntity.Generated;
        }

        /// <summary>
        /// Regenerates the selected entity's service template.
        /// </summary>
        private void RegenerateSelectedEntityServiceTemplate()
        {
            var selectedEntity = SelectedEntity;

            //Detach entities
            DetachGeneratedServices();

            //Regenerate the SelectedEntity's ServiceTemplate (to clear changes)
            selectedEntity.ServiceTemplate = selectedEntity.RecurringServiceParent.ServiceTemplate.MakeChild(ServiceTemplateLevel.ServiceDefined);

            //Reselect the SelectedEntity
            SelectedEntity = selectedEntity;

            //Clear the tracked property's changes
            SelectedEntity.ServiceHasChanges = false;
        }

        protected override void OnDiscard()
        {
            if (SelectedEntity != null)
            {
                //Clear the tracked property's changes
                SelectedEntity.ServiceHasChanges = false;

                if (SelectedEntity.ServiceIsNew)
                {
                    _visibleServices.Remove(SelectedEntity);
                    DirectSetSelectedEntity(null);
                    DomainCollectionView.MoveCurrentTo(null);
                }
            }

            base.OnDiscard();
        }

        #endregion

        #region Selection

        protected override bool BeforeSelectedEntityChanges(Service oldValue, Service newValue)
        {
            if (!IsInDetailsView)
                return true;

            //Discard changes if the old selected service does not have any changes
            if (oldValue != null && !oldValue.ServiceHasChanges)
                this.DiscardCommand.Execute(null);

            return true;
        }

        protected override void OnSelectedEntityChanged(Service oldValue, Service newValue)
        {
            if (!IsInDetailsView) return;

            //Only track changes/add the selected generated service to the Context if the ServicesVM is in the DetailsView
            //This will prevent unwanted changes being made

            DetachGeneratedServices();
            TrackSelectedEntity();
        }

        //Track the selected service's changes/add it to the DomainContext if it is generated
        private void TrackSelectedEntity()
        {
            if (SelectedEntity == null) return;

            if (SelectedEntity.Generated)
                this.Context.Services.Add(SelectedEntity);

            //If the SelectedEntity is Generated and does not have changes, regenerate the ServiceTemplate
            if (SelectedEntity.Generated && !SelectedEntity.HasChanges)
                RegenerateSelectedEntityServiceTemplate();

            SelectedEntity.TrackChanges();
            SelectedEntity.ServiceHasChanges = false;
        }

        #endregion

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.ServiceModel.DomainServices.Client;
using FoundOps.Common.Silverlight.Tools.ExtensionMethods;
using FoundOps.Common.Silverlight.UI.Interfaces;
using FoundOps.SLClient.Data.Services;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using FoundOps.Server.Services.CoreDomainService;
using RiaServicesContrib;

//Partial class must be part of same namespace
// ReSharper disable CheckNamespace
namespace FoundOps.Core.Models.CoreEntities
// ReSharper restore CheckNamespace
{
    public partial class ServiceHolder : ILoadDetails
    {
        #region Public Properties

        #region Implementation of ILoadDetails

        private bool _detailsLoaded;

        /// <summary>
        /// Gets or sets a value indicating whether [details loaded].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [details loading]; otherwise, <c>false</c>.
        /// </value>
        public bool DetailsLoaded
        {
            get { return _detailsLoaded; }
            set
            {
                _detailsLoaded = value;
                this.RaisePropertyChanged("DetailsLoaded");
            }
        }

        #endregion

        private Service _service;

        /// <summary>
        /// The loaded or generated service.
        /// </summary>
        public Service Service
        {
            get { return _service; }
            set
            {
                _service = value;
                this.RaisePropertyChanged("Service");
                this.RaisePropertyChanged("HasChanges");
            }
        }

        /// <summary>
        /// Whether or not the Service is generated.
        /// </summary>
        public bool ServiceIsGenerated
        {
            get
            {
                //The ExistingServiceId will only have a value when there is an existing service.
                return !ExistingServiceId.HasValue && RecurringServiceId.HasValue;
            }
        }

        /// <summary>
        /// Whether or not this Service is new.
        /// This is only true for new Services that are not generated from a ServiceHolder.
        /// </summary>
        public bool ServiceIsNew
        {
            get { return !ExistingServiceId.HasValue && !RecurringServiceId.HasValue; }
        }

        /// <summary>
        /// Override HasChanges.
        /// </summary>
        public new bool HasChanges
        {
            //If the ServiceIsNew or if the Service has changes return true
            get
            {
                return ServiceIsNew ||
                       (_entityGraph != null &&
                        _entityGraph.Any(
                            e =>
                            e.EntityState == EntityState.Modified || e.EntityState == EntityState.New ||
                            e.EntityState == EntityState.Deleted));
            }
        }

        /// <summary>
        /// If this is a generated service and this is selected
        /// when changes are rejected this will reload the details.
        /// NOTE: Must keep this updated to prevent unnecessary data loading.
        /// </summary>
        public bool IsSelected { get; set; }

        #endregion

        #region Locals

        /// <summary>
        /// The existing or generated service's entity graph.
        /// It is stored as a local property so the PropertyChanged event can be unhandled.
        /// </summary>
        private EntityGraph<Entity> _entityGraph;

        /// <summary>
        /// Used to cancel the last reload details query.
        /// </summary>
        private Subject<bool> _cancelLastReloadDetails;

        /// <summary>
        /// Store the original occur date in case reject changes is called.
        /// </summary>
        private DateTime _originalOccurDate;

        private IDisposable _serviceSubscription;

        #endregion

        #region Constructor

        #region Initialization

        partial void OnCreation()
        {
            InitializeHelper();
        }

        protected override void OnLoaded(bool isInitialLoad)
        {
            if (isInitialLoad)
                InitializeHelper();

            base.OnLoaded(isInitialLoad);
        }

        private void InitializeHelper()
        {
            _originalOccurDate = OccurDate;

            //Follow OwnedParty changes
            Observable2.FromPropertyChangedPattern(this, x => x.Service).ObserveOnDispatcher()
                .Subscribe(_ =>
                {
                    //Clear the last subscriptions and handlers
                    ClearSubscriptionsHandlers();

                    if (this.Service == null) return;

                    //Update this DisplayName whenever OwnedParty.DisplayName changes
                    _serviceSubscription = Observable2.FromPropertyChangedPattern(this.Service,
                                                                                  x => x.ServiceDate).
                        DistinctUntilChanged()
                        .ObserveOnDispatcher().Subscribe(
                            __ => this.CompositeRaiseEntityPropertyChanged("ServiceDate"));
                });

            //Follow property changes and update the Service
            Observable2.FromPropertyChangedPattern(this, x => x.OccurDate).Throttle(TimeSpan.FromMilliseconds(100))
                .ObserveOnDispatcher().Subscribe(_ =>
                {
                    //If there is a time associated with the OccurDate, clear it
                    if (this.OccurDate != this.OccurDate.Date)
                    {
                        this.OccurDate = this.OccurDate.Date;
                        return;
                    }

                    //Prevent changes when the details are not loaded
                    if (!DetailsLoaded)
                        return;

                    //Service will only be null while reloading details. This SHOULD only happen in the OnRejectChangedReloadDetails method when the OccurDate is set
                    if (Service == null)
                        return;

                    //If the RecurringServiceParent is not null, update its ExcludedDates to exclude the old ServiceDate and include the new ServiceDate
                    if (Service.RecurringServiceParent != null)
                    {
                        var newExcludedDatesList =
                            Service.RecurringServiceParent.ExcludedDates.ToList();

                        //If the old ServiceDate does not equal the OccurDate, exclude it from the RecurringService
                        if (Service.ServiceDate != OccurDate)
                            newExcludedDatesList.Add(Service.ServiceDate.Date);

                        //Remove the new Occur date from the ExcludedDates
                        newExcludedDatesList.RemoveAll(ed => ed.Date == OccurDate);

                        Service.RecurringServiceParent.ExcludedDates =
                            newExcludedDatesList;
                    }

                    //Update the ServiceDate
                    if (this.OccurDate != DateTime.MinValue)
                        Service.ServiceDate = this.OccurDate;
                });
        }

        private void ClearSubscriptionsHandlers()
        {
            if (_serviceSubscription == null) return;

            _serviceSubscription.Dispose();
            _serviceSubscription = null;
        }

        #endregion

        /// <summary>
        /// Use this constructor to create a new Service from the passed Service.
        /// </summary>
        public ServiceHolder(Service newService)
        {
            Service = newService;
            OccurDate = newService.ServiceDate;
            ServiceName = newService.ServiceTemplate.Name;

            //The details will already be loaded from the newService
            DetailsLoaded = true;

            //When this is saved, convert it to an existing service
            newService.PropertyChanged += OnSaveConvertToExistingService;
        }

        #endregion

        #region Logic

        #region Public

        /// <summary>
        /// Load or generate the Service.
        /// </summary>
        /// <param name="cancelDetailsLoad">An observable when pushed should cancel the details load.</param>
        /// <param name="loadedCallback">(Optional) A callback to call when the details are loaded.</param>
        public void LoadDetails(Subject<bool> cancelDetailsLoad, Action loadedCallback = null)
        {
            //If this is a new service return (the details are already loaded)
            if (ServiceIsNew)
                return;

            //Reload or regenerate the Service

            #region Load Existing Service details

            if (ExistingServiceId.HasValue)
            {
                Manager.CoreDomainContext.LoadAsync(Manager.CoreDomainContext.GetServiceDetailsForRoleQuery(Manager.Context.RoleId, ExistingServiceId.Value), cancelDetailsLoad)
                    .ContinueWith(task =>
                    {
                        if (task.IsCanceled || !task.Result.Any()) return;
                        Service = task.Result.First();

                        OnLoadServiceDetails(Service);

                        if (loadedCallback != null)
                            loadedCallback();
                    }, TaskScheduler.FromCurrentSynchronizationContext());
            }

            #endregion

            #region Regenerate service

            else if (RecurringServiceId.HasValue)
            {
                Manager.CoreDomainContext.LoadAsync(Manager.CoreDomainContext.GetRecurringServiceDetailsForRoleQuery(Manager.Context.RoleId, RecurringServiceId.Value), cancelDetailsLoad)
                    .ContinueWith(task =>
                    {
                        if (task.IsCanceled || !task.Result.Any()) return;

                        var recurringServiceToGenerateFrom = task.Result.First();
                        OnLoadRecurringServiceDetails(recurringServiceToGenerateFrom);

                        if (loadedCallback != null)
                            loadedCallback();
                    }, TaskScheduler.FromCurrentSynchronizationContext());
            }

            #endregion
        }

        /// <summary>
        /// Load details for multiple service holders.
        /// </summary>
        /// <param name="serviceHoldersToLoad">The service holders to load details for.</param>
        /// <param name="cancel">(Optional) A cancel observable. Push once to cancel this.</param>
        /// <param name="refresh">If false it will not load the details if they are already loaded. Otherwise it will always reload the details. Defaults to true.</param>
        /// <param name="onLoad">Called on a succesful load.</param>
        public static void LoadDetails(IEnumerable<ServiceHolder> serviceHoldersToLoad, IObservable<bool> cancel = null, bool refresh = true, Action onLoad = null)
        {
            //Only load details for existing services
            //New services details are already loaded (and not in the server yet)
            serviceHoldersToLoad = serviceHoldersToLoad.Where(sh => !sh.ServiceIsNew);

            //If false do not load the details if they are already loaded
            if (!refresh)
                serviceHoldersToLoad = serviceHoldersToLoad.Where(sh => !sh.DetailsLoaded);

            //Load the ServiceHolders with existing services and the ServiceHolders to generate seperately
            var serviceHoldersWithExistingServices = serviceHoldersToLoad.Where(sh => sh.ExistingServiceId.HasValue).ToArray();

            var existingServicesLoaded = false;
            var serviceHoldersToGenerateLoaded = false;

            //Load the existing services ServiceHolders
            var existingServicesQuery = Manager.CoreDomainContext.GetServicesDetailsForRoleQuery(Manager.Context.RoleId,
                serviceHoldersWithExistingServices.Select(sh => sh.ExistingServiceId.Value).Distinct());

            Manager.CoreDomainContext.LoadAsync(existingServicesQuery, cancel)
                .ContinueWith(task =>
                {
                    if (task.IsCanceled) return;

                    //Call the finished OnLoadServiceDetails for the loaded existing services
                    if (task.Result.Any())
                    {
                        foreach (var existingService in task.Result)
                        {
                            var serviceHolders = serviceHoldersWithExistingServices.Where(sh => sh.ExistingServiceId == existingService.Id);
                            foreach (var serviceHolder in serviceHolders)
                                serviceHolder.OnLoadServiceDetails(existingService);
                        }
                    }

                    existingServicesLoaded = true;

                    if (onLoad != null && existingServicesLoaded && serviceHoldersToGenerateLoaded)
                        onLoad();
                }, TaskScheduler.FromCurrentSynchronizationContext());

            var serviceHoldersToGenerate = serviceHoldersToLoad.Where(sh => !sh.ExistingServiceId.HasValue && sh.RecurringServiceId.HasValue).ToArray();
            //Load the ServiceHolders to generate 
            var recurringServiceParentsQuery = Manager.CoreDomainContext.GetRecurringServicesWithDetailsForRoleQuery(Manager.Context.RoleId,
               serviceHoldersToGenerate.Select(sh => sh.RecurringServiceId.Value).Distinct());

            Manager.CoreDomainContext.LoadAsync(recurringServiceParentsQuery, cancel)
                .ContinueWith(task =>
                {
                    if (task.IsCanceled) return;

                    //Call the finished OnLoadServiceDetails for the loaded recurring service parents
                    if (task.Result.Any())
                    {
                        foreach (var recurringService in task.Result)
                        {
                            var serviceHolders =
                                serviceHoldersToGenerate.Where(sh => sh.RecurringServiceId == recurringService.Id);
                            foreach (var serviceHolder in serviceHolders)
                                serviceHolder.OnLoadRecurringServiceDetails(recurringService);
                        }
                    }

                    serviceHoldersToGenerateLoaded = true;

                    if (onLoad != null && existingServicesLoaded && serviceHoldersToGenerateLoaded)
                        onLoad();
                }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// Clears the previously loaded details.
        /// </summary>
        private void ClearPreviouslyLoadedDetails()
        {
            DetailsLoaded = false;
            if (Service != null)
            {
                //a) clear the old GeneratedService handlers
                if (ServiceIsGenerated)
                {
                    Service.PropertyChanged -= OnSaveConvertToExistingService;
                    Manager.CoreDomainContext.ChangesRejected -= OnRejectChangedReloadDetails;
                    if (_entityGraph != null)
                        _entityGraph.PropertyChanged -= IfChangesAddToDomainContext;
                }

                //b) clear the Service before reloading
                Service = null;
            }
        }

        /// <summary>
        /// Called when the Service details are loaded.
        /// </summary>
        /// <param name="service">The loaded service for this ServiceHolder.</param>
        public void OnLoadServiceDetails(Service service)
        {
            ClearPreviouslyLoadedDetails();

            this.Service = service;
            _entityGraph = this.Service.EntityGraph();

            this.OccurDate = Service.ServiceDate;
            this.RecurringServiceId = Service.RecurringServiceId;

            DetailsLoaded = true;
        }

        /// <summary>
        /// Called when the parent RecurringService's details are loaded.
        /// </summary>
        /// <param name="recurringService">The loaded recurring service parent for this ServiceHolder.</param>
        public void OnLoadRecurringServiceDetails(RecurringService recurringService)
        {
            ClearPreviouslyLoadedDetails();

            var generatedService = recurringService.GenerateServiceOnDate(OccurDate);
            generatedService.ParentServiceHolder = this;

            _entityGraph = generatedService.EntityGraph();

            //Detach the generated service. Then manually add it to the DomainContext if there are changes
            Manager.Data.DetachEntities(_entityGraph);

            //Track the generated service's entity changes manually because it is detached
            _entityGraph.PropertyChanged += IfChangesAddToDomainContext;
            generatedService.PropertyChanged += OnSaveConvertToExistingService;
            Manager.CoreDomainContext.ChangesRejected += OnRejectChangedReloadDetails;

            Service = generatedService;
            DetailsLoaded = true;
        }

        #endregion

        #region Private Methods for GeneratedServices

        /// <summary>
        /// If this is a generated service when there are changes add this back to the DomainContext.
        /// (Cannot make a lambda, because those cannot be unhandled)
        /// </summary>
        private void IfChangesAddToDomainContext(object sender, PropertyChangedEventArgs e)
        {
            // Check if the property that is changing is a DataMember if not do not consider it as a change.
            if (!sender.GetType().GetProperties().Any(pi => pi.Name == e.PropertyName && pi.IsDefined(typeof(System.Runtime.Serialization.DataMemberAttribute), true)))
                return;

            //Since there are changes, add this back to the DomainContext
            if (ServiceIsGenerated)
                Manager.CoreDomainContext.Services.Add(Service);
        }

        /// <summary>
        /// If the generated service was added to the database. (The EntityState is now Unmodified).
        /// change this from being a GeneratedService to an ExistingService
        /// </summary>
        private void OnSaveConvertToExistingService(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "EntityState" || ((Service)sender).EntityState != EntityState.Unmodified)
                return;

            //Update ExistingServiceId to the (now saved) Service.Id
            this.ExistingServiceId = this.Service.Id;

            //No longer need to listen to the EntityGraph PropertyChanged (for generated services)
            if (_entityGraph != null)
                _entityGraph.PropertyChanged -= IfChangesAddToDomainContext;
        }

        /// <summary>
        /// If this is a generated service, when changes are rejected reload the details.
        /// TODO Optimize
        /// </summary>
        private void OnRejectChangedReloadDetails(ChangedRejectedEventArgs e)
        {
            if (!ServiceIsGenerated || !IsSelected)
                return;

            //Reset OccurDate to originalOccurDate
            OccurDate = _originalOccurDate;

            //Regenerate the Service on RejectChanges in case the generated service was part of the DomainContext 
            //and had its info cleared as part of the RejectChanges

            if (_cancelLastReloadDetails == null)
                _cancelLastReloadDetails = new Subject<bool>();
            _cancelLastReloadDetails.OnNext(true);

            LoadDetails(_cancelLastReloadDetails);
        }

        #endregion

        #endregion
    }
}

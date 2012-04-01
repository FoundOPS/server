using System;
using System.Reactive.Linq;
using System.ServiceModel.DomainServices.Client;
using FoundOps.Common.Silverlight.Tools.ExtensionMethods;
using FoundOps.Common.Silverlight.UI.Interfaces;
using FoundOps.SLClient.Data.Services;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
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
            get
            {
                return !ExistingServiceId.HasValue && !RecurringServiceId.HasValue;
            }
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
                        _entityGraph.Any(e => e.EntityState == EntityState.Modified || e.EntityState == EntityState.New || e.EntityState == EntityState.Deleted));
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

        #endregion

        #region Constructor

        /// <summary>
        /// Use this constructor to create a new Service from the passed Service.
        /// </summary>
        public ServiceHolder(Service newService)
        {
            Service = newService;
            OccurDate = newService.ServiceDate;
            ServiceName = newService.ServiceTemplate.Name;

            //Keep the OccurDate updated with the ServiceDate
            Observable2.FromPropertyChangedPattern(newService, x => x.ServiceDate).ObserveOnDispatcher()
                .Subscribe(newDate => OccurDate = newDate);

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

            //Reload or regenerate the Service

            #region Load Existing Service details

            if (ExistingServiceId.HasValue)
            {
                Manager.CoreDomainContext.LoadAsync(Manager.CoreDomainContext.GetServiceDetailsForRoleQuery(Manager.Context.RoleId, ExistingServiceId.Value), cancelDetailsLoad)
                .ContinueWith(task =>
                {
                    if (task.IsCanceled || !task.Result.Any()) return;
                    Service = task.Result.First();
                    _entityGraph = this.Service.EntityGraph();

                    DetailsLoaded = true;

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

                    var generatedService = recurringServiceToGenerateFrom.GenerateServiceOnDate(OccurDate);
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

                    if (loadedCallback != null)
                        loadedCallback();
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            #endregion
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
        /// </summary>
        private void OnRejectChangedReloadDetails(DomainContext sender, Entity[] rejectedaddedentities, Entity[] rejectedmodifiedentities)
        {
            if (!ServiceIsGenerated || !IsSelected)
                return;

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

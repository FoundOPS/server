using System;
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

        private bool _hasChanges;
        /// <summary>
        /// Used for manual tracking of changes.
        /// This is used because we need to detached generated services from the DomainContext.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [service has changes]; otherwise, <c>false</c>.
        /// </value>
        public new bool HasChanges
        {
            get { return _hasChanges; }
            set
            {
                _hasChanges = value;
                this.RaisePropertyChanged("HasChanges");
            }
        }

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
                return !ExistingServiceId.HasValue;
            }
        }

        #endregion

        #region Locals

        /// <summary>
        /// The existing or generated service's entity graph.
        /// It is stored as a local property so the PropertyChanged event can be unhandled.
        /// </summary>
        private EntityGraph<Entity> _entityGraph = null;

        #endregion

        #region Logic

        #region Public

        /// <summary>
        /// Load or generate the Service.
        /// </summary>
        /// <param name="cancelDetailsLoad">An observable when pushed should cancel the details load.</param>
        public void LoadDetails(Subject<bool> cancelDetailsLoad)
        {
            DetailsLoaded = false;

            //If there is a service already
            //a) clear the entity graph PropertyChanged handlers
            //b) clear HasChanges
            //c) clear the Service before reloading
            if (Service != null)
            {
                if (_entityGraph != null)
                    _entityGraph.PropertyChanged -= CheckHasDataMemberChanges;

                HasChanges = false;
                Service = null;
            }

            if (ExistingServiceId.HasValue)
            {
                Manager.CoreDomainContext.LoadAsync(Manager.CoreDomainContext.GetServiceDetailsForRoleQuery(Manager.Context.RoleId, ExistingServiceId.Value), cancelDetailsLoad)
                .ContinueWith(task =>
                {
                    if (task.IsCanceled || !task.Result.Any()) return;
                    DetailsLoaded = true;
                    Service = task.Result.First();

                    _entityGraph = this.Service.EntityGraph();
                    _entityGraph.PropertyChanged += CheckHasDataMemberChanges;

                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            else if (RecurringServiceId.HasValue)
            {
                Manager.CoreDomainContext.LoadAsync(Manager.CoreDomainContext.GetRecurringServiceDetailsForRoleQuery(Manager.Context.RoleId, RecurringServiceId.Value), cancelDetailsLoad)
                .ContinueWith(task =>
                {
                    if (task.IsCanceled || !task.Result.Any()) return;
                    DetailsLoaded = true;

                    var recurringServiceToGenerateFrom = task.Result.First();

                    var generatedService = recurringServiceToGenerateFrom.GenerateServiceOnDate(OccurDate);
                    generatedService.ParentServiceHolder = this;

                    _entityGraph = generatedService.EntityGraph();

                    //Detach the generated service. Then manually add it to the DomainContext if there are changes
                    Manager.Data.DetachEntities(_entityGraph);

                    //Track the generated service's entity changes manually because it is detached
                    _entityGraph.PropertyChanged += CheckHasDataMemberChanges;

                    Service = generatedService;
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        /// <summary>
        /// A method to call if the generated service was added to the database.
        /// It will change this from being a GeneratedService to an ExistingService
        /// by setting the ExistingServiceId and clearing HasChanges
        /// </summary>
        public void ConvertToExistingService()
        {
            this.ExistingServiceId = this.Service.Id;
            HasChanges = false;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Check if the property that is changing is a DataMember, if not do not consider it as a change.
        /// (Cannot make a lambda, because those cannot be unhandled)
        /// </summary>
        private void CheckHasDataMemberChanges(object sender, PropertyChangedEventArgs e)
        {
            if (!sender.GetType().GetProperties().Any(pi => pi.Name == e.PropertyName && pi.IsDefined(typeof(System.Runtime.Serialization.DataMemberAttribute), true)))
                return;

            HasChanges = true;
        }

        #endregion

        #endregion
    }
}

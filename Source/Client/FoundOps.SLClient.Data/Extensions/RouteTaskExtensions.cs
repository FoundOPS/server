using FoundOps.Common.Silverlight.Interfaces;
using System;
using FoundOps.Common.Silverlight.UI.Interfaces;
using FoundOps.SLClient.Data.Services;

//Partial class must be part of same namespace
// ReSharper disable CheckNamespace
namespace FoundOps.Core.Models.CoreEntities
// ReSharper restore CheckNamespace
{
    public partial class RouteTask : IReject, ILoadDetails
    {
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

        #region Public Properties

        /// <summary>
        /// Gets the generated service route task parent which cloned this RouteTask.
        /// </summary>
        [Obsolete("Delete before optimization push")]
        public RouteTask GeneratedRouteTaskParent { get; private set; }

        /// <summary>
        /// Returns the LocationName of the current RouteTask
        /// </summary>
        public string LocationName
        {
            get
            {
                if (Location != null)
                    return Location.Name;

                return ParentRouteTaskHolder != null ? ParentRouteTaskHolder.LocationName : "";
            }
        }

        /// <summary>
        /// This is the link to the parent RouteTaskHolder.
        /// It will have a value if this was recently generated.
        /// </summary>
        public TaskHolder ParentRouteTaskHolder { get; set; }

        private ServiceHolder _serviceHolder;
        /// <summary>
        /// The ServiceHolder.
        /// </summary>
        public ServiceHolder ServiceHolder
        {
            get { return _serviceHolder; }
            set
            {
                _serviceHolder = value;
                this.RaisePropertyChanged("ServiceHolder");
            }
        }

        #endregion

        #region Logic

        #region Overridden Methods

        partial void OnDateChanged()
        {
            SetupServiceHolder();
            ServiceHolder.OccurDate = Date;
        }

        partial void OnServiceIdChanged()
        {
            SetupServiceHolder();
            ServiceHolder.RecurringServiceId = Service.ServiceProviderId;
            ServiceHolder.ServiceId = ServiceId;
        }

        private void SetupServiceHolder()
        {
            if (ServiceHolder != null) return;


            ServiceHolder = new ServiceHolder { ExistingServiceId = ServiceId, OccurDate = Date, RecurringServiceId = Service == null ? null : Service.RecurringServiceId};
            Manager.Data.DetachEntities(new[] { ServiceHolder });
        }

        #endregion

        ///<summary>
        /// Remove this RouteTask from it's route destination.
        ///</summary>
        public void RemoveRouteDestination()
        {
            this.RouteDestination = null;
            this.RouteDestinationId = null;
        }

        public void Reject()
        {
            this.RejectChanges();
        }

        #endregion
    }
}

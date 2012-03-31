using FoundOps.Common.Silverlight.Interfaces;
using FoundOps.Common.Silverlight.UI.Interfaces;
using FoundOps.SLClient.Data.Services;

//Partial class must be part of same namespace
// ReSharper disable CheckNamespace
namespace FoundOps.Core.Models.CoreEntities
// ReSharper restore CheckNamespace
{
    public partial class RouteTask : IReject, ILoadDetails
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

        private TaskHolder _parentRouteTaskHolder;
        /// <summary>
        /// This is the link to the parent RouteTaskHolder.
        /// It will have a value if this was recently generated.
        /// </summary>
        public TaskHolder ParentRouteTaskHolder
        {
            get { return _parentRouteTaskHolder; }
            set
            {
                _parentRouteTaskHolder = value;
                if (ParentRouteTaskHolder == null)
                {
                    ServiceHolder = null;
                    return;
                }

                ServiceHolder = new ServiceHolder { ExistingServiceId = ParentRouteTaskHolder.ServiceId, OccurDate = ParentRouteTaskHolder.OccurDate, RecurringServiceId = ParentRouteTaskHolder.RecurringServiceId };
            }
        }

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

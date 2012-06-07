using FoundOps.Common.Silverlight.Interfaces;
using FoundOps.Common.Silverlight.UI.Interfaces;

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
                //Cannot clear details loaded. This is prevent issues when saving.
                if (_detailsLoaded)
                    return;

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
                this.RaisePropertyChanged("TaskHolder");
            }
        }

        #endregion

        #region Logic

        /// <summary>
        /// Sets up the ParentRouteTask holder if there is not one yet.
        /// This is used when loading existing RouteTasks.
        /// </summary>
        public void SetupTaskHolder()
        {
            if (this.ParentRouteTaskHolder != null)
                return;

            var taskHolder = new TaskHolder
            {
                ClientId = ClientId,
                ChildRouteTask = this,
                OccurDate = Date,
                LocationName = LocationName,
                ServiceName = Name,
                ServiceId = ServiceId,
                RecurringServiceId = RecurringServiceId
            };

            if (Client != null)
                taskHolder.ClientName = Client.Name;

            if (Location != null)
            {
                if (Location.Region != null)
                    taskHolder.RegionName = Location.Region.Name;

                taskHolder.AddressLine = Location.AddressLineOne;
                taskHolder.Latitude = Location.Latitude;
                taskHolder.Longitude = Location.Longitude;

                taskHolder.LocationId = Location.Id;
            }

            ParentRouteTaskHolder = taskHolder;
        }

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

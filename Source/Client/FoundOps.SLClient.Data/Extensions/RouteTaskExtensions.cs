using System;
using System.Reactive.Linq;
using FoundOps.Common.Silverlight.Interfaces;
using FoundOps.Common.Silverlight.UI.Interfaces;
using FoundOps.Common.Tools;

//Partial class must be part of same namespace
// ReSharper disable CheckNamespace
namespace FoundOps.Core.Models.CoreEntities
// ReSharper restore CheckNamespace
{
    public partial class RouteTask : IReject, ILoadDetails, IGeoLocation
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

        #region Implementation of IGeoLocation

        double IGeoLocation.Latitude
        {
            get { return (double)this.Location.Latitude.Value; }
        }

        double IGeoLocation.Longitude
        {
            get { return (double)this.Location.Longitude.Value; }
        }

        public double LatitudeRad
        {
            get { return DegreeToRadian(((IGeoLocation)this).Latitude); }
        }

        public double LongitudeRad
        {
            get { return DegreeToRadian(((IGeoLocation)this).Longitude); }
        }

        public static double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        public static double RadianToDegree(double angle)
        {
            return angle * (180.0 / Math.PI);
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

                return Location != null ? Location.Name : "";
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

        /// <summary>
        /// Sets up the ServiceHolder on this RouteTask 
        /// Also, sets up tracking changes on that ServiceHolder and moving those changes to the RouteTask
        /// </summary>
        private void InitializeHelper()
        {
            //Initialize the ServiceHolder
            ServiceHolder = new ServiceHolder { ExistingServiceId = ServiceId, OccurDate = Date, RecurringServiceId = RecurringServiceId };

            //Follow property changes and update the ServiceHolder
            this.FromAnyPropertyChanged().Where(e => e.PropertyName == "OccurDate" || e.PropertyName == "RecurringServiceId" || e.PropertyName == "ServiceId")
            .Throttle(TimeSpan.FromMilliseconds(100)).ObserveOnDispatcher().Subscribe(_ =>
            {
                ServiceHolder.OccurDate = this.Date;
                ServiceHolder.RecurringServiceId = this.RecurringServiceId;
                ServiceHolder.ExistingServiceId = this.ServiceId;
            });

            //Follow any ServiceHolder property changes and update this RouteTask
            this.ServiceHolder.FromAnyPropertyChanged().Where(e => e.PropertyName == "OccurDate" || e.PropertyName == "RecurringServiceId" || e.PropertyName == "ExistingServiceId")
            .Throttle(TimeSpan.FromMilliseconds(100)).ObserveOnDispatcher().Subscribe(_ =>
            {
                this.Date = ServiceHolder.OccurDate;
                this.RecurringServiceId = ServiceHolder.RecurringServiceId;
                this.ServiceId = ServiceHolder.ExistingServiceId;
            });
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

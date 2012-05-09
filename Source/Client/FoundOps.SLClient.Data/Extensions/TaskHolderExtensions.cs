using FoundOps.Common.Tools;
using FoundOps.SLClient.Data.Services;
using System;
using System.Reactive.Linq;

//Partial class must be part of same namespace
// ReSharper disable CheckNamespace
namespace FoundOps.Core.Models.CoreEntities
// ReSharper restore CheckNamespace
{
    public partial class TaskHolder : IGeoLocation
    {
        #region Public Properties

        #region Implementation of IGeoLocation

        double IGeoLocation.Latitude
        {
            get { return (double)this.Latitude.Value; }
        }

        double IGeoLocation.Longitude
        {
            get { return (double)this.Longitude; }
        }

        public double LatitudeRad
        {
            get { return DegreeToRadian((double)Latitude.Value); }
        }

        public double LongitudeRad
        {
            get { return DegreeToRadian((double)Longitude.Value); }
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

        private RouteTask _childRouteTask;
        /// <summary>
        /// This is the link to the child RouteTask.
        /// It will have a value if this was recently generated.
        /// </summary>
        public RouteTask ChildRouteTask
        {
            get { return _childRouteTask; }
            set
            {
                _childRouteTask = value;
                this.RaisePropertyChanged("ChildRouteTask");
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

        private void InitializeHelper()
        {
            //Initialize the ServiceHolder
            ServiceHolder = new ServiceHolder { ExistingServiceId = ServiceId, OccurDate = OccurDate, RecurringServiceId = RecurringServiceId };

            //Follow property changes and update the ServiceHolder
            this.FromAnyPropertyChanged().Where(e => e.PropertyName == "OccurDate" || e.PropertyName == "RecurringServiceId" || e.PropertyName == "ServiceId")
            .Throttle(TimeSpan.FromMilliseconds(100)).ObserveOnDispatcher().Subscribe(_ =>
            {
                ServiceHolder.OccurDate = this.OccurDate;
                ServiceHolder.RecurringServiceId = this.RecurringServiceId;
                ServiceHolder.ExistingServiceId = this.ServiceId;
            });

            //Follow any ServiceHolder property changes and update this TaskHolder
            this.ServiceHolder.FromAnyPropertyChanged().Where(e => e.PropertyName == "OccurDate" || e.PropertyName == "RecurringServiceId" || e.PropertyName == "ExistingServiceId")
            .Throttle(TimeSpan.FromMilliseconds(100)).ObserveOnDispatcher().Subscribe(_ =>
            {
                this.OccurDate = ServiceHolder.OccurDate;
                this.RecurringServiceId = ServiceHolder.RecurringServiceId;
                this.ServiceId = ServiceHolder.ExistingServiceId;
            });
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates the route task.
        /// </summary>
        public void CreateRouteTask()
        {
            var routedTask = new RouteTask
            {
                Id = Guid.NewGuid(),
                BusinessAccountId = Manager.Context.OwnerAccount.Id,
                ClientId = ClientId,
                Date = OccurDate,
                LocationId = LocationId,
                Name = ServiceName,
                ParentRouteTaskHolder = this,
                ServiceId = ServiceId,
                RecurringServiceId = RecurringServiceId//,
                //StatusInt = StatusInt
            };

            this.ChildRouteTask = routedTask;
        }

        #endregion
    }
}

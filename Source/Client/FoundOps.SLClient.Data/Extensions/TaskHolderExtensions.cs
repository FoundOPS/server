using FoundOps.Common.Silverlight.Tools.ExtensionMethods;
using FoundOps.Common.Tools;
using FoundOps.SLClient.Data.Services;
using System;
using System.Reactive.Linq;

//Partial class must be part of same namespace
// ReSharper disable CheckNamespace
namespace FoundOps.Core.Models.CoreEntities
// ReSharper restore CheckNamespace
{
    public partial class TaskHolder
    {
        #region Public Properties

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

        //if (ParentRouteTaskHolder == null)
        //    {
        //        ServiceHolder = null;
        //        return;
        //    }

        //    ServiceHolder = new ServiceHolder { ExistingServiceId = ParentRouteTaskHolder.ServiceId, OccurDate = ParentRouteTaskHolder.OccurDate, RecurringServiceId = ParentRouteTaskHolder.RecurringServiceId };

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
                RecurringServiceId = RecurringServiceId,
                StatusInt = StatusInt
            };

            this.ChildRouteTask = routedTask;
        }

        #endregion
    }
}

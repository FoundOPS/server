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
                ServiceHolder.RecurringServiceId = this.RecurringServiceId;
                ServiceHolder.ExistingServiceId = this.ServiceId;
            });

            //Follow any ServiceHolder property changes and update this TaskHolder
            this.ServiceHolder.FromAnyPropertyChanged().Where(e => e.PropertyName == "RecurringServiceId" || e.PropertyName == "ExistingServiceId")
            .Throttle(TimeSpan.FromMilliseconds(100)).ObserveOnDispatcher().Subscribe(_ =>
            {
                this.RecurringServiceId = ServiceHolder.RecurringServiceId;
                this.ServiceId = ServiceHolder.ExistingServiceId;
            });
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Takes a TaskHolder and creates a RouteTask.
        /// </summary>
        /// <param name="taskHolder">The parent taskHolder</param>
        /// <returns>The new RouteTask.</returns>
        public static RouteTask ConvertToRouteTask(TaskHolder taskHolder)
        {
            var routedTask = new RouteTask
            {
                Id = Guid.NewGuid(),
                BusinessAccountId = Manager.Context.OwnerAccount.Id,
                ClientId = taskHolder.ClientId,
                Date = taskHolder.OccurDate,
                LocationId = taskHolder.LocationId,
                Name = taskHolder.ServiceName,
                ParentRouteTaskHolder = taskHolder,
                ServiceId = taskHolder.ServiceId,
                RecurringServiceId = taskHolder.RecurringServiceId
            };

            return routedTask;
        }

        #endregion
    }
}

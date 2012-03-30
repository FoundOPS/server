using System;
using FoundOps.SLClient.Data.Services;

//Partial class must be part of same namespace
// ReSharper disable CheckNamespace
namespace FoundOps.Core.Models.CoreEntities
// ReSharper restore CheckNamespace
{
    public partial class TaskHolder
    {
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
                ClientId = taskHolder.ClientId,
                Date = taskHolder.OccurDate,
                LocationId = taskHolder.LocationId,
                Name = taskHolder.ServiceName,
                ParentRouteTaskHolder = taskHolder,
                ServiceId = taskHolder.ServiceId
            };

            return routedTask;
        }

        #endregion
    }
}

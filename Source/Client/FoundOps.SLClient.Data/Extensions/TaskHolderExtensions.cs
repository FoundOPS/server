using System;

//Partial class must be part of same namespace
// ReSharper disable CheckNamespace
namespace FoundOps.Core.Models.CoreEntities
// ReSharper restore CheckNamespace
{
    public partial class TaskHolder
    {
        public static RouteTask ConvertToRouteTask(TaskHolder taskholder)
        {
            var routedTask = new RouteTask
            {
                Id = Guid.NewGuid(),
                ClientId = taskholder.ClientId,
                Date = taskholder.OccurDate,
                LocationId = taskholder.LocationId,
                Name = taskholder.ServiceName,
                ParentRouteTaskHolder = taskholder,
                ServiceId = taskholder.ServiceId
            };

            return routedTask;
        }
    }
}

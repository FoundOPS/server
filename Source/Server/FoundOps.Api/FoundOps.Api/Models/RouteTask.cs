﻿using System;

namespace FoundOps.Api.Models
{
    public class RouteTask : ITrackable
    {
        public Guid Id { get; set; }

        public DateTime CreatedDate { get; private set; }
        public DateTime? LastModified { get; private set; }
        public Guid? LastModifyingUserId { get; private set; }

        public DateTime Date { get; set; }
        public string Name { get; set; }

        public Guid? RecurringServiceId { get; set; }
        public Guid? ServiceId { get; set; }

        public Guid? TaskStatusId { get; set; }

        public RouteTask()
        {
            CreatedDate = DateTime.UtcNow;
        }

        public static RouteTask ConvertModel(FoundOps.Core.Models.CoreEntities.RouteTask routeTaskModel)
        {
            var routeTask = new RouteTask
                {
                    Id = routeTaskModel.Id, 
                    CreatedDate = routeTaskModel.CreatedDate,
                    Name = routeTaskModel.Name, 
                    Date = routeTaskModel.Date, 
                    RecurringServiceId = routeTaskModel.RecurringServiceId, 
                    ServiceId = routeTaskModel.ServiceId, 
                    TaskStatusId = routeTaskModel.TaskStatusId
                };

            routeTask.SetLastModified(routeTaskModel.LastModified, routeTaskModel.LastModifyingUserId);

            return routeTask;
        }

        public void SetLastModified(DateTime? lastModified, Guid? userId)
        {
            LastModified = lastModified;
            LastModifyingUserId = userId;
        }
    }
}

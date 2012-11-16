using System;

namespace FoundOps.Api.Models
{
    public class RouteTask
    {
        public Guid Id { get; set; }

        public DateTime Date { get; set; }
        public string Name { get; set; }

        public Guid? RecurringServiceId { get; set; }
        public Guid? ServiceId { get; set; }

        public Guid? TaskStatusId { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public Guid? LastModifyingUserId { get; set; }

        public static RouteTask ConvertModel(FoundOps.Core.Models.CoreEntities.RouteTask routeTaskModel)
        {
            var routeTask = new RouteTask
                {
                    Id = routeTaskModel.Id, 
                    Name = routeTaskModel.Name, 
                    Date = routeTaskModel.Date, 
                    RecurringServiceId = routeTaskModel.RecurringServiceId, 
                    ServiceId = routeTaskModel.ServiceId, 
                    TaskStatusId = routeTaskModel.TaskStatusId,
                    CreatedDate = routeTaskModel.CreatedDate,
                    LastModifiedDate = routeTaskModel.LastModifiedDate,
                    LastModifyingUserId = routeTaskModel.LastModifyingUserId
                };

            return routeTask;
        }
    }
}

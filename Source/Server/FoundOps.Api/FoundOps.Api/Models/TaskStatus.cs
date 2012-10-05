using System;

namespace FoundOps.Api.Models
{
    public class TaskStatus
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public Guid? BusinessAccountId { get; set; }
        public int? DefaultTypeInt { get; set; }
        public bool RemoveFromRoute { get; set; }

        public static TaskStatus ConvertModel(FoundOps.Core.Models.CoreEntities.TaskStatus model)
        {
            var status = new TaskStatus
            {
                Id = model.Id,
                Name = model.Name,
                Color = model.Color,
                DefaultTypeInt = model.DefaultTypeInt,
                RemoveFromRoute = model.RemoveFromRoute,
                BusinessAccountId = model.BusinessAccountId
            };

            return status;
        }

        public static FoundOps.Core.Models.CoreEntities.TaskStatus CreateFromModel(TaskStatus taskStatus)
        {
            var status = new FoundOps.Core.Models.CoreEntities.TaskStatus
            {
                Id = taskStatus.Id,
                Name = taskStatus.Name,
                Color = taskStatus.Color,
                DefaultTypeInt = taskStatus.DefaultTypeInt,
                RemoveFromRoute = taskStatus.RemoveFromRoute,
                BusinessAccountId = taskStatus.BusinessAccountId
            };

            return status;
        }
    }
}
using System;

namespace FoundOps.Api.Models
{
    public class TaskStatus : ITrackable
    {
        public Guid Id { get; set; }

        public DateTime CreatedDate { get; private set; }
        public DateTime? LastModified { get; private set; }
        public Guid? LastModifyingUserId { get; private set; }

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
                CreatedDate = model.CreatedDate,
                Name = model.Name,
                Color = model.Color,
                DefaultTypeInt = model.DefaultTypeInt,
                RemoveFromRoute = model.RemoveFromRoute,
                BusinessAccountId = model.BusinessAccountId
            };

            status.SetLastModified(model.LastModified, model.LastModifyingUserId);

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
                BusinessAccountId = taskStatus.BusinessAccountId,
                CreatedDate = taskStatus.CreatedDate,
                LastModified = taskStatus.LastModified,
                LastModifyingUserId = taskStatus.LastModifyingUserId
            };

            return status;
        }

        public void SetLastModified(DateTime? lastModified, Guid? userId)
        {
            LastModified = lastModified;
            LastModifyingUserId = userId;
        }
    }
}
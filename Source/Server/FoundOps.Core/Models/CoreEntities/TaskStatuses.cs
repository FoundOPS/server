using System;
using System.Collections.Generic;

namespace FoundOps.Core.Models.CoreEntities
{
    public static class TaskStatuses
    {
        public static IEnumerable<TaskStatus> CreateDefaultTaskStatuses()
        {
            var taskStatuses = new List<TaskStatus>();

            var taskStatus = new TaskStatus
            {
                Id = Guid.NewGuid(),
                Name = "Created",
                Color = "#FFFFFF",
                DefaultTypeInt = ((int)StatusDetail.CreatedDefault),
                RemoveFromRoute = true
            };

            taskStatuses.Add(taskStatus);

            taskStatus = new TaskStatus
            {
                Id = Guid.NewGuid(),
                Name = "Routed",
                Color = "#0D9EFF",
                DefaultTypeInt = ((int)StatusDetail.RoutedDefault),
                RemoveFromRoute = false
            };

            taskStatuses.Add(taskStatus);

            taskStatus = new TaskStatus
            {
                Id = Guid.NewGuid(),
                Name = "On Hold",
                Color = "#FFCC44",
                RemoveFromRoute = false
            };

            taskStatuses.Add(taskStatus);

            taskStatus = new TaskStatus
            {
                Id = Guid.NewGuid(),
                Name = "Completed",
                Color = "#32CD32",
                DefaultTypeInt = ((int)StatusDetail.CompletedDefault),
                RemoveFromRoute = false
            };

            taskStatuses.Add(taskStatus);

            return taskStatuses.ToArray();
        }
    }
}
using System;
using FoundOps.Core.Models.CoreEntities;

namespace FoundOPS.API.Models
{
    public class TaskStatus
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public Guid? BusinessAccountId { get; set; }
        public int? DefaultTypeInt { get; set; }
        public bool RouteRequired { get; set; }

        public static TaskStatus ConvertModel(FoundOps.Core.Models.CoreEntities.TaskStatus modelStatus)
        {
            var status = new TaskStatus
                             {
                                 Id = modelStatus.Id,
                                 Name = modelStatus.Name,
                                 Color = modelStatus.Color,
                                 DefaultTypeInt = modelStatus.DefaultTypeInt,
                                 RouteRequired = modelStatus.RouteRequired,
                                 BusinessAccountId = modelStatus.BusinessAccountId
                             };

            return status;
        }

        public static FoundOps.Core.Models.CoreEntities.TaskStatus ConvertFromModel(TaskStatus taskStatus)
        {
            var status = new FoundOps.Core.Models.CoreEntities.TaskStatus
                             {
                                 Id = taskStatus.Id,
                                 Name = taskStatus.Name,
                                 Color = taskStatus.Color,
                                 DefaultTypeInt = taskStatus.DefaultTypeInt,
                                 RouteRequired = taskStatus.RouteRequired,
                                 BusinessAccountId = taskStatus.BusinessAccountId
                             };

            return status;
        }
    }
}
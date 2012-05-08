using System;
using System.Collections.Generic;

namespace FoundOPS.API.Models
{
    public class TaskStatus
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public bool RoutedStatusBool { get; set; }
        public bool CreationDefaultBool { get; set; }
        public bool RoutedDefaultBool { get; set; }
        public bool CompletedDefaultBool { get; set; }
        public Guid BusinessAccountId { get; set; }

        public static FoundOps.Core.Models.CoreEntities.TaskStatus ConvertModel(TaskStatus modelStatus)
        {
            var status = new FoundOps.Core.Models.CoreEntities.TaskStatus
                             {
                                 Name = modelStatus.Name,
                                 Color = modelStatus.Color,
                                 RoutedStatusBool = modelStatus.RoutedStatusBool,
                                 CreationDefaultBool = modelStatus.CreationDefaultBool,
                                 RoutedDefaultBool = modelStatus.RoutedDefaultBool,
                                 CompletedDefaultBool = modelStatus.CompletedDefaultBool,
                                 BusinessAccountId = modelStatus.BusinessAccountId
                             };

            return status;
        }
    }
}
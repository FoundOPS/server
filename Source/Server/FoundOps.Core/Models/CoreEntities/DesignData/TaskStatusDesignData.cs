using System;
using System.Collections.Generic;

namespace FoundOps.Core.Models.CoreEntities.DesignData
{
    public class TaskStatusDesignData
    {
        public static string[] DesignColors = new[] { "#FFFF00", "#FF0000", "#FFA500", "#4682B4", "#00FF00", "#FF00FF" };

        public IEnumerable<TaskStatus> DesignStatus = new List<TaskStatus>
                                                   {
                                                       new TaskStatus
                                                           {
                                                               Id = Guid.NewGuid(),
                                                               Name = "Unrouted",
                                                               Color = DesignColors[0],
                                                               RouteRequired = false,
                                                               DefaultTypeInt = ((int)StatusDetail.CreatedDefault)
                                                           },
                                                           new TaskStatus
                                                           {
                                                               Id = Guid.NewGuid(),
                                                               Name = "Routed",
                                                               Color = DesignColors[1],
                                                               RouteRequired = true,
                                                               DefaultTypeInt = ((int)StatusDetail.RoutedDefault)
                                                           },
                                                           new TaskStatus
                                                           {
                                                               Id = Guid.NewGuid(),
                                                               Name = "In Progress",
                                                               Color = DesignColors[2],
                                                               RouteRequired = true,
                                                               DefaultTypeInt = null
                                                           },
                                                           new TaskStatus
                                                           {
                                                               Id = Guid.NewGuid(),
                                                               Name = "On Hold",
                                                               Color = DesignColors[3],
                                                               RouteRequired = false,
                                                               DefaultTypeInt = null
                                                           },
                                                           new TaskStatus
                                                           {
                                                               Id = Guid.NewGuid(),
                                                               Name = "Incomplete",
                                                               Color = DesignColors[4],
                                                               RouteRequired = false,
                                                               DefaultTypeInt = null
                                                           },
                                                           new TaskStatus
                                                           {
                                                               Id = Guid.NewGuid(),
                                                               Name = "Completed",
                                                               Color = DesignColors[5],
                                                               RouteRequired = true,
                                                               DefaultTypeInt = ((int)StatusDetail.CompletedDefault)
                                                           },
                                                   };
    }
}

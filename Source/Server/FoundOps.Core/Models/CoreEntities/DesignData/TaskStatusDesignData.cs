using System;
using System.Collections.Generic;

namespace FoundOps.Core.Models.CoreEntities.DesignData
{
    public class TaskStatusDesignData
    {
        public static string[] DesignColors = new[] { "FFFF0000", "FF00FF00", "FF0000FF", "FFFFFF00", "FFFF00FF", "FF00FFFF" };

        public IEnumerable<TaskStatus> DesignStatus = new List<TaskStatus>
                                                   {
                                                       new TaskStatus
                                                           {
                                                               Id = Guid.NewGuid(),
                                                               Name = "UnRouted",
                                                               Color = DesignColors[0],
                                                               StatusDetailInt = 13
                                                           },
                                                           new TaskStatus
                                                           {
                                                               Id = Guid.NewGuid(),
                                                               Name = "Routed",
                                                               Color = DesignColors[1],
                                                               StatusDetailInt = 24
                                                           },
                                                           new TaskStatus
                                                           {
                                                               Id = Guid.NewGuid(),
                                                               Name = "In Progress",
                                                               Color = DesignColors[2],
                                                               StatusDetailInt = 2
                                                           },
                                                           new TaskStatus
                                                           {
                                                               Id = Guid.NewGuid(),
                                                               Name = "On Hold",
                                                               Color = DesignColors[3],
                                                               StatusDetailInt = null
                                                           },
                                                           new TaskStatus
                                                           {
                                                               Id = Guid.NewGuid(),
                                                               Name = "Incomplete",
                                                               Color = DesignColors[4],
                                                               StatusDetailInt = 1
                                                           },
                                                           new TaskStatus
                                                           {
                                                               Id = Guid.NewGuid(),
                                                               Name = "Completed",
                                                               Color = DesignColors[5],
                                                               StatusDetailInt = 25
                                                           },
                                                   };
    }
}

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
        public int? StatusDetailInt { get; set; }
        public string InRouteString
        {
            get
            {
                var statusDetailString = StatusDetailInt.ToString();

                if (statusDetailString.Contains("1"))
                    return "";
                
                
                return "Checked";
           }

            set { throw new NotImplementedException(); }
        }

        public static TaskStatus ConvertModel(FoundOps.Core.Models.CoreEntities.TaskStatus modelStatus)
        {
            var status = new TaskStatus
                             {
                                 Name = modelStatus.Name,
                                 Color = modelStatus.Color,
                                 StatusDetailInt = modelStatus.StatusDetailInt,
                                 BusinessAccountId = modelStatus.BusinessAccountId
                             };

            return status;
        }

        public static FoundOps.Core.Models.CoreEntities.TaskStatus ConvertFromModel(TaskStatus taskStatus)
        {
            var status = new FoundOps.Core.Models.CoreEntities.TaskStatus
                             {
                                 Name = taskStatus.Name,
                                 Color = taskStatus.Color,
                                 StatusDetailInt = taskStatus.StatusDetailInt,
                                 BusinessAccountId = taskStatus.BusinessAccountId
                             };

            return status;
        }
    }
}
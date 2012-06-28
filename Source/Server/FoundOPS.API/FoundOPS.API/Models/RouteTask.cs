using System;

namespace FoundOPS.API.Models
{
    public class RouteTask
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public static RouteTask ConvertModel(FoundOps.Core.Models.CoreEntities.RouteTask routeTaskModel)
        {
            var routeTask = new RouteTask { Id = routeTaskModel.Id, Name = routeTaskModel.Name };

            return routeTask;
        }
    }
}

using FoundOps.Api.Models;
using FoundOps.Api.Tools;
using FoundOps.Core.Tools;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace FoundOps.Api.Controllers.Rest
{
    public class RouteTasksController : BaseApiController
    {
        // PUT api/routetasks
        /// <summary>
        /// Used to update a RouteTask.
        /// Right now you can only change it's status.
        /// </summary>
        /// <param name="routeTask">The task to update</param>
        /// <returns>The result</returns>
        public HttpResponseMessage Put(RouteTask routeTask)
        {
            var routeTaskModel = CoreEntitiesContainer.RouteTasks.FirstOrDefault(rt => rt.Id == routeTask.Id);
            if (routeTaskModel == null)
                return Request.NotFound();

            //Check the user has access to the
            if (!CoreEntitiesContainer.BusinessAccount(routeTaskModel.BusinessAccountId).Any())
                throw Request.NotAuthorized();

            //status is the only thing that can change
            routeTaskModel.TaskStatusId = routeTask.TaskStatusId;

            //find the status
            var taskStatus = CoreEntitiesContainer.TaskStatuses.FirstOrDefault(ts => ts.Id == routeTask.TaskStatusId);
            if (taskStatus == null)
                return Request.NotFound("Status");

            //Remove the task from the route if the task status says to
            if (taskStatus.RemoveFromRoute && routeTaskModel.RouteDestinationId.HasValue)
            {
                routeTaskModel.RouteDestinationReference.Load();
                routeTaskModel.RouteDestination.RouteTasks.Load();

                //if this is the only task, delete the route destination
                if (routeTaskModel.RouteDestination.RouteTasks.Count() == 1)
                {
                    CoreEntitiesContainer.RouteDestinations.DeleteObject(routeTaskModel.RouteDestination);
                }
                //otherwise just remove this task
                else
                {
                    routeTaskModel.RouteDestinationId = null;
                }
            }

            SaveWithRetry();
            return Request.CreateResponse(HttpStatusCode.Accepted);
        }
    }
}
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Tools;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Location = FoundOPS.API.Models.Location;
using Route = FoundOPS.API.Models.Route;
using RouteTask = FoundOPS.API.Models.RouteTask;

namespace FoundOPS.API.Api
{
    /// <summary>
    /// An api controller which exposes Routes.
    /// </summary>
    [FoundOps.Core.Tools.Authorize]
    public class RoutesController : ApiController
    {
        private readonly CoreEntitiesContainer _coreEntitiesContainer;

        public RoutesController()
        {
            _coreEntitiesContainer = new CoreEntitiesContainer();
            _coreEntitiesContainer.ContextOptions.LazyLoadingEnabled = false;
        }

        #region Get

        // GET /api/route?roleId={Guid}
        /// <summary>
        /// Returns Routes, RouteDestinations and their Client and Location
        /// Ordered by name
        /// </summary>
        /// <param name="roleId">Find routes for a business account. TODO Remove: if null it will return the first business the user has access to.</param>
        /// <param name="serviceDateUtc">The date of the service (in UTC). If null it will return todays routes.</param>
        /// <param name="deep">Load the RouteTasks and contact info for the RouteDestination's Clients and Locations. TODO: Change to default to false</param>
        [AcceptVerbs("GET", "POST")]
        public IQueryable<Route> GetRoutes(Guid? roleId, DateTime? serviceDateUtc, bool? deep)
        {
            //TODO remove following two lines once mobile app is updated (and change deep to false)
            var userAccount = _coreEntitiesContainer.CurrentUserAccount().Include(ua => ua.RoleMembership).First();
            var role = roleId.HasValue
                           ? roleId.Value
                           : userAccount.RoleMembership.First().Id;

            //Find routes for the passed service date
            //if the date is null use today (adjusted for the user)
            var date = serviceDateUtc.HasValue
                           ? serviceDateUtc.Value.Date
                           : userAccount.Now().Date;
            //TODO replace above with when mobile app is updated
            //: _coreEntitiesContainer.CurrentUserAccount().First().Now().Date;

            var loadedRoutes = _coreEntitiesContainer.Owner(role).Include(ba => ba.Routes)
                            .SelectMany(ba => ba.Routes).Where(r => r.Date == date)
                            .Include(r => r.RouteDestinations)
                            .Include("RouteDestinations.Client").Include("RouteDestinations.Location");

            if (!deep.HasValue || deep.Value)
            {
                loadedRoutes = loadedRoutes.Include("RouteDestinations.RouteTasks")
                    .Include("RouteDestinations.Client.ContactInfoSet")
                    .Include("RouteDestinations.Location.ContactInfoSet");
            }

            //Order the routes by name
            return loadedRoutes.OrderBy(r => r.Name).Select(Route.ConvertModel).AsQueryable();
        }

        /// <summary>
        /// Gets the depot Location(s) for a BusinessAccount based on the roleId
        /// </summary>
        /// <param name="roleId">Used to get the BusinessAccount</param>
        [AcceptVerbs("GET", "POST")]
        public IQueryable<Location> GetDepots(Guid roleId)
        {
            var currentBusinessAccount = _coreEntitiesContainer.Owner(roleId).Include(ba => ba.Depots).FirstOrDefault();

            if (currentBusinessAccount == null)
                ExceptionHelper.ThrowNotAuthorizedBusinessAccount();

            return currentBusinessAccount.Depots.Select(Location.ConvertModel).AsQueryable();
        }
        #endregion

        #region Post

        /// <summary>
        /// Pushes changes made to a RouteTask from the API model to the FoundOPS model
        /// </summary>
        /// <param name="routeTask">The API model of a RouteTask</param>
        [AcceptVerbs("POST")]
        public HttpResponseMessage UpdateRouteTask(RouteTask routeTask)
        {
            var routeTaskModel = _coreEntitiesContainer.RouteTasks.First(rt => rt.Id == routeTask.Id);

            //status is the only thing that can change
            routeTaskModel.TaskStatusId = routeTask.TaskStatusId;

            var taskStatus = _coreEntitiesContainer.TaskStatuses.FirstOrDefault(ts => ts.Id == routeTask.TaskStatusId);
            if (taskStatus == null)
                return Request.CreateResponse(HttpStatusCode.NotFound, "Task statuses have been changed. Please reload Routes");

            //Remove the task from the route if the task status says to
            if (taskStatus.RemoveFromRoute && routeTaskModel.RouteDestinationId.HasValue)
            {
                routeTaskModel.RouteDestinationReference.Load();
                routeTaskModel.RouteDestination.RouteTasks.Load();

                //if this is the only task, delete the route destination
                if (routeTaskModel.RouteDestination.RouteTasks.Count() == 1)
                {
                    _coreEntitiesContainer.RouteDestinations.DeleteObject(routeTaskModel.RouteDestination);
                }
                //otherwise just remove this task
                else
                {
                    routeTaskModel.RouteDestinationId = null;
                }
            }

            _coreEntitiesContainer.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.Accepted);
        }

        #endregion
    }
}

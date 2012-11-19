using FoundOps.Api.Tools;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Tools;
using System;
using System.Data.Entity;
using System.Linq;
using Route = FoundOps.Api.Models.Route;

namespace FoundOps.Api.Controllers.Rest
{
    /// <summary>
    /// An api controller which exposes Routes.
    /// </summary>
    [Authorize]
    public class RoutesController : BaseApiController
    {
        /// <summary>
        /// Returns Routes, RouteDestinations, Employees and their Client and Location
        /// Ordered by name
        /// </summary>
        /// <param name="roleId">Find routes for a business account</param>
        /// <param name="serviceDateUtc">The date of the service (in UTC). If null it will return todays routes.</param>
        /// <param name="deep">Load the RouteTasks and contact info for the RouteDestination's Clients and Locations. Defaults to false</param>
        /// <param name="assigned">Defaults to false. Only return routes the current user is assigned to</param>
        public IQueryable<Route> Get(Guid roleId, DateTime? serviceDateUtc, bool? deep = false, bool? assigned = false)
        {
            Request.CheckAuthentication();

            //load the user account if it is used
            UserAccount userAccount = null;
            if (!serviceDateUtc.HasValue || (assigned.HasValue && assigned.Value))
            {
                userAccount = CoreEntitiesContainer.CurrentUserAccount();
            }

            //Find routes for the passed service date
            //if the date is null use today (adjusted for the user)
            var date = serviceDateUtc.HasValue
                           ? serviceDateUtc.Value.Date
                           : userAccount.Now().Date;

            var loadedRoutes = CoreEntitiesContainer.Owner(roleId, new [] { RoleType.Administrator, RoleType.Regular, RoleType.Mobile }).Include(ba => ba.Routes)
                            .SelectMany(ba => ba.Routes).Where(r => r.Date == date)
                            .Include(r => r.RouteDestinations).Include(e => e.Employees)
                            .Include("RouteDestinations.Client").Include("RouteDestinations.Location");

            //only load routes for the current user account's employee (for this role)
            if (assigned.HasValue && assigned.Value)
                loadedRoutes = loadedRoutes.Where(r => r.Employees.Any(e => e.LinkedUserAccountId == userAccount.Id));

            if (deep.HasValue && deep.Value)
            {
                loadedRoutes = loadedRoutes.Include("RouteDestinations.RouteTasks")
                    .Include("RouteDestinations.Client.ContactInfoSet")
                    .Include("RouteDestinations.Location.ContactInfoSet");
            }

            //Order the routes by name
            return loadedRoutes.OrderBy(r => r.Name).Select(Route.ConvertModel).AsQueryable();
        }
    }
}

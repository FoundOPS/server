using FoundOps.Api.Models;
using FoundOps.Core.Tools;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;

namespace FoundOps.Api.Controllers.Rest
{
    /// <summary>
    /// An api controller which exposes Routes.
    /// </summary>
    [FoundOps.Core.Tools.Authorize]
    public class RoutesController : BaseApiController
    {
        // GET /api/route?roleId={Guid}
        /// <summary>
        /// Returns Routes, RouteDestinations and their Client and Location
        /// Ordered by name
        /// </summary>
        /// <param name="roleId">Find routes for a business account. TODO Remove: if null it will return the first business the user has access to.</param>
        /// <param name="serviceDateUtc">The date of the service (in UTC). If null it will return todays routes.</param>
        /// <param name="deep">Load the RouteTasks and contact info for the RouteDestination's Clients and Locations. TODO: Change to default to false</param>
        /// <param name="assigned">Defaults to false. Only return routes the current user is assigned to</param>
        [AcceptVerbs("GET", "POST")]
        public IQueryable<Route> GetRoutes(Guid? roleId, DateTime? serviceDateUtc, bool? deep, bool? assigned)
        {
            //TODO remove following two lines once mobile app is updated (and change deep to false)
            var userAccount = CoreEntitiesContainer.CurrentUserAccount().Include(ua => ua.RoleMembership).First();
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

            var loadedRoutes = CoreEntitiesContainer.Owner(role).Include(ba => ba.Routes)
                            .SelectMany(ba => ba.Routes).Where(r => r.Date == date)
                            .Include(r => r.RouteDestinations)
                            .Include("RouteDestinations.Client").Include("RouteDestinations.Location");

            if (assigned.HasValue && assigned.Value)
            {
                loadedRoutes = loadedRoutes.Where(r => r.Employees.Any(e => e.LinkedUserAccountId == userAccount.Id));
            }

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
            var businessAccount = GetBusinessAccount(roleId, null, new[] { "Depots" });
            return businessAccount.Depots.Select(Location.ConvertModel).AsQueryable();
        }
    }
}

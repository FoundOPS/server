using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Tools;
using Route = FoundOPS.API.Models.Route;
using Location = FoundOPS.API.Models.Location;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;

namespace FoundOPS.API.Controllers
{
#if !DEBUG
    [Authorize]
#endif
    /// <summary>
    /// An api controller which exposes Routes.
    /// </summary>
    public class RoutesController : ApiController
    {
        private readonly CoreEntitiesContainer _coreEntitiesContainer = new CoreEntitiesContainer();

        #region Get

        // GET /api/route <- User must be authenticated and have Routes for today
        // GET /api/route?roleId={Guid}
        /// <summary>
        /// Returns Routes & RouteDestinations & RouteDestinations.Client & Client.OwnedParty.ContactInfoSet
        /// & RouteDestinations.Location & Location.ContactInfoSet.
        /// Ordered by name.
        /// </summary>
        /// <param name="roleId">If the roleId has a value, it will find routes for a business account. Otherwise it will find routes for the current user account.</param>
        public IQueryable<Route> GetRoutes(Guid? roleId)
        {
            var apiRoutes = new List<Route>();

            //Get the UtcDate of today
            var today = DateTime.UtcNow.Date;

            IEnumerable<FoundOps.Core.Models.CoreEntities.Route> loadedRoutes;

            //If there is a roleId: return the Routes for the role's BusinessAccount (Map View)
            //Otherwise: return the current user account's Routes (Mobile Application)
            if (roleId.HasValue)
            {
                loadedRoutes = _coreEntitiesContainer.BusinessAccountOwnerOfRoleQueryable(roleId.Value).Include(ba => ba.Routes)
                    .Include("Routes.RouteDestinations").Include("Routes.RouteDestinations.Client").Include("Routes.RouteDestinations.Client.ContactInfoSet")
                    .Include("Routes.RouteDestinations.Location").Include("Routes.RouteDestinations.Location.ContactInfoSet")
                    .SelectMany(ba => ba.Routes).Where(r => r.Date == today);
            }
            else
            {
                //Finds all LinkedEmployees for the CurrentUserAccount
                //Finds all Routes (today) associated with those Employees
                loadedRoutes = AuthenticationLogic.CurrentUserAccountQueryable(_coreEntitiesContainer)
                               .SelectMany(cu => cu.LinkedEmployees)
                               .SelectMany(e => e.Routes).Where(r => r.Date == today);
            }

            //Converts the FoundOPS model Routes to the API model Routes
            //Adds those APIRoutes to the list of APIRoutes to return
            apiRoutes.AddRange(loadedRoutes.Select(Route.ConvertModel));

            //Order the routes by name
            return apiRoutes.OrderBy(r => r.Name).AsQueryable();
        }

        /// <summary>
        /// Gets the depot Location(s) for a BusinessAccount based on the roleId
        /// </summary>
        /// <param name="roleId">Used to get the BusinessAccount</param>
        public IQueryable<Location> GetDepots(Guid roleId)
        {
            var currentBusinessAccount = _coreEntitiesContainer.BusinessAccountOwnerOfRoleQueryable(roleId).FirstOrDefault();

            if (currentBusinessAccount == null)
                ExceptionHelper.ThrowNotAuthorizedBusinessAccount();

            return currentBusinessAccount.Depots.Select(Location.ConvertModel).AsQueryable();
        }

        //public IQueryable<Status> GetStatuses(Guid roleId)
        //{
        //    var currentBusinessAccount = _coreEntitiesContainer.BusinessAccountOwnerOfRoleQueryable(roleId).FirstOrDefault();

        //    if (currentBusinessAccount == null)
        //        ExceptionHelper.ThrowNotAuthorizedBusinessAccount();

        //    return currentBusinessAccount..AsQueryable(); 
        //}

        #endregion
    }
}

using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Tools;
using Location = FoundOPS.API.Models.Location;
using Route = FoundOPS.API.Models.Route;
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
        private readonly CoreEntitiesContainer _coreEntitiesContainer;

        public RoutesController()
        {
            _coreEntitiesContainer = new CoreEntitiesContainer();
            _coreEntitiesContainer.ContextOptions.LazyLoadingEnabled = false;
        }

        #region Get

        // GET /api/route <- User must be authenticated and have Routes for today
        // GET /api/route?roleId={Guid}
        /// <summary>
        /// Returns Routes & RouteDestinations & RouteDestinations.Client & Client.OwnedParty.ContactInfoSet
        /// & RouteDestinations.Location & Location.ContactInfoSet.
        /// Ordered by name.
        /// </summary>
        /// <param name="serviceDateUtc">The date of the service (in UTC). If null it will return todays routes.</param>
        /// <param name="roleId">If the roleId has a value, it will find routes for a business account. Otherwise it will find routes for the current user account.</param>
        [AcceptVerbs("GET", "POST")]
        public IQueryable<Route> GetRoutes(DateTime? serviceDateUtc, Guid? roleId)
        {
            var apiRoutes = new List<Route>();

            //Find routes for the passed service date. 
            //If it is null used the UtcDate of today
            var date = DateTime.UtcNow.Date;
            if (serviceDateUtc.HasValue)
                date = serviceDateUtc.Value.Date;

            IEnumerable<FoundOps.Core.Models.CoreEntities.Route> loadedRoutes;

            //If there is a roleId: return the Routes for the role's BusinessAccount (Map View)
            //Otherwise: return the current user account's Routes (Mobile Application)
            if (roleId.HasValue)
            {
                loadedRoutes = _coreEntitiesContainer.BusinessAccountOwnerOfRoleQueryable(roleId.Value).Include(ba => ba.Routes)
                    .SelectMany(ba => ba.Routes).Where(r => r.Date == date)
                    .Include("Routes.RouteDestinations").Include("Routes.RouteDestinations.Client").Include("Routes.RouteDestinations.Location");
            }
            else
            {
                //Finds all LinkedEmployees for the CurrentUserAccount
                //Finds all Routes (today) associated with those Employees
                loadedRoutes = AuthenticationLogic.CurrentUserAccountQueryable(_coreEntitiesContainer)
                    .SelectMany(cu => cu.LinkedEmployees)
                    .SelectMany(e => e.Routes).Where(r => r.Date == date)
                    .Include("Routes.RouteDestinations").Include("Route.RouteDestinations.RouteTasks")
                    .Include("Routes.RouteDestinations.Client").Include("Routes.RouteDestinations.Client.ContactInfoSet")
                    .Include("Routes.RouteDestinations.Location").Include("Routes.RouteDestinations.Location.ContactInfoSet");
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
        [AcceptVerbs("GET", "POST")]
        public IQueryable<Location> GetDepots(Guid roleId)
        {
            var currentBusinessAccount = _coreEntitiesContainer.BusinessAccountOwnerOfRoleQueryable(roleId).FirstOrDefault();

            if (currentBusinessAccount == null)
                ExceptionHelper.ThrowNotAuthorizedBusinessAccount();

            return currentBusinessAccount.Depots.Select(Location.ConvertModel).AsQueryable();
        }

        //[AcceptVerbs("POST")]
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

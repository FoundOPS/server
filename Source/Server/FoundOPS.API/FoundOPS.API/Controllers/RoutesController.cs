using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Route = FoundOPS.API.Models.Route;

namespace FoundOPS.API.Controllers
{
    //[Authorize]
    public class RoutesController : ApiController
    {
        private readonly CoreEntitiesContainer _coreEntitiesContainer = new CoreEntitiesContainer();

        #region Get

        // GET /api/route
        public IQueryable<Route> GetRoutes(Guid? roleId)
        {
            //Gets the Current UserAccount
            var currentUser = AuthenticationLogic.CurrentUserAccountQueryable(_coreEntitiesContainer).FirstOrDefault();

#if DEBUG
            var currentBusinessAccount = _coreEntitiesContainer.Parties.OfType<BusinessAccount>().FirstOrDefault(ba => ba.Id == roleId);
#else
            var currentBusinessAccount = _coreEntitiesContainer.BusinessAccountOwnerOfRole((Guid)roleId);
#endif

            if (currentUser == null)
                return null;

            var apiRoutes = new List<Route>();

            var today = DateTime.UtcNow.Date;

            //Used in the mobile app to pull all Routes assigned to a UserAccount
            if (roleId == null)
            {
                //Finds all LinkedEmployees for the UserAccount
                //Finds all Routes associated with those Employees
                //Converts the FoundOPS model Routes to the API model Routes
                //Adds those APIRoutes to the list of APIRoutes to return
                foreach (var employeeRoutes in currentUser.LinkedEmployees.Select(employee => _coreEntitiesContainer.Routes
                                                .Where(r => r.Technicians.Select(t => t.Id).Contains(employee.Id) && r.Date == today).ToArray()))
                    apiRoutes.AddRange(employeeRoutes.Select(Route.ConvertModel));
            }
            //Will be used in the dispatcher for MapView 
            else
            {
                var routesForBusinessAccount = _coreEntitiesContainer.Routes.Where(r => r.OwnerBusinessAccountId == currentBusinessAccount.Id);

                apiRoutes.AddRange(routesForBusinessAccount.Select(Route.ConvertModel));
            }

            return apiRoutes.AsQueryable();
        }

        #endregion
    }
}

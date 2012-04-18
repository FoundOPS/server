using System;
using System.Collections.Generic;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Models.CoreEntities.DesignData;
using System.Linq;
using System.Web.Http;
using FoundOps.Core.Tools;
using Route = FoundOPS.API.Models.Route;

namespace FoundOPS.API.Controllers
{
    //[Authorize]
    public class RoutesController : ApiController
    {
        private readonly CoreEntitiesContainer _coreEntitiesContainer = new CoreEntitiesContainer();

        #region Get

        // GET /api/route
        public IQueryable<Route> GetRoutes()
        {
            //Gets the Current UserAccount
            var currentUser = AuthenticationLogic.CurrentUserAccountQueryable(_coreEntitiesContainer).FirstOrDefault();

            if (currentUser == null) 
                return null;

            var apiRoutes = new List<Route>();

            //Finds all LinkedEmployees for the UserAccount
            //Finds all Routes associated with those Employees
            //Converts the FoundOPS model Routes to the API model Routes
            //Adds those APIRoutes to the list of APIRoutes to return
            foreach (var employeeRoutes in currentUser.LinkedEmployees.Select(employee => _coreEntitiesContainer.Routes.Where(r => r.Technicians.Contains(employee)).ToArray()))
                apiRoutes.AddRange(employeeRoutes.Select(Route.ConvertModel));
            
            return apiRoutes.AsQueryable();
        }

        #endregion
    }
}

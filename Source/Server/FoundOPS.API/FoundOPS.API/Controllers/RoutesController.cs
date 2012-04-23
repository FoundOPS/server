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
        public IQueryable<Route> GetRoutes()
        {
            //Gets the Current UserAccount
            var currentUser = AuthenticationLogic.CurrentUserAccountQueryable(_coreEntitiesContainer).FirstOrDefault();

            if (currentUser == null) 
                return null;

            var apiRoutes = new List<Route>();

            var today = DateTime.UtcNow.Date;

            //Finds all LinkedEmployees for the UserAccount
            //Finds all Routes associated with those Employees
            //Converts the FoundOPS model Routes to the API model Routes
            //Adds those APIRoutes to the list of APIRoutes to return
            foreach (var employee in currentUser.LinkedEmployees)
            {
                var employeeRoutes = _coreEntitiesContainer.Routes.Where(r => r.Date == today && r.Technicians.Select(t => t.Id).Contains(employee.Id)).ToArray(); //SelectMany(r => r.Technicians.Select(t => t.Id)).Where(tId => tId == employee.Id)
                apiRoutes.AddRange(employeeRoutes.Select(Route.ConvertModel));
            }

            return apiRoutes.AsQueryable();
        }

        #endregion
    }
}

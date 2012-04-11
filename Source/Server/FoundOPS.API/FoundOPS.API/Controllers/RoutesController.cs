using FoundOps.Core.Models.CoreEntities.DesignData;
using FoundOPS.API.Models;
using System.Linq;
using System.Web.Http;

namespace FoundOPS.API.Controllers
{
    [Authorize]
    public class RoutesController : ApiController
    {
        private readonly RoutesDesignData _routesDesignData = new RoutesDesignData();

        // GET /api/route
        public IQueryable<Route> GetRoutes()
        {
            //Convert the routes to the API model route
            var apiRoutes = _routesDesignData.DesignRoutes.Select(Models.Route.ConvertModel).ToList();

            return apiRoutes.AsQueryable();
        }

        //// GET /api/route/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST /api/route
        //public void Post(string value)
        //{
        //}

        //// PUT /api/route/5
        //public void Put(int id, string value)
        //{
        //}

        //// DELETE /api/route/5
        //public void Delete(int id)
        //{
        //}
    }
}

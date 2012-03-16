using FoundOps.Common.NET;
using FoundOps.Core.Models.CoreEntities.DesignData;
using System.Linq;
using System.Web.Mvc;

namespace FoundOPS.API.Controllers
{
    /// <summary>
    /// Holds the API for Technician information.
    /// </summary>
    [JsonpFilter] //Return Jsonp so other browsers accept cross domain requests
    public class TechnicianController : Controller
    {
        private readonly RoutesDesignData _routesDesignData = new RoutesDesignData();
        //
        // GET: /Technician/GetRoutes

        public JsonResult GetRoutes()
        {
            //Convert the routes to the API model route
            var apiRoutes = _routesDesignData.DesignRoutes.Select(Models.Route.ConvertModel).ToList();

            //Commented out because I do not think this is necessary
                //HttpContext.Response.AddHeader("Access-Control-Allow-Origin", "*"); 
            return Json(apiRoutes, JsonRequestBehavior.AllowGet);
        }

        public bool InsertTrackPoint(string json)
        {
            //TODO Insert the GPS trackpoint
            return true;
        }
    }
}
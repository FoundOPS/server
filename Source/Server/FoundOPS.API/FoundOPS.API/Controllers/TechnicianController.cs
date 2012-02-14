using System.Web.Mvc;
using FoundOps.Common.NET;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Models.CoreEntities.DesignData;

namespace FoundOPS.API.Controllers
{
    /// <summary>
    /// Holds the API for Technician information.
    /// </summary>
    [JsonpFilter] //Return Jsonp so other browsers accept cross domain requests
    public class TechnicianController : Controller
    {
        private readonly LocationsDesignData _locationsDesignData = new LocationsDesignData();
        //
        // GET: /Technician/GetRoutes

        public JsonResult GetRoutes()
        {
            var route = new Route();

            var routeStop = new RouteDestination { Location = _locationsDesignData.DesignLocation, OrderInRoute = 1 };
            route.RouteDestinations.Add(routeStop);
            
            routeStop = new RouteDestination { Location = _locationsDesignData.DesignLocationTwo, OrderInRoute = 2, };
            route.RouteDestinations.Add(routeStop);

            routeStop = new RouteDestination { Location = _locationsDesignData.DesignLocationThree, OrderInRoute = 3 };
            route.RouteDestinations.Add(routeStop);

            //Commented out because I do not think this is necessary
            //HttpContext.Response.AddHeader("Access-Control-Allow-Origin", "*"); 

            return Json(Models.Route.ConvertModel(route), JsonRequestBehavior.AllowGet);
        }
    }
}

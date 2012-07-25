using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using FoundOPS.API.Tools;
using FoundOps.Core.Models;
using Thinktecture.IdentityModel.Http.Cors.WebApi;

namespace FoundOPS.API
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    //Using https://github.com/thinktecture/Thinktecture.Web.Http

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.MapHttpRoute(
            //    name: "API Default",
            //    routeTemplate: "api/{controller}"
            //    );

            var apiRoute = routes.MapHttpRoute(
                name: "ActionApi",
                routeTemplate: "api/{controller}/{action}"
                );

            apiRoute.DataTokens["Namespaces"] = new[] { "FoundOPS.API.Api" };

            var mvcRoute = routes.MapRoute(
                 "Default", // Route name
                 "{controller}/{action}"
                 );

            mvcRoute.DataTokens["Namespaces"] = new[] { "FoundOPS.API.Controllers" };
        }

        protected void Application_Start()
        {
            //Remove the XML formatter
            GlobalConfiguration.Configuration.Formatters.RemoveAt(1);

            //Switch JSON to JSONP formatter
            GlobalConfiguration.Configuration.Formatters[0] = new JsonpMediaTypeFormatter();
            GlobalFilters.Filters.Add(new HandleErrorAttribute());

            AreaRegistration.RegisterAllAreas();
            RegisterRoutes(RouteTable.Routes);

            RegisterCors(GlobalConfiguration.Configuration);
        }

        public static void RegisterCors(HttpConfiguration httpConfig)
        {
            var corsConfig = new WebApiCorsConfiguration();

            // this adds the CorsMessageHandler to the HttpConfiguration’s
            // MessageHandlers collection
            corsConfig.RegisterGlobal(httpConfig);

            // this allow all CORS requests to the Settings controller
            // from the http://localhost:31820 origin.
            corsConfig.ForResources("Settings")
                .ForOrigins(ServerConstants.RootApplicationUrl)
                .AllowAll();
        }
    }
}
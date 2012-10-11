using FoundOps.Api.Tools;
using FoundOps.Core.Models;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Thinktecture.IdentityModel.Http.Cors.WebApi;

namespace FoundOps.Api
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    //Using https://github.com/thinktecture/Thinktecture.Web.Http

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            var apiRoute = routes.MapHttpRoute("RestApiRoute", "api/{controller}/{id}", new { id = RouteParameter.Optional });
            apiRoute.DataTokens["Namespaces"] = new[] { "FoundOps.Api.Controllers.Rest" };

            var mvcRoute = routes.MapRoute("MvcRoute", "{controller}/{action}");
            mvcRoute.DataTokens["Namespaces"] = new[] { "FoundOps.Api.Controllers.Mvc" };
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

            // this allow all CORS requests
            corsConfig.ForAllResources().AllowAllOrigins().AllowAll();
        }
    }
}
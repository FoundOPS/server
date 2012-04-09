using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Thinktecture.Web.Http.Formatters;

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

            routes.MapHttpRoute(
                name: "API Default",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
                );

            routes.MapHttpRoute(
                name: "ActionApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new {id = RouteParameter.Optional}
                );
        }

        protected void Application_Start()
        {
            GlobalConfiguration.Configuration.Formatters[0] = new JsonpMediaTypeFormatter();
            GlobalFilters.Filters.Add(new HandleErrorAttribute());
          
            AreaRegistration.RegisterAllAreas();
            RegisterRoutes(RouteTable.Routes);
        }
    }
}
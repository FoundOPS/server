using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace FoundOps.Server
{
    public class Global : System.Web.HttpApplication
    {
#if DEBUG
        public static string Mode = "DEBUG";
#elif TESTRELEASE
        public static string Mode = "TESTRELEASE";
#elif RELEASE
        public static string Mode = "RELEASE";
#endif
     
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{*allsvc}", new { allsvc = @".*\.svc(/.*)?" });
            //Ignore All Services, For WCF RIA Services
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //Default route: app.foundops.com/Controller/Action
            routes.MapRoute(
                // Route name
                "Default",
                // URL with parameters
                "{controller}/{action}/{id}",
                // Parameter defaults
                new { controller = "Home", action = "Index", id = "" }
                );

            routes.IgnoreRoute("clientaccesspolicy.xml");
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            RegisterRoutes(RouteTable.Routes);
        }
    }
}

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

        public static string RootApplicationUrl;
        public static string RootFrontSiteUrl;

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
#if DEBUG
            IsDebugMode = true;
            RootApplicationUrl = "http://localhost:31820";
            RootFrontSiteUrl = "http://localhost:55206";
#elif TESTRELEASE
            RootApplicationUrl = "https://test.foundops.com";
            RootFrontSiteUrl = "http://foundops.com";
#elif RELEASE
            RootApplicationUrl = "https://app.foundops.com";
            RootFrontSiteUrl = "http://foundops.com";
#endif
            RegisterRoutes(RouteTable.Routes);
        }
    }
}

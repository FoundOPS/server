using System;
using System.Diagnostics;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;

namespace FoundOps.Core.Server
{
    public class Global : System.Web.HttpApplication
    {
        public static bool IsDebugMode;

        public static string RootUrl;
        public static string RootBlogUrl;

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{*allsvc}", new { allsvc = @".*\.svc(/.*)?" });
            //Ignore All Services, For WCF RIA Services
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //Route for the Beta page: foundops.com/beta
            routes.MapRoute("WP pages",
                "{page}",
                new { controller = "Home", action = "WP" },
                new { page = @"^beta$|^product$|^team$|^values$|^jobs$|^contact$|^blog$" }
                );

            //Route for team profiles: foundops.com/namehere to Home/Team
            routes.MapRoute(
                // Route name
                "Names",
                // URL with parameters
                "{id}",
                new { controller = "Home", action = "Team" },
                new { id = @"((?:[a-z][a-z0-9_]*))" }
                );

            //Route for qrcodes from foundops.com/qrcode to Home/Card
            routes.MapRoute(
                // Route name
                "Cards",
                // URL with parameters
                "{id}",
                new { controller = "Home", action = "Card" },
                new { id = @"^415919$||^112091$||^317067$" }
                );

            //Default route: foundops.com/Controller/Action
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
            RootUrl = "http://localhost:31820";
            RootBlogUrl = "http://localhost:55206";
#else
            RootUrl = "https://app.foundops.com";
            RootBlogUrl = "http://foundops.com";
#endif
            RegisterRoutes(RouteTable.Routes);
        }

        void Application_End(object sender, EventArgs e)
        {
            //  Code that runs on application shutdown

        }

        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs

        }

        void Session_Start(object sender, EventArgs e)
        {
            // Code that runs when a new session is started

        }

        void Session_End(object sender, EventArgs e)
        {
            // Code that runs when a session ends. 
            // Note: The Session_End event is raised only when the sessionstate mode
            // is set to InProc in the Web.config file. If session mode is set to StateServer 
            // or SQLServer, the event is not raised.

        }

        private bool IsAssemblyDebugBuild(Assembly assembly)
        {
            foreach (var attribute in assembly.GetCustomAttributes(false))
            {
                var debuggableAttribute = attribute as DebuggableAttribute;
                if (debuggableAttribute != null)
                {
                    return debuggableAttribute.IsJITTrackingEnabled;
                }
            }
            return false;
        }
    }
}

using System.Web;
using System.Web.Mvc;

namespace FoundOps.Server.Tools
{
    public class AddTestUsersThenAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            #if DEBUG
            if (Core.Models.ServerConstants.AutomaticLoginOPSManager)
            {
                FormsAuthentication.SignOut();
                FormsAuthentication.SetAuthCookie("jperl@foundops.com", false);
                return true;
            }
            if (Core.Models.ServerConstants.AutomaticLoginFoundOPSAdmin)
            {
                FormsAuthentication.SignOut();
                FormsAuthentication.SetAuthCookie("david@gotgrease.net", false);
                return true;
            }
            #endif
            return base.AuthorizeCore(httpContext);
        }
    }
}
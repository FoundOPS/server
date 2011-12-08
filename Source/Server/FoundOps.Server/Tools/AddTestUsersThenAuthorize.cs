using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using FoundOps.Common.Server;

namespace FoundOps.Server.Tools
{
    public class AddTestUsersThenAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            //#if DEBUG
            if (UserSpecificResourcesWrapper.GetBool("AutomaticLoginJonathan"))
            {
                FormsAuthentication.SignOut();
                FormsAuthentication.SetAuthCookie("jperl@foundops.com", false);
                return true;
            }
            else if (UserSpecificResourcesWrapper.GetBool("AutomaticLoginDavid"))
            {
                FormsAuthentication.SignOut();
                FormsAuthentication.SetAuthCookie("david@gotgrease.net", false);
                return true;
            }
            //#endif
            return base.AuthorizeCore(httpContext);
        }
    }
}
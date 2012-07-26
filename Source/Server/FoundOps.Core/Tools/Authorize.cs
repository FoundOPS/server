using System.Web;
using System.Web.Security;

namespace FoundOps.Core.Tools
{
    /// <summary>
    /// A copy of the Authorize attribute.
    /// If in debug mode, it adds a test user.
    /// </summary>
    public class AuthorizeAttribute : System.Web.Mvc.AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
#if DEBUG
            if (Core.Models.ServerConstants.AutomaticLoginFoundOPSAdmin)
            {
                FormsAuthentication.SignOut();
                FormsAuthentication.SetAuthCookie("jperl@foundops.com", false);
                return true;
            }
#endif

            return base.AuthorizeCore(httpContext);
        }
    }
}
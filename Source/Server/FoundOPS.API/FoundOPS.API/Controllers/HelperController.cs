using System.Web.Mvc;
using FoundOPS.API.Api;

namespace FoundOPS.API.Controllers
{
    public class HelperController : Controller
    {
        /// <summary>
        /// Logs in the specified email address and redirects them.
        /// </summary>
        /// <param name="email">The email address.</param>
        /// <param name="pass">The password.</param>
        /// <param name="redirect">The url to redirect them to.</param>
        public ActionResult Login(string email, string pass, string redirect)
        {
            var authController = new AuthController();
            return Redirect(authController.Login(email, pass) ? redirect : "www.foundops.com");
        }
    }
}

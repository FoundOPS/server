using System.Web.Mvc;

namespace FoundOps.Core.Server.Controllers
{
    public class ErrorController : Controller
    {
        /// <summary>
        /// This is fired when the site gets a bad URL
        /// </summary>
        /// <returns></returns>
        public ActionResult NotFound()
        {
            // log here, perhaps you want to know when a user reaches a 404?
            return View();
        }
    }
}

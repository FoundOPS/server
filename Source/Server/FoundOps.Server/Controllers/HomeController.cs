using System.IO;
using System.Net;
using System.Web.Mvc;
using FoundOps.Common.Server;
using FoundOps.Server.Tools;

namespace FoundOps.Server.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
#if DEBUG
            if (UserSpecificResourcesWrapper.GetBool("AutomaticLoginJonathan") || UserSpecificResourcesWrapper.GetBool("AutomaticLoginDavid"))
                return RedirectToAction("Silverlight", "Home");
#endif

            if (!HttpContext.User.Identity.IsAuthenticated)
                return Redirect(Global.RootBlogUrl);

            return RedirectToAction("Silverlight", "Home");
        }

#if !DEBUG
        [RequireHttps]
#endif
        [AddTestUsersThenAuthorize]
        public ActionResult Silverlight()
        {
            var request = (HttpWebRequest)WebRequest.Create("http://fstore.blob.core.windows.net/xaps/version.txt");

            // *** Retrieve request info headers
            var response = (HttpWebResponse)request.GetResponse();
            var stream = new StreamReader(response.GetResponseStream());

            var version = stream.ReadToEnd();
            response.Close();
            stream.Close();

            //Must cast the string as an object so it uses the correct overloaded method
            return View((object)version);
        }

#if DEBUG

        /// <summary>
        /// Clear, create database, and populate design data.
        /// </summary>
        /// <returns></returns>
        public ActionResult CCDAPDD()
        {
#if DEBUG
            if (UserSpecificResourcesWrapper.GetBool("AutomaticLoginJonathan") ||
                UserSpecificResourcesWrapper.GetBool("AutomaticLoginDavid") || UserSpecificResourcesWrapper.GetBool("TestServer"))
            {
                CoreEntitiesServerManagement.ClearCreateCoreEntitiesDatabaseAndPopulateDesignData();
                return View();
            }
#endif

            throw new Exception("Invalid attempted access logged for investigation.");
        }

        [AddTestUsersThenAuthorize]
        public ActionResult PerformServerOperations()
        {
            //TODO Authenticate first
            ServerManagement.PerformServerOperations();
            return View();
        }
#endif
    }
}
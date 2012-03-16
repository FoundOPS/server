using System;
using System.IO;
using System.Net;
using System.Web.Mvc;
using FoundOps.Core.Models;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Server.Tools;

namespace FoundOps.Server.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
#if DEBUG
            if (ServerConstants.AutomaticLoginFoundOPSAdmin || ServerConstants.AutomaticLoginOPSManager)
                return RedirectToAction("Silverlight", "Home");
#endif

            if (!HttpContext.User.Identity.IsAuthenticated) 
                return Redirect(Global.RootBlogUrl);

            return RedirectToAction("Silverlight", "Home");
        }

        //Cannot require the page to be HTTPS until we have our own tile server
        //#if !DEBUG
        //        [RequireHttps]
        //#endif
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
            if (ServerConstants.AutomaticLoginFoundOPSAdmin || ServerConstants.AutomaticLoginOPSManager)
            {
                CoreEntitiesServerManagement.ClearCreateCoreEntitiesDatabaseAndPopulateDesignData();
                return View();
            }

            throw new Exception("Invalid attempted access logged for investigation.");
        }

        /// <summary>
        /// An action for triggering one time server operations.
        /// </summary>
        /// <returns></returns>
        [AddTestUsersThenAuthorize]
        public ActionResult PerformServerOperations()
        {
            ServerManagement.PerformServerOperations();
            return View();
        }
#endif
    }
}
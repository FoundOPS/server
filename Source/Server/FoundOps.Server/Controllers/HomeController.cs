using FoundOps.Core.Models;
using FoundOps.Server.Tools;
using System;
using System.Web.Mvc;
#if !DEBUG //RELEASE or TESTRELEASE
using System.IO;
using System.Net;
using FoundOps.Common.Tools;
#endif

namespace FoundOps.Server.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
#if DEBUG
            if (ServerConstants.AutomaticLoginFoundOPSAdmin)
                return RedirectToAction("Silverlight", "Home");
#endif

            if (!HttpContext.User.Identity.IsAuthenticated)
                return Redirect(ServerConstants.RootFrontSiteUrl);

            return RedirectToAction("Silverlight", "Home");
        }

        //Cannot require the page to be HTTPS until we have our own tile server
        //#if !DEBUG
        //        [RequireHttps]
        //#endif
        [AddTestUsersThenAuthorize]
        public ActionResult Silverlight()
        {
#if DEBUG
            var random = new Random();
            var version = random.Next(10000).ToString();
#else
            var request = (HttpWebRequest)WebRequest.Create(AzureTools.BlobStorageUrl + "xaps/version.txt");

            // *** Retrieve request info headers
            var response = (HttpWebResponse)request.GetResponse();
            var stream = new StreamReader(response.GetResponseStream());

            var version = stream.ReadToEnd();
            response.Close();
            stream.Close();
#endif

            //Must cast the string as an object so it uses the correct overloaded method
            return View((object)version);
        }

        [AddTestUsersThenAuthorize]
        public ActionResult MapView()
        {
            return View();
        }
    }
}
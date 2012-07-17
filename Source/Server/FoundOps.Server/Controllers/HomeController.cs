using FoundOps.Server.Tools;
using System.Collections.Generic;
using System.Web.Mvc;

#if !DEBUG //RELEASE or TESTRELEASE
using System.IO;
using System.Net;
using FoundOps.Core.Models;
using FoundOps.Common.Tools;
#endif

namespace FoundOps.Server.Controllers
{
    public class HomeController : Controller
    {
        //Cannot require the page to be HTTPS until we have our own tile server
        //#if !DEBUG
        //        [RequireHttps]
        //#endif
        public ActionResult Index()
        {
#if !DEBUG
            if (!HttpContext.User.Identity.IsAuthenticated)
                return Redirect(ServerConstants.RootFrontSiteUrl);
#endif

#if DEBUG
            var random = new System.Random();
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
            var model = new Dictionary<string, object> { { "SilverlightVersion", version } };

            return View(model);
        }

#if !RELEASE
        //Purely for debugging
        public ActionResult TestSilverlight()
        {
            var random = new System.Random();
            var version = random.Next(10000).ToString();
            var model = new Dictionary<string, object> { { "SilverlightVersion", version } };
            return View(model);
        }
#endif

        [AddTestUsersThenAuthorize]
        public ActionResult MapView()
        {
            return View();
        }
    }
}
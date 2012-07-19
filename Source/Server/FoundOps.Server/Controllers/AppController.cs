using System.Collections.Generic;
using System.Web.Mvc;
using FoundOps.Common.Tools;

#if !DEBUG //RELEASE or TESTRELEASE
using System.IO;
using System.Net;
using FoundOps.Core.Models;
#endif

namespace FoundOps.Server.Controllers
{
    [FoundOps.Core.Tools.Authorize]
    public class AppController : Controller
    {
        //
        // GET: /App/
        //Cannot require the page to be HTTPS until we have our own tile server
        //#if !DEBUG
        //        [RequireHttps]
        //#endif
        public ActionResult Index()
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
                return Redirect(ServerConstants.RootFrontSiteUrl);

#if DEBUG
            var random = new System.Random();
            var version = random.Next(10000).ToString();
#else
            var request = (HttpWebRequest)WebRequest.Create(SharedConstants.BlobStorageUrl + "xaps/version.txt");

            // *** Retrieve request info headers
            var response = (HttpWebResponse)request.GetResponse();
            var stream = new StreamReader(response.GetResponseStream());

            var version = stream.ReadToEnd();
            response.Close();
            stream.Close();
#endif

#if DEBUG
            var splashSource = Url.Content("/ClientBin/SplashScreen.xaml");
            var xapSource = Url.Content("/ClientBin/FoundOps.SLClient.Navigator.xap");
#elif TESTRELEASE
            var splashSource = Url.Content(SharedConstants.BlobStorageUrl + "xaps/SplashScreen.xaml");
            var xapSource = Url.Content(SharedConstants.BlobStorageUrl + "xaps/FoundOps.SLClient.Navigator.xap");
#elif RELEASE
            var splashSource = Url.Content(SharedConstants.BlobStorageUrl + "xaps/SplashScreen.xaml");
            var xapSource = Url.Content(SharedConstants.BlobStorageUrl + "xaps/FoundOps.SLClient.Navigator.xap");
#endif

            //Setup version
            var sourceParam = xapSource + "?ignore=" + version;

            var model = new Dictionary<string, object>
                {
                    {"SplashSource", splashSource},
                    {"XapSource", sourceParam},
                    {"BlobRoot", SharedConstants.BlobStorageUrl + "app/"}
                };

            //#if DEBUG
            //            //if full source
            //            return View("IndexFullSource", model);
            //#endif
            return View("IndexBuilt", model);
        }
    }
}
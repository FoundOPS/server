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
    [FoundOps.Server.Tools.Authorize]
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

#if DEBUG
            var splashSource = Url.Content("/ClientBin/SplashScreen.xaml");
            var xapSource = Url.Content("/ClientBin/FoundOps.SLClient.Navigator.xap");
#elif TESTRELEASE
            var splashSource = Url.Content("http://bt.foundops.com/xaps/SplashScreen.xaml");
            var xapSource = Url.Content("http://bt.foundops.com/xaps/FoundOps.SLClient.Navigator.xap");
#elif RELEASE
            var splashSource = Url.Content("http://bp.foundops.com/xaps/SplashScreen.xaml");
            var xapSource = Url.Content("http://bp.foundops.com/xaps/FoundOps.SLClient.Navigator.xap");
#endif

            //Setup version
            var sourceParam = xapSource + "?ignore=" + version;

            var model = new Dictionary<string, object> { { "SplashSource", splashSource }, { "Source", sourceParam } };
            return View(model);
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web.Caching;
using System.Web.Mvc;
using FoundOps.Common.Tools;
using FoundOps.Core.Models;
using FoundOps.Core.Models.Azure;

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
            //load the index page from cache
            //if it expired, refetch it from blob storage
            var indexPage = HttpContext.Cache["IndexPage"] as string;
            if (indexPage == null)
            {
#if DEBUG
                var request = (HttpWebRequest)WebRequest.Create(AppConstants.LocalApplicationServer);
#else
                var request = (HttpWebRequest)WebRequest.Create(AzureServerHelpers.BlobStorageUrl + "app/index.html");
#endif

                var response = (HttpWebResponse)request.GetResponse();
                var stream = new StreamReader(response.GetResponseStream());

                indexPage = stream.ReadToEnd();
                response.Close();
                stream.Close();

                //cache the page for 6 hours
                HttpContext.Cache.Add("IndexPage", indexPage, null, DateTime.UtcNow.AddHours(6), Cache.NoSlidingExpiration,
                                      CacheItemPriority.Normal, null);
            }

            return Content(indexPage, "text/html");
        }
    }
}
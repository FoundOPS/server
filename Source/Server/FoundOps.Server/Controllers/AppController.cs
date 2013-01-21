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
#if DEBUG
            var redirectUrl = AppConstants.LocalApplicationServer;
#else
            var redirectUrl = AzureServerHelpers.BlobStorageUrl + "app/index.html";
#endif

            return RedirectPermanent(redirectUrl);
        }
    }
}
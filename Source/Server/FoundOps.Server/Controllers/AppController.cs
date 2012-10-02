using System.Collections.Generic;
using System.Web.Mvc;
using FoundOps.Common.Tools;
using FoundOps.Core.Models;

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
            var model = new Dictionary<string, object>
                {
                    {"BlobRoot", SharedConstants.BlobStorageUrl + "app/"}
                };

#if DEBUG
            //if full source
            return View("IndexFullSource", model);
#endif
            return View("IndexBuilt", model);
        }
    }
}
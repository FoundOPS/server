using FoundOps.Core.Models;
using FoundOps.Core.Models.Azure;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Server.Tools;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using System;
using System.Data.Services.Client;
using System.Linq;
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
            if (ServerConstants.AutomaticLoginFoundOPSAdmin || ServerConstants.AutomaticLoginOPSManager)
                return RedirectToAction("Silverlight", "Home");
#endif

            if (!HttpContext.User.Identity.IsAuthenticated)
                return Redirect(Global.RootFrontSiteUrl);

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

#endif

#if !RELEASE
        public ActionResult ClearCreateHistoricalTrackPoints()
        {
            if (ServerConstants.AutomaticLoginFoundOPSAdmin || ServerConstants.AutomaticLoginOPSManager)
            {
                CoreEntitiesServerManagement.ClearCreateHistoricalTrackPoints();
                return View();
            }

            throw new Exception("Invalid attempted access logged for investigation.");
        }
#endif
    }
}
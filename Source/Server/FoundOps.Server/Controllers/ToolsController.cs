using System;
using System.Web.Mvc;
using FoundOps.Core.Models;
using FoundOps.Core.Models.CoreEntities;

namespace FoundOps.Server.Controllers
{
    public class ToolsController : Controller
    {
#if !RELEASE
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

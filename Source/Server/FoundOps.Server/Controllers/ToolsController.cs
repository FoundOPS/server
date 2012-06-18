using System;
using System.Web.Mvc;
using FoundOps.Core.Models;
using FoundOps.Core.Models.CoreEntities;

namespace FoundOps.Server.Controllers
{
#if DEBUG
    public class ToolsController : Controller
    {

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

        public ActionResult ClearHistoricalTrackPoints()
        {
            if (ServerConstants.AutomaticLoginFoundOPSAdmin || ServerConstants.AutomaticLoginOPSManager)
            {
                CoreEntitiesServerManagement.ClearHistoricalTrackPoints();
                return View();
            }

            throw new Exception("Invalid attempted access logged for investigation.");
        }

        public ActionResult CreateHistoricalTrackPoints()
        {
            if (ServerConstants.AutomaticLoginFoundOPSAdmin || ServerConstants.AutomaticLoginOPSManager)
            {
                CoreEntitiesServerManagement.CreateHistoricalTrackPoints();
                return View();
            }

            throw new Exception("Invalid attempted access logged for investigation.");
        }
    }
#endif
}

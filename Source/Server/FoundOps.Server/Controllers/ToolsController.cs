using FoundOps.Core.Models.CoreEntities;
using System.Web.Mvc;

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
            CoreEntitiesServerManagement.ClearCreateCoreEntitiesDatabaseAndPopulateDesignData();
            return View();
        }

        public ActionResult ClearHistoricalTrackPoints()
        {
            CoreEntitiesServerManagement.ClearHistoricalTrackPoints();
            return View();
        }

        public ActionResult CreateHistoricalTrackPoints()
        {
            CoreEntitiesServerManagement.CreateHistoricalTrackPoints();
            return View();
        }
    }
#endif
}

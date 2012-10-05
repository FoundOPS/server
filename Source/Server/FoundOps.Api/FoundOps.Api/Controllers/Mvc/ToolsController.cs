using FoundOps.Core.Models.CoreEntities;
using System.Web.Mvc;

namespace FoundOps.Api.Controllers.Mvc
{
#if DEBUG

    public class ToolsController : Controller
    {
        /// <summary>
        /// Clears and creates the database and populates design data.
        /// </summary>
        public bool Ccdapdd()
        {
            CoreEntitiesServerManagement.ClearCreateCoreEntitiesDatabaseAndPopulateDesignData();
            return true;
        }

        /// <summary>
        /// Deletes the HistoricalTrackPoints tables.
        /// Must do this seperately from Create, because deleting tables takes time.
        /// </summary>
        public bool ClearHistoricalTrackPoints()
        {
            CoreEntitiesServerManagement.ClearHistoricalTrackPoints();
            return true;
        }

        /// <summary>
        /// Populates HistoricalTrackPoints design data on the Azure tables.
        /// </summary>
        public bool CreateHistoricalTrackPoints()
        {
            CoreEntitiesServerManagement.CreateHistoricalTrackPoints();
            return true;
        }
    }

#endif
}

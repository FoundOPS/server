using FoundOps.Core.Models.CoreEntities;
using System.Web.Http;

namespace FoundOps.Api.Controllers.Rest
{
#if DEBUG

    public class ToolsController : BaseApiController
    {
        //GET /api/tools/CCDAPDD
        /// <summary>
        /// Clears and creates the database and populates design data.
        /// </summary>
        [AcceptVerbs("GET", "POST")]
        public bool CCDAPDD()
        {
            CoreEntitiesServerManagement.ClearCreateCoreEntitiesDatabaseAndPopulateDesignData();
            return true;
        }

        //GET /api/tools/ClearHistoricalTrackPoints
        /// <summary>
        /// Deletes the HistoricalTrackPoints tables.
        /// Must do this seperately from Create, because deleting tables takes time.
        /// </summary>
        [AcceptVerbs("GET", "POST")]
        public bool ClearHistoricalTrackPoints()
        {
            CoreEntitiesServerManagement.ClearHistoricalTrackPoints();
            return true;
        }

        //GET /api/tools/CreateHistoricalTrackPoints
        /// <summary>
        /// Populates HistoricalTrackPoints design data on the Azure tables.
        /// </summary>
        [AcceptVerbs("GET", "POST")]
        public bool CreateHistoricalTrackPoints()
        {
            CoreEntitiesServerManagement.CreateHistoricalTrackPoints();
            return true;
        }
    }

#endif
}

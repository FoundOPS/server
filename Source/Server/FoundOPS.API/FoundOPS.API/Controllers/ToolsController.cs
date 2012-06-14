using FoundOps.Core.Models;
using FoundOps.Core.Models.CoreEntities;
using System;
using System.Web.Http;

namespace FoundOPS.API.Controllers
{
#if DEBUG

    public class ToolsController : ApiController
    {
        //GET /api/tools/CCDAPDD
        /// <summary>
        /// Clears and creates the database and populates design data.
        /// </summary>
        public bool CCDAPDD()
        {
            if (ServerConstants.AutomaticLoginFoundOPSAdmin || ServerConstants.AutomaticLoginOPSManager)
            {
                CoreEntitiesServerManagement.ClearCreateCoreEntitiesDatabaseAndPopulateDesignData();
                return true;
            }

            throw new Exception("Invalid attempted access logged for investigation.");
        }

        //GET /api/tools/ClearCreateHistoricalTrackPoints
        /// <summary>
        /// Clears and creates historical trackpoints in the azure tables.
        /// </summary>
        public bool ClearCreateHistoricalTrackPoints()
        {
            if (ServerConstants.AutomaticLoginFoundOPSAdmin || ServerConstants.AutomaticLoginOPSManager)
            {
                CoreEntitiesServerManagement.ClearCreateHistoricalTrackPoints();
                return true;
            }

            throw new Exception("Invalid attempted access logged for investigation.");
        }
    }

#endif
}

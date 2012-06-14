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

        //GET /api/tools/ClearHistoricalTrackPoints
        /// <summary>
        /// Clears historical trackpoints in the azure tables.
        /// </summary>
        public bool ClearHistoricalTrackPoints()
        {
            if (ServerConstants.AutomaticLoginFoundOPSAdmin || ServerConstants.AutomaticLoginOPSManager)
            {
                CoreEntitiesServerManagement.ClearHistoricalTrackPoints();
                return true;
            }

            throw new Exception("Invalid attempted access logged for investigation.");
        }

        //GET /api/tools/CreateHistoricalTrackPoints
        /// <summary>
        /// Creates historical trackpoints in the azure tables.
        /// </summary>
        public bool CreateHistoricalTrackPoints()
        {
            if (ServerConstants.AutomaticLoginFoundOPSAdmin || ServerConstants.AutomaticLoginOPSManager)
            {
                CoreEntitiesServerManagement.CreateHistoricalTrackPoints();
                return true;
            }

            throw new Exception("Invalid attempted access logged for investigation.");
        }
    }

#endif
}

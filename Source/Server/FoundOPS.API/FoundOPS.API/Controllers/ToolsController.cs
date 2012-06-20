﻿using FoundOps.Core.Models;
using FoundOps.Core.Models.CoreEntities;
using System;
using System.Web.Http;
using System.Web.Services.Protocols;

namespace FoundOPS.API.Controllers
{
#if DEBUG

    public class ToolsController : ApiController
    {
        //GET /api/tools/CCDAPDD
        /// <summary>
        /// Clears and creates the database and populates design data.
        /// </summary>
        [AcceptVerbs("GET", "POST")]
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
        /// Deletes the HistoricalTrackPoints tables.
        /// Must do this seperately from Create, because deleting tables takes time.
        /// </summary>
        [AcceptVerbs("GET", "POST")]
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
        /// Populates HistoricalTrackPoints design data on the Azure tables.
        /// </summary>
        [AcceptVerbs("GET", "POST")]
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

using FoundOps.Api.Models;
using FoundOps.Core.Tools;
using System.Collections.Generic;
using System.Linq;

namespace FoundOps.Api.Controllers.Rest
{
    public class TimeZonesController : BaseApiController
    {
        /// <summary>
        /// Get the available time zones
        /// </summary>
        public IEnumerable<TimeZone> Get()
        {
            return TimeZoneTools.GetTimeZones().Select(TimeZone.ConvertModel);
        }
    }
}
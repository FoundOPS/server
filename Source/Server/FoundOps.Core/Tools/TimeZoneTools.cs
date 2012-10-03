using System;
using System.Collections.Generic;
using System.Linq;

namespace FoundOps.Core.Tools
{
    public static class TimeZoneTools
    {
        /// <summary>
        /// Returns all time zones
        /// NOTE: Right now it is filtered by the US
        /// </summary>
        public static IEnumerable<TimeZoneInfo> GetTimeZones()
        {
            var allTimeZones = TimeZoneInfo.GetSystemTimeZones();

            //Filter to US time zones
            var usTimeZones = allTimeZones.Where(tz => tz.DisplayName.Contains("US") || tz.Id == "Hawaiian Standard Time" || tz.Id == "Alaskan Standard Time");
            return usTimeZones;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoundOPS.API.Models
{
    public class TimeZoneInfo
    {
        public string DisplayName { get; set; }

        public string TimeZoneId { get; set; }

        public static TimeZoneInfo ConvertModel(System.TimeZoneInfo timeZoneIfo)
        {
            var newTimeZone = new TimeZoneInfo
                {
                    DisplayName = timeZoneIfo.DisplayName,
                    TimeZoneId = timeZoneIfo.Id
                };

            return newTimeZone;
        }
    }
}
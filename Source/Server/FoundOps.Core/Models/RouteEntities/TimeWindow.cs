using System;
using System.Runtime.Serialization;

namespace FoundOps.Core.Models.RouteEntities
{
    [DataContract]
    public class TimeWindow
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
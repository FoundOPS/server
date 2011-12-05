using System;
using System.Runtime.Serialization;

namespace FoundOps.Core.Models.RouteEntities
{
    /// <summary>
    /// Represents a calculated route.
    /// </summary>
    [DataContract]
    public class RouteResponse : ComplexObjectBase
    {
        /// <summary>
        /// A RouteLeg Class array where each element represents a portion of the route between waypoints.
        /// </summary>
        public RouteLeg[] Legs { get; set; }

        /// <summary>
        /// Gets or sets the total route time.
        /// </summary>
        public TimeSpan TotalRouteTime { get; set; }
    }
}
using System.Runtime.Serialization;

namespace FoundOps.Core.Models.RouteEntities
{
    [DataContract]
    public class RouteRequest : ComplexObjectBase
    {
        /// <summary>
        /// Gets or sets the waypoints to route.
        /// </summary>
        public Location[] Waypoints { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="RouteResponse"/> should be reordered based on shortest time. The first and last waypoint will remain fixed.
        /// </summary>
        /// <value>
        ///   <c>true</c> will reorganize the waypoints (except the first and last) to optimize for the shortest time; <c>false</c> will keep their order.
        /// </value>
        public bool Optimize { get; set; }
    }
}
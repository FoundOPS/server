using System;
using System.Runtime.Serialization;

namespace FoundOps.Core.Models.RouteEntities
{
    [DataContract]
    public class RouteLeg
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the itinerary items.
        /// </summary>
        /// <value>
        /// The itinerary items from the start to the end location. This is representative of turn-by-turn stops.
        /// </value>
        public Location[] ItineraryItems { get; set; }

        /// <summary>
        /// Gets or sets the arrival time.
        /// </summary>
        /// <value>
        /// The arrival time to the last itinerary item.
        /// </value>
        public DateTime ArrivalTime { get; set; }

        #endregion
    }
}
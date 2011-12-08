using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FoundOps.Core.Models.RouteEntities
{
    /// <summary>
    /// This specifies details about a <see cref="JobRoute"/>.
    /// </summary>
    [DataContract]
    public class JobRouteDetails
    {
        /// <summary>
        /// Gets or sets the eligible service types of the job route.
        /// </summary>
        public List<int> EligibleServiceTypes { get; set; }

        /// <summary>
        /// Gets or sets the earliest start time of the job route.
        /// </summary>
        public DateTime EarliestStartTime { get; set; }

        /// <summary>
        /// Gets or sets the latest end time of the job route.
        /// </summary>
        public DateTime LatestEndTime { get; set; }

        /// <summary>
        /// Gets or sets the hourly cost of the job route.
        /// </summary>
        public decimal HourlyCost { get; set; }
    }
}
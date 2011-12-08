using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FoundOps.Core.Models.RouteEntities
{
    [DataContract]
    public class Job : ComplexObjectBase
    {
        /// <summary>
        /// Gets or sets the type of the service. 
        /// When the ServiceType is set (>0) then the job must be in a route where the ServiceType is eligible.
        /// </summary>
        /// <value>
        /// The type of the service. If set to 0 there is no contraint on which route it needs to be in.
        /// </value>
        public int ServiceType { get; set; }

        /// <summary>
        /// Gets or sets the eligible arrival time windows.
        /// </summary>
        /// <value>
        /// Must arrive at the job between one of these time windows. 
        /// If the list is empty there is no time window requirement.
        /// </value>
        public List<TimeWindow> EligibleArrivalTimeWindows { get; set; }

        public Location Location { get; set; }

        /// <summary>
        /// Gets or sets the revenue of the job.
        /// </summary>
        /// <value>
        /// The revenue for the job in a consistent currency. Ex. 250.55
        /// </value>
        public decimal Revenue { get; set; }
    }
}
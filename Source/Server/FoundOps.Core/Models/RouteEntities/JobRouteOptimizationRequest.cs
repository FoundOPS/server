using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FoundOps.Core.Models.RouteEntities
{
    [DataContract]
    public class JobRouteOptimizationRequest : RouteResponse
    {
        /// <summary>
        /// Gets or sets the list of JobRouteDetails.
        /// </summary>
        /// <value>
        /// The list of JobRouteDetails. Each one corresponds to a seperate route to fill/optimize.
        /// </value>
        public List<JobRouteDetails> JobRouteDetails { get; set; }

        /// <summary>
        /// Gets or sets the jobs to organize into routes.
        /// </summary>
        /// <value>
        /// The jobs to organize into routes.
        /// </value>
        public List<Job> Jobs { get; set; }
    }
}
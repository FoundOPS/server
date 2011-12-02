using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FoundOps.Core.Models.RouteEntities
{
    [DataContract]
    public class JobRoute : ComplexObjectBase
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the details about the JobRoute.
        /// </summary>
        public JobRouteDetails JobRouteDetails { get; set; }

        /// <summary>
        /// Gets or sets the jobs in order.
        /// </summary>
        public List<Job> OrderedJobs { get; set; }

        /// <summary>
        /// Gets or sets the calculated route.
        /// </summary>
        public RouteResponse CalculatedRoute { get; set; }

        #endregion
    }
}
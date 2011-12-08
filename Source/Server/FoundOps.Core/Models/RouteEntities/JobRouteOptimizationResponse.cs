using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace FoundOps.Core.Models.RouteEntities
{
    [DataContract]
    public class JobRouteOptimizationResponse
    {
        public List<JobRoute> OptimizedJobRoutes { get; set; }

        /// <summary>
        /// Gets the total profit. This is the measure of success for the optimization response.
        /// </summary>
        /// <value>
        /// The profit of all JobRoutes.
        /// </value>
        public decimal Profit
        {
            get
            {
                decimal totalCost = OptimizedJobRoutes.Sum(jobRoute => jobRoute.CalculatedRoute.TotalRouteTime.Hours * jobRoute.JobRouteDetails.HourlyCost);
                decimal totalRevenue = OptimizedJobRoutes.Sum(jobRoute => jobRoute.OrderedJobs.Sum(job => job.Revenue));

                return totalRevenue - totalCost;
            }
        }

        /// <summary>
        /// Gets or sets the jobs that couldn't be completed due to time constraints.
        /// </summary>
        /// <value>
        /// The incomplete jobs.
        /// </value>
        public List<Job> IncompleteJobs { get; set; }
    }
}
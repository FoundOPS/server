using System;

namespace FoundOps.Core.Models.RouteEntities
{
    public interface IRoutingService
    {
        /// <summary>
        /// Calculates a route between specified stops and returns route directions. In the future will return other route data as well. Hopefully similar to http://msdn.microsoft.com/en-us/library/cc981072.aspx
        /// </summary>
        /// <param name="request">
        /// A RouteRequest object that contains the necessary information for the service operation.
        /// </param>
        /// <returns> Returns a RouteResponse Class, which contains a RouteResult Class.</returns>
        RouteResponse CalculateRoute(RouteRequest request);

        /// <summary>
        /// Optimizes the job routes using a memtic algorithm to organize jobs into routes. This should continue to try and find the highest profit until <see cref="runtime"/> has been satisfied.
        /// </summary>
        /// <param name="request">A JobRouteOptimizationRequest request which includes a list of JobRouteDetails.</param>
        /// <param name="runtime">Time to run the optimization</param>
        /// <param name="threads">Number of threads to run the optimization on</param>
        /// <returns>A JobRouteOptimizationResponse. The OptimizedJobRoutes should correspond with the <see cref="request"/>.JobRouteDetails </returns>
        JobRouteOptimizationResponse OptimizeJobRoutes(JobRouteOptimizationRequest request, TimeSpan runtime, int threads);
    }
}

using ClearLines.Bumblebee;
using FoundOps.Common.Silverlight.Tools.ExtensionMethods;
using FoundOps.SLClient.Data.Tools.TSP;
using System;
using System.Collections.Generic;

namespace FoundOps.SLClient.Data.Tools
{
    /// <summary>
    /// 
    /// </summary>
    public class HiveRouteCalculator
    {
        private readonly IList<GeoLocation> _cities;
        private readonly Solver<IList<GeoLocation>> _solver;

        /// <summary>
        /// Calculates a route based on the hive
        /// </summary>
        public HiveRouteCalculator(IList<GeoLocation> cities)
        {
            _cities = cities;
            _solver = new Solver<IList<GeoLocation>>();
            _solver.FoundSolution += SolverFoundSolution;
        }

        /// <summary>
        /// Start the search
        /// </summary>
        public void StartSearch()
        {
            var generator = new Func<Random, IList<GeoLocation>>(random => Tsp.Shuffle(random, _cities));

            var mutator = new Func<IList<GeoLocation>, Random, IList<GeoLocation>>((solution, random) => Tsp.Swap(random, solution));

            var evaluator = new Func<IList<GeoLocation>, double>(circuit => -Tsp.Length(circuit));

            var problem = new Problem<IList<GeoLocation>>(generator, mutator, evaluator);

            this._solver.Search(problem);

            Rxx3.RunDelayed(TimeSpan.FromMinutes(1), () => this._solver.Stop());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void SolverFoundSolution(object sender, SolutionMessage<IList<GeoLocation>> args)
        {
            //this.Quality = args.Quality;
            //this.DiscoveryTime = args.DateTime;
        }
    }
}

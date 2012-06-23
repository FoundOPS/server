using ClearLines.Bumblebee;
using FoundOps.Common.Silverlight.Tools.ExtensionMethods;
using FoundOps.Common.Tools;
using System;
using System.Collections.Generic;

namespace FoundOps.SLClient.Data.Tools
{
    /// <summary>
    /// A calculator for organizing routes with a hive algorithm
    /// </summary>
    public class HiveRouteCalculator
    {
        private readonly IList<IGeoLocation> _cities;
        private readonly Solver<IList<IGeoLocation>> _solver;

        /// <summary>
        /// Calculates a route based on the hive
        /// </summary>
        public HiveRouteCalculator(IList<IGeoLocation> cities)
        {
            _cities = cities;
            _solver = new Solver<IList<IGeoLocation>>();
            _solver.FoundSolution += SolverFoundSolution;
        }

        /// <summary>
        /// Start the search
        /// </summary>
        public void StartSearch()
        {
            var generator = new Func<Random, IList<IGeoLocation>>(random => Tsp.Shuffle(random, _cities));

            var mutator = new Func<IList<IGeoLocation>, Random, IList<IGeoLocation>>((solution, random) => Tsp.Swap(random, solution));

            var evaluator = new Func<IList<IGeoLocation>, double>(circuit => -Tsp.Length(circuit));

            var problem = new Problem<IList<IGeoLocation>>(generator, mutator, evaluator);

            this._solver.Search(problem);

            Rxx3.RunDelayed(TimeSpan.FromMinutes(1), () => this._solver.Stop());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void SolverFoundSolution(object sender, SolutionMessage<IList<IGeoLocation>> args)
        {
            //this.Quality = args.Quality;
            //this.DiscoveryTime = args.DateTime;
        }
    }
}

using ClearLines.Bumblebee;
using System.Reactive.Subjects;
using FoundOps.Common.Tools;
using System;
using System.Collections.Generic;

namespace FoundOps.SLClient.Algorithm
{
    /// <summary>
    /// A calculator for organizing routes with a hive algorithm
    /// </summary>
    public class HiveRouteCalculator
    {
        private readonly IList<IGeoLocation> _locations;
        private readonly Solver<IList<IGeoLocation>> _solver;

        /// <summary>
        /// Pushes whenever the solver finds a solution.
        /// Cannot expost IObservable because it conflicts with FSharp.Core which contains the same type.
        /// This conflict is why FoundOPS.SLClient.Algorithm is a seperate project.
        /// </summary>
        public readonly Subject<SolutionMessage<IList<IGeoLocation>>> FoundSolution = new Subject<SolutionMessage<IList<IGeoLocation>>>();

        /// <summary>
        /// Calculates the best organization of routes based on a bee hive algorithm
        /// </summary>
        public HiveRouteCalculator(IList<IGeoLocation> locationsToRoute)
        {
            _locations = locationsToRoute;
            _solver = new Solver<IList<IGeoLocation>>();

            _solver.FoundSolution += (s, e) => FoundSolution.OnNext(e);
        }

        /// <summary>
        /// Starts the search
        /// </summary>
        public void Search()
        {
            var generator = new Func<Random, IList<IGeoLocation>>(random => Tsp.Shuffle(random, _locations));
            var mutator = new Func<IList<IGeoLocation>, Random, IList<IGeoLocation>>((solution, random) => Tsp.Swap(random, solution));
            var evaluator = new Func<IList<IGeoLocation>, double>(circuit => -Tsp.Length(circuit));
            var problem = new Problem<IList<IGeoLocation>>(generator, mutator, evaluator);
            this._solver.Search(problem);
        }

        /// <summary>
        /// Stops the search
        /// </summary>
        public void Stop()
        {
            this._solver.Stop();
        }
    }
}

using ClearLines.Bumblebee;
using System.Reactive.Subjects;
using FoundOps.Common.Silverlight.Tools.ExtensionMethods;
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
        private readonly Solver<IList<IGeoLocation>> _solver = new Solver<IList<IGeoLocation>>();

        /// <summary>
        /// Starts the search. Pushes whenever the solver finds a solution.
        /// Cannot expost IObservable because it conflicts with FSharp.Core which contains the same type, 
        /// this conflict is why FoundOPS.SLClient.Algorithm is a seperate project.
        /// </summary>
        /// <param name="locationsToRoute">The locations to route</param>
        public Subject<SolutionMessage<IList<IGeoLocation>>> Search(IList<IGeoLocation> locationsToRoute)
        {
            var foundSolution = new Subject<SolutionMessage<IList<IGeoLocation>>>();

            _solver.FoundSolution += (s, e) => foundSolution.OnNext(e);

            var generator = new Func<Random, IList<IGeoLocation>>(random => Tsp.Shuffle(random, locationsToRoute));
            var mutator = new Func<IList<IGeoLocation>, Random, IList<IGeoLocation>>((solution, random) => Tsp.Swap(random, solution));
            var evaluator = new Func<IList<IGeoLocation>, double>(circuit => -Tsp.Length(circuit));
            var problem = new Problem<IList<IGeoLocation>>(generator, mutator, evaluator);
            _solver.Search(problem);

            return foundSolution;
        }

        public void StopSearch()
        {
            _solver.Stop();
        }
    }
}

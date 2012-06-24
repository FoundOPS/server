using FoundOps.Common.Silverlight.Tools.ExtensionMethods;
using FoundOps.Common.Tools;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.SLClient.Algorithm;
using FoundOps.SLClient.Data.ViewModels;
using FoundOps.SLClient.UI.Controls.Dispatcher;
using MEFedMVVM.ViewModelLocator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using System.Windows;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// Manages the logic for the routing algorithm and hooks it up to the UI
    /// </summary>
    [ExportViewModel("AlgorithmVM")]
    public class AlgorithmVM : DataFedVM
    {
        private int _currentDistance;
        /// <summary>
        /// The current calculated distance (in miles)
        /// </summary>
        public int CurrentDistance
        {
            get { return _currentDistance; }
            set
            {
                _currentDistance = value;
                this.RaisePropertyChanged("CurrentDistance");
            }
        }

        private TimeSpan _timeRemaining;

        /// <summary>
        /// The time remaining for the algorithm to run.
        /// </summary>
        public TimeSpan TimeRemaining
        {
            get
            {
                return _timeRemaining;
            }
            set
            {
                _timeRemaining = value;
                this.RaisePropertyChanged("TimeRemaining");
            }
        }

        /// <summary>
        /// Calculate the best organization of the tasks.
        /// It will seperate tasks based on service type, and then push the ordered task collections when complete.
        /// This will open the AlgorithmProgress window.
        /// </summary>
        /// <param name="unroutedTaskHolders">The task holders to order.</param>
        /// <param name="serviceTypes">The service types to consider.</param>
        /// <returns>An observable to stay asynchronous. The result is pushed once, so take the first item.</returns>
        public IObservable<IEnumerable<IEnumerable<TaskHolder>>> OrderTasks(IEnumerable<TaskHolder> unroutedTaskHolders, IEnumerable<string> serviceTypes)
        {
            var algorithmStatus = new AlgorithmStatus();
            var result = new Subject<IEnumerable<IEnumerable<TaskHolder>>>();

            //Organize the unroutedTaskHolders by ServiceTemplateName
            //only choose task holders that have LocationIds, a ServiceName, and a Latitude and a Longitude
            var taskHoldersToRoute =
                unroutedTaskHolders.Where(th =>
                    th.LocationId.HasValue && th.ServiceName != null && th.Latitude.HasValue && th.Longitude.HasValue)
                    .ToArray();

            //Find which service types should be routed by choosing the (distinct) ServiceTypes of the unroutedTaskHolders
            //only choose the types that should be routes
            //Then only choose route types Make sure each of those service types have at least one route with that RouteType
            var distinctServiceTemplates = taskHoldersToRoute.Select(th => th.ServiceName).Distinct()
                .Where(st => serviceTypes.Any(t => t == st))
                .ToArray();

            //Seperate the TaskHolders by ServiceTemplate Name to prevent routing different service types together
            var taskHolderCollections =
                distinctServiceTemplates.Select(serviceTemplateName => taskHoldersToRoute.Where(th => th.ServiceName == serviceTemplateName));

            #region Depot no longer used
            ////If there is not a depot set. Default to FoundOPS, 1305 Cumberland Ave, 47906: 40.460335, -86.929840
            //var depot = new GeoLocation(40.460335, -86.929840);

            ////Try to get the depot from the business account
            //var ownerBusinessAccount = Manager.Context.OwnerAccount as BusinessAccount;
            //if (ownerBusinessAccount != null && ownerBusinessAccount.Depots.Any())
            //{
            //    var depotLocation = ownerBusinessAccount.Depots.First();
            //    if (depotLocation.Latitude.HasValue && depotLocation.Longitude.HasValue)
            //        depot = new GeoLocation((double)depotLocation.Latitude.Value, (double)depotLocation.Longitude.Value);
            //}
            #endregion

            //Setup countdown timer
            //a) aggregate the total time to route
            //b) countdown
            var totalTime = new TimeSpan();
            totalTime = taskHolderCollections.Select(taskHolderCollection => taskHolderCollection.Count())
                .Aggregate(totalTime, (current, totalTasks) => current + TimeToCalculate(totalTasks));

            //b) countdown
            this.TimeRemaining = totalTime;
            //var dispatcherTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            //dispatcherTimer.Tick += (s, e) =>
            //{
            //    TimeRemaining -= TimeSpan.FromSeconds(1);

            //    if (TimeRemaining > new TimeSpan()) return;
            //    TimeRemaining = new TimeSpan();
            //    dispatcherTimer.Stop();
            //};

            //Calculate the order, one collection at a time
            //pefomr the calculations asynchronously
            Observable.Start(() =>
            {
                PerformNextCalcuation(taskHolderCollections, 0, new List<IEnumerable<TaskHolder>>(), algorithmStatus, result);
            });

            algorithmStatus.Show();
            return result.AsObservable();
        }

        /// <summary>
        /// A recursive method that will calculate the order of the routes, one route at a time
        /// </summary>
        /// <param name="geoLocationCollections">The collections to order</param>
        /// <param name="index">The index of the collection to calculate</param>
        /// <param name="orderedTaskHolderCollections">The ordered taskHolder collections</param>
        /// <param name="statusWindow">The window to close when complete</param>
        /// <param name="resultSubject">The subject to push when complete</param>
        private void PerformNextCalcuation(IEnumerable<IEnumerable<IGeoLocation>> geoLocationCollections, int index, List<IEnumerable<TaskHolder>> orderedTaskHolderCollections,
            AlgorithmStatus statusWindow, Subject<IEnumerable<IEnumerable<TaskHolder>>> resultSubject)
        {
            //if all routes have been calculated, close the algorithm status window and return
            if (index >= orderedTaskHolderCollections.Count())
            {
                Application.Current.MainWindow.Dispatcher.BeginInvoke(statusWindow.Close);
                resultSubject.OnNext(orderedTaskHolderCollections);
                return;
            }

            var collectionToCalculate = geoLocationCollections.ElementAt(index).ToList();

            var timeToCalculate = TimeToCalculate(collectionToCalculate.Count);
            var calculator = new HiveRouteCalculator();
            var foundSolution = calculator.Search(collectionToCalculate);
            IList<IGeoLocation> bestSolution = null;
            foundSolution.Subscribe(solutionMessage =>
            {
                bestSolution = solutionMessage.Solution;
                Application.Current.MainWindow.Dispatcher.BeginInvoke(() =>
                    //convert meters to miles
                    CurrentDistance = (int)(solutionMessage.Quality / 0.000621371192));
            });

            //Stop after timeToCalculate is up, then perform next calculation
            Rxx3.RunDelayed(timeToCalculate, () =>
            {
                calculator.StopSearch();
                orderedTaskHolderCollections.Add(bestSolution.Cast<TaskHolder>());
                index++;
                PerformNextCalcuation(geoLocationCollections, index, orderedTaskHolderCollections, statusWindow, resultSubject);
            });
        }

        /// <summary>
        /// Returns the time the algorithm should spend calculating a set of tasks
        /// </summary>
        private static TimeSpan TimeToCalculate(int numberTasks)
        {
            //anything less than 15 stops: 5 seconds, otherwise # tasks * 1 second
            return numberTasks < 15 ? TimeSpan.FromSeconds(5) : TimeSpan.FromSeconds(numberTasks);
        }
    }
}

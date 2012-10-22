using FoundOps.Core.Models.CoreEntities;
using FoundOps.SLClient.Data.Services;
using FoundOps.SLClient.Data.ViewModels;
using FoundOps.SLClient.UI.Controls.Dispatcher;
using ReactiveUI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Threading;
using TaskOptimizer.API;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// Manages the logic for the routing algorithm and hooks it up to the UI
    /// </summary>
    public class AlgorithmVM : DataFedVM
    {
        private ReactiveCommand _cancelCommand = new ReactiveCommand();
        /// <summary>
        /// A command to cancel the calculation
        /// </summary>
        public ReactiveCommand CancelCommand
        {
            get { return _cancelCommand; }
            private set { _cancelCommand = value; }
        }

        /// <summary>
        /// The percent (from 0-100) complete for the current algorithm calculation.
        /// </summary>
        public int PercentProgress
        {
            get { return _percentProgress; }
            private set
            {
                _percentProgress = value;
                this.RaisePropertyChanged("PercentProgress");
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
                this.RaisePropertyChanged("PercentProgress");


                var percentage = (TotalCalculationTime.Ticks - TimeRemaining.Ticks) / (double)TotalCalculationTime.Ticks;
                PercentProgress = Convert.ToInt32(percentage * 100.0);
            }
        }

        private TimeSpan _totalCalculationTime;
        private int _percentProgress;

        /// <summary>
        /// The calculated responses
        /// </summary>
        private readonly List<OSMResponse> _responses = new List<OSMResponse>();

        /// <summary>
        /// The route task collections to calculate
        /// </summary>
        private IEnumerable<IEnumerable<RouteTask>> _routeTaskCollections;

        /// <summary>
        /// The time remaining for the algorithm to run.
        /// </summary>
        public TimeSpan TotalCalculationTime
        {
            get { return _totalCalculationTime; }
            private set
            {
                _totalCalculationTime = value;
                this.RaisePropertyChanged("TotalCalculationTime");
            }
        }

        AlgorithmStatus _algorithmStatus;
        /// <summary>
        /// Calculate the best organization of the tasks.
        /// It will seperate tasks based on service type, and then push the ordered task collections when complete.
        /// This will open the AlgorithmProgress window.
        /// </summary>
        /// <param name="unroutedRouteTasks">The task holders to order.</param>
        /// <param name="serviceTypes">The service types to consider.</param>
        /// <returns>An observable to stay asynchronous. The result is pushed once, so take the first item.</returns>
        public IObservable<IEnumerable<IEnumerable<RouteTask>>> OrderTasks(IEnumerable<RouteTask> unroutedRouteTasks, IEnumerable<string> serviceTypes)
        {
            _responses.Clear();
            _routeTaskCollections = null;

            _algorithmStatus = new AlgorithmStatus { AlgorithmVM = this };
            var result = new Subject<IEnumerable<IEnumerable<RouteTask>>>();

            //Organize the unroutedRouteTasks by ServiceTemplateName
            //only choose task holders that have LocationIds, a ServiceName, and a Latitude and a Longitude
            var routeTasksToRoute =
                unroutedRouteTasks.Where(th =>
                    th.LocationId.HasValue && th.Name != null && th.Location.Latitude.HasValue && th.Location.Longitude.HasValue)
                    //TODO Remove, for testing
                    .Take(25)
                    .ToArray();

            //Find which service types should be routed by choosing the (distinct) ServiceTypes of the unroutedRouteTasks
            //only choose the types that should be routes
            //Then only choose route types Make sure each of those service types have at least one route with that RouteType
            var distinctServiceTemplates = routeTasksToRoute.Select(th => th.Name).Distinct()
                .Where(st => serviceTypes.Any(t => t == st))
                .ToArray();

            //Seperate the RouteTasks by ServiceTemplate Name to prevent routing different service types together
            var routeTaskCollections =
                distinctServiceTemplates.Select(serviceTemplateName => routeTasksToRoute.Where(th => th.Name == serviceTemplateName));

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

            int totalTasksToRoute = routeTaskCollections.Select(RouteTaskCollection => RouteTaskCollection.Count()).Sum();

            if (totalTasksToRoute <= 0)
                return Observable.Empty<IEnumerable<IEnumerable<RouteTask>>>();

            //Setup countdown timer
            //a) aggregate the total time to route
            //b) countdown
            TotalCalculationTime = TimeToCalculate(totalTasksToRoute);
            this.TimeRemaining = TotalCalculationTime;
            var dispatcherTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            dispatcherTimer.Tick += (s, e) =>
            {
                TimeRemaining -= TimeSpan.FromSeconds(1);

                if (TimeRemaining > new TimeSpan()) return;

                //When there is no TimeRemaining (and this has not been stopped), add 25% initial estimated time
                TimeRemaining = TimeToCalculate(totalTasksToRoute / 2);
            };
            dispatcherTimer.Start();

            _routeTaskCollections = routeTaskCollections;

            //Calculate the order, in parallel & asynchronously
            if (totalTasksToRoute >= 0)
            {
                foreach (var routeTaskCollection in routeTaskCollections)
                {
                    var coordinates = routeTaskCollection.Select(l => String.Format("{0},{1}", l.Location.Latitude.ToString(), l.Location.Longitude.ToString())).Aggregate((a, b) => a + "$" + b);
                    Manager.Data.DomainContext.MakeTransaction(coordinates, (invokOp) => OnCalculated(invokOp, result), routeTaskCollection);
                }
            }

            _algorithmStatus.Show();

            //Stop the algorithm on cancel search and return
            CancelCommand.Subscribe(_ =>
            {
                dispatcherTimer.Stop();
                TimeRemaining = new TimeSpan();

                _algorithmStatus.Close();
            });

            return result.AsObservable();
        }

        private void OnCalculated(System.ServiceModel.DomainServices.Client.InvokeOperation<OSMResponse> invokOp, Subject<IEnumerable<IEnumerable<RouteTask>>> result)
        {
            if (invokOp.IsCanceled)
                return;

            if (invokOp.HasError)
            {
                //TODO Perform retry logic
                //obj.UserState
                return;
            }

            _responses.Add(invokOp.Value);

            //if all routes have been calculated, close the algorithm status window and return
            if (_routeTaskCollections != null && _responses.Count == _routeTaskCollections.Count())
            {
                _algorithmStatus.Close();
                //_responses.Select(r=>r
                //result.OnNext(orderedRouteTaskCollections);
            }
        }

        /// <summary>
        /// Returns the time the algorithm should spend calculating a set of tasks
        /// </summary>
        private static TimeSpan TimeToCalculate(int numberTasks)
        {
            //anything less than 15 stops: 5 seconds, otherwise # tasks * .75 second
            return numberTasks < 15 ? TimeSpan.FromSeconds(5) : TimeSpan.FromSeconds(Math.Round(numberTasks * .75));
        }
    }
}

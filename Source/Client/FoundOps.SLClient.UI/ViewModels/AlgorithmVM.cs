using System.Reactive.Linq;
using FoundOps.Common.Silverlight.Tools.ExtensionMethods;
using FoundOps.Common.Tools;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.SLClient.Algorithm;
using FoundOps.SLClient.Data.ViewModels;
using MEFedMVVM.ViewModelLocator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Windows;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// Manages the logic for the routing algorithm and hooks it up to the UI
    /// </summary>
    [ExportViewModel("AlgorithmVM")]
    public class AlgorithmVM : DataFedVM
    {
        private TimeSpan _totalTime;
        /// <summary>
        /// The TotalTime for the algorithm to run.
        /// </summary>
        public TimeSpan TotalTime
        {
            get { return _totalTime; }
            set
            {
                _totalTime = value;
                this.RaisePropertyChanged("TotalTime");
            }
        }
        //return Observable.Interval(delay).Take(1).ObserveOnDispatcher().Subscribe(_ => action());

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
            var result = new Subject<IEnumerable<IEnumerable<TaskHolder>>>();

            var routedTaskHolders = new List<List<TaskHolder>>();

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

            //Go through each collection to route and aggregate the time to route
            var totalTime = new TimeSpan();
            totalTime = taskHolderCollections.Select(taskHolderCollection => taskHolderCollection.Count())
                .Aggregate(totalTime, (current, totalTasks) => current + TimeToCalculate(totalTasks));

            //update the VM (to update the UI)
            Application.Current.RootVisual.Dispatcher.BeginInvoke(() => this.TotalTime = totalTime);


            //Go through each different service type's route tasks (each routeTaskHolderCollection) and route them
            foreach (var routeTaskHolderCollection in taskHolderCollections)
            {
                //Only route tasks with lat/longs
                var unorganizedTaskHolders = routeTaskHolderCollection.ToList();

                var geoLocations = unorganizedTaskHolders.Cast<IGeoLocation>().ToList();

                var whenToStop = TimeToCalculate(geoLocations.Count);
                var calculator = new HiveRouteCalculator(geoLocations);
                Rxx3.RunDelayed(whenToStop, calculator.Stop);
            }

            return result.AsObservable();
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

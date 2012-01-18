using System;
using System.Linq;
using FoundOps.Common.Silverlight.Tools.ExtensionMethods;
using FoundOps.Common.Tools;
using System.Reactive.Linq;
using Telerik.Windows.Controls;
using System.Reactive.Subjects;
using MEFedMVVM.ViewModelLocator;
using System.Collections.ObjectModel;
using FoundOps.Core.Context.Extensions;
using FoundOps.SLClient.Data.Services;
using FoundOps.SLClient.Data.ViewModels;
using FoundOps.Core.Models.CoreEntities;
using Microsoft.Windows.Data.DomainServices;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// Gets the necessary data for the RouteScheduleVM
    /// </summary>
    [ExportViewModel("RouteScheduleVM")]
    public class RouteScheduleVM : CoreEntityCollectionVM<Route>
    {
        #region Independent Variables

        private ObservableCollection<ResourceType> _resourceTypesCollection;

        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<ResourceType> ResourceTypesCollection
        {
            get { return _resourceTypesCollection; }
            private set
            {
                _resourceTypesCollection = value;
                this.RaisePropertyChanged("ResourceTypesCollection");
            }
        }

        private ObservableCollection<ScheduleViewAppointment> _appointmentCollection;

        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<ScheduleViewAppointment> AppointmentCollection
        {
            get { return _appointmentCollection; }
            private set
            {
                _appointmentCollection = value;
                this.RaisePropertyChanged("AppointmentCollection");
            }
        }

        /// <summary>
        /// Gets the earliest start time of the routes
        /// </summary>
        public TimeSpan EarliestStartTime
        {
            get
            {
                return DomainCollectionView.Count() <= 0 ? new TimeSpan(9, 0, 0) :
                    DomainCollectionView.OrderBy(r => r.StartTime).First().StartTime.TimeOfDay;
            }
        }

        /// <summary>
        /// Gets the latest end time of the routes
        /// </summary>
        public TimeSpan LatestEndTime
        {
            get
            {
                return DomainCollectionView.Count() <= 0 ? new TimeSpan(17, 0, 0) :
                    DomainCollectionView.OrderBy(r => r.EndTime).Last().EndTime.TimeOfDay;
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="RouteScheduleVM"/> class.
        /// </summary>
        /// <param name="domainCollectionView">The domain collection view.</param>
        /// <param name="dataManager">The data manager.</param>
        public RouteScheduleVM(DomainCollectionView<Route> domainCollectionView, DataManager dataManager)
            : base(false, dataManager)
        {
            //DomainCollectionViewObservable.OnNext(domainCollectionView);

            //var routes = this.DomainCollectionView.SourceCollection as ObservableCollection<Route>;

            ////Create a stream of the earliest Route.StartTime

            ////For each collection change
            //routes.FromCollectionChanged().SelectMany(
            //    //Select the earliest route's StartTime
            //new BehaviorSubject<DateTime>(routes.Select(rt => rt.StartTime).OrderBy(st => st).FirstOrDefault()).Merge(
            //    //Merge with the StartTime property change's earliest StartTime
            //routes.Select(route => Observable2.FromPropertyChangedPattern(route, rt => route.StartTime)).Merge()))
            //    //Select the EarliestStartTime
            //.ObserveOnDispatcher().Select(est => this.EarliestStartTime).DistinctUntilChanged()
            //    //Whenever the earliest Route.StartTime changes, update EarliestStartTime
            //.SubscribeOnDispatcher().Subscribe(rtst => this.RaisePropertyChanged("EarliestStartTime"));

            ////Create a stream of the latest Route.EndTime

            ////For each collection change
            //routes.FromCollectionChanged().SelectMany(
            //    //Select the latest route's EndTime
            //new BehaviorSubject<DateTime>(routes.Select(rt => rt.EndTime).OrderByDescending(st => st).FirstOrDefault()).Merge(
            //    //Merge with the EndTime property change's latest EndTime
            //routes.Select(route => Observable2.FromPropertyChangedPattern(route, rt => route.EndTime)).Merge()))
            //    //Select the LatestEndTime
            //.ObserveOnDispatcher().Select(est => this.LatestEndTime).DistinctUntilChanged()
            //    //Whenever the earliest Route.EndTime changes, update LatestEndTime
            //.SubscribeOnDispatcher().Subscribe(rtet => this.RaisePropertyChanged("LatestEndTime"));

            //ResourceTypesCollection = new ObservableCollection<ResourceType>();
            //AppointmentCollection = new ObservableCollection<ScheduleViewAppointment>();

            //routes.FromCollectionChanged().SubscribeOnDispatcher().Subscribe(routesCollectionChanged =>
            //{
            //    //Update the ResourceTypesCollection
            //    var routeResourceType = new ResourceType("Route");
            //    foreach (var route in DomainCollectionView)
            //        routeResourceType.Resources.Add(new Resource(route.Name));

            //    ResourceTypesCollection = new ObservableCollection<ResourceType> { routeResourceType };

            //    //Update the AppointmentCollection
            //    AppointmentCollection = new ObservableCollection<ScheduleViewAppointment>(
            //        DomainCollectionView.SelectMany(route => route.RouteDestinations.Select(rd => rd.ScheduleViewAppointment)));
            //});
        }
    }
}

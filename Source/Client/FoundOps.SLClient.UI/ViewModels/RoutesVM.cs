using System;
using FoundOps.SLClient.UI.Controls.Dispatcher.Manifest;
using ReactiveUI;
using System.Linq;
using ReactiveUI.Xaml;
using RiaServicesContrib;
using FoundOps.Common.Tools;
using System.Reactive.Linq;
using System.Windows.Controls;
using System.Reactive.Subjects;
using Telerik.Windows.Controls;
using MEFedMVVM.ViewModelLocator;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using FoundOps.Core.Context.Tools;
using System.Collections.ObjectModel;
using FoundOps.SLClient.Data.Services;
using FoundOps.Core.Models.CoreEntities;
using System.ComponentModel.Composition;
using FoundOps.SLClient.Data.ViewModels;
using Microsoft.Windows.Data.DomainServices;
using FoundOps.SLClient.UI.Controls.Dispatcher;
using FoundOps.Server.Services.CoreDomainService;
using FoundOps.Common.Silverlight.Tools.ExtensionMethods;
using System.ServiceModel.DomainServices.Client;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// Contains the logic for displaying and dispatching Routes.
    /// </summary>
    [ExportViewModel("RoutesVM")]
    public class RoutesVM : CoreEntityCollectionVM<Route>
    {
        /// <summary>
        /// The custom queries the RoutesVM uses
        /// </summary>
        public enum Query
        {
            /// <summary>
            /// Load the Routes for the Selected Date
            /// </summary>
            RoutesForServiceProviderOnDay,
            /// <summary>
            /// Load the unrouted RouteTasks for the Selected Date
            /// </summary>
            UnroutedRouteTasks
        }

        #region Public

        #region Commands

        /// <summary>
        /// A command to automatically place the unrouted tasks into Routes
        /// </summary>
        public IReactiveCommand AutoCalculateRoutes { get; set; }

        /// <summary>
        /// A command to open the Route Manifests
        /// </summary>
        public IReactiveCommand OpenRouteManifests { get; set; }

        #region Route Tasks

        /// <summary>
        /// A command to add a route task.
        /// </summary>
        public IReactiveCommand AddRouteTask { get; private set; }

        /// <summary>
        /// A command to delete a route task.
        /// </summary>
        public IReactiveCommand DeleteRouteTask { get; private set; }

        #endregion

        #region Date Modifiers

        /// <summary>
        /// A command to set the selected day to today.
        /// </summary>
        public RelayCommand SetSelectedDayToToday { get; private set; }

        /// <summary>
        /// A command to set the selected day one day previous than the selected date.
        /// </summary>
        public RelayCommand SetSelectedDayOneDayPrevious { get; private set; }

        /// <summary>
        /// A command to set the selected day one day past the selected date.
        /// </summary>
        public RelayCommand SetSelectedDayOneDayForward { get; private set; }

        #endregion

        #endregion

        #region Filters

        private readonly ObservableCollection<string> _routeTypes = new ObservableCollection<string>();
        /// <summary>
        /// A temporary solution for filtering. Currently represents ServiceTemplates
        /// </summary>
        public ObservableCollection<string> RouteTypes { get { return _routeTypes; } }

        private readonly ObservableCollection<string> _regions = new ObservableCollection<string>();
        /// <summary>
        /// A temporary solution for filtering. Currently represents ServiceTemplates
        /// </summary>
        public ObservableCollection<string> Regions { get { return _regions; } }

        private readonly ObservableCollection<string> _selectedRegions = new ObservableCollection<string>();
        /// <summary>
        /// Gets the selected regions of routes to display.
        /// </summary>
        public ObservableCollection<string> SelectedRegions { get { return _selectedRegions; } }

        private readonly ObservableCollection<string> _selectedRouteTypes = new ObservableCollection<string>();
        /// <summary>
        /// Gets the selected route types of routes to display.
        /// </summary>
        public ObservableCollection<string> SelectedRouteTypes { get { return _selectedRouteTypes; } }

        private readonly ObservableAsPropertyHelper<int> _forceFilterCountUpdate;

        /// <summary>
        /// Used to force the bindings on the Dispatching filter counts. 
        /// </summary>
        public int ForceFilterCountUpdate { get { return _forceFilterCountUpdate.Value; } }

        #endregion

        #region Loading

        private ObservableAsPropertyHelper<bool> _tasksLoading;
        /// <summary>
        /// Gets a value indicating whether [tasks is loading].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [tasks is loading]; otherwise, <c>false</c>.
        /// </value>
        public bool TasksLoading { get { return _tasksLoading.Value; } }

        private ObservableAsPropertyHelper<bool> _routesLoading;
        /// <summary>
        /// Gets a value indicating whether [routes is loading].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [routes is loading]; otherwise, <c>false</c>.
        /// </value>
        public bool RoutesLoading { get { return _routesLoading.Value; } }

        #endregion

        #region SelectedItems

        /// <summary>
        /// True if AutoAssignButton has been clicked
        /// </summary>
        public bool AutoAssignButtonHasBeenClicked;
        /// <summary>
        /// Tracks if the auto assign button has been clicked (for analytics)
        /// </summary>
        public Button AutoAsignButtonClickedOn
        {
            set
            {
                if (AutoAssignButtonHasBeenClicked)
                    return;

                AutoAssignButtonHasBeenClicked = true;
            }
        }

        private DateTime _selectedDate = DateTime.Now;
        /// <summary>
        /// The SelectedDate. This is used to load the proper Routes and RouteTasks
        /// </summary>
        public DateTime SelectedDate
        {
            get { return _selectedDate; }
            set
            {
                _selectedDate = value;
                this.RaisePropertyChanged("SelectedDate");
            }
        }

        private readonly Subject<RouteDestination> _selectedRouteDestinationSubject = new Subject<RouteDestination>();
        private readonly ObservableAsPropertyHelper<RouteDestination> _selectedRouteDestination;
        /// <summary>
        /// The first selected route destination in route tree view
        /// </summary>
        public RouteDestination SelectedRouteDestination { get { return _selectedRouteDestination.Value; } set { _selectedRouteDestinationSubject.OnNext(value); } }

        private ObservableCollection<RouteDestination> _selectedRouteDestinations = new ObservableCollection<RouteDestination>();
        /// <summary>
        /// The route destinations selected in route tree view
        /// </summary>
        public ObservableCollection<RouteDestination> SelectedRouteDestinations
        {
            get { return _selectedRouteDestinations; }
            set
            {
                _selectedRouteDestinations = value;
                this.RaisePropertyChanged("SelectedRouteDestinations");
            }
        }

        private readonly Subject<RouteTask> _selectedTaskSubject = new Subject<RouteTask>();
        private readonly ObservableAsPropertyHelper<RouteTask> _selectedTask;
        /// <summary>
        /// The first selected Task in route tree view
        /// </summary>
        public RouteTask SelectedTask { get { return _selectedTask.Value; } set { _selectedTaskSubject.OnNext(value); } }

        #endregion

        #region Sub ViewModels

        /// <summary>
        /// Gets the RouteManifestVM.
        /// </summary>
        public RouteManifestVM RouteManifestVM { get; private set; }

        private RouteScheduleVM _routeScheduleVM;
        public RouteScheduleVM RouteScheduleVM
        {
            get { return _routeScheduleVM; }
            set
            {
                _routeScheduleVM = value;
                this.RaisePropertyChanged("RouteScheduleVM");
            }
        }

        private readonly Subject<ContactInfoVM> _selectedRouteDestinationContactInfoVMSubject = new Subject<ContactInfoVM>();
        private readonly ObservableAsPropertyHelper<ContactInfoVM> _selectedRouteDestinationContactInfoVM;
        /// <summary>
        /// Gets the ContactInfoVM for the selected route destination.
        /// </summary>
        public ContactInfoVM SelectedRouteDestinationContactInfoVM { get { return _selectedRouteDestinationContactInfoVM.Value; } }

        private readonly Subject<SelectedEmployeesVM> _selectedRouteSelectedEmployeesVMSubject = new Subject<SelectedEmployeesVM>();
        private readonly ObservableAsPropertyHelper<SelectedEmployeesVM> _selectedRouteSelectedEmployeesVM;
        /// <summary>
        /// Gets the SelectedEmployeesVM for the selected route.
        /// </summary>
        public SelectedEmployeesVM SelectedRouteSelectedEmployeesVM { get { return _selectedRouteSelectedEmployeesVM.Value; } }

        private readonly Subject<SelectedVehiclesVM> _selectedRouteSelectedVehiclesVMSubject = new Subject<SelectedVehiclesVM>();
        private readonly ObservableAsPropertyHelper<SelectedVehiclesVM> _selectedRouteSelectedVehiclesVM;
        /// <summary>
        /// Gets the SelectedVehiclesVM for the selected route.
        /// </summary>
        public SelectedVehiclesVM SelectedRouteSelectedVehiclesVM { get { return _selectedRouteSelectedVehiclesVM.Value; } }

        #endregion

        private readonly ObservableAsPropertyHelper<ObservableCollection<RouteTask>> _unroutedTasks;
        /// <summary>
        /// The UnroutedTasks.
        /// It is not an EntityList so that Items can be added/removed without affecting the EntitySet for
        /// a) DragAndDrop and b) auto route calculation.
        /// </summary>
        public ObservableCollection<RouteTask> UnroutedTasks { get { return _unroutedTasks.Value; } }

        #endregion

        #region Locals

        private readonly Subject<ObservableCollection<RouteTask>> _loadedRouteTasks = new Subject<ObservableCollection<RouteTask>>();
        private readonly ObservableAsPropertyHelper<ObservableCollection<RouteTask>> _loadedRouteTasksHelper;
        /// <summary>
        /// The loaded route tasks (disconnected from an EntitySet)
        /// </summary>
        public ObservableCollection<RouteTask> LoadedRouteTasks { get { return _loadedRouteTasksHelper.Value; } }

        /// <summary>
        /// Called whenever the Filter needs updating.
        /// </summary>
        private readonly IObservable<bool> _updateFilter;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="RoutesVM"/> class.
        /// </summary>
        /// <param name="dataManager">The data manager.</param>
        [ImportingConstructor]
        public RoutesVM(DataManager dataManager)
            : base(false, dataManager)
        {
            #region Setup SelectedItems properties

            _selectedRouteDestination = _selectedRouteDestinationSubject.ToProperty(this, x => x.SelectedRouteDestination);
            _selectedTask = _selectedTaskSubject.ToProperty(this, x => x.SelectedTask);

            #endregion

            #region Setup Sub ViewModels

            //Setup the RouteManifestVM
            RouteManifestVM = new RouteManifestVM(dataManager);

            //Setup the ContactInfoVM, SelectedEmployeesVM, and SelectedVehiclesVM properties
            _selectedRouteDestinationContactInfoVM = _selectedRouteDestinationContactInfoVMSubject.ToProperty(this, x => x.SelectedRouteDestinationContactInfoVM);
            _selectedRouteSelectedEmployeesVM = _selectedRouteSelectedEmployeesVMSubject.ToProperty(this, x => x.SelectedRouteSelectedEmployeesVM);
            _selectedRouteSelectedVehiclesVM = _selectedRouteSelectedVehiclesVMSubject.ToProperty(this, x => x.SelectedRouteSelectedVehiclesVM);

            //Whenever the Selected route changes: update the SelectedEmployeesVM and SelectedVehiclesVM
            SelectedEntityObservable.Where(sr => sr != null).Subscribe(selectedRoute =>
            {
                _selectedRouteSelectedEmployeesVMSubject.OnNext(new SelectedEmployeesVM(DataManager, selectedRoute.Technicians));
                _selectedRouteSelectedVehiclesVMSubject.OnNext(new SelectedVehiclesVM(DataManager, selectedRoute.Vehicles));
            });

            //Whenever the Selected Route Destination changes: update the _selectedRouteDestinationContactInfoVM
            _selectedRouteDestinationSubject.Where(srd => srd != null && srd.Location != null).Subscribe(selectedRouteDestination =>
                _selectedRouteDestinationContactInfoVMSubject.OnNext(
                new ContactInfoVM(DataManager, ContactInfoType.Locations, selectedRouteDestination.Location.ContactInfoSet)));

            #endregion

            #region Setup RouteTask properties

            //Setup the LoadedRouteTasks property
            _loadedRouteTasksHelper = this._loadedRouteTasks.ToProperty(this, x => x.LoadedRouteTasks);

            //Get all of the unroutedRouteTasks when a) the collection is changed or set, or b) a RouteTasks destination changes
            //a) the collection is changed or set
            var loadedRouteTasksA = _loadedRouteTasks.FromCollectionChangedOrSet();
            //b) a RouteTasks destination changes
            var loadedRouteTasksB =
                _loadedRouteTasks.FromCollectionChangedOrSet() //whenever the collection is changed or set
                .SelectLatest(rts => rts.Select(rt => //select all of the RouteDestination property changes
                        Observable2.FromPropertyChangedPattern(rt, t => t.RouteDestination)).Merge().Select(rt => rts));

            //Create an ObservableCollection of the unroutedRouteTasks whenever a or b
            var unroutedRouteTasks = loadedRouteTasksA.Merge(loadedRouteTasksB)
                //throttle by .2 seconds to prevent overcalculation
                .Throttle(new TimeSpan(0, 0, 0, 0, 200))
                //Select the unroutedRouteTasks
                   .Select(rts => rts.Where(rt => rt.RouteDestination == null || rt.RouteDestination.Route == null).ToObservableCollection());

            //Setup the UnroutedTasks property
            _unroutedTasks = unroutedRouteTasks.ToProperty(this, x => x.UnroutedTasks);

            #endregion

            LoadData();

            #region Setup UpdateFilter observable

            //Update Filter whenever: a) routes are changed or is set, b) the SelectedRouteTypes changes,
            //c) the SelectedRegions changes, d) a route's RouteType is changed

            var loadedRoutesChanged = DataManager.GetEntityListObservable<Route>(Query.RoutesForServiceProviderOnDay);

            _updateFilter =
                //a) routes are changed or set
               loadedRoutesChanged.FromCollectionChangedOrSet().AsGeneric()
                //b) the SelectedRouteType changes
                .Merge(SelectedRouteTypes.FromCollectionChangedGeneric())
                //c) the SelectedRegions changes
                .Merge(SelectedRegions.FromCollectionChangedGeneric())
                //d) a route's RouteType is changed
                .Merge(loadedRoutesChanged.FromCollectionChangedOrSet().SelectLatest(rts => 
                    rts.Select(rt => Observable2.FromPropertyChangedPattern(rt, x => x.RouteType)).Merge())
                    .AsGeneric());

            #endregion

            RegisterCommands();

            #region Filters

            //Subscribe to the ServiceTemplates entityList
            this.DataManager.Subscribe<ServiceTemplate>(DataManager.Query.ServiceTemplates, ObservationState, null);

            //Whenever the service templates collection changes, SetupRouteTypes
            DataManager.GetEntityListObservable<ServiceTemplate>(DataManager.Query.ServiceTemplates)
                .FromCollectionChangedOrSet().Subscribe(SetupRouteTypesFilter);

            //Setup the RegionsFilter from loaded Locations
            DataManager.GetEntityListObservable<Location>(DataManager.Query.Locations)
                .FromCollectionChangedOrSet().Subscribe(SetupRegionsFilter);

            //Filter Routes depending on the RouteType and Region
            Predicate<object> filter = obj =>
            {
                var route = obj as Route;
                if (route == null) return false;

                //It meets the region filter if: a) a route has no destinations
                //or b) a route has a destination with no location (because it hasnt loaded) or a location of a selected region
                //var meetsRegionFilter = route.NumberOfRouteDestinations == 0 ||
                //    route.RouteDestinations.Any(rd => rd.Location == null || rd.Location.Region == null
                //        || SelectedRegions.Contains(rd.Location.Region.Name));

                //TODO: Uncomment above
                var meetsRegionFilter = true;

                //It meets the route type filter if: a) a route has no RouteType selected or
                //b) a route is the type of a SelectedRouteType
                var meetsRouteTypeFilter = route.RouteType == null || SelectedRouteTypes.Contains(route.RouteType);

                return meetsRegionFilter && meetsRouteTypeFilter;
            };

            //Throttle .2 seconds to allow add/delete to DCV to happen first, and prevent too many updates
            _updateFilter.Throttle(new TimeSpan(0, 0, 0, 0, 200)).ObserveOnDispatcher().SubscribeOnDispatcher().Subscribe(obj =>
                 {
                     DomainCollectionView.Filter = null;
                     DomainCollectionView.Filter = filter;
                 });

            //Update the counts on the Dispatcher Filter whenever:
            var updateFilterCounts =
                //a) routesOrFiltersChanged
            _updateFilter.Throttle(new TimeSpan(0, 0, 0, 0, 250)).AsGeneric() //delay to wait until DCV is updated
                //merge with b) whenever a Task is added to a route
                .Merge(
                //whenever loadedRoutes is changed or set
                loadedRoutesChanged.FromCollectionChangedOrSet().SelectLatest(lrs =>
                    //whenever loadedRoutes.RouteDestinations is changed (and now)
                  lrs.Select(lr => lr.RouteDestinations.FromCollectionChangedAndNow()
                      //whenever the loadedRoutes.RouteDestinations.RouteTasks is changed (and now)
                      .Select(ea=> (EntityCollection<RouteDestination>)ea.Sender)
                      .SelectLatest(routeDestinations =>
                          routeDestinations.Select(rd => rd.RouteTasks.FromCollectionChangedAndNow())
                          .Merge()) //Merge loadedRoutes.RouteDestinations.RouteTasks collection changed events
                        ).Merge() //Merge loadedRoutes.RouteDestinations collection changed events
                       ).AsGeneric()
                       );

            //Setup ForceFilterCountUpdate property
            _forceFilterCountUpdate = updateFilterCounts.Select(_ => new Random().Next()).ToProperty(this, x => x.ForceFilterCountUpdate);

            #endregion
        }

        /// <summary>
        /// Used in the constructor to setup the data loading process.
        /// </summary>
        private void LoadData()
        {
            //Setup an observable for whenever the Dispatcher is entered/reentered
            var enteredDispatcher = NavigateToObservable.Where(navigateTo => navigateTo.Contains("Dispatcher")).Select(nt => true);

            //Load whenever: the RoleId changes, SelectedDate changes, the Dispatcher is entered, or Discard was called 
            var loadData =
                this.WhenAny(x => x.ContextManager.RoleId, x => x.SelectedDate, (roleId, selectedDate) => roleId.Value != Guid.Empty)
                .Merge(enteredDispatcher).Merge(DiscardObservable).Where(ld => ld).Select(ld => this.ContextManager.RoleId).Throttle(TimeSpan.FromMilliseconds(250));

            #region Load Routes

            //Add RoutesForServiceProviderOnDay query. It should load routes whenever loadData changes
            this.DataManager.AddQuery(Query.RoutesForServiceProviderOnDay,
                                      roleId => this.Context.GetRoutesForServiceProviderOnDayQuery(roleId, SelectedDate),
                                      Context.Routes, loadData);

            //Setup the MainQuery
            this.SetupMainQuery(Query.RoutesForServiceProviderOnDay, routes => RouteScheduleVM = new RouteScheduleVM(DomainCollectionView, DataManager), null, false);

            //Setup RoutesLoading property
            var routesLoading = this.DataManager.GetIsLoadingObservable(Query.RoutesForServiceProviderOnDay)
                .CombineLatest(this.DataManager.GetIsLoadingObservable(DataManager.Query.ServiceTemplates), (a, b) => a || b);

            _routesLoading = routesLoading.ToProperty(this, x => x.RoutesLoading);

            #endregion

            #region Load Route Tasks

            //Add UnroutedRouteTasks query. It should load routes whenever loadData changes)
            this.DataManager.AddQuery(Query.UnroutedRouteTasks, roleId => this.Context.GetUnroutedRouteTasksQuery(roleId, SelectedDate), Context.RouteTasks, loadData);

            //Subscribe to UnroutedRouteTasks (and setup TasksLoading property)
            _tasksLoading = this.DataManager.Subscribe<RouteTask>(Query.UnroutedRouteTasks, ObservationState, OnRouteTasksLoaded).ToProperty(this, x => x.TasksLoading);

            #endregion
        }

        //The only route manifest viewer
        private RouteManifestViewer _routeManifestViewer;
        /// <summary>
        /// Used in the constructor to setup the commands
        /// </summary>
        private void RegisterCommands()
        {
            //Create an observable for when at least 1 route exists
            var routesExist = DataManager.GetEntityListObservable<Route>(Query.RoutesForServiceProviderOnDay) //select the routes EntityList observable
                .FromCollectionChangedOrSet() //on collection changed or set
                .Select(routeEntityList => routeEntityList.Any());  // select routes > 0

            AutoCalculateRoutes = new ReactiveCommand(routesExist);

            //Populate routes with the UnroutedTasks and refresh the filter counts
            AutoCalculateRoutes.Subscribe(_ => SimpleRouteCalculator.PopulateRoutes(this.UnroutedTasks, DomainCollectionView));

            //Allow the user to open the route manifests whenever there is more than one route visible
            OpenRouteManifests = new ReactiveCommand(_updateFilter.ObserveOnDispatcher().Throttle(new TimeSpan(0, 0, 0, 0, 250)).Select(_ => this.DomainCollectionView.Count() > 0));

            OpenRouteManifests.ObserveOnDispatcher().Subscribe(_ =>
            {
                //Setup the route manifest viewer if there is not one yet
                if(_routeManifestViewer == null)
                    _routeManifestViewer = new RouteManifestViewer();

                _routeManifestViewer.Show();

                //Whenever the manifests are opened save the Routes
                if (this.SaveCommand.CanExecute(null))
                    this.SaveCommand.Execute(null);
            });

            #region Date Modifiers

            SetSelectedDayToToday = new RelayCommand(() => this.SelectedDate = DateTime.Now);
            SetSelectedDayOneDayPrevious = new RelayCommand(() => this.SelectedDate = SelectedDate.Date.AddDays(-1));
            SetSelectedDayOneDayForward = new RelayCommand(() => this.SelectedDate = SelectedDate.Date.AddDays(1));

            #endregion

            #region Route Tasks

            //AddRouteTask can add whenever the OwnerAccount != null && tasks are not loading
            AddRouteTask = new ReactiveCommand(this.WhenAny(x => x.ContextManager.OwnerAccount, x => x.TasksLoading,
                (ownerAccount, tasksLoading) => ownerAccount.Value != null && !tasksLoading.Value));

            //DeleteRouteTask can delete whenever the SelectedTask != null
            DeleteRouteTask = new ReactiveCommand(this.WhenAny(x => x.SelectedTask, st => st.Value != null));

            //Setup AddRouteTask command
            AddRouteTask.SubscribeOnDispatcher().Subscribe(_ =>
            {
                //Add a new RouteTask with the information we have
                var routeTask = new RouteTask
                {
                    Date = SelectedDate,
                    OwnerBusinessAccount =
                        (BusinessAccount)ContextManager.OwnerAccount,
                    Name = "New Route Task"
                };

                this.LoadedRouteTasks.Add(routeTask);

                //Set the SelectedTask to the newly added routeTask
                this.SelectedTask = routeTask;
            });

            //Setup DeleteRouteTask command
            DeleteRouteTask.SubscribeOnDispatcher().Subscribe(_ =>
            {
                this.LoadedRouteTasks.Remove(SelectedTask);

                if (this.Context.RouteTasks.Contains(SelectedTask))
                    this.Context.RouteTasks.Remove(SelectedTask);
            });

            #endregion
        }

        /// <summary>
        /// Setup the route types filter.
        /// </summary>
        /// <param name="serviceTemplates">The source service templates.</param>
        private void SetupRouteTypesFilter(IEnumerable<ServiceTemplate> serviceTemplates)
        {
            //Update RouteTypes to the passed serviceTemplates
            RouteTypes.Clear();
            SelectedRouteTypes.Clear();

            //choose the distinct service templates (by name) & select their Name
            var distinctServiceTemplates = serviceTemplates.Select(st => st.Name).Distinct().ToArray().OrderBy(name => name);

            RouteTypes.AddRange(distinctServiceTemplates);
            SelectedRouteTypes.AddRange(distinctServiceTemplates);
        }

        /// <summary>
        /// Setup the regions filter.
        /// </summary>
        /// <param name="locations">The source locations w regions.</param>
        private void SetupRegionsFilter(ObservableCollection<Location> locations)
        {
            //Update Regions to the passed locations' regions

            Regions.Clear();
            SelectedRegions.Clear();

            var distinctRegions = locations.Where(l => l.Region != null).Select(l => l.Region.Name).Distinct().ToArray().OrderBy(name => name);

            Regions.AddRange(distinctRegions);
            SelectedRegions.AddRange(distinctRegions);
        }

        /// <summary>
        /// Called when [route tasks loaded]. Removes Generated RouteTasks and replaces them with clones
        /// </summary>
        /// <param name="routeTasks">The route tasks.</param>
        private void OnRouteTasksLoaded(EntityList<RouteTask> routeTasks)
        {
            var routeTasksToKeep = new ObservableCollection<RouteTask>();

            //Many route tasks are generated on the server and not actually part of the database
            //This is done because RouteTasks get generated from variables that change day to day (ex. RecurringServices).
            //Therefore we want to save the RouteTasks as late as possible, when they are part of Routes

            //Keep the non-generated RouteTasks (to track changes)
            foreach (var nongeneratedRouteTask in routeTasks.Where(rt => !rt.GeneratedOnServer).ToArray())
                routeTasksToKeep.Add(nongeneratedRouteTask);

            //TODO: Change this to happen when hooked up to routes

            //Add the generated route tasks (and their generated services/service templates) to the database
            //Do this by cloning them using RIA Services Contrib's Entity Graph

            var generatedTasksToClone = routeTasks.Where(rt => rt.GeneratedOnServer).ToArray();

            foreach (var generatedRouteTask in generatedTasksToClone)
            {
                var clonedRouteTask =
                    //Clone the service/service template if the Service is generated
                    generatedRouteTask.Clone(generatedRouteTask.Service != null && generatedRouteTask.Service.Generated);

                //Add the clonedRouteTask to the database
                routeTasksToKeep.Add(clonedRouteTask);
            }

            //Detach all related entities to RouteTasks that are not part of a route (do not have a RouteDestination) except the ones to keep
            //You must exclude the ones to keep (because existing entities will become new ones if you detach and add them)
            DataManager.DetachEntities(Context.RouteTasks.Where(rt => rt.RouteDestination == null).Except(routeTasksToKeep).SelectMany(rt => new EntityGraph<Entity>(rt, rt.EntityGraphWithServiceShape)));

            _loadedRouteTasks.OnNext(routeTasksToKeep);
        }

        #endregion

        #region Logic

        #region Public
        /// <summary>
        /// Exposes a method to delete a route destination.
        /// </summary>
        /// <param name="routeDestination">The route destination to delete.</param>
        public void DeleteRouteDestination(RouteDestination routeDestination)
        {
            this.Context.RouteDestinations.Remove(routeDestination);
        }

        #endregion

        protected override void OnAddEntity(Route newRoute)
        {
            newRoute.Date = SelectedDate;
            newRoute.OwnerBusinessAccount = (BusinessAccount)ContextManager.OwnerAccount;
            newRoute.RouteType = SelectedRouteTypes.FirstOrDefault();
        }

        /// <summary>
        /// Called when delete route is called.
        /// </summary>
        /// <param name="deletedRoute">The deleted route.</param>
        protected override void OnDeleteEntity(Route deletedRoute)
        {
            //Remove the routeTasks from the deletedRoute (and keep them)
            var routeTasksToKeep =
                  deletedRoute.RouteDestinations.SelectMany(routeDestination => routeDestination.RouteTasks).ToArray();

            //Delete the routeDestinations
            DataManager.RemoveEntities(deletedRoute.RouteDestinations);

            foreach (var routeTask in routeTasksToKeep)
            {
                //Clear RouteDestination reference
                routeTask.RouteDestinationId = null;

                //Add the routeTask back to LoadedRouteTasks
                if (!this.LoadedRouteTasks.Contains(routeTask))
                    LoadedRouteTasks.Add(routeTask);

                //Add the routeTask back to the EntitySet
                if (!this.Context.RouteTasks.Contains(routeTask))
                    this.Context.RouteTasks.Add(routeTask);
            }
        }

        #endregion
    }
}

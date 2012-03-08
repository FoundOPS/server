using System;
using System.Diagnostics;
using FoundOps.Common.Silverlight.UI.Controls;
using ReactiveUI;
using System.Linq;
using ReactiveUI.Xaml;
using System.Collections;
using RiaServicesContrib;
using System.Reactive.Linq;
using FoundOps.Common.Tools;
using System.Reactive.Subjects;
using Telerik.Windows.Controls;
using MEFedMVVM.ViewModelLocator;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using FoundOps.SLClient.UI.Tools;
using FoundOps.SLClient.Data.Tools;
using System.Collections.ObjectModel;
using FoundOps.SLClient.Data.Services;
using FoundOps.Core.Models.CoreEntities;
using System.ComponentModel.Composition;
using FoundOps.SLClient.Data.ViewModels;
using System.ServiceModel.DomainServices.Client;
using FoundOps.Server.Services.CoreDomainService;
using FoundOps.SLClient.UI.Controls.Dispatcher.Manifest;
using FoundOps.Common.Silverlight.Tools.ExtensionMethods;
using FoundOps.Common.Silverlight.UI.Controls.AddEditDelete;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// Contains the logic for displaying and dispatching Routes.
    /// </summary>
    [ExportViewModel("RoutesVM")]
    public class RoutesVM : CoreEntityCollectionVM<Route>,
        IAddToDeleteFromDestination<Employee>, IAddNewExisting<Employee>, IRemoveDelete<Employee>,
        IAddToDeleteFromDestination<Vehicle>, IAddNewExisting<Vehicle>, IRemoveDelete<Vehicle>
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

        #region Implementation of IAddToDeleteFromDestination

        /// <summary>
        /// Links to the LinkToAddToDeleteFromControl events.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="sourceType">Type of the source.</param>
        public void LinkToAddToDeleteFromEvents(AddToDeleteFrom control, Type sourceType)
        {
            if (sourceType == typeof(Employee))
            {
                control.AddExistingItem += (s, existingItem) => this.AddExistingItemEmployee((Employee)existingItem);
                control.AddNewItem += (s, newItemText) => this.AddNewItemEmployee(newItemText);
                control.RemoveItem += (s, e) => this.RemoveItemEmployee();
                control.DeleteItem += (s, e) => this.DeleteItemEmployee();
            }

            if (sourceType == typeof(Vehicle))
            {
                control.AddExistingItem += (s, existingItem) => this.AddExistingItemVehicle((Vehicle)existingItem);
                control.AddNewItem += (s, newItemText) => this.AddNewItemVehicle(newItemText);
                control.RemoveItem += (s, e) => this.RemoveItemVehicle();
                control.DeleteItem += (s, e) => this.DeleteItemVehicle();
            }
        }

        /// <summary>
        /// Gets the user account destination items source.
        /// ToArray creates a new IEnumerable, part of a workaround for the RadGridView displaying incorrect items when an item is removed.
        /// </summary>
        public IEnumerable EmployeesDestinationItemsSource { get { return SelectedEntity == null ? null : SelectedEntity.Technicians.ToArray(); } }

        /// <summary>
        /// Gets the service templates destination items source.
        /// ToArray creates a new IEnumerable, part of a workaround for the RadGridView displaying incorrect items when an item is removed.
        /// </summary>
        public IEnumerable VehiclesDestinationItemsSource { get { return SelectedEntity == null ? null : SelectedEntity.Vehicles.ToArray(); } }

        #endregion

        #region Implementation of IAddNewExisting<Employee> & IRemoveDelete<Employee>

        /// <summary>
        /// An action to add a new Employee to the current Employee.
        /// </summary>
        public Func<string, Employee> AddNewItemEmployee { get; private set; }
        Func<string, Employee> IAddNew<Employee>.AddNewItem { get { return AddNewItemEmployee; } }

        /// <summary>
        /// An action to add an existing Employee to the current Employee.
        /// </summary>
        public Action<Employee> AddExistingItemEmployee { get; private set; }
        Action<Employee> IAddNewExisting<Employee>.AddExistingItem { get { return AddExistingItemEmployee; } }

        /// <summary>
        /// An action to remove a Employee from the current Employee.
        /// </summary>
        public Func<Employee> RemoveItemEmployee { get; private set; }
        Func<Employee> IRemove<Employee>.RemoveItem { get { return RemoveItemEmployee; } }

        /// <summary>
        /// An action to remove a Employee from the current Employee and delete it.
        /// </summary>
        public Func<Employee> DeleteItemEmployee { get; private set; }
        Func<Employee> IRemoveDelete<Employee>.DeleteItem { get { return DeleteItemEmployee; } }

        #endregion
        #region Implementation of IAddNewExisting<Vehicle> & IRemoveDelete<Vehicle>

        /// <summary>
        /// An action to add a new Vehicle to the current Route.
        /// </summary>
        public Func<string, Vehicle> AddNewItemVehicle { get; private set; }
        Func<string, Vehicle> IAddNew<Vehicle>.AddNewItem { get { return AddNewItemVehicle; } }

        /// <summary>
        /// Gets the add existing item user account.
        /// </summary>
        public Action<Vehicle> AddExistingItemVehicle { get; private set; }
        Action<Vehicle> IAddNewExisting<Vehicle>.AddExistingItem { get { return AddExistingItemVehicle; } }

        /// <summary>
        /// An action to remove a Vehicle from the current Route.
        /// </summary>
        public Func<Vehicle> RemoveItemVehicle { get; private set; }
        Func<Vehicle> IRemove<Vehicle>.RemoveItem { get { return RemoveItemVehicle; } }

        /// <summary>
        /// An action to remove a Vehicle from the current Route and delete it.
        /// </summary>
        public Func<Vehicle> DeleteItemVehicle { get; private set; }
        Func<Vehicle> IRemoveDelete<Vehicle>.DeleteItem { get { return DeleteItemVehicle; } }

        #endregion

        #region Analytics
        /// <summary>
        /// Tracks if the auto assign button has been clicked (for analytics)
        /// </summary>
        public bool AutoAssignButtonHasBeenClicked { get; set; }
        #endregion

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

        /// <summary>
        /// A command to delete a set of route tasks.
        /// </summary>
        public IReactiveCommand DeleteRouteTasks { get; private set; }

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

        #region Selected Route detail items

        private Employee _selectedRouteSelectedEmployee;
        /// <summary>
        /// Gets or sets the selected route's selected employee.
        /// </summary>
        /// <value>
        /// The selected route's selected employee.
        /// </value>
        public Employee SelectedRouteSelectedEmployee
        {
            get { return _selectedRouteSelectedEmployee; }
            set
            {
                _selectedRouteSelectedEmployee = value;
                this.RaisePropertyChanged("SelectedRouteSelectedEmployee");
            }
        }

        private Vehicle _selectedRouteSelectedVehicle;
        /// <summary>
        /// Gets or sets the selected route's selected vehicle.
        /// </summary>
        /// <value>
        /// The selected route's selected vehicle.
        /// </value>
        public Vehicle SelectedRouteSelectedVehicle
        {
            get { return _selectedRouteSelectedVehicle; }
            set
            {
                _selectedRouteSelectedVehicle = value;
                this.RaisePropertyChanged("SelectedRouteSelectedVehicle");
            }
        }

        #endregion

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
        public RouteDestination SelectedRouteDestination
        {
            get { return _selectedRouteDestination.Value; }
            set
            {
                //Set the SelectedRoute to the selected RouteDestination's Route
                if (value != null && value.Route != null)
                    SelectedEntity = value.Route;

                _selectedRouteDestinationSubject.OnNext(value);
            }
        }

        private readonly Subject<RouteTask> _selectedTaskSubject = new Subject<RouteTask>();
        private readonly ObservableAsPropertyHelper<RouteTask> _selectedTask;
        /// <summary>
        /// The first selected task in the list of selected items. 
        /// The list is either from the task board or route tree view, depending on what was selected last.
        /// </summary>
        public RouteTask SelectedTask { get { return _selectedTask.Value; } set { _selectedTaskSubject.OnNext(value); } }

        private IEnumerable<RouteTask> _selectedTaskBoardTasks;

        ///<summary>
        /// The selected route tasks in the task board.
        ///</summary>
        public IEnumerable<RouteTask> SelectedTaskBoardTasks
        {
            get { return _selectedTaskBoardTasks; }
            set
            {
                _selectedTaskBoardTasks = value;
                this.RaisePropertyChanged("SelectedTaskBoardTasks");

                SelectedTask = SelectedTaskBoardTasks == null ? null : SelectedTaskBoardTasks.FirstOrDefault();
            }
        }

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

        private readonly Subject<ContactInfoVM> _selectedRouteDestinationClientContactInfoVMSubject = new Subject<ContactInfoVM>();
        private readonly ObservableAsPropertyHelper<ContactInfoVM> _selectedRouteDestinationClientContactInfoVM;
        /// <summary>
        /// Gets the ContactInfoVM for the selected route destination's client.
        /// </summary>
        public ContactInfoVM SelectedRouteDestinationClientContactInfoVM { get { return _selectedRouteDestinationClientContactInfoVM.Value; } }

        private readonly Subject<ContactInfoVM> _selectedRouteDestinationLocationContactInfoVMSubject = new Subject<ContactInfoVM>();
        private readonly ObservableAsPropertyHelper<ContactInfoVM> _selectedRouteDestinationLocationContactInfoVM;
        /// <summary>
        /// Gets the ContactInfoVM for the selected route destination's location.
        /// </summary>
        public ContactInfoVM SelectedRouteDestinationLocationContactInfoVM { get { return _selectedRouteDestinationLocationContactInfoVM.Value; } }

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
        /// The loaded route tasks (connected to the RouteTasks EntitySet)
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
            #region Implementation of IAddToDeleteFromDestination<Employee> and IAddToDeleteFromDestination<Vehicle>

            //Notify the DestinationItemsSources changed

            //a) Whenever the current route's technicians or employees changes notify the itemssource updated. 
            //  Part of a workaround for the RadGridView displaying incorrect items when an item is removed.
            var techniciansOrEmployeesChanged =
                this.SelectedEntityObservable.WhereNotNull().SelectLatest(
                    r => r.Technicians.FromCollectionChangedGeneric().Merge(r.Vehicles.FromCollectionChangedGeneric()));

            //b) Whenever the SelectedEntity changes
            this.SelectedEntityObservable.AsGeneric().Merge(techniciansOrEmployeesChanged)
                .Throttle(TimeSpan.FromMilliseconds(250)).ObserveOnDispatcher().Subscribe(_ =>
            {
                //Notify the DestinationItemsSources changed
                this.RaisePropertyChanged("EmployeesDestinationItemsSource");
                this.RaisePropertyChanged("VehiclesDestinationItemsSource");
            });

            #endregion

            #region Implementation of IAddNewExisting<Employee> & IRemoveDelete<Employee>

            AddNewItemEmployee = name =>
            {
                var newEmployee = VM.Employees.CreateNewItem(name);
                SelectedEntity.Technicians.Add(newEmployee);
                return newEmployee;
            };

            AddExistingItemEmployee = existingItem => SelectedEntity.Technicians.Add(existingItem);

            RemoveItemEmployee = () =>
            {
                var employeeToRemove = SelectedRouteSelectedEmployee;

                foreach (Employee t in EmployeesDestinationItemsSource)
                    Debug.WriteLine("B " + t.DisplayName);

                this.SelectedEntity.Technicians.Remove(employeeToRemove);

                foreach (Employee t in EmployeesDestinationItemsSource)
                    Debug.WriteLine("A " + t.DisplayName);

                return employeeToRemove;
            };

            DeleteItemEmployee = () =>
            {
                var employeeToDelete = SelectedRouteSelectedEmployee;
                this.SelectedEntity.Technicians.Remove(employeeToDelete);
                VM.Employees.DeleteEntity(employeeToDelete);
                return employeeToDelete;
            };

            #endregion

            #region Implementation of IAddNewExisting<Vehicle> & IRemoveDelete<Vehicle>

            AddNewItemVehicle = name =>
            {
                var newVehicle = VM.Vehicles.CreateNewItem(name);
                this.SelectedEntity.Vehicles.Add(newVehicle);
                return newVehicle;
            };

            AddExistingItemVehicle = existingItem => SelectedEntity.Vehicles.Add(existingItem);

            RemoveItemVehicle = () =>
            {
                var vehicleToRemove = SelectedRouteSelectedVehicle;
                this.SelectedEntity.Vehicles.Remove(vehicleToRemove);
                return vehicleToRemove;
            };

            DeleteItemVehicle = () =>
            {
                var selectedVehicle = RemoveItemVehicle();
                this.SelectedEntity.Vehicles.Remove(selectedVehicle);
                VM.Vehicles.DeleteEntity(selectedVehicle);
                return selectedVehicle;
            };

            #endregion

            #region Setup SelectedItems properties

            _selectedRouteDestination = _selectedRouteDestinationSubject.ToProperty(this, x => x.SelectedRouteDestination);
            _selectedTask = _selectedTaskSubject.ToProperty(this, x => x.SelectedTask);

            #endregion

            #region Setup Sub ViewModels

            //Setup the RouteManifestVM
            RouteManifestVM = new RouteManifestVM(dataManager);

            //Setup the ContactInfoVM properties
            _selectedRouteDestinationClientContactInfoVM = _selectedRouteDestinationClientContactInfoVMSubject.ToProperty(this, x => x.SelectedRouteDestinationClientContactInfoVM);
            _selectedRouteDestinationLocationContactInfoVM = _selectedRouteDestinationLocationContactInfoVMSubject.ToProperty(this, x => x.SelectedRouteDestinationLocationContactInfoVM);

            //Whenever the Selected Route Destination changes: update the _selectedRouteDestinationClientContactInfoVM
            _selectedRouteDestinationSubject.Where(srd => srd != null && srd.Client != null && srd.Client.OwnedParty != null).Subscribe(selectedRouteDestination =>
                _selectedRouteDestinationClientContactInfoVMSubject.OnNext(
                new ContactInfoVM(DataManager, ContactInfoType.OwnedParties, selectedRouteDestination.Client.OwnedParty.ContactInfoSet)));

            //Whenever the Selected Route Destination changes: update the _selectedRouteDestinationLocationContactInfoVM
            _selectedRouteDestinationSubject.Where(srd => srd != null && srd.Location != null).Subscribe(selectedRouteDestination =>
                _selectedRouteDestinationLocationContactInfoVMSubject.OnNext(
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

            SetupDataLoading();

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
                      .Select(ea => (EntityCollection<RouteDestination>)ea.Sender)
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
        private void SetupDataLoading()
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
            _tasksLoading = this.DataManager.Subscribe<RouteTask>(Query.UnroutedRouteTasks, ObservationState,
                loadedRouteTasks => _loadedRouteTasks.OnNext(loadedRouteTasks)).ToProperty(this, x => x.TasksLoading);

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

            //Can calculate routes when there are routes, and when the context is not submitting
            var canCalculateRoutes = DataManager.DomainContextIsSubmittingObservable.CombineLatest(routesExist, (isSubmitting, areRoutes) => !isSubmitting && areRoutes);
            AutoCalculateRoutes = new ReactiveCommand(canCalculateRoutes);

            //Populate routes with the UnroutedTasks and refresh the filter counts
            AutoCalculateRoutes.SubscribeOnDispatcher().Subscribe(_ =>
            {
                //Populate the routes with the unrouted tasks
                var routedTasks = SimpleRouteCalculator.PopulateRoutes(this.UnroutedTasks, DomainCollectionView);

                //Remove the routedTasks from the task board
                foreach (var routeTaskToRemove in routedTasks.ToArray())
                    UnroutedTasks.Remove(routeTaskToRemove);
            });

            //Allow the user to open the route manifests whenever there is more than one route visible
            OpenRouteManifests = new ReactiveCommand(_updateFilter.ObserveOnDispatcher().Throttle(new TimeSpan(0, 0, 0, 0, 250)).Select(_ => this.DomainCollectionView.Count() > 0));

            OpenRouteManifests.ObserveOnDispatcher().Subscribe(_ =>
            {
                //Setup the route manifest viewer if there is not one yet
                if (_routeManifestViewer == null)
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
            //DeleteRouteTasks can delete whenever SelectedTaskBoardTasks has at least 1 route task
            DeleteRouteTasks = new ReactiveCommand(this.WhenAny(x => x.SelectedTaskBoardTasks, sts => sts.Value != null && sts.Value.Count() > 0));

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
                //Generated services are not tracked
                if (SelectedTask.GeneratedOnServer) return;

                this.LoadedRouteTasks.Remove(SelectedTask);

                if (this.Context.RouteTasks.Contains(SelectedTask))
                    this.Context.RouteTasks.Remove(SelectedTask);
            });

            //Setup DeleteRouteTasks command
            DeleteRouteTasks.SubscribeOnDispatcher().Subscribe(_ =>
            {
                //Generated services should not be deleted
                var tasksToDelete = this.SelectedTaskBoardTasks.Where(rt => !rt.GeneratedOnServer);

                foreach (var routeTask in tasksToDelete)
                    this.LoadedRouteTasks.Remove(routeTask);

                foreach (var routeTask in tasksToDelete)
                    if (this.Context.RouteTasks.Contains(routeTask))
                        this.Context.RouteTasks.Remove(routeTask);
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

        #endregion

        #region Logic

        #region Public
        /// <summary>
        /// Exposes a method to delete a route destination.
        /// </summary>
        /// <param name="routeDestination">The route destination to delete.</param>
        public void DeleteRouteDestination(RouteDestination routeDestination)
        {
            //Clear references to the RouteDestination
            foreach (var routeTask in routeDestination.RouteTasks)
                routeTask.RemoveRouteDestination();

            this.Context.RouteDestinations.Remove(routeDestination);
        }

        #endregion

        protected override void OnAddEntity(Route newRoute)
        {
            newRoute.Date = SelectedDate;
            newRoute.OwnerBusinessAccount = (BusinessAccount)ContextManager.OwnerAccount;
            newRoute.RouteType = SelectedRouteTypes.FirstOrDefault();
        }

        protected override bool BeforeAdd()
        {
            if (((BusinessAccount)ContextManager.OwnerAccount).MaxRoutes == null)
                ((BusinessAccount)ContextManager.OwnerAccount).MaxRoutes = 0;

            if (((BusinessAccount)ContextManager.OwnerAccount).MaxRoutes <= DomainCollectionView.Count())
            {
                var tooManyRoutes = new TooManyRoutesError
                {
                    MaxNumberOfRoutes = (int)((BusinessAccount)ContextManager.OwnerAccount).MaxRoutes
                };

                tooManyRoutes.Show();

                return false;
            }
            return true;
        }

        /// <summary>
        /// Called when delete route is called.
        /// </summary>
        /// <param name="deletedRoute">The deleted route.</param>
        protected override void OnDeleteEntity(Route deletedRoute)
        {
            //Remove the routeTasks from the deletedRoute (and keep them)
            var routeTasksToKeep =
                deletedRoute.RouteDestinations.SelectMany(routeDestination => routeDestination.RouteTasks)
                .Where(rt => !rt.GeneratedOnServer).ToList();

            var generatedRouteTasks = deletedRoute.RouteDestinations.SelectMany(routeDestination => routeDestination.RouteTasks)
                .Where(rt => rt.GeneratedOnServer).ToArray();

            //Add the generated route task parents back to the task board
            routeTasksToKeep.AddRange(generatedRouteTasks.Select(rt => rt.GeneratedRouteTaskParent));

            //Delete the generatedRouteTasks
            DataManager.RemoveEntities(generatedRouteTasks.SelectMany(rt => new EntityGraph<Entity>(rt, rt.EntityGraphWithServiceShape)));

            //Delete the routeDestinations
            DataManager.RemoveEntities(deletedRoute.RouteDestinations);

            //Add the route tasks to keep back to the task board
            foreach (var routeTask in routeTasksToKeep)
            {
                //Clear RouteDestination reference
                routeTask.RouteDestinationId = null;

                //Add the routeTask back to LoadedRouteTasks
                if (!this.LoadedRouteTasks.Contains(routeTask))
                    LoadedRouteTasks.Add(routeTask);

                //TODO: Check if this is needed
                //Add the routeTask back to the EntitySet (if it is not generated)
                if (!routeTask.GeneratedOnServer && !this.Context.RouteTasks.Contains(routeTask))
                    this.Context.RouteTasks.Add(routeTask);
            }

        }

        #endregion
    }
}

using FoundOps.Common.Silverlight.MVVM.Messages;
using FoundOps.Common.Silverlight.Services;
using FoundOps.Common.Silverlight.Tools.ExtensionMethods;
using FoundOps.Common.Tools;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Common.Silverlight.UI.Controls;
using FoundOps.SLClient.Data.Tools;
using FoundOps.SLClient.Data.ViewModels;
using FoundOps.SLClient.UI.Controls.Dispatcher.Manifest;
using FoundOps.SLClient.UI.Tools;
using GalaSoft.MvvmLight.Command;
using MEFedMVVM.ViewModelLocator;
using Microsoft.Windows.Data.DomainServices;
using ReactiveUI;
using ReactiveUI.Xaml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.ServiceModel.DomainServices.Client;
using System.Threading.Tasks;
using System.Windows;

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

        private readonly ObservableAsPropertyHelper<bool> _manifestOpenHelper;
        private readonly Subject<bool> _manifestOpenSubject = new Subject<bool>();
        /// <summary>
        /// An Observable determining when the manifest is open (and loaded) or closed.
        /// </summary>
        public IObservable<bool> ManifestOpenObservable { get { return _manifestOpenSubject.AsObservable(); } }

        /// <summary>
        /// Whether or not the manifest is open (and loaded).
        /// </summary>
        public bool ManifestOpen { get { return _manifestOpenHelper.Value; } set { _manifestOpenSubject.OnNext(value); } }

        private readonly ObservableAsPropertyHelper<DateTime> _selectedDateHelper;
        private readonly Subject<DateTime> _selectedDateSubject = new Subject<DateTime>();
        /// <summary>
        /// An Observable of the SelectedDate.
        /// </summary>
        public IObservable<DateTime> SelectedDateObservable { get { return _selectedDateSubject.AsObservable(); } }

        /// <summary>
        /// The SelectedDate.
        /// </summary>
        public DateTime SelectedDate { get { return _selectedDateHelper.Value; } set { _selectedDateSubject.OnNext(value); } }

        /// <summary>
        /// For backwards compatibility
        /// </summary>
        public RouteManifestVM RouteManifestVM { get { return VM.RouteManifest; } }

        private readonly ObservableAsPropertyHelper<IEnumerable<string>> _routeTypesHelper;
        /// <summary>
        /// The Route Types observable from DispatcherFilterVM
        /// </summary>
        public IObservable<IEnumerable<string>> RouteTypesObservable
        {
            get
            {
                //Whenever the ServiceTemplateOptions changes, push the RouteTypes collection
                return VM.DispatcherFilter.ServiceTemplateOptions.FromCollectionChanged().Select(e => e.Sender)
                       .Cast<ObservableCollection<EntityOption>>().Throttle(TimeSpan.FromMilliseconds(100))
                       .Select(collection => collection.Select(o => ((ServiceTemplate)o.Entity).Name));
            }
        }

        /// <summary>
        /// The Route Types
        /// </summary>
        public IEnumerable<string> RouteTypes { get { return _routeTypesHelper.Value; } }

        #region Selected Entity's and Sub ViewModels

        private RouteVM _selectedRouteVM;
        /// <summary>
        /// The SelectedRoute's VM
        /// </summary>
        public RouteVM SelectedRouteVM
        {
            get { return _selectedRouteVM; }
            set
            {
                _selectedRouteVM = value;
                this.RaisePropertyChanged("SelectedRouteVM");
            }
        }

        private readonly Subject<RouteDestination> _selectedRouteDestinationSubject = new Subject<RouteDestination>();
        private readonly ObservableAsPropertyHelper<RouteDestination> _selectedRouteDestinationHelper;
        /// <summary>
        /// The first selected route destination in route tree view
        /// </summary>
        public RouteDestination SelectedRouteDestination
        {
            get { return _selectedRouteDestinationHelper.Value; }
            set
            {
                //Set the SelectedRoute to the selected RouteDestination's Route
                if (value != null && value.Route != null)
                    SelectedEntity = value.Route;

                _selectedRouteDestinationSubject.OnNext(value);
            }
        }

        private RouteDestinationVM _selectedRouteDestinationVM;
        /// <summary>
        /// The SelectedRouteDestination's VM
        /// </summary>
        public RouteDestinationVM SelectedRouteDestinationVM
        {
            get { return _selectedRouteDestinationVM; }
            set
            {
                _selectedRouteDestinationVM = value;
                this.RaisePropertyChanged("SelectedRouteDestinationVM");
            }
        }

        private readonly Subject<RouteTask> _selectedRouteTaskSubject = new Subject<RouteTask>();
        private readonly ObservableAsPropertyHelper<RouteTask> _selectedRouteTaskHelper;
        /// <summary>
        /// The first selected route destination in route tree view
        /// </summary>
        public RouteTask SelectedRouteTask
        {
            get { return _selectedRouteTaskHelper.Value; }
            set
            {
                //Set the SelectedRoute to the selected RouteTask's RouteDestination's Route
                if (value != null && value.RouteDestination.Route != null)
                    SelectedEntity = value.RouteDestination.Route;

                _selectedRouteTaskSubject.OnNext(value);
            }
        }

        #endregion

        #endregion

        #region Locals

        // Used to cancel the previous RoutesLoad.
        private readonly Subject<bool> _cancelLastRoutesLoad = new Subject<bool>();

        //The only route manifest viewer
        private RouteManifestViewer _routeManifestViewer;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="RoutesVM"/> class.
        /// </summary>
        [ImportingConstructor]
        public RoutesVM()
            : base(false, false, false) //Do not initialize a default queryable collection view (or else the filter will not work)
        {
            //Setup ObservableAsPropertyHelpers
            _manifestOpenHelper = _manifestOpenSubject.ToProperty(this, x => x.ManifestOpen);
            _selectedDateHelper = this.ObservableToProperty(_selectedDateSubject, x => x.SelectedDate, DateTime.UtcNow.Date);
            _selectedRouteDestinationHelper = _selectedRouteDestinationSubject.ToProperty(this, x => x.SelectedRouteDestination);
            _selectedRouteTaskHelper = _selectedRouteTaskSubject.ToProperty(this, x => x.SelectedRouteTask);
            _routeTypesHelper = RouteTypesObservable.ToProperty(this, x => x.RouteTypes);

            //Update the SelectedRouteVM whenever the RouteDestination changes
            SelectedEntityObservable.ObserveOnDispatcher().Subscribe(r =>
            {
                if (SelectedRouteVM != null)
                    SelectedRouteVM.Dispose();

                this.SelectedRouteVM = new RouteVM(r);
            });

            //Update the SelectedRouteDestinationVM whenever the RouteDestination changes
            _selectedRouteDestinationSubject.ObserveOnDispatcher().Subscribe(rd =>
            {
                if (this.SelectedRouteDestinationVM != null)
                    this.SelectedRouteDestinationVM.Dispose();

                this.SelectedRouteDestinationVM = new RouteDestinationVM(rd);
            });

            SetupDataLoading();
            RegisterCommands();
        }

        /// <summary>
        /// Used in the constructor to setup the data loading process.
        /// </summary>
        private void SetupDataLoading()
        {
            #region Load Routes

            //Load the Routes whenever
            //a) the Dispatcher is entered/re-entered
            //b) the SelectedDate changes
            //c) Discard was called
            MessageBus.Current.Listen<NavigateToMessage>().Where(m => m.UriToNavigateTo.ToString().Contains("Dispatcher")).AsGeneric()
            .Merge(SelectedDateObservable.AsGeneric())
            .Merge(DiscardObservable.AsGeneric())
            .Merge(DataManager.RejectChangesDueToError)
            .Throttle(TimeSpan.FromMilliseconds(250))
            .ObserveOnDispatcher().Subscribe(_ =>
            {
                //Cancel the last RoutesLoad (if it is in the process of loading)
                _cancelLastRoutesLoad.OnNext(true);
                //Notify the RoutesVM is loading Routes
                IsLoadingSubject.OnNext(true);

                //Detach the current routes so there is a fresh load
                if (SourceCollection != null)
                    this.DataManager.DetachEntities(SourceCollection.SelectMany(r => r.EntityGraphToDetach));

                DomainContext.LoadAsync(DomainContext.GetRoutesForServiceProviderOnDayQuery(ContextManager.RoleId, SelectedDate), _cancelLastRoutesLoad)
                .ContinueWith(task =>
                {
                    //Notify the RoutesVM has completed loading Routes
                    IsLoadingSubject.OnNext(false);

                    if (task.IsCanceled || !task.Result.Any())
                    {
                        ViewObservable.OnNext(new DomainCollectionViewFactory<Route>(new ObservableCollection<Route>()).View);
                        return;
                    }

                    //Setup RouteTaskHolders for existing RouteTasks
                    foreach (var routeTask in task.Result.SelectMany(r => r.RouteDestinations.SelectMany(rd => rd.RouteTasks)))
                        routeTask.SetupTaskHolder();

                    //Update the CollectionView
                    ViewObservable.OnNext(new DomainCollectionViewFactory<Route>(new EntityList<Route>(DomainContext.Routes, task.Result)).View);
                }, TaskScheduler.FromCurrentSynchronizationContext());
            });

            #region Load Route Details

            SetupDetailsLoading(selectedEntity => DomainContext.GetRouteDetailsQuery(ContextManager.RoleId, selectedEntity.Id));

            #endregion

            #endregion

            #region Setup RouteTask details loading

            var cancelLastDetailsLoad = new Subject<bool>();
            //Whenever the selectedRouteTask changes load the RouteTask's details
            //a) cancel the last loads
            //b) load the details
            _selectedRouteTaskSubject.Where(se => se != null).ObserveOnDispatcher().Subscribe(selectedRouteTask =>
            {
                //a) cancel the last loads
                cancelLastDetailsLoad.OnNext(true);

                //Load the RouteTask's ServiceHolder details
                selectedRouteTask.ParentRouteTaskHolder.ServiceHolder.LoadDetails(cancelLastDetailsLoad, () => selectedRouteTask.DetailsLoaded = true);
            });

            #endregion

            #region Setup RouteDestinations details loading

            //Whenever the selectedRouteDestination changes load the RouteDestination's details
            //a) cancel the last loads
            //b) load the details
            _selectedRouteDestinationSubject.Where(se => se != null).ObserveOnDispatcher().Subscribe(selectedRouteDestination =>
            {
                //a) cancel the last loads
                cancelLastDetailsLoad.OnNext(true);

                //if the RouteDestination is not new, load its details
                if (selectedRouteDestination.EntityState != EntityState.New)
                {
                    DomainContext.LoadAsync(DomainContext.GetRouteDestinationDetailsQuery(ContextManager.RoleId, selectedRouteDestination.Id), cancelLastDetailsLoad)
                        .ContinueWith(task =>
                        {
                            if (!task.IsCanceled)
                                selectedRouteDestination.DetailsLoaded = true;
                        }, TaskScheduler.FromCurrentSynchronizationContext());
                }
                else //if the RouteDestination is new, load it's Location/ContactInfoSet and Client/ContactInfoSet
                {
                    var clientLoaded = false;
                    var locationLoaded = false;

                    if (selectedRouteDestination.ClientId.HasValue)
                        DomainContext.LoadAsync(DomainContext.GetClientsWithContactInfoSetQuery(ContextManager.RoleId, new[] { selectedRouteDestination.ClientId.Value }), cancelLastDetailsLoad)
                          .ContinueWith(task =>
                            {
                                if (!task.IsCanceled && locationLoaded)
                                    selectedRouteDestination.DetailsLoaded = true;
                                clientLoaded = true;
                            }, TaskScheduler.FromCurrentSynchronizationContext());

                    if (selectedRouteDestination.LocationId.HasValue)
                        DomainContext.LoadAsync(DomainContext.GetLocationsWithContactInfoSetQuery(ContextManager.RoleId, new[] { selectedRouteDestination.LocationId.Value }), cancelLastDetailsLoad)
                            .ContinueWith(task =>
                            {
                                if (!task.IsCanceled && clientLoaded)
                                    selectedRouteDestination.DetailsLoaded = true;
                                locationLoaded = true;
                            }, TaskScheduler.FromCurrentSynchronizationContext());
                }
            });

            #endregion

            //Update the filter whenever
            //a) the filter changes
            //b) the SourceCollection (an ObservableCollection) changes or the CollectionView is set

            VM.DispatcherFilter.FilterUpdatedObservable.AsGeneric()
           .Merge(SourceCollectionChangedOrSet)
           .Throttle(TimeSpan.FromMilliseconds(100)).ObserveOnDispatcher()
           .Subscribe(_ =>
            {
                if (CollectionView == null) return;
                UpdateFilter();
            });
        }

        /// <summary>
        /// Updates the filtered Routes
        /// </summary>
        private void UpdateFilter()
        {
            CollectionView.Filter = null;
            CollectionView.Filter += route => VM.DispatcherFilter.RouteIncludedInFilter((Route)route);
        }

        /// <summary>
        /// Used in the constructor to setup the commands
        /// </summary>
        private void RegisterCommands()
        {
            //Create an observable for when at least 1 route exists
            var routesExist = CollectionViewObservable //select the routes EntityList observable
                .DistinctUntilChanged() //on collection changed or set
                .Select(routes => routes.Cast<object>().Any()) // select routes > 0
                .Merge(SourceCollectionChangedOrSet);

            //Can calculate routes when there are routes, and when the context is not submitting
            var canCalculateRoutes = DataManager.DomainContextIsSubmittingObservable.CombineLatest(routesExist, (isSubmitting, areRoutes) => !isSubmitting && areRoutes);
            AutoCalculateRoutes = new ReactiveCommand(canCalculateRoutes);

            var cancelLoadLocationDetails = new Subject<bool>();

            //Populate routes with the UnroutedTasks and refresh the filter counts
            AutoCalculateRoutes.SubscribeOnDispatcher().Subscribe(_ =>
            {
                //Populate the routes with the unrouted tasks
                var routedTaskHolders = SimpleRouteCalculator.PopulateRoutes((IEnumerable<TaskHolder>)VM.TaskBoard.CollectionView, CollectionView.Cast<Route>()).ToArray();

                //Remove the routedTasks from the task board
                foreach (var taskHolderToRemove in routedTaskHolders)
                    ((ObservableCollection<TaskHolder>)VM.TaskBoard.CollectionView.SourceCollection).Remove(taskHolderToRemove);

                //Whenever the Routes are Auto Calculated, save
                this.SaveCommand.Execute(null);

                //Load the locations/contact info set for the newly populated task that do not have loaded locations
                //No need to load the regions because they are already loaded (from the filter)
                cancelLoadLocationDetails.OnNext(true);
                var locationIdsToLoad = routedTaskHolders.Select(rth => rth.ChildRouteTask).Where(crt => crt.Location == null && crt.LocationId.HasValue)
                    .Select(crt => crt.LocationId.Value).Distinct();

                if (!locationIdsToLoad.Any()) return;
                //Send a TaskLocationsUpdated message. To refresh Region colors for any loaded locations
                MessageBus.Current.SendMessage(new TaskLocationsUpdated());

                var locationsQuery = DomainContext.GetLocationsQuery(ContextManager.RoleId, locationIdsToLoad);
                DomainContext.LoadAsync(locationsQuery, cancelLoadLocationDetails).ContinueWith(task =>
                {
                    if (task.IsCanceled) return;

                    //Send a TaskLocationsUpdated message. To refresh Region colors for any loaded locations
                    MessageBus.Current.SendMessage(new TaskLocationsUpdated());
                }, TaskScheduler.FromCurrentSynchronizationContext());
            });

            //Allow the user to open the route manifests whenever there is one or more route visible
            //and the DomainContext is not loading or submitting
            var canOpenManifests = routesExist.CombineLatest(DataManager.DomainContextIsSubmittingObservable, DataManager.DomainContextIsLoadingObservable,
                                                             (areRoutes, isSubmitting, isLoading) => areRoutes && !isSubmitting && !isLoading);
            OpenRouteManifests = new ReactiveCommand(canOpenManifests);

            SetupManifests();

            #region Date Modifiers

            SetSelectedDayToToday = new RelayCommand(() => this.SelectedDate = DateTime.UtcNow);
            SetSelectedDayOneDayPrevious = new RelayCommand(() => this.SelectedDate = SelectedDate.Date.AddDays(-1));
            SetSelectedDayOneDayForward = new RelayCommand(() => this.SelectedDate = SelectedDate.Date.AddDays(1));

            #endregion
        }

        /// <summary>
        /// Sets up the logic for manifests.
        /// </summary>
        private void SetupManifests()
        {
            //Setup logic to open the RouteManifestViewer whenever the OpenRouteManifests command is executed
            OpenRouteManifests.ObserveOnDispatcher().Subscribe(_ =>
            {
                //If no route is selected, select the first route that is visible
                if (SelectedEntity == null)
                    SelectedEntity = CollectionView.OfType<Route>().FirstOrDefault();

                //Whenever the manifests are opened save the Routes
                this.SaveCommand.Execute(null);

                //Setup the route manifest viewer if there is not one yet
                if (_routeManifestViewer == null)
                    _routeManifestViewer = new RouteManifestViewer();

                _routeManifestViewer.Show();
            });

            var cancelLoadDetails = new Subject<bool>();

            //Load the Route's ServiceTemplates and Fields
            //when the manifest is open, the SelectedEntity != null, and the DomainContext is not submitting
            ManifestOpenObservable.CombineLatest(SelectedEntityObservable, DataManager.DomainContextIsSubmittingObservable,
                (opened, route, isSubmitting) => opened && route != null && !isSubmitting)
                .Where(v => v).Throttle(TimeSpan.FromMilliseconds(100)).ObserveOnDispatcher().Subscribe(_ =>
                {
                    cancelLoadDetails.OnNext(true);

                    var clientsLoaded = false;
                    var locationsLoaded = false;
                    var fieldsLoaded = false;

                    //Load the Clients/ContactInfoSet of the RouteTasks
                    var clientsWithContactInfoQuery = DomainContext.GetClientsWithContactInfoSetQuery(ContextManager.RoleId,
                        SelectedEntity.RouteDestinations.Where(rd => rd.ClientId.HasValue).Select(rd => rd.ClientId.Value));

                    DomainContext.LoadAsync(clientsWithContactInfoQuery, cancelLoadDetails)
                        .ContinueWith(task =>
                        {
                            if (task.IsCanceled) return;

                            clientsLoaded = true;
                            if (clientsLoaded && locationsLoaded && fieldsLoaded)
                                SelectedEntity.ManifestDetailsLoaded = true;
                        }, TaskScheduler.FromCurrentSynchronizationContext());

                    //Load the Locations/ContactInfoSet of the RouteTasks
                    var locationsWithContactInfoQuery = DomainContext.GetLocationsWithContactInfoSetQuery(ContextManager.RoleId,
                        SelectedEntity.RouteDestinations.Where(rd => rd.LocationId.HasValue).Select(rd => rd.LocationId.Value));

                    DomainContext.LoadAsync(locationsWithContactInfoQuery, cancelLoadDetails)
                        .ContinueWith(task =>
                        {
                            if (task.IsCanceled) return;

                            locationsLoaded = true;
                            if (clientsLoaded && locationsLoaded && fieldsLoaded)
                                SelectedEntity.ManifestDetailsLoaded = true;
                        }, TaskScheduler.FromCurrentSynchronizationContext());

                    //Load the Fields info of the RouteTasks
                    var serviceHoldersToLoad = SelectedEntity.RouteDestinations.SelectMany(rd => rd.RouteTasks).Select(rt => rt.ParentRouteTaskHolder.ServiceHolder);
                    ServiceHolder.LoadDetails(serviceHoldersToLoad, cancelLoadDetails, false, () =>
                    {
                        fieldsLoaded = true;
                        if (clientsLoaded && locationsLoaded && fieldsLoaded)
                            SelectedEntity.ManifestDetailsLoaded = true;
                    });
                });
        }

        #endregion

        #region Logic

        #region Protected

        /// <summary>
        /// Before add make the account has enough routes.
        /// </summary>
        /// <param name="checkCompleted">The action to call after checking.</param>
        protected override void CheckAdd(Action<bool> checkCompleted)
        {
            if (((BusinessAccount)ContextManager.OwnerAccount).MaxRoutes <= CollectionView.Cast<object>().Count())
            {
                var tooManyRoutes = new TooManyRoutesError
                {
                    MaxNumberOfRoutes = ((BusinessAccount)ContextManager.OwnerAccount).MaxRoutes
                };

                tooManyRoutes.Show();

                checkCompleted(false);
            }

            var firstAvailableRouteType = VM.DispatcherFilter.ServiceTemplateOptions.Where(o => o.IsSelected).Select(o => ((ServiceTemplate)o.Entity).Name).FirstOrDefault();
            if (firstAvailableRouteType == null)
            {
                MessageBox.Show("You must first select a route type in the Route Capabilities Filter before adding a new route.", "Oh no!", MessageBoxButton.OK);
                checkCompleted(false);
            }

            checkCompleted(true);
        }

        protected override Route AddNewEntity(object commandParameter)
        {
            var newRoute = new Route
            {
                Id = Guid.NewGuid(),
                Date = SelectedDate,
                DetailsLoaded = true,
                OwnerBusinessAccount = (BusinessAccount)ContextManager.OwnerAccount,
                RouteType = VM.DispatcherFilter.ServiceTemplateOptions.Where(o => o.IsSelected).Select(o => ((ServiceTemplate)o.Entity).Name).First()
            };

            ((ObservableCollection<Route>)SourceCollection).Add(newRoute);

            return newRoute;
        }

        /// <summary>
        /// Called when delete route is called.
        /// </summary>
        /// <param name="deletedRoute">The deleted route.</param>
        protected override void OnDeleteEntity(Route deletedRoute)
        {
            //Remove the Employees and Vehicles
            foreach (var employee in deletedRoute.Technicians.ToArray())
                deletedRoute.Technicians.Remove(employee);
            foreach (var vehicle in deletedRoute.Vehicles.ToArray())
                deletedRoute.Vehicles.Remove(vehicle);

            //Add the TaskHolders back to the task board
            var tasksForTaskBoard = deletedRoute.RouteDestinations.SelectMany(routeDestination => routeDestination.RouteTasks).ToArray();

            //Add the task holders to keep back to the task board
            foreach (var routeTask in tasksForTaskBoard)
            {
                //Get the ParentRouteTaskHolder
                var taskHolder = routeTask.ParentRouteTaskHolder;

                taskHolder.StatusInt = (int) Status.Created;

                //Add the TaskHolder back to VM.TaskBoard.LoadedTaskHolders 
                ((ObservableCollection<TaskHolder>)VM.TaskBoard.CollectionView.SourceCollection).Add(taskHolder);
            }

            //Delete the Route, RouteDestinations, and RouteTasks
            DataManager.RemoveEntities(new Entity[] { deletedRoute }.Union(deletedRoute.RouteDestinations).Union(tasksForTaskBoard).ToArray());
        }

        /// <summary>
        /// Checks to be sure that you are not trying to delete a Route in the past
        /// </summary>
        /// <param name="checkCompleted"></param>
        protected override void CheckDelete(Action<bool> checkCompleted)
        {
            //If there is no Route selected, you can not delete 
            if (SelectedEntity == null)
                checkCompleted(false);
            //Id the Route's date is in the past, you can not delete
            else if (SelectedEntity.Date < DateTime.UtcNow.Date)
                checkCompleted(false);
            else
                checkCompleted(true);
        }

        #endregion

        #region Public

        /// <summary>
        /// Exposes a method to delete a route destination.
        /// </summary>
        /// <param name="routeDestination">The route destination to delete.</param>
        public void DeleteRouteDestination(RouteDestination routeDestination)
        {
            this.DomainContext.RouteDestinations.Remove(routeDestination);
        }

        /// <summary>
        /// Exposes a method to delete a route task.
        /// </summary>
        /// <param name="routeTask">The route task to delete.</param>
        public void DeleteRouteTask(RouteTask routeTask)
        {
            this.DomainContext.RouteTasks.Remove(routeTask);
        }

        /// <summary>
        /// Saves Drag and Drop.
        /// </summary>
        public void DispatcherSave()
        {
            DataManager.EnqueueSubmitOperation();
        }

        #endregion

        #endregion
    }
    /// <summary>
    /// A message to notify that the Task's locations have been loaded/updated. 
    /// </summary>
    public class TaskLocationsUpdated { }
}
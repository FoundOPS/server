using System.Threading.Tasks;
using FoundOps.Common.Silverlight.MVVM.Messages;
using FoundOps.Common.Silverlight.Services;
using FoundOps.Common.Silverlight.Tools.ExtensionMethods;
using FoundOps.Common.Silverlight.UI.Interfaces;
using FoundOps.Common.Tools;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Common.Silverlight.UI.Controls;
using FoundOps.SLClient.Data.Tools;
using FoundOps.SLClient.Data.ViewModels;
using FoundOps.SLClient.UI.Controls.Dispatcher.Manifest;
using FoundOps.SLClient.UI.Tools;
using GalaSoft.MvvmLight.Command;
using MEFedMVVM.ViewModelLocator;
using ReactiveUI;
using ReactiveUI.Xaml;
using RiaServicesContrib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
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

        #region Selected Entity's and Sub ViewModels

        /// <summary>
        /// Gets the RouteManifestVM.
        /// </summary>
        public RouteManifestVM RouteManifestVM { get; private set; }

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
            _selectedDateHelper = this.ObservableToProperty(_selectedDateSubject, x => x.SelectedDate, DateTime.Now.Date);
            _selectedRouteDestinationHelper = _selectedRouteDestinationSubject.ToProperty(this, x => x.SelectedRouteDestination);
            _selectedRouteTaskHelper = _selectedRouteTaskSubject.ToProperty(this, x => x.SelectedRouteTask);

            //Update the SelectedRouteVM whenever the RouteDestination changes
            SelectedEntityObservable.ObserveOnDispatcher().Subscribe(r =>
            {
                if (SelectedRouteVM != null)
                    SelectedRouteVM.Dispose();

                this.SelectedRouteVM = new RouteVM(r);
            });

            //Setup the RouteManifestVM
            RouteManifestVM = new RouteManifestVM();

            //Update the SelectedRouteDestinationVM whenever the RouteDestination changes
            _selectedRouteDestinationSubject.ObserveOnDispatcher().Subscribe(rd => this.SelectedRouteDestinationVM = new RouteDestinationVM(rd));

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
            .Throttle(TimeSpan.FromMilliseconds(250))
            .ObserveOnDispatcher().Subscribe(_ =>
            {
                //Cancel the last RoutesLoad (if it is in the process of loading)
                _cancelLastRoutesLoad.OnNext(true);
                //Notify the RoutesVM is loading Routes
                IsLoadingSubject.OnNext(true);

                DomainContext.LoadAsync(DomainContext.GetRoutesForServiceProviderOnDayQuery(ContextManager.RoleId, SelectedDate), _cancelLastRoutesLoad)
                .ContinueWith(task =>
                {
                    if (task.IsCanceled || !task.Result.Any())
                    {
                        ViewObservable.OnNext(new DomainCollectionViewFactory<Route>(new ObservableCollection<Route>()).View);
                        return;
                    }
                    //Notify the RoutesVM has completed loading Routes
                    IsLoadingSubject.OnNext(false);

                    //Update the CollectionView
                    ViewObservable.OnNext(new DomainCollectionViewFactory<Route>(task.Result).View);
                }, TaskScheduler.FromCurrentSynchronizationContext());
            });

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

                //If the RouteTask is not new, use the one GetRouteTaskDetailsQuery method 
                if (selectedRouteTask.EntityState != EntityState.New)
                {
                    DomainContext.LoadAsync(DomainContext.GetRouteTaskDetailsQuery(ContextManager.RoleId, selectedRouteTask.Id), cancelLastDetailsLoad)
                    .ContinueWith(task =>
                    {
                        if (!task.IsCanceled)
                            selectedRouteTask.DetailsLoaded = true;
                    }, TaskScheduler.FromCurrentSynchronizationContext());

                } //If the RouteTask is new and it has a ParentRouteTaskHolder, load it's ServiceHolder details
                else if (selectedRouteTask.ParentRouteTaskHolder != null)
                {
                    selectedRouteTask.ServiceHolder.LoadDetails(cancelLastDetailsLoad);
                }
            });

            #endregion

            //Hookup the CollectionView Filter whenever the CollectionViewObservable changes
            ViewObservable.Where(cv => cv != null).DistinctUntilChanged()
            .ObserveOnDispatcher().Subscribe(collectionView => UpdateFilter());

            //Update the view whenever the filter changes
            VM.DispatcherFilter.FilterUpdatedObservable.ObserveOnDispatcher().Subscribe(_ =>
            {
                if (CollectionView == null) return;
                UpdateFilter();
            });
        }

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
                .Select(routes => routes.Cast<object>().Any());  // select routes > 0

            //Can calculate routes when there are routes, and when the context is not submitting
            var canCalculateRoutes = DataManager.DomainContextIsSubmittingObservable.CombineLatest(routesExist, (isSubmitting, areRoutes) => !isSubmitting && areRoutes);
            AutoCalculateRoutes = new ReactiveCommand(canCalculateRoutes);

            //Populate routes with the UnroutedTasks and refresh the filter counts
            AutoCalculateRoutes.SubscribeOnDispatcher().Subscribe(_ =>
            {
                //Populate the routes with the unrouted tasks
                var routedTaskHolders = SimpleRouteCalculator.PopulateRoutes((IEnumerable<TaskHolder>)VM.TaskBoard.CollectionView, CollectionView.Cast<Route>());

                //Remove the routedTasks from the task board
                foreach (var taskHoldersToRemove in routedTaskHolders.ToArray())
                    VM.TaskBoard.LoadedTaskHolders.Remove(taskHoldersToRemove);
            });

            //Allow the user to open the route manifests whenever there is one or more route visible
            OpenRouteManifests = new ReactiveCommand(routesExist);

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
        }


        #endregion

        #region Logic

        #region Protected

        protected override bool BeforeAdd()
        {
            if (((BusinessAccount)ContextManager.OwnerAccount).MaxRoutes <= CollectionView.Cast<object>().Count())
            {
                var tooManyRoutes = new TooManyRoutesError
                {
                    MaxNumberOfRoutes = ((BusinessAccount)ContextManager.OwnerAccount).MaxRoutes
                };

                tooManyRoutes.Show();

                return false;
            }
            return true;
        }

        protected override void OnAddEntity(Route newRoute)
        {
            newRoute.Date = SelectedDate;
            newRoute.OwnerBusinessAccount = (BusinessAccount)ContextManager.OwnerAccount;
            newRoute.RouteType = VM.DispatcherFilter.ServiceTemplateOptions.Where(o => o.IsSelected).Select(o => ((ServiceTemplate)o.Entity).Name).FirstOrDefault();
        }

        /// <summary>
        /// Called when delete route is called.
        /// </summary>
        /// <param name="deletedRoute">The deleted route.</param>
        protected override void OnDeleteEntity(Route deletedRoute)
        {
            //Remove the routeTasks from the deletedRoute (and keep them)
            var routeTasksToKeep =
                deletedRoute.RouteDestinations.SelectMany(routeDestination => routeDestination.RouteTasks).ToList();

            //Delete the routeDestinations
            DataManager.RemoveEntities(deletedRoute.RouteDestinations);

            //Add the route tasks to keep back to the task board
            foreach (var routeTask in routeTasksToKeep)
            {
                //Clear RouteDestination reference
                routeTask.RouteDestinationId = null;

                //TODO?
                ////Add the routeTask back to LoadedRouteTasks
                //if (!this.LoadedRouteTasks.Contains(routeTask))
                //    LoadedRouteTasks.Add(routeTask);
                ////Add the routeTask back to the EntitySet (if it is not generated)
                //if (!routeTask.GeneratedOnServer && !this.DomainContext.RouteTasks.Contains(routeTask))
                //    this.DomainContext.RouteTasks.Add(routeTask);
            }

        }

        #endregion

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

            this.DomainContext.RouteDestinations.Remove(routeDestination);
        }

        #endregion

        #endregion
    }
}
using FoundOps.Common.Silverlight.Services;
using FoundOps.Common.Silverlight.Tools.ExtensionMethods;
using FoundOps.Common.Tools;
using FoundOps.Common.Tools.ExtensionMethods;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.SLClient.Data.Services;
using FoundOps.SLClient.Data.ViewModels;
using FoundOps.SLClient.UI.Tools;
using MEFedMVVM.ViewModelLocator;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// Logic for populating the TaskBoard.
    /// </summary>
    [ExportViewModel("TaskBoardVM")]
    public class TaskBoardVM : CoreEntityCollectionVM<RouteTask>
    {
        #region Public

        private readonly Subject<RouteTask> _selectedRouteTaskSubject = new Subject<RouteTask>();
        private readonly ObservableAsPropertyHelper<RouteTask> _selectedRouteTask;
        /// <summary>
        /// The first selected task in the list of selected items. 
        /// The list is either from the task board or route tree view, depending on what was selected last.
        /// </summary>
        public RouteTask SelectedRouteTask { get { return _selectedRouteTask.Value; } set { _selectedRouteTaskSubject.OnNext(value); } }

        private IEnumerable<RouteTask> _selectedRouteTasks;
        ///<summary>
        /// The selected task
        ///</summary>
        public IEnumerable<RouteTask> SelectedRouteTasks
        {
            get { return _selectedRouteTasks; }
            set
            {
                _selectedRouteTasks = value;
                this.RaisePropertyChanged("SelectedRouteTasks");

                SelectedRouteTask = SelectedRouteTasks == null ? null : SelectedRouteTasks.LastOrDefault();
            }
        }

        private readonly Subject<bool> _reloadTasks = new Subject<bool>();
        /// <summary>
        /// Used to force reload the tasks.
        /// </summary>
        public IObserver<bool> ForceReloadTasks { get { return _reloadTasks; } }

        #endregion

        #region Locals

        // Used to cancel the previous TasksLoad.
        private readonly Subject<bool> _cancelLastTasksLoad = new Subject<bool>();

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskBoardVM"/> class.
        /// </summary>
        [ImportingConstructor]
        public TaskBoardVM()
            : base(true, false, false) //Do not initialize a default collection view
        {
            _selectedRouteTask = _selectedRouteTaskSubject.ToProperty(this, x => x.SelectedRouteTask);
            SetupDataLoading();
        }

        private void SetupDataLoading()
        {
            #region Load Tasks

            //Load the Tasks whenever
            //a) the Dispatcher is entered/re-entered
            //b) the current role changed, while the dispatcher was open
            //c) the SelectedDate changes
            //d) RejectChanges is called due to an error
            VM.Navigation.CurrentSectionObservable.Where(section => section == "Dispatcher").AsGeneric()
            .Merge(ContextManager.OwnerAccountObservable.Where(o => VM.Navigation.CurrentSectionObservable.First() == "Dispatcher").AsGeneric())
            .Merge(VM.Routes.SelectedDateObservable.AsGeneric())
            .Merge(DataManager.RejectChangesDueToError)
            .Merge(_reloadTasks)
            .Throttle(TimeSpan.FromMilliseconds(100))
            .ObserveOnDispatcher().Subscribe(_ =>
            {
                _cancelLastTasksLoad.OnNext(true);

                if(VM.Routes.SelectedDate == new DateTime())
                    return;

                Manager.CoreDomainContext.LoadAsync(DomainContext.GetUnroutedServicesQuery(ContextManager.RoleId, VM.Routes.SelectedDate), _cancelLastTasksLoad).ContinueWith(
                    task =>
                    {
                        if (task.IsCanceled)
                            return;

                        //Notify the TaskBoardVM has completed loading RouteTasks
                        IsLoadingSubject.OnNext(false);

                        ViewObservable.OnNext(new DomainCollectionViewFactory<RouteTask>(task.Result.ToObservableCollection()).View);
                    },
                    TaskScheduler.FromCurrentSynchronizationContext());
            });

            //Listen to RejectChanges and put any new RouteTasks back into the TaskBoard
            DomainContext.ChangesRejectedObservable.ObserveOnDispatcher().Subscribe(e =>
            {
                var tasksToReturnToTaskBoard = e.RejectedAddedEntities.OfType<RouteTask>();

                if (SourceCollection as ObservableCollection<RouteTask> != null)
                    ((ObservableCollection<RouteTask>)SourceCollection).AddRange(tasksToReturnToTaskBoard);
            });

            #endregion

            #region Setup Filter

            //Hookup the CollectionView Filter whenever the CollectionViewObservable changes
            ViewObservable.Where(cv => cv != null).DistinctUntilChanged()
            .ObserveOnDispatcher().Subscribe(collectionView => UpdateFilter());

            //Update the view whenever the filter changes
            VM.DispatcherFilter.FilterUpdatedObservable.ObserveOnDispatcher().Subscribe(_ =>
            {
                if (CollectionView == null) return;
                UpdateFilter();
            });

            #endregion
        }

        /// <summary>
        /// Updates the CollectionView filter
        /// </summary>
        private void UpdateFilter()
        {
            CollectionView.Filter = null;
            CollectionView.Filter += routeTask => VM.DispatcherFilter.RouteTaskIncludedInFilter((RouteTask)routeTask);
        }

        #endregion
    }
}

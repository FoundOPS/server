using System.Collections.ObjectModel;
using FoundOps.Common.Silverlight.MVVM.Messages;
using FoundOps.Common.Silverlight.Services;
using FoundOps.Common.Silverlight.Tools.ExtensionMethods;
using FoundOps.Common.Tools;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.SLClient.Data.Services;
using FoundOps.SLClient.Data.ViewModels;
using FoundOps.SLClient.UI.Tools;
using MEFedMVVM.ViewModelLocator;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using ReactiveUI.Xaml;
using Telerik.Windows.Controls;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// Logic for populating the TaskBoard.
    /// </summary>
    [ExportViewModel("TaskBoardVM")]
    public class TaskBoardVM : CoreEntityCollectionVM<TaskHolder>
    {
        #region Public

        private readonly Subject<TaskHolder> _selectedTaskSubject = new Subject<TaskHolder>();
        private readonly ObservableAsPropertyHelper<TaskHolder> _selectedTask;
        /// <summary>
        /// The first selected task in the list of selected items. 
        /// The list is either from the task board or route tree view, depending on what was selected last.
        /// </summary>
        public TaskHolder SelectedTask { get { return _selectedTask.Value; } set { _selectedTaskSubject.OnNext(value); } }

        private IEnumerable<TaskHolder> _selectedTasks;
        ///<summary>
        /// The selected task
        ///</summary>
        public IEnumerable<TaskHolder> SelectedTasks
        {
            get { return _selectedTasks; }
            set
            {
                _selectedTasks = value;
                this.RaisePropertyChanged("SelectedTasks");

                SelectedTask = SelectedTasks == null ? null : SelectedTasks.LastOrDefault();
            }
        }

        /// <summary>
        /// The loaded task holders.
        /// </summary>
        public ObservableCollection<TaskHolder> LoadedTaskHolders
        {
            get
            {
                if (CollectionView == null)
                    return new ObservableCollection<TaskHolder>();

                return (ObservableCollection<TaskHolder>)CollectionView.SourceCollection;
            }
        }

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
            _selectedTask = _selectedTaskSubject.ToProperty(this, x => x.SelectedTask);
            SetupDataLoading();
        }

        private void SetupDataLoading()
        {
            #region Load Tasks

            //Load the TaskHolders whenever
            //a) the Dispatcher is entered/re-entered
            //b) the SelectedDate changes
            //c) RejectChanges is called due to an error
            MessageBus.Current.Listen<NavigateToMessage>().Where(m => m.UriToNavigateTo.ToString().Contains("Dispatcher")).AsGeneric()
            .Merge(VM.Routes.SelectedDateObservable.AsGeneric())
            .Merge(DataManager.RejectChangesDueToError)
            .Throttle(TimeSpan.FromMilliseconds(100))
            .ObserveOnDispatcher().Subscribe(_ =>
            {
                _cancelLastTasksLoad.OnNext(true);

                Manager.CoreDomainContext.LoadAsync(DomainContext.GetUnroutedServicesQuery(ContextManager.RoleId, VM.Routes.SelectedDate), _cancelLastTasksLoad).ContinueWith(
                    task =>
                    {
                        if (task.IsCanceled)
                            return;

                        //Notify the TaskBoardVM has completed loading TaskHolders
                        IsLoadingSubject.OnNext(false);

                        //Detach all TaskHolders so that changes are not tracked
                        DataManager.DetachEntities(task.Result.ToArray());

                        foreach (var taskHolder in task.Result.ToArray())
                            taskHolder.CreateRouteTask();

                        ViewObservable.OnNext(new DomainCollectionViewFactory<TaskHolder>(task.Result.ToObservableCollection()).View);
                    },
                    TaskScheduler.FromCurrentSynchronizationContext());
            });

            //Listen to RejectChanges and put any new RouteTasks back into the TaskBoard
            DomainContext.ChangesRejectedObservable.ObserveOnDispatcher().Subscribe(e =>
            {
                var tasksToReturnToTaskBoard = e.RejectedAddedEntities.OfType<RouteTask>();
                if (SourceCollection as ObservableCollection<TaskHolder> != null)
                    ((ObservableCollection<TaskHolder>)SourceCollection).AddRange(tasksToReturnToTaskBoard.Select(rt => rt.ParentRouteTaskHolder));
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
            CollectionView.Filter += taskHolder => VM.DispatcherFilter.TaskHolderIncludedInFilter((TaskHolder)taskHolder);
        }

        #endregion
    }
}

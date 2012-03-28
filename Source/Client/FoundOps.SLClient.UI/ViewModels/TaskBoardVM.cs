using System.ComponentModel;
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

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// Logic for populating the TaskBoard.
    /// </summary>
    [ExportViewModel("TaskBoardVM")]
    public class TaskBoardVM : CoreEntityCollectionVM<TaskHolder>
    {
        #region Public

        private readonly ObservableAsPropertyHelper<DateTime> _selectedDate;
        private readonly Subject<DateTime> _selectedDateSubject = new Subject<DateTime>();
        /// <summary>
        /// An Observable of the SelectedDate.
        /// </summary>
        public IObservable<DateTime> SelectedDateObservable { get { return _selectedDateSubject.AsObservable(); } }

        /// <summary>
        /// The SelectedDate.
        /// </summary>
        public DateTime SelectedDate
        {
            get { return _selectedDate.Value; }
            set { _selectedDateSubject.OnNext(value); }
        }

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

        #endregion

        #region Locals

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskBoardVM"/> class.
        /// </summary>
        [ImportingConstructor]
        public TaskBoardVM()
            : base(true, false, false) //Do not initialize a default collection view
        {
            _selectedDate = this.ObservableToProperty(_selectedDateSubject, x => x.SelectedDate, DateTime.Now.Date);
            _selectedTask = _selectedTaskSubject.ToProperty(this, x => x.SelectedTask);
            SetupDataLoading();
        }

        private void SetupDataLoading()
        {
            //Load the ServiceHolders whenever
            //a) the Dispatcher is entered/re-entered
            //b) the SelectedDate changes
            MessageBus.Current.Listen<NavigateToMessage>().Where(m => m.UriToNavigateTo.ToString().Contains("Dispatcher")).AsGeneric()
            .Merge(SelectedDateObservable.AsGeneric())
            .ObserveOnDispatcher().Subscribe(_ =>
            {
                var query = DomainContext.GetUnroutedServicesQuery(ContextManager.RoleId, SelectedDate);

                Manager.CoreDomainContext.LoadAsync(query).ContinueWith(
                    task => CollectionViewObservable.OnNext(new DomainCollectionViewFactory<TaskHolder>(task.Result).View),
                    TaskScheduler.FromCurrentSynchronizationContext());
            });

            //Hookup the CollectionView Filter whenever the CollectionViewObservable changes
            CollectionViewObservable.Where(cv => cv != null).DistinctUntilChanged()
            .ObserveOnDispatcher().Subscribe(collectionView => UpdateFilter());

            //Update the view whenever the filter changes
            VM.DispatcherFilter.FilterUpdatedObservable.ObserveOnDispatcher().Subscribe(_ =>
            {
                if (CollectionView == null) return;
                UpdateFilter();
            });
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

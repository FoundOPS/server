using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using FoundOps.Common.Silverlight.MVVM.Messages;
using FoundOps.Common.Silverlight.Services;
using FoundOps.Common.Silverlight.Tools.ExtensionMethods;
using FoundOps.Common.Tools;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.SLClient.Data.Services;
using FoundOps.SLClient.Data.ViewModels;
using MEFedMVVM.ViewModelLocator;
using ReactiveUI;

namespace FoundOps.SLClient.UI.ViewModels
{
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

        private readonly ObservableAsPropertyHelper<ObservableCollection<TaskHolder>> _unroutedTaskHolders;
        /// <summary>
        /// The UnroutedTaskHolders.
        /// It is not an EntityList so that Items can be added/removed without affecting the EntitySet for
        /// a) DragAndDrop and b) auto route calculation.
        /// </summary>
        public ObservableCollection<TaskHolder> UnroutedTaskHolders { get { return _unroutedTaskHolders.Value; } }

        #endregion

        #region Locals

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskBoardVM"/> class.
        /// </summary>
        [ImportingConstructor]
        public TaskBoardVM()
        {
            _selectedDate = this.ObservableToProperty(_selectedDateSubject, x => x.SelectedDate, DateTime.Now.Date);
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
        }

        #endregion
    }
}

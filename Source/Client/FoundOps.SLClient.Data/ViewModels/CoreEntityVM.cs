using System;
using ReactiveUI;
using ReactiveUI.Xaml;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using FoundOps.SLClient.Data.Services;
using FoundOps.Common.Silverlight.UI.ViewModels;
using System.ServiceModel.DomainServices.Client;
using FoundOps.Common.Silverlight.MVVM.Messages;
using FoundOps.Server.Services.CoreDomainService;

namespace FoundOps.SLClient.Data.ViewModels
{
    /// <summary>
    /// A view model which encapsulates some standard logic/properties for an entity.
    /// Commands: Save, Discard Changes
    /// </summary>
    public abstract class CoreEntityVM : DataFedVM, ISaveDiscardChangesCommands
    {
        #region Public Properties

        // ISaveDiscardChangesCommands

        /// <summary>
        /// A command for saving changes.
        /// </summary>
        public IReactiveCommand SaveCommand { get; protected set; }
        /// <summary>
        /// A command for discarding changes.
        /// </summary>
        public IReactiveCommand DiscardCommand { get; protected set; }

        private readonly Subject<string> _navigateToObservable = new Subject<string>();
        /// <summary>
        /// Publishes a stream of NavigateTo message uris (as strings)
        /// </summary>
        public IObservable<string> NavigateToObservable { get { return _navigateToObservable; } }

        #endregion

        #region Protected

        private CoreDomainContext _context;
        /// <summary>
        /// Gets or sets the CoreDomainContext.
        /// </summary>
        /// <value>
        /// The CoreDomainContext.
        /// </value>
        public CoreDomainContext Context
        {
            get
            {
                return _context;
            }
            private set
            {
                _context = value;
                this.RaisePropertyChanged("Context");
            }
        }

        /// <summary>
        /// The DataManager.
        /// </summary>
        protected readonly DataManager DataManager;

        /// <summary>
        /// Gets the context manager.
        /// </summary>
        public ContextManager ContextManager { get; private set; }

        private readonly Subject<bool> _discardObservable = new Subject<bool>();
        /// <summary>
        /// Gets an observable which streams the DateTimes discards are called.
        /// </summary>
        protected IObservable<bool> DiscardObservable
        {
            get { return _discardObservable.AsObservable(); }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreEntityVM"/> class.
        /// </summary>
        /// <param name="dataManager">The data manager.</param>
        protected CoreEntityVM(DataManager dataManager)
        {
            DataManager = dataManager;
            ContextManager = dataManager.ContextManager;
            Context = DataManager.Context;

            //Hookup NavigateToObservable
            MessageBus.Current.Listen<NavigateToMessage>().Subscribe(message => _navigateToObservable.OnNext(message.UriToNavigateTo.ToString()));

            #region Register Commands

            //Can save when: context has changes and is not submitting. Check every .25 second
            var canSaveCommand = Observable.Interval(new TimeSpan(0, 0, 0, 0, 250))
                .Select(_ => this.Context.HasChanges && !this.Context.IsSubmitting)
                //Combine with the CanSaveObservable
                .CombineLatest(CanSaveObservable, (canSave, canSaveTwo) => canSave && canSaveTwo).DistinctUntilChanged();

            SaveCommand = new ReactiveCommand(canSaveCommand);
            SaveCommand.Subscribe(param =>
            {
                if (!BeforeSaveCommand()) return;

                DataManager.EnqueueSubmitOperation(OnSave);
            });

            //Can discard when: context has changes and is not submitting. Check every .25 second
            var canDiscardCommand = Observable.Interval(new TimeSpan(0, 0, 0, 0, 250))
                .Select(_ => this.Context.HasChanges && !this.Context.IsSubmitting)
                //Combine with the CanDiscardObservable
                .CombineLatest(CanDiscardObservable, (canDiscard, canDiscardTwo) => canDiscard && canDiscardTwo).DistinctUntilChanged();

            DiscardCommand = new ReactiveCommand(canDiscardCommand);
            DiscardCommand.Subscribe(param =>
            {
                if (!BeforeDiscardCommand()) return;
                Context.RejectChanges();
                OnDiscard();
                _discardObservable.OnNext(true);
            });

            #endregion
        }

        #region Logic

        #region Empty methods to override

        /// <summary>
        /// Before the discard command.
        /// </summary>
        /// <returns></returns>
        protected virtual bool BeforeDiscardCommand() { return true; }

        /// <summary>
        /// Called when after changes are Discarded.
        /// </summary>
        protected virtual void OnDiscard() { }

        private readonly BehaviorSubject<bool> _canSaveObservable = new BehaviorSubject<bool>(true);
        private IDisposable _canSaveDisposable; //Keeps track of the subscription to the last set CanSaveObservable
        /// <summary>
        /// Determines whether this instance can save (in addition to CanSave). This is now the preferred method over CanSave.
        /// </summary>
        protected IObservable<bool> CanSaveObservable
        {
            get
            {
                return _canSaveObservable;
            }
            set
            {
                if (_canSaveDisposable != null)
                    _canSaveDisposable.Dispose(); //Dispose the last CanSaveObservable

                if (value != null)
                    //Subscribe _canSaveObservable to the new CanSaveObservable
                    _canSaveDisposable = value.Subscribe(canSave => _canSaveObservable.OnNext(canSave));
            }
        }

        /// <summary>
        /// Before the save command.
        /// </summary>
        /// <returns></returns>
        protected virtual bool BeforeSaveCommand() { return true; }

        private readonly BehaviorSubject<bool> _canDiscardObservable = new BehaviorSubject<bool>(true);
        private IDisposable _canDiscardDisposable; //Keeps track of the subscription to the last set CanSaveObservable
        /// <summary>
        /// Determines whether this instance can discard.
        /// </summary>
        protected IObservable<bool> CanDiscardObservable
        {
            get
            {
                return _canDiscardObservable;
            }
            set
            {
                if (_canDiscardDisposable != null)
                    _canDiscardDisposable.Dispose(); //Dispose the last CanDiscardObservable

                if (value != null)
                    //Subscribe _canDiscardDisposable to the new CanDiscardObservable
                    _canDiscardDisposable = value.Subscribe(canSave => _canSaveObservable.OnNext(canSave));
            }
        }

        /// <summary>
        /// Called after changes are saved.
        /// </summary>
        /// <param name="submitOperation">The submit operation.</param>
        protected virtual void OnSave(SubmitOperation submitOperation) { }

        #endregion

        #endregion
    }
}

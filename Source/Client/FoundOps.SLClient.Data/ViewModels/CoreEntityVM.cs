using FoundOps.Common.Silverlight.UI.ViewModels;
using FoundOps.Server.Services.CoreDomainService;
using FoundOps.SLClient.Data.Services;
using ReactiveUI.Xaml;
using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.ServiceModel.DomainServices.Client;

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

        #endregion

        #region Protected

        /// <summary>
        /// Gets the CoreDomainContext.
        /// </summary>
        public CoreDomainContext DomainContext { get { return Manager.Data.DomainContext; } }

        /// <summary>
        /// Gets the data manager.
        /// </summary>
        protected DataManager DataManager { get { return Manager.Data; } }

        /// <summary>
        /// Gets the context manager.
        /// </summary>
        public ContextManager ContextManager { get { return Manager.Context; } }

        //Only to be used if overriding the DiscardCommand
        protected readonly Subject<bool> DiscardSubject = new Subject<bool>();

        /// <summary>
        /// Gets an observable which streams the DateTimes discards are called.
        /// </summary>
        protected IObservable<bool> DiscardObservable { get { return DiscardSubject.AsObservable(); } }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreEntityVM"/> class.
        /// </summary>
        protected CoreEntityVM()
        {
            #region Register Commands

            //Can save or discard when context has changes and is not submitting
            var canSaveDiscardCommand =
                DataManager.DomainContextHasChangesObservable.CombineLatest(DataManager.DomainContextIsSubmittingObservable,
                (hasChanges, isSubmitting) => hasChanges && !isSubmitting);

            SaveCommand = new ReactiveCommand(canSaveDiscardCommand);
            SaveCommand.ObserveOnDispatcher().Subscribe(param =>
            {
                if (!BeforeSaveCommand()) return;

                DataManager.EnqueueSubmitOperation(OnSave);
            });

            DiscardCommand = new ReactiveCommand(canSaveDiscardCommand);
            DiscardCommand.ObserveOnDispatcher().Subscribe(param =>
            {
                if (!BeforeDiscardCommand()) return;
                DomainContext.RejectChanges();
                AfterDiscard();
                DiscardSubject.OnNext(true);
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
        /// Called after changes are Discarded.
        /// </summary>
        protected virtual void AfterDiscard() { }

        /// <summary>
        /// Before the save command.
        /// </summary>
        /// <returns></returns>
        protected virtual bool BeforeSaveCommand() { return true; }

        /// <summary>
        /// Called after changes are saved.
        /// </summary>
        /// <param name="submitOperation">The submit operation.</param>
        protected virtual void OnSave(SubmitOperation submitOperation) { }

        #endregion

        #endregion
    }
}

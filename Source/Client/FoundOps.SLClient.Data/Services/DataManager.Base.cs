using FoundOps.Common.Silverlight.Tools.ExtensionMethods;
using FoundOps.Server.Services.CoreDomainService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.ServiceModel.DomainServices.Client;
using ReactiveUI;

namespace FoundOps.SLClient.Data.Services
{
    /// <summary>
    /// Subscribes to ContextManager to load the proper data
    /// </summary>
    [Export]
    public partial class DataManager : ReactiveObject
    {
        #region Public

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        /// <summary>
        /// The ContextManager
        /// </summary>
        public ContextManager ContextManager { get; private set; }

        private readonly CoreDomainContext _coreDomainContext;
        /// <summary>
        /// The CoreDomainContext
        /// </summary>
        public CoreDomainContext DomainContext { get { return _coreDomainContext; } }

        private readonly BehaviorSubject<bool> _domainContextHasChangesSubject = new BehaviorSubject<bool>(false);
        /// <summary>
        /// An Observable which notifies whenever the DomainContext has changes or does not have changes.
        /// </summary>
        public IObservable<bool> DomainContextHasChangesObservable { get { return _domainContextHasChangesSubject.DistinctUntilChanged(); } }

        private readonly ObservableAsPropertyHelper<bool> _domainContextHasChangesHelper;
        /// <summary>
        /// Gets or sets a value indicating whether the domain context has changes.
        /// Updated every .25 seconds.
        /// </summary>
        public bool DomainContextHasChanges { get { return _domainContextHasChangesHelper.Value; } }

        private readonly BehaviorSubject<bool> _domainContextIsSubmittingSubject = new BehaviorSubject<bool>(false);
        /// <summary>
        /// An Observable which pushes whenever the DomainContext is submitting, or stops submitting.
        /// </summary>
        public IObservable<bool> DomainContextIsSubmittingObservable { get { return _domainContextIsSubmittingSubject; } }

        private readonly ObservableAsPropertyHelper<bool> _domainContextIsSubmittingHelper;
        /// <summary>
        /// Gets or sets a value indicating whether the domain context is submitting.
        /// Updated every .25 seconds.
        /// </summary>
        public bool DomainContextIsSubmitting { get { return _domainContextIsSubmittingHelper.Value; } }

        #endregion

        #region Locals

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="DataManager"/> class.
        /// </summary>
        /// <param name="contextManager">The context manager.</param>
        /// <param name="coreDomainContext">The core domain context.</param>
        [ImportingConstructor]
        public DataManager(ContextManager contextManager, CoreDomainContext coreDomainContext)
        {
            ContextManager = contextManager;
            _coreDomainContext = coreDomainContext;

            _domainContextIsSubmittingHelper = _domainContextIsSubmittingSubject.ToProperty(this, x => x.DomainContextIsSubmitting);
            _domainContextHasChangesHelper = _domainContextHasChangesSubject.ToProperty(this, x => x.DomainContextHasChanges);

            //Keep track of domain context changes and domain context is submitting every quarter second (and update this public properties)
            Observable.Interval(TimeSpan.FromMilliseconds(250)).Subscribe(_ => 
            {
                _domainContextHasChangesSubject.OnNext(this.DomainContext.HasChanges);
                _domainContextIsSubmittingSubject.OnNext(this.DomainContext.IsSubmitting);
            });

            SetupConsolidatedSubmit();
        }

        #region Entity Methods

        /// <summary>
        /// Removes the entities from the Context.
        /// </summary>
        /// <param name="entitiesToRemove">The entities to remove.</param>
        public void RemoveEntities(IEnumerable<Entity> entitiesToRemove)
        {
            foreach (var entity in entitiesToRemove.ToArray())
            {
                var entitySet = DomainContext.EntityContainer.GetEntitySet(entity.GetType());
                var entitySetContainsEntity = entitySet.Cast<object>().Any(e => e == entity);
                if (entitySetContainsEntity)
                    entitySet.Remove(entity);
            }
        }

        /// <summary>
        /// Detaches the entities from the Context.
        /// </summary>
        /// <param name="entitiesToDetach">The entities to detach.</param>
        public void DetachEntities(IEnumerable<Entity> entitiesToDetach)
        {
            foreach (var entity in entitiesToDetach.ToArray())
            {
                var entitySet = DomainContext.EntityContainer.GetEntitySet(entity.GetType());
                var entitySetContainsEntity = entitySet.Cast<object>().Any(e => e == entity);
                if (entitySetContainsEntity)
                    entitySet.Detach(entity);
            }
        }

        #endregion
    }
}

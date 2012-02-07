using System;
using System.Linq;
using System.Reactive.Linq;
using System.ComponentModel;
using System.Reactive.Subjects;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ServiceModel.DomainServices.Client;
using FoundOps.Server.Services.CoreDomainService;

namespace FoundOps.SLClient.Data.Services
{
    /// <summary>
    /// Subscribes to ContextManager to load the proper data
    /// </summary>
    [Export]
    public partial class DataManager
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
        public CoreDomainContext Context
        {
            get { return _coreDomainContext; }
        }

        private readonly BehaviorSubject<bool> _domainContextHasChangesSubject = new BehaviorSubject<bool>(false);

        /// <summary>
        /// An Observable which notifies whenever the DomainContext has changes or does not have changes.
        /// </summary>
        public IObservable<bool> DomainContextHasChangesObservable
        {
            get { return _domainContextHasChangesSubject; }
        }

        private bool _domainContextHasChanges;

        /// <summary>
        /// Gets or sets a value indicating whether the domain context has changes.
        /// Updated every .25 seconds.
        /// </summary>
        public bool DomainContextHasChanges
        {
            get { return _domainContextHasChanges; }
            set
            {
                if (value == _domainContextHasChanges)
                    return;

                _domainContextHasChanges = value;
                _domainContextHasChangesSubject.OnNext(value);
                this.RaisePropertyChanged("DomainContextHasChanges");
            }
        }

        private readonly BehaviorSubject<bool> _domainContextIsSubmittingSubject = new BehaviorSubject<bool>(false);

        /// <summary>
        /// An Observable which pushes whenever the DomainContext is submitting, or stops submitting.
        /// </summary>
        public IObservable<bool> DomainContextIsSubmittingObservable
        {
            get { return _domainContextIsSubmittingSubject; }
        }

        private bool _domainContextIsSubmitting;

        /// <summary>
        /// Gets or sets a value indicating whether the domain context is submitting.
        /// Updated every .25 seconds.
        /// </summary>
        public bool DomainContextIsSubmitting
        {
            get { return _domainContextIsSubmitting; }
            set
            {
                if (value == _domainContextIsSubmitting)
                    return;

                _domainContextIsSubmitting = value;
                _domainContextIsSubmittingSubject.OnNext(value);
                this.RaisePropertyChanged("DomainContextIsSubmitting");
            }
        }

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

            //Keep track of domain context changes and domain context is submitting every quarter second (and update this public properties)
            Observable.Interval(TimeSpan.FromMilliseconds(250)).ObserveOnDispatcher().Subscribe(_ =>
            {
                DomainContextHasChanges=this.Context.HasChanges;
                DomainContextIsSubmitting=this.Context.IsSubmitting;
            });

            SetupQueries();
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
                var entitySet = Context.EntityContainer.GetEntitySet(entity.GetType());
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
                var entitySet = Context.EntityContainer.GetEntitySet(entity.GetType());
                var entitySetContainsEntity = entitySet.Cast<object>().Any(e => e == entity);
                if (entitySetContainsEntity)
                    entitySet.Detach(entity);
            }
        }

        #endregion
    }
}

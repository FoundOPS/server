using System;
using ReactiveUI;
using System.Linq;
using System.Reactive.Linq;
using FoundOps.Common.Tools;
using System.Reactive.Subjects;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FoundOps.Core.Context.Services;
using FoundOps.Core.Models.CoreEntities;
using System.ComponentModel.Composition;
using FoundOps.Common.Silverlight.UI.Controls.InfiniteAccordion;

namespace FoundOps.SLClient.Data.Services
{
    /// <summary>
    /// Manages the appication's contexts
    /// </summary>
    [Export]
    public class ContextManager : ReactiveObject
    {
        #region Current Account and Role Properties

        readonly ObservableAsPropertyHelper<Guid> _roleId;
        /// <summary>
        /// Gets the current roleId.
        /// </summary>
        public Guid RoleId
        {
            get { return _roleId.Value; }
        }

        private readonly ReplaySubject<Guid> _roleIdObservable = new ReplaySubject<Guid>(1);
        ///<summary>
        /// A distinct observable for the current RoleId. It will Replay the last RoleId.
        ///</summary>
        public IObservable<Guid> RoleIdObservable { get { return _roleIdObservable; } }

        readonly Subject<Guid> _roleIdObserver = new Subject<Guid>();
        /// <summary>
        /// An observer Guid of the current RoleId
        /// </summary>
        public IObserver<Guid> RoleIdObserver { get { return _roleIdObserver; } }

        /// <summary>
        /// An observable Party of the current OwnerAccount
        /// </summary>
        public IObservable<Party> OwnerAccountObservable { get; private set; }

        readonly ObservableAsPropertyHelper<Party> _ownerAccount;
        /// <summary>
        /// Gets the current owner account.
        /// </summary>
        public Party OwnerAccount
        {
            get { return _ownerAccount.Value; }
        }

        /// <summary>
        /// An observable of the current UserAccount
        /// </summary>
        public IObservable<UserAccount> UserAccountObservable { get; private set; }

        readonly ObservableAsPropertyHelper<UserAccount> _userAccount;
        /// <summary>
        /// Gets the current user account.
        /// </summary>
        public UserAccount UserAccount
        {
            get { return _userAccount.Value; }
        }
        #endregion

        #region Infinite Accordion

        /// <summary>
        /// The CurrentContext. It publishes whenever a Context is Added, Removed.
        /// </summary>
        private readonly Subject<ObservableCollection<object>> _currentContextSubject = new Subject<ObservableCollection<object>>();
        /// <summary>
        /// The current context provider Observable.
        /// </summary>
        public IObservable<ObservableCollection<object>> CurrentContextObservable { get { return _currentContextSubject.AsObservable(); } }

        private readonly ObservableAsPropertyHelper<ObservableCollection<object>> _currentContext;
        /// <summary>
        /// Gets or sets the current context.
        /// </summary>
        /// <value>
        /// The current context.
        /// </value>
        public ObservableCollection<object> CurrentContext { get { return _currentContext.Value; } set { _currentContextSubject.OnNext(value); } }

        /// <summary>
        /// Gets the closest context of the types in order of the CurrentContextProvider.SelectedContext, then the last CurrentContext.
        /// </summary>
        /// <param name="types">The different types.</param>
        public object ClosestContext(IEnumerable<Type> types)
        {
            if (CurrentContextProvider != null && CurrentContextProvider.SelectedContext != null
                && types.Contains(CurrentContextProvider.SelectedContext.GetType()))
                return CurrentContextProvider.SelectedContext;

            return CurrentContext.LastOrDefault(context => types.Contains(context.GetType()));
        }

        private readonly Subject<IProvideContext> _currentContextProviderSubject = new Subject<IProvideContext>();
        /// <summary>
        /// The current context provider Observable.
        /// </summary>
        public IObservable<IProvideContext> CurrentContextProviderObservable { get { return _currentContextProviderSubject.AsObservable(); } }

        private readonly ObservableAsPropertyHelper<IProvideContext> _currentContextProvider;
        /// <summary>
        /// The current context provider.
        /// </summary>
        public IProvideContext CurrentContextProvider { get { return _currentContextProvider.Value; } }

        private readonly IObservable<bool> _contextChangedObservable;
        /// <summary>
        /// Gets the context changed observable.
        /// It publishes whenever:
        /// a) the CurrentContext changes
        /// b) the CurrentContextProvider changes
        /// c) the CurrentContextProvider's SelectedContext changes
        /// </summary>
        public IObservable<bool> ContextChangedObservable { get { return _contextChangedObservable.Throttle(new TimeSpan(0, 0, 0, 0, 150)).AsObservable(); } }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextManager"/> class.
        /// </summary>
        [ImportingConstructor]
        public ContextManager(IPartyDataService partyDataService)
        {
            #region Current Account and Role Properties

            //Subcribe _roleIdObservable to the distinct RoleIdObserver changes
            ((Subject<Guid>)RoleIdObserver).DistinctUntilChanged()
                .Throttle(new TimeSpan(0, 0, 0, 1)) //Thottle for 1 second to allow controls to unload
                .Subscribe(_roleIdObservable);

            _roleId = RoleIdObservable.ToProperty(this, x => x.RoleId);

            OwnerAccountObservable = new BehaviorSubject<Party>(null);
            _ownerAccount = OwnerAccountObservable.ToProperty(this, x => x.OwnerAccount);

            UserAccountObservable = new BehaviorSubject<UserAccount>(null);
            _userAccount = UserAccountObservable.ToProperty(this, x => x.UserAccount);

            //Load the current user account
            partyDataService.GetCurrentUserAccount(userAccount => ((BehaviorSubject<UserAccount>)UserAccountObservable).OnNext(userAccount));

            //Whenever the RoleId changes load the OwnerAccount
            RoleIdObservable.Subscribe(roleId => partyDataService.GetCurrentParty(roleId, account => ((BehaviorSubject<Party>)OwnerAccountObservable).OnNext(account)));

            #endregion

            #region Infinite Accordion

            #region CurrentContext property

            //Setup the CurrentContext property
            _currentContext = this._currentContextSubject.ToProperty(this, x => x.CurrentContext, new ObservableCollection<object>());

            //Whenever the collection is changed or set, publish the CurrentContext
            this._currentContextSubject.FromCollectionChangedOrSet().Subscribe(this._currentContextSubject);

            //Listen to the AddContext and RemoveContext messages, and update the CurrentContext
            MessageBus.Current.Listen<AddContextMessage>().Where(message => message.Context != null)
                .Subscribe(message => this.CurrentContext.Add(message.Context));
            MessageBus.Current.Listen<RemoveContextMessage>().Where(message => message.Context != null)
                .Subscribe(message => this.CurrentContext.Remove(message.Context));

            #endregion

            //Setup the CurrentContextProvider property
            _currentContextProvider = this._currentContextProviderSubject.ToProperty(this, x => x.CurrentContextProvider);

            //Listen to the ContextProviderChanged messages, and update the CurrentContextProviderSubject
            MessageBus.Current.Listen<ContextProviderChangedMessage>().Subscribe(message => this._currentContextProviderSubject.OnNext(message.CurrentContextProvider));

            //Setup ContextChangedObservable to trigger whenever:
            _contextChangedObservable =
                //a) the CurrentContext changes
                this.CurrentContextObservable.Select(_ => true)
                //b) the CurrentContextProvider changes
                .Merge(this.CurrentContextProviderObservable.Select(_ => true))
                //c) the CurrentContextProvider's SelectedContext changes
                .Merge(this.CurrentContextProviderObservable.SelectMany(ccp =>
                    ccp == null ? new[] { true }.ToObservable() : //If the CurrentContextProvider is empty, update once
                    ccp.SelectedContextObservable.Select(_ => true))); //Select the SelectedContextObservable changes

            #endregion
        }

        //Logic

        #region Infinite Accordion

        /// <summary>
        /// Gets the infinite accordion context for a type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetContext<T>() where T : class
        {
            var currentContextProviderContext = CurrentContextProvider != null && CurrentContextProvider.SelectedContext != null ? CurrentContextProvider.SelectedContext : null;

            //If the current contextProvider's SelectedContext is T, return it
            if (currentContextProviderContext as T != null)
                return (T)currentContextProviderContext;

            //If the CurrentContext != null, return the last context of that type
            return CurrentContext != null ? CurrentContext.OfType<T>().LastOrDefault() : null;
        }

        /// <summary>
        /// An observable which publishes whenever a specific context type changes. Throttled every 250 milliseconds.
        /// </summary>
        /// <typeparam name="T">The type of context to track.</typeparam>
        public IObservable<T> GetContextObservable<T>() where T : class
        {
            var clientContextSubject = new Subject<T>();
            //When any context changes, publish the GetContext<T>
            this.ContextChangedObservable.Throttle(new TimeSpan(0, 0, 0, 0, 250)).Subscribe(_ => clientContextSubject.OnNext(GetContext<T>()));
            //Only return the Distinct changes
            return clientContextSubject.DistinctUntilChanged().AsObservable();
        }

        #endregion
    }
}

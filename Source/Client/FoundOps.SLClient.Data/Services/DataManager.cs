using System;
using System.Linq;
using System.Diagnostics;
using System.Reactive.Linq;
using FoundOps.Common.Tools;
using FoundOps.Common.Models;
using System.Reactive.Subjects;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using FoundOps.Common.Silverlight.Controls;
using Microsoft.Windows.Data.DomainServices;
using System.ServiceModel.DomainServices.Client;
using FoundOps.Server.Services.CoreDomainService;

namespace FoundOps.SLClient.Data.Services
{
    /// <summary>
    /// Subscribes to ContextManager to load the proper data
    /// </summary>
    [Export]
    public class DataManager
    {
        /// <summary>
        /// Each query of the DataManager.
        /// This is used to setup each query's: a) list of ObservationState observables and b) isLoading observable
        /// </summary>
        public enum Query
        {
            ///<summary>
            /// Loads all of the BusinessAccounts related to the current RoleId
            ///</summary>
            BusinessAccounts,
            ///<summary>
            /// Loads all of the Clients related to the current RoleId
            ///</summary>
            Clients,
            ///<summary>
            /// Loads all of the Contacts related to the current RoleId
            ///</summary>
            Contacts,
            ///<summary>
            /// Loads all of the Contacts's ClientTitles related to the current RoleId
            ///</summary>
            ClientTitles,
            ///<summary>
            /// Loads all of the Employee's related to the current RoleId
            ///</summary>
            Employees,
            /// <summary>
            /// Loads all of the EmployeeHistory related to the current RoleId
            /// </summary>
            EmployeeHistory,
            /// <summary>
            /// Loads all of the Locations related to the current RoleId
            /// </summary>
            Locations,
            /// <summary>
            /// Loads all of the RecurringServices related to the current RoleId
            /// </summary>
            RecurringServices,
            /// <summary>
            /// Loads all of the Regions related to the current RoleId
            /// </summary>
            Regions,
            /// <summary>
            /// Loads all of the RouteLog related to the current RoleId
            /// </summary>
            RouteLog,
            /// <summary>
            /// Loads all of SalesTerms related to the current RoleId
            /// </summary>
            SalesTerms,
            /// <summary>
            /// Loads all of the existing Services related to the current RoleId
            /// </summary>
            Services,
            /// <summary>
            /// Loads all of the ServiceTemplates related to the current RoleId
            /// </summary>
            ServiceTemplates,
            /// <summary>
            /// Loads all of the UserAccounts related to the current RoleId
            /// </summary>
            UserAccounts,
            ///<summary>
            /// Loads all of the Vehicle's related to the current RoleId
            ///</summary>
            Vehicles,
            /// <summary>
            /// Loads all of the Vehicles' maintenance related to the current RoleId
            /// </summary>
            VehicleMaintenance
        }

        #region Public

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

        #endregion

        #region Locals

        #region Query Properties

        /// <summary>
        /// Stores each query's CombinedObservationState.
        /// </summary>
        private readonly Dictionary<object, CombinedObservationState> _observationStateQueries = new Dictionary<object, CombinedObservationState>();

        /// <summary>
        /// Stores each query's loading status observable.
        /// </summary>
        private readonly Dictionary<object, BehaviorSubject<bool>> _queriesIsLoadingSubjects = new Dictionary<object, BehaviorSubject<bool>>();

        /// <summary>
        /// Stores each query's entityList subject. NOTE: object is a BehaviorSubject'EntityList' (could not figure a way to make it covariant)
        /// It is a BehaviorSubject so that it remembers the last loaded entities
        /// </summary>
        private readonly Dictionary<object, object> _queriesEntityListObservables = new Dictionary<object, object>();

        #endregion

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

            #region Setup Queries

            //Setup BusinessAccounts query
            SetupQuery(Query.BusinessAccounts, roleId => _coreDomainContext.GetBusinessAccountsForRoleQuery(roleId), _coreDomainContext.Parties);

            //Setup Clients query
            SetupQuery(Query.Clients, roleId => _coreDomainContext.GetClientsForRoleQuery(roleId), _coreDomainContext.Clients);

            //Setup Contacts query
            SetupQuery(Query.Contacts, roleId => _coreDomainContext.GetContactsForRoleQuery(roleId), _coreDomainContext.Contacts);

            //Setup ClientTitles query
            SetupQuery(Query.ClientTitles, roleId => _coreDomainContext.GetClientTitlesForRoleQuery(roleId), _coreDomainContext.ClientTitles);

            //Setup Employees query
            SetupQuery(Query.Employees, roleId => _coreDomainContext.GetEmployeesForRoleQuery(roleId), _coreDomainContext.Employees);

            //Setup EmployeeHistory query
            SetupQuery(Query.EmployeeHistory, roleId => _coreDomainContext.GetEmployeeHistoryEntriesForRoleQuery(roleId), _coreDomainContext.EmployeeHistoryEntries);

            //Setup Locations query
            SetupQuery(Query.Locations, roleId => _coreDomainContext.GetLocationsToAdministerForRoleQuery(roleId), _coreDomainContext.Locations);

            //Setup RecurringServices query
            SetupQuery(Query.RecurringServices, roleId => _coreDomainContext.GetRecurringServicesForServiceProviderQuery(roleId), _coreDomainContext.RecurringServices);

            //Setup Regions query
            SetupQuery(Query.Regions, roleId => _coreDomainContext.GetRegionsForServiceProviderQuery(roleId), _coreDomainContext.Regions);

            //Setup RouteLog query
            SetupQuery(Query.RouteLog, roleId => _coreDomainContext.GetRouteLogForServiceProviderQuery(roleId), _coreDomainContext.Routes);

            //Setup SalesTerm query
            SetupQuery(Query.SalesTerms, roleId => _coreDomainContext.GetSalesTermsQuery(roleId), _coreDomainContext.SalesTerms);

            //Setup Services query
            SetupQuery(Query.Services, roleId => _coreDomainContext.GetServicesForRoleQuery(roleId), _coreDomainContext.Services);

            //Setup ServiceTemplates query
            SetupQuery(Query.ServiceTemplates, roleId => _coreDomainContext.GetServiceTemplatesForServiceProviderQuery(roleId), _coreDomainContext.ServiceTemplates);

            //Setup UserAccounts query
            SetupQuery(Query.UserAccounts, roleId => _coreDomainContext.GetAllUserAccountsQuery(roleId), _coreDomainContext.Parties);

            //Setup Vehicles query
            SetupQuery(Query.Vehicles, roleId => _coreDomainContext.GetVehiclesForPartyQuery(roleId), _coreDomainContext.Vehicles);

            //Setup VehicleMaintenance query
            SetupQuery(Query.VehicleMaintenance, roleId => _coreDomainContext.GetVehicleMaintenanceLogForPartyQuery(roleId), _coreDomainContext.VehicleMaintenanceLogEntries);

            #endregion

            //Tracks if a PerformNextSave is hookup up to the currentSubmitOperation.Completed event
            var performNextSaveHooked = false;
            //Whenever a SubmitOperation is enqueued: try to Submit as soon as possible
            this._nextSubmitOperationsQueue.FromCollectionChangedEvent().Where(e => e.Action == NotifyCollectionChangedAction.Add)
                //Wait until after CollectionChangedEvent. Otherwise you will not be able to clear the collection in PerformNextSave().
                .Throttle(new TimeSpan(0, 0, 0, 0, 100)).SubscribeOnDispatcher().ObserveOnDispatcher()
                .Subscribe(_ =>
                {
                    if (_currentSubmitOperation == null)
                        PerformNextSubmitOperation();
                    else if (!performNextSaveHooked) //prevent multiple hooks
                    {
                        //Hookup PerformNextSave whenever the _currentSubmitOperation is completed
                        performNextSaveHooked = true;
                        _currentSubmitOperation.Completed +=
                            (__, ___) =>
                            {
                                PerformNextSubmitOperation();
                                performNextSaveHooked = false;
                            };
                    }
                });
        }

        #region Subscription Methods

        /// <summary>
        /// Subscribes to an EntityList IObservable that will publish as they are loaded.
        /// </summary>
        /// <param name="queryKey">The key of the query you want to subscribe to.</param>
        /// <param name="observableObservationState">The observable ObservationState watching the EntityList. 
        /// It will only update when the RoleId changes and at least one observable is active.</param>
        /// <param name="action">Action to invoke for each element in the observable sequence.</param>
        /// <returns>The isLoading Observable</returns>
        public IObservable<bool> Subscribe<T>(object queryKey, IObservable<ObservationState> observableObservationState, Action<EntityList<T>> action) where T : Entity
        {
            //Add the ObservationState observable to the the CombinedObservationState
            _observationStateQueries[queryKey].AddObservable(observableObservationState);

            //Subscribe the action to the corresponding EntityList observable
            if (action != null)
                GetEntityListObservable<T>(queryKey).Subscribe(action);

            //Return the corresponding IsLoading observable
            return GetIsLoadingObservable(queryKey);
        }

        ///<summary>
        /// Get the IsLoadingObservable for a Query.
        ///</summary>
        ///<param name="queryKey">The key of the query to track.</param>
        ///<returns>An Observable that tracks if a Query is loading.</returns>
        public IObservable<bool> GetIsLoadingObservable(object queryKey)
        {
            //Return the corresponding IsLoading observable
            return _queriesIsLoadingSubjects[queryKey];
        }

        ///<summary>
        /// Gets the EntityListObservable for a Query.
        ///</summary>
        ///<param name="queryKey">The key of the query to track.</param>
        ///<returns>An Observable that passes the loaded entity list.</returns>
        public IObservable<EntityList<TEntity>> GetEntityListObservable<TEntity>(object queryKey) where TEntity : Entity
        {
            //Return the corresponding EntityList observable
            return (IObservable<EntityList<TEntity>>)_queriesEntityListObservables[queryKey];
        }

        #endregion

        #region Query Helper Methods

        /// <summary>
        /// Add a Query to the DataManager.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="queryKey">The query.</param>
        /// <param name="entityQuery">The entity query function (it is passed the roleId).</param>
        /// <param name="entitySet">The entity set.</param>
        /// <param name="loadQuery">Optional: The query will load whenever this has changed. It defaults to the RoleId stream. </param>
        public void AddQuery<T>(object queryKey, Func<Guid, EntityQuery<T>> entityQuery, EntitySet<T> entitySet, IObservable<Guid> loadQuery) where T : Entity
        {
            //setup an EntityList BehaviorSubject
            //Start of with an empty EntityList
            _queriesEntityListObservables.Add(queryKey, new BehaviorSubject<EntityList<T>>(new EntityList<T>(entitySet)));

            //setup a CombinedObservationState
            //setup an IsLoading subject
            _observationStateQueries.Add(queryKey, new CombinedObservationState());
            _queriesIsLoadingSubjects.Add(queryKey, new BehaviorSubject<bool>(false));

            //setup the QueryExecution
            SetupExecuteQueryHelper(queryKey, entityQuery, entitySet, loadQuery);
        }

        /// <summary>
        /// Setup the Query on the DataManager. Register the EntityListObservable and setup the QueryExecution
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="entityQuery">The entity query function (it is passed the roleId).</param>
        /// <param name="entitySet">The entity set.</param>
        private void SetupQuery<T>(Query query, Func<Guid, EntityQuery<T>> entityQuery, EntitySet<T> entitySet) where T : Entity
        {
            //setup an EntityList BehaviorSubject
            //Start of with an empty EntityList
            _queriesEntityListObservables.Add(query, new BehaviorSubject<EntityList<T>>(new EntityList<T>(entitySet)));

            //setup a CombinedObservationState
            //setup an IsLoading subject
            _observationStateQueries.Add(query, new CombinedObservationState());
            _queriesIsLoadingSubjects.Add(query, new BehaviorSubject<bool>(false));

            //setup the QueryExecution
            SetupExecuteQueryHelper(query, entityQuery, entitySet);
        }

        /// <summary>
        /// Setup the ExecuteObservable and subscribe the query's execution to it.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="queryKey">The key of the query.</param>
        /// <param name="entityQuery">The entity query function (it is passed the roleId).</param>
        /// <param name="entitySet">The entity set.</param>
        /// <param name="loadQuery">Optional: The query will load whenever this has changed. It defaults to the RoleId stream. </param>
        private void SetupExecuteQueryHelper<T>(object queryKey, Func<Guid, EntityQuery<T>> entityQuery, EntitySet<T> entitySet, IObservable<Guid> loadQuery = null) where T : Entity
        {
            //The executeObservable triggers whenever the query should be loaded
            var executeObservable = SetupExecuteObservable(queryKey, loadQuery);

            executeObservable
                .ObserveOnDispatcher().Subscribe(roleId =>
                    ExecuteQuery(entityQuery(roleId),
                        /*On the response, update the EntityListObservable */
                    loadedEntities => ((IObserver<EntityList<T>>)GetEntityListObservable<T>(queryKey)).OnNext(new EntityList<T>(entitySet, loadedEntities)),
                    _queriesIsLoadingSubjects[queryKey]));
        }

        /// <summary>
        /// Executes the query on the common CoreDomainContext.
        /// </summary>
        /// <typeparam name="T">The entity Type</typeparam>
        /// <param name="query">The EntityQuery.</param>
        /// <param name="response">The response.</param>
        /// <param name="isLoadingObserver">The is loading observer.</param>
        private void ExecuteQuery<T>(EntityQuery<T> query, Action<IEnumerable<T>> response, IObserver<bool> isLoadingObserver) where T : Entity
        {
            //Let the isLoadingObserver know this query started loading
            isLoadingObserver.OnNext(true);

            _coreDomainContext.Load(query, (callback) =>
            {
                //Call the response action and pass the Entities as an IEnumerable
                response(callback.Entities);

                //Let the isLoadingObserver know this query stopped loading
                isLoadingObserver.OnNext(false);
            }, null);
        }

        /// <summary>
        /// Fires a stream of RoleIds when you should reload.
        /// This happens whenever an Observer is active and loadQuery (or the RoleId) has changed
        /// </summary>
        /// <param name="queryKey">The key of the query to setup the execute observable for</param>
        /// <param name="loadQuery">Optional: A Guid stream. The query will load whenever this has changed. It defaults to the RoleId stream. </param>
        /// <returns>
        /// An Observable that fires the RoleId whenever data should be updated.
        /// </returns>
        private IObservable<Guid> SetupExecuteObservable(object queryKey, IObservable<Guid> loadQuery = null)
        {
            if (loadQuery == null) //default to the RoleId stream
                loadQuery = ContextManager.RoleIdObservable;

            var combinedObservationStateChanges = _observationStateQueries[queryKey].CombinedObservationStateObservable;

            //loadQuery.Subscribe(s => Debug.WriteLine(String.Format("{0} RoleId {1}", queryKey, s)));
            //combinedObservationStateChanges.Subscribe(s => Debug.WriteLine(String.Format("{0} ObservationState {1}", queryKey, s)));

            //See http://stackoverflow.com/questions/7716114/rx-window-join-groupjoin for explanation
            var loadQueryUpdatesWhileInactive = new ReplaySubject<Guid>(1);
            var disposer = new SerialDisposable();

            //the getSwitch will 
            //a) publish loadQueryUpdates if the observation state is active
            //b) track loadQueryUpdates when ObservationState = suspended, then publish the loadQueryUpdate once active
            Func<ObservationState, IObservable<ObservationState>, IObservable<Guid>, IObservable<Guid>> getSwitch =
                (observationStateUpdate, observationStateUpdates, loadQueryUpdates) =>
                {
                    //a) because the observationState is active publish the loadQueryUpdatesWhileInactive & the loadQueryUpdates
                    if (observationStateUpdate == ObservationState.Active)
                    {
                        //Merge the loadQueryUpdatesWhileInactive with the loadQueryUpdates
                        return loadQueryUpdatesWhileInactive.Merge(loadQueryUpdates);
                    }

                    //b) because the ObservationState is suspended:
                    //   setup loadQueryUpdatesWhileInactive to keep track of the loadQueryUpdates while inactive

                    //dispose the last loadQueryUpdatesWhileInactive subscription
                    loadQueryUpdatesWhileInactive.Dispose();

                    //setup loadQueryUpdatesWhileInactive to track 1 (the last) loadQuery update
                    loadQueryUpdatesWhileInactive = new ReplaySubject<Guid>(1);

                    //dispose the temporary subscription to loadQueryUpdates
                    disposer.Disposable =
                        //track loadQueryUpdates until the next observationStateUpdates
                        loadQueryUpdates.TakeUntil(observationStateUpdates)
                        .Subscribe(loadQueryUpdatesWhileInactive); //setup loadQueryUpdatesWhileInactive

                    //return an Empty Guid Observable so that executeQuery does not publish anything 
                    return Observable.Empty<Guid>();
                };

            //Create an Observable that fires the RoleId whenever data should be updated
            //The logic for when it should fire is defined in the getSwitch
            var executeQuery =
                combinedObservationStateChanges.DistinctUntilChanged() //whenever the combineObservationState changes
                    .Publish(observationStateUpdates => loadQuery  //publish the observationStateUpdates
                        .Publish(loadQueryUpdates =>  //publish the loadQueryUpdates
                            observationStateUpdates.Select( //select according to the getSwitch logic
                            observationStateUpdate =>
                                getSwitch(observationStateUpdate, observationStateUpdates, loadQueryUpdates))))
                    .Switch()
                    .Throttle(new TimeSpan(0, 0, 0, 0, 200));//Throttle for .2 second so it does not execute too often

            executeQuery.Subscribe(s => Debug.WriteLine(String.Format("Execute Query:{0} Id:{1}", queryKey, s)));

            return executeQuery;
        }

        #endregion

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

        #region SubmitOperation and SaveDiscardCancel Methods

        #region SubmitOperation

        //The current Submit Operation (null if there is none)
        private SubmitOperation _currentSubmitOperation;

        //ObservableCollection of SubmitOperations waiting for a response from the next (not the current) submitOperation
        private readonly ObservableCollection<Subject<SubmitOperation>> _nextSubmitOperationsQueue
            = new ObservableCollection<Subject<SubmitOperation>>();

        /// <summary>
        /// Queues a SubmitOperation. Returns an observable which publishes the submitOperationCallback.
        /// </summary>
        /// <param name="action">An action to perform on completed.</param>
        public IObservable<SubmitOperation> EnqueueSubmitOperation(Action<SubmitOperation> action)
        {
            var submitOperationObservable = EnqueueSubmitOperation();
            submitOperationObservable.SubscribeOnDispatcher().Subscribe(action);
            return submitOperationObservable;
        }

        /// <summary>
        /// Queues a SubmitOperation. Returns an observable which publishes the submitOperationCallback.
        /// </summary>
        public IObservable<SubmitOperation> EnqueueSubmitOperation()
        {
            var submitOperation = new Subject<SubmitOperation>();
            _nextSubmitOperationsQueue.Add(submitOperation);
            return submitOperation;
        }

        //NOTE: There is no dequeue because that is managed automatically (in the constructor)

        /// <summary>
        /// It dequeues the _nextSubmitOperationsQueue observables and performs a single SubmitOperation.
        /// When the SubmitOperation is completed it publishes the submitOperationCallback on the dequeued observables.
        /// </summary>
        private void PerformNextSubmitOperation()
        {
            //Dequeue the SubmitOperations
            var dequeuedObservables = _nextSubmitOperationsQueue.ToArray();
            _nextSubmitOperationsQueue.Clear();

            //For debugging
            var changes = this.Context.EntityContainer.GetChanges();

            //Perform a submit operation for the dequeued submit operation observables
            _currentSubmitOperation = this.Context.SubmitChanges(
                submitOperationCallback =>
                {
                    //When the submit operation is completed, inform the dequeuedObservables
                    foreach (var submitOperationQueued in dequeuedObservables)
                    {
                        submitOperationQueued.OnNext(submitOperationCallback);
                        submitOperationQueued.OnCompleted();
                    }

                    //Clear the _currentSubmitOperation
                    _currentSubmitOperation = null;
                }, null);
        }

        #endregion

        //Keeps track of the current SaveDiscardCancel. It will be null if there is no promptopened.
        private SaveDiscardCancel _currentSaveDiscardCancel;
        private DateTime _lastSaveDiscardCancelPrompt = new DateTime();
        private readonly List<Tuple<Action, Action, Action, Action, Action, Action>> _saveDiscardCancelActionQueue = new List<Tuple<Action, Action, Action, Action, Action, Action>>();

        /// <summary>
        /// Open a Save Discard Cancel prompt (only one at a time).
        /// </summary>
        /// <param name="beforeSave">The action to perform after the user selects save.</param>
        /// <param name="afterSave">The action to perform after saving.</param>
        /// <param name="beforeDiscard">The action to perform after the user selects discard.</param>
        /// <param name="customDiscardAction">A replacement action for discarding.</param>
        /// <param name="afterDiscard">The action to perform after discarding.</param>
        /// <param name="afterCancel">The action to perform after canceling.</param>
        public void SaveDiscardCancelPrompt(Action beforeSave = null, Action afterSave = null, Action beforeDiscard = null, Action customDiscardAction = null, Action afterDiscard = null, Action afterCancel = null)
        {
            //Add actions to the action queue
            _saveDiscardCancelActionQueue.Add(                                    //Item1     //Item2    //Item3
                        new Tuple<Action, Action, Action, Action, Action, Action>(beforeSave, afterSave, beforeDiscard,
                //Item4              //Item5       //Item6
                                                                                  customDiscardAction, afterDiscard, afterCancel));

            //Show a prompt only if it is not visible and was not called within the last 3 seconds
            if (_currentSaveDiscardCancel != null || DateTime.Now.Subtract(_lastSaveDiscardCancelPrompt) < new TimeSpan(0, 0, 0, 3))
                return;

            //If there are no changes, call Cancelled
            var changes = this.Context.EntityContainer.GetChanges();
            if(changes.Count()==0)
            {
                //Dequeue Actions
                var dequeuedActions = _saveDiscardCancelActionQueue.ToArray();
                _saveDiscardCancelActionQueue.Clear();

                //call each afterCancel action
                foreach (var actionTuple in dequeuedActions.Where(actionTuple => actionTuple.Item6 != null))
                    actionTuple.Item6();
                
                return;
            }

            //Ask the user to: Save or Discard their changes before moving on, or Cancel
            _currentSaveDiscardCancel = new SaveDiscardCancel();

            //Save Selected
            _currentSaveDiscardCancel.SaveButton.Click += (sender, e) =>
            {
                //Dequeue Actions
                var dequeuedActions = _saveDiscardCancelActionQueue.ToArray();
                _saveDiscardCancelActionQueue.Clear();

                //call each beforeSave action
                foreach (var actionTuple in dequeuedActions.Where(actionTuple => actionTuple.Item1 != null))
                    actionTuple.Item1();

                EnqueueSubmitOperation(callback =>
                {
                    //call each afterSave action
                    foreach (var actionTuple in dequeuedActions.Where(actionTuple => actionTuple.Item2 != null))
                        actionTuple.Item2();
                });

                //Close and clear _currentSaveDiscardCancel
                _currentSaveDiscardCancel.Close();
                _currentSaveDiscardCancel = null;
            };

            //Discard selected
            _currentSaveDiscardCancel.DiscardButton.Click += (sender, e) =>
            {
                //Dequeue Actions
                var dequeuedActions = _saveDiscardCancelActionQueue.ToArray();
                _saveDiscardCancelActionQueue.Clear();

                //call each beforeDiscard action
                foreach (var actionTuple in dequeuedActions.Where(actionTuple => actionTuple.Item3 != null))
                    actionTuple.Item3();

                //call each customDiscardAction action
                foreach (var actionTuple in dequeuedActions.Where(actionTuple => actionTuple.Item4 != null))
                    actionTuple.Item4();

                //call RejectChanges if any dequeuedAction does not have a customDiscardAction
                if (dequeuedActions.Any(a => a.Item4 == null))
                    Context.RejectChanges();

                //call each afterDiscard action
                foreach (var actionTuple in dequeuedActions.Where(actionTuple => actionTuple.Item5 != null))
                    actionTuple.Item5();

                //Close and clear _currentSaveDiscardCancel
                _currentSaveDiscardCancel.Close();
                _currentSaveDiscardCancel = null;
            };

            //Cancel selected
            _currentSaveDiscardCancel.CancelButton.Click += (sender, e) =>
            {
                //Dequeue Actions
                var dequeuedActions = _saveDiscardCancelActionQueue.ToArray();
                _saveDiscardCancelActionQueue.Clear();

                //call each afterCancel action
                foreach (var actionTuple in dequeuedActions.Where(actionTuple => actionTuple.Item6 != null))
                    actionTuple.Item6();

                //Close and clear _currentSaveDiscardCancel
                _currentSaveDiscardCancel.Close();
                _currentSaveDiscardCancel = null;
            };

            //Show the Prompt
            _currentSaveDiscardCancel.Show();
            _lastSaveDiscardCancelPrompt = DateTime.Now;
        }

        #endregion
    }
}

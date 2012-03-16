using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.ServiceModel.DomainServices.Client;
using FoundOps.Common.Models;
using FoundOps.Common.NET;
using FoundOps.Core.Models.CoreEntities;
using Microsoft.Windows.Data.DomainServices;

namespace FoundOps.SLClient.Data.Services
{
    /// <summary>
    /// The old query system.
    /// </summary>
    public partial class DataManager
    {
        /// <summary>
        /// Each query of the DataManager.
        /// This is used to setup each query's: a) list of ObservationState observables and b) isLoading observable
        /// </summary>
        public enum Query
        {
            /// <summary>
            /// Loads all of the Blocks
            /// </summary>
            Blocks,
            ///<summary>
            /// Loads all of the BusinessAccounts related to the current RoleId
            ///</summary>
            BusinessAccounts,
            ///<summary>
            /// Loads all of the Clients related to the current RoleId
            ///</summary>
            Clients,
            ///<summary>
            /// Loads all of the Contacts's ClientTitles related to the current RoleId
            ///</summary>
            ClientTitles,
            ///<summary>
            /// Loads all of the Contacts related to the current RoleId
            ///</summary>
            Contacts,
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

        #region Locals

        /// <summary>
        /// Stores each query's CombinedObservationState.
        /// </summary>
        private readonly Dictionary<object, CombinedObservationState> _observationStateQueries = new Dictionary<object, CombinedObservationState>();

        /// <summary>
        /// Stores each query's loading status observable.
        /// </summary>
        private readonly Dictionary<object, BehaviorSubject<bool>> _queriesIsLoadingSubjects = new Dictionary<object, BehaviorSubject<bool>>();

        /// <summary>
        /// Stores each query's current load operation (until it is loaded).
        /// </summary>
        private readonly Dictionary<object, object> _queriesLoadOperations = new Dictionary<object, object>();

        /// <summary>
        /// Stores each query's entityList subject. NOTE: object is a BehaviorSubject'EntityList' (could not figure a way to make it covariant)
        /// It is a BehaviorSubject so that it remembers the last loaded entities
        /// </summary>
        private readonly Dictionary<object, object> _queriesEntityListObservables = new Dictionary<object, object>();

        #endregion

        private void SetupQueries()
        {
            //Setup Blocks query
            SetupQuery(Query.Blocks, roleId => _coreDomainContext.GetBlocksQuery(), _coreDomainContext.Blocks);

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
            SetupQuery(Query.RouteLog, roleId => _coreDomainContext.GetRouteLogForServiceProviderQuery(roleId, Guid.Empty, Guid.Empty), _coreDomainContext.Routes);

            //Setup Services query
            SetupQuery(Query.Services, roleId => _coreDomainContext.GetServicesForRoleQuery(roleId), _coreDomainContext.Services);

            //Setup ServiceTemplates query
            SetupQuery(Query.ServiceTemplates, roleId => _coreDomainContext.GetServiceTemplatesForServiceProviderQuery(roleId), _coreDomainContext.ServiceTemplates);

            //Setup UserAccounts query
            //SetupQuery(Query.UserAccounts, roleId => _coreDomainContext.GetUserAccountsQuery(roleId), _coreDomainContext.Parties);

            //Setup Vehicles query
            SetupQuery(Query.Vehicles, roleId => _coreDomainContext.GetVehiclesForPartyQuery(roleId), _coreDomainContext.Vehicles);

            //Setup VehicleMaintenance query
            SetupQuery(Query.VehicleMaintenance, roleId => _coreDomainContext.GetVehicleMaintenanceLogForPartyQuery(roleId), _coreDomainContext.VehicleMaintenanceLogEntries);
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

            executeObservable.ObserveOnDispatcher().Subscribe(roleId =>
            {
                //Cancel the last query if possible
                if (_queriesLoadOperations.ContainsKey(queryKey))
                {
                    var lastQuery = (LoadOperation<T>)_queriesLoadOperations[queryKey];
                    if (lastQuery.CanCancel) lastQuery.Cancel();
                }

                LoadOperation<T> thisLoadOperation = null;

                //On the response, update the EntityListObservable
                Action<IEnumerable<T>> responseAction =
                    loadedEntities =>
                    {
                        //Only perform the action if the current load operation is the same as the load operation which originated this response
                        if (thisLoadOperation != _queriesLoadOperations[queryKey]) return;

                        ((IObserver<EntityList<T>>)GetEntityListObservable<T>(queryKey)).OnNext(new EntityList<T>(entitySet, loadedEntities));

                        _queriesLoadOperations.Remove(queryKey);
                    };

                //Execute the query, keep track of the load operation
                thisLoadOperation = ExecuteQuery(queryKey, entityQuery(roleId), responseAction, _queriesIsLoadingSubjects[queryKey]);
            });
        }

        /// <summary>
        /// Executes the query on the common CoreDomainContext.
        /// </summary>
        /// <typeparam name="T">The entity Type</typeparam>
        /// <param name="queryKey">The query key.</param>
        /// <param name="query">The EntityQuery.</param>
        /// <param name="response">The response.</param>
        /// <param name="isLoadingObserver">The is loading observer.</param>
        private LoadOperation<T> ExecuteQuery<T>(object queryKey, EntityQuery<T> query, Action<IEnumerable<T>> response, IObserver<bool> isLoadingObserver) where T : Entity
        {
            //Let the isLoadingObserver know this query started loading
            isLoadingObserver.OnNext(true);

            var loadOperation = _coreDomainContext.Load(query, (callback) =>
            {
                //Call the response action and pass the Entities as an IEnumerable
                response(callback.Entities);

                //Let the isLoadingObserver know this query stopped loading
                isLoadingObserver.OnNext(false);
            }, null);

            //Add the load operation to the dictionary (so it can be cancelled if needed)
            _queriesLoadOperations.Add(queryKey, loadOperation);

            return loadOperation;
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
            //Turn off old data manager
            return Observable.Empty<Guid>();

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

        /// <summary>
        /// Loads a single entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="callback">The callback.</param>
        public void LoadSingle<TEntity>(EntityQuery<TEntity> query, Action<TEntity> callback) where TEntity : Entity
        {
            Context.Load(query, loadOperation =>
            {
                if (loadOperation.HasError) return; //TODO Setup error callback
                callback(loadOperation.Entities.FirstOrDefault());
            }, null);
        }

        /// <summary>
        /// Loads a collection.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="callback">The callback.</param>
        public void LoadCollection<TEntity>(EntityQuery<TEntity> query, Action<IEnumerable<TEntity>> callback) where TEntity : Entity
        {
            Context.Load(query, loadOperation =>
            {
                if (loadOperation.HasError) return; //TODO Setup error callback
                callback(loadOperation.Entities);
            }, null);
        }

        #endregion

        #region Query Methods

        /// <summary>
        /// Gets the current party.
        /// </summary>
        /// <param name="roleId">The role id.</param>
        /// <param name="getCurrentPartyCallback">The get current party callback.</param>
        public void GetCurrentParty(Guid roleId, Action<Party> getCurrentPartyCallback)
        {
            LoadSingle(Context.PartyForRoleQuery(roleId), getCurrentPartyCallback);
        }

        /// <summary>
        /// Gets the current user account.
        /// </summary>
        /// <param name="getCurrentUserAccountCallback">The get current user account callback.</param>
        public void GetCurrentUserAccount(Action<UserAccount> getCurrentUserAccountCallback)
        {
            LoadSingle(Context.CurrentUserAccountQuery(), getCurrentUserAccountCallback);
        }

        /// <summary>
        /// Tries to geocode search text.
        /// </summary>
        /// <param name="searchText">The search text.</param>
        /// <param name="geocoderResultsCallback">The geocoder results callback.</param>
        /// <param name="userState">State of the user.</param>
        public void TryGeocode(string searchText, Action<IEnumerable<GeocoderResult>, object> geocoderResultsCallback, object userState = null)
        {
            Context.TryGeocode(searchText, callback => geocoderResultsCallback(callback.Value, userState), userState);
        }

        /// <summary>
        /// Gets the public blocks.
        /// </summary>
        /// <param name="getPublicBlocksCallback">The get public blocks callback.</param>
        public void GetPublicBlocks(Action<ObservableCollection<Block>> getPublicBlocksCallback)
        {
            var query = Context.GetPublicBlocksQuery();
            Context.Load(query, loadOperation =>
            {
                if (!loadOperation.HasError)
                {
                    getPublicBlocksCallback(new ObservableCollection<Block>(loadOperation.Entities));
                }
            }
                , null);
        }

        #endregion
    }
}

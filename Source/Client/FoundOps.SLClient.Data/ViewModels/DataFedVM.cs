using System;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Windows.Threading;
using FoundOps.SLClient.Data.Services;
using ReactiveUI;
using System.Windows;
using System.Reactive.Linq;
using FoundOps.Common.Models;
using System.Reactive.Subjects;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace FoundOps.SLClient.Data.ViewModels
{
    /// <summary>
    /// A VM that requires data to be loaded.
    /// It will only update the data when there are controls that require this viewmodel.
    /// </summary>
    public abstract class DataFedVM : ReactiveObject
    {
        #region Public

        #region IsLoading

        readonly ObservableAsPropertyHelper<bool> _isLoading;
        /// <summary>
        /// 	<c>true</c> if this instance is loading; otherwise, <c>false</c>.
        /// </summary>
        public bool IsLoading
        {
            get { return _isLoading.Value; }
        }

        //The Observable for IsLoadingObservable
        protected readonly Subject<bool> IsLoadingSubject = new Subject<bool>();

        //Holds a subscription to the last IsLoadingObservable set value
        private IDisposable _isLoadingDisposable;
        ///<summary>
        /// An observable of the IsLoading stream
        ///</summary>
        public IObservable<bool> IsLoadingObservable
        {
            get { return IsLoadingSubject.AsObservable(); }
            protected set
            {
                //Dispose and clear the _isLoadingDisposable whenever the value changes
                //that way the IsLoadingObservable is not improperly updated
                if (_isLoadingDisposable != null)
                {
                    _isLoadingDisposable.Dispose();
                    _isLoadingDisposable = null;
                }

                //If the value is not null Subscribe the _isLoadingSubject to it
                if (value != null)
                    _isLoadingDisposable = value.Subscribe(isLoading => IsLoadingSubject.OnNext(isLoading));
            }
        }

        #endregion

        #endregion

        #region Locals

        /// <summary>
        /// The stream of ObservationState notifications
        /// </summary>
        /// <value>
        ///     <c>ObservationState.Active</c> if there are > 0 ControlsThatCurrentlyRequireThisVM; 
        ///     otherwise, <c>ObservationState.Suspended</c>.
        /// </value>
        protected BehaviorSubject<ObservationState> ObservationState { get; private set; }

        ///<summary>
        /// A list of controls that require this VM
        ///</summary>
        protected ObservableCollection<object> ControlsThatCurrentlyRequireThisVM { get; set; }

        private readonly Dispatcher _currentDispatcher = Deployment.Current.Dispatcher;
        /// <summary>
        /// Gets the current dispatcher.
        /// </summary>
        protected Dispatcher CurrentDispatcher { get { return _currentDispatcher; } }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="DataFedVM"/> class.
        /// </summary>
        protected DataFedVM()
        {
            ControlsThatCurrentlyRequireThisVM = new ObservableCollection<object>();

            ObservationState = new BehaviorSubject<ObservationState>(Common.Models.ObservationState.Suspended);

            //Sets up ObservationState to turn Active when there are >0 controls that require this VM 
            //and inactive when there are 0 controls that require this VM
            Observable.FromEventPattern<NotifyCollectionChangedEventArgs>(ControlsThatCurrentlyRequireThisVM, "CollectionChanged")
            .Select(args => ControlsThatCurrentlyRequireThisVM.Count > 0 ? Common.Models.ObservationState.Active : Common.Models.ObservationState.Suspended)
            .DistinctUntilChanged().Subscribe(state => ObservationState.OnNext(state));

            //Setup IsLoading property
            _isLoading = IsLoadingSubject.ToProperty(this, x => x.IsLoading);
        }

        #region Public

        /// <summary>
        /// Whenever the dependentFrameworkElement is Loaded it will require this VM
        /// </summary>
        /// <param name="dependentFrameworkElement">The dependent FrameworkElement</param>
        public void DependentWhenVisible(FrameworkElement dependentFrameworkElement)
        {
            dependentFrameworkElement.Loaded += (s, e) => ControlsThatCurrentlyRequireThisVM.Add(dependentFrameworkElement);
            dependentFrameworkElement.Unloaded += (s, e) => ControlsThatCurrentlyRequireThisVM.Remove(dependentFrameworkElement);
        }

        #endregion

        #region Protected

        protected void RaisePropertyChanged(string propertyName)
        {
            this.raisePropertyChanged(propertyName);
        }

        /// <summary>
        /// Fires a stream of RoleIds when you should reload.
        /// This happens whenever an Observer is active and the RoleId has changed.
        /// </summary>
        /// <returns>
        /// An Observable that fires the RoleId whenever data should be updated.
        /// </returns>
        protected IObservable<Guid> SetupExecuteObservable()
        {
            var loadQuery = Manager.Context.RoleIdObservable;

            var combinedObservationStateChanges = this.ObservationState;

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
                    if (observationStateUpdate == Common.Models.ObservationState.Active)
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

            executeQuery.Subscribe(s => Debug.WriteLine(String.Format("Load data for {0} with RoleId {1}", GetType(), s)));

            return executeQuery;
        }

        #endregion
    }
}

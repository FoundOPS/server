using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace FoundOps.Common.Models
{
    /// <summary>
    /// What state the observer is in.
    /// </summary>
    public enum ObservationState
    {
        /// <summary>
        /// Active is when the Observer is actively listening to the Subject
        /// </summary>
        Active,
        /// <summary>
        /// Suspended is when the Observer is not listening to the Subject
        /// </summary>
        Suspended
    }

    /// <summary>
    /// Combines ObservationState observables into a CombinedObservationStateObservable.
    /// The CombinedObservationStateObservable returns Active when any of the Observables are active or Suspended if all are suspended, and the Time it switched states
    /// </summary>
    public class CombinedObservationState
    {
        private readonly Subject<ObservationState> _combinedObservationStateObservable;

        /// <summary>
        /// A combined ObservationStateObservable. If any of the states are active, returns Active. Otherwise returns Suspended
        /// </summary>
        public IObservable<ObservationState> CombinedObservationStateObservable
        {
            get { return _combinedObservationStateObservable; }
        }

        public CombinedObservationState()
        {
            _combinedObservationStateObservable = new Subject<ObservationState>();
            _combinedObservationStateObservableHelper = new BehaviorSubject<ObservationState>(ObservationState.Suspended);
        }

        private IObservable<ObservationState> _combinedObservationStateObservableHelper;
        private IDisposable _combinedObservationStateObservableHelperSubscription;

        /// <summary>
        /// Adds the observable to the CombinedObservationState
        /// </summary>
        /// <param name="observableToAdd">The ObservationState observable.</param>
        public void AddObservable(IObservable<ObservationState> observableToAdd)
        {
            if (_combinedObservationStateObservableHelperSubscription != null)
                _combinedObservationStateObservableHelperSubscription.Dispose();

            //Combine the last _combinedObservationStateObservableHelper with the new observableToAdd
            _combinedObservationStateObservableHelper = _combinedObservationStateObservableHelper.CombineLatest(observableToAdd,
                           (a, b) =>
                           {
                               //If any of the states are active, return true
                               var state = a == ObservationState.Active || b == ObservationState.Active
                                               ? ObservationState.Active
                                               : ObservationState.Suspended;
                               return state;  //Return the combined state
                           });

            //Whenever the _lastCombinedObservationStateObservable updates, update the _combinedObservationStateObservable
            _combinedObservationStateObservableHelperSubscription = 
                _combinedObservationStateObservableHelper.DistinctUntilChanged().Subscribe(t => _combinedObservationStateObservable.OnNext(t));
        }
    }
}

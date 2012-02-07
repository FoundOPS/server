using System;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FoundOps.Common.Tools;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Common.Silverlight.Controls;
using System.ServiceModel.DomainServices.Client;

namespace FoundOps.SLClient.Data.Services
{
    /// <summary>
    /// This class manages the save and discard operations of the DataManager.
    /// </summary>
    public partial class DataManager
    {
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

            var changes = this.Context.EntityContainer.GetChanges();

            //Cancel changes on generated route tasks
            foreach (var generatedRouteTask in changes.OfType<RouteTask>().Where(rt => rt.GeneratedOnServer))
            {
                //Save the destination so we can re-add the cloned task to it later
                var destinationToSave = generatedRouteTask.RouteDestination;

                //If the task was added to a destination then clone it and save it to the database
                if (destinationToSave != null)
                {
                    //Clone this
                    var clone = generatedRouteTask.Clone(generatedRouteTask.Service != null);

                    //Add the clone to the route destination (if there is one)
                    destinationToSave.RouteTasks.Add(clone);

                    //Remove this from the context (and remove its changes)
                    this.DetachEntities(new[] { generatedRouteTask });
                }
                else //Cancel changes on the generatedRouteTask
                    generatedRouteTask.Reject();
            }

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
            if (!changes.Any())
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

        /// <summary>
        /// Listens to submit operation queue. Throttles submit calls every 1/10 second
        /// </summary>
        private void SetupConsolidatedSubmit()
        {
            //Tracks if a PerformNextSave is hooked up to the currentSubmitOperation.Completed event
            var performNextSaveHooked = false;
            //Whenever a SubmitOperation is enqueued: try to Submit as soon as possible
            this._nextSubmitOperationsQueue.FromCollectionChanged().Where(e => e.EventArgs != null && e.EventArgs.Action == NotifyCollectionChangedAction.Add)
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
    }
}

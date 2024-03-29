﻿using System.Windows;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Common.Silverlight.Controls;
using FoundOps.Common.Silverlight.UI.Controls;
using FoundOps.Common.Tools;
using FoundOps.Common.Tools.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.ServiceModel.DomainServices.Client;

namespace FoundOps.SLClient.Data.Services
{
    /// <summary>
    /// This class manages the save and discard operations of the DataManager.
    /// </summary>
    public partial class DataManager
    {
        #region Public Events

        /// <summary>
        /// The event handler delegate for the ChangesSaved event.
        /// </summary>
        /// <param name="submitOperation"></param>
        public delegate void ChangesSavedHandler(SubmitOperation submitOperation);

        /// <summary>
        /// An event triggered when changes are saved.
        /// </summary>
        public event ChangesSavedHandler ChangesSaved;

        private readonly Subject<bool> _changesSavedSubject = new Subject<bool>();
        /// <summary>
        /// Pushes whenever changes are saved.
        /// </summary>
        public IObservable<bool> ChangesSavedObservable
        {
            get { return _changesSavedSubject.AsObservable(); }
        }

        private readonly Subject<bool> _rejectChangesDueToErrorSubject= new Subject<bool>(); 
        /// <summary>
        /// Pushes whenever rejects changes is called due to an error.
        /// </summary>
        public IObservable<bool> RejectChangesDueToError { get { return _rejectChangesDueToErrorSubject; } } 

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

            var changes = this.DomainContext.GetChangeSet();

            //Perform a submit operation for the dequeued submit operation observables
            _currentSubmitOperation = this.DomainContext.SubmitChanges(
                submitOperationCallback =>
                {
                    //If there is a validation error, let the user know
                    var validationErrors = submitOperationCallback.EntitiesInError.SelectMany(ee => ee.ValidationErrors);
                    if(validationErrors.Any())
                    {
                        var error = validationErrors.ElementAt(0).ErrorMessage;

                        for (var i = 1; i < validationErrors.Count(); i++)
                            error += " and " + validationErrors.ElementAt(i).ErrorMessage;

                        MessageBox.Show(error, "Please Fix Validation Errors", MessageBoxButton.OK);
                        return;
                    }

                    //Log error (if it is not a validation error)
                    if (submitOperationCallback.HasError)
                    {
                        DomainContext.RejectChanges();
                        _rejectChangesDueToErrorSubject.OnNext(true);

                        var error = submitOperationCallback.Error.Message;

                        if(error == "Location Error")
                        {
                            var window = new LocationErrorWindow();

                            window.Show();
                        }
                        else
                        {
                            //Setup the ErrorWindow prompt
                            var errorWindow = new ErrorWindow();

                            //The ErrorWindow is now setup, show it to the user
                            errorWindow.Show();
                        }
                        
                        //track error with analytics
                        Analytics.Track("Server Error");
                        
                        return;
                    }

                    //When the submit operation is completed, inform the dequeuedObservables
                    foreach (var submitOperationQueued in dequeuedObservables)
                    {
                        submitOperationQueued.OnNext(submitOperationCallback);
                        submitOperationQueued.OnCompleted();
                    }

                    if (ChangesSaved != null)
                        ChangesSaved(submitOperationCallback);

                    _changesSavedSubject.OnNext(true);

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
            if (_currentSaveDiscardCancel != null || DateTime.UtcNow.Subtract(_lastSaveDiscardCancelPrompt) < new TimeSpan(0, 0, 0, 3))
                return;

            //If there are no changes, call Cancelled
            var changes = this.DomainContext.GetChangeSet();
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
                    DomainContext.RejectChanges();

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
            _lastSaveDiscardCancelPrompt = DateTime.UtcNow;
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

using System;
using System.Reactive.Disposables;
using System.Windows.Threading;
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

        private readonly Dispatcher _currentDispatcher = System.Windows.Deployment.Current.Dispatcher;
        /// <summary>
        /// Gets the current dispatcher.
        /// </summary>
        protected Dispatcher CurrentDispatcher
        {
            get { return _currentDispatcher; }
        }

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
            .Throttle(new TimeSpan(0, 0, 0, 0, 250)).DistinctUntilChanged()
            .Subscribe(state => ObservationState.OnNext(state));

            //Setup IsLoading property
            _isLoading = IsLoadingObservable.ToProperty(this, x => x.IsLoading);
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

        /// <summary>
        /// Whenever this ViewModel's ObservationState is Active, subscribe.
        /// </summary>
        public IDisposable SubscribeWhenActive<TSource>(IObservable<TSource> source, Action<TSource> onNext)
        {
            //Subscribe observeWhenActive to the source whenever this is Active
            var observeWhenActive = new Subject<TSource>();

            var serialDisposable = new SerialDisposable();
            this.ObservationState.Subscribe(state =>
            {
                //Subscribe observeWhenActive to the source whenever this is Active
                if (state == Common.Models.ObservationState.Active)
                    serialDisposable.Disposable = source.Subscribe(observeWhenActive);
                //Dispose whenever InActive
                else if (!serialDisposable.IsDisposed)
                    serialDisposable.Dispose();
            });

            //Subscribe the onNext to observeWhenActive
            return observeWhenActive.Subscribe(onNext);
        }

        #endregion

        //Protected

        protected void RaisePropertyChanged(string propertyName)
        {
            this.raisePropertyChanged(propertyName);
        }
    }
}

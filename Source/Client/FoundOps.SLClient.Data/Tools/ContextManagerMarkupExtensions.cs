using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using FoundOps.Common.Composite.Tools;
using FoundOps.Common.Silverlight.UI.Controls.InfiniteAccordion;
using FoundOps.Common.Silverlight.UI.Tools.MarkupExtensions;
using FoundOps.SLClient.Data.Services;
using FoundOps.Common.Silverlight.Tools;

// Markup extensions for the ContextManager

namespace FoundOps.SLClient.Data.Tools
{
    /// <summary>
    /// Gets the ContextObservable from the ContextManager for the ContextType, and returns the value whenever it changes. 
    /// </summary>
    public class GetContextObservableValue : UpdatableMarkupExtension<object>
    {
        #region Properties and Variables

        //Public

        private Type _contextType;
        /// <summary>
        /// The ContextType to get the context observable for.
        /// </summary>
        public Type ContextType
        {
            get { return _contextType; }
            set
            {
                _contextType = value;
                SetupContextObservable();
            }
        }

        /// <summary>
        /// The (optional) path to return.
        /// </summary>
        public string Path { get; set; }

        #region Private

        //Clears the LastValue updating subscription whenever the Context changes
        private readonly SerialDisposable _serialDisposable = new SerialDisposable();

        private object _lastValue;
        /// <summary>
        /// The last known value of the observable.
        /// </summary>
        private object LastValue
        {
            get { return _lastValue; }
            set
            {
                if (_lastValue as INotifyPropertyChanged != null)
                    ((INotifyPropertyChanged)_lastValue).PropertyChanged -= LastValuePropertyChanged;

                _lastValue = value;

                if (_lastValue as INotifyPropertyChanged != null)
                    ((INotifyPropertyChanged)_lastValue).PropertyChanged += LastValuePropertyChanged;
            }
        }

        #endregion

        #endregion

        #region Logic

        //Returns the Value after considering the Path
        private object GetValue()
        {
            //Take path into consideration
            return LastValue != null && Path != null ? LastValue.GetProperty<object>(Path) : LastValue;
        }

        //Listen to property changes. If the changed property is the same as Path then UpdateValue
        void LastValuePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (String.IsNullOrEmpty(Path)) return;

            if (Path == e.PropertyName)
                UpdateValue(GetValue());
        }

        protected override object ProvideValueInternal(IServiceProvider serviceProvider)
        {
            return GetValue();
        }

        private void SetupContextObservable()
        {
            if (ContextType == null) return;

            //Set the LastValue to the current Context
            //LastValue = Manager.Context.GetContext<ContextType>
            LastValue = typeof(ContextManager).GetMethod("GetContext")
                .MakeGenericMethod(ContextType).Invoke(Manager.Context, null);

            //Update the last value whenever the ContextChanges
            //Set contextObservable = Manager.Context.GetContextObservable<ContextType>
            var contextObservable =
                (IObservable<object>)typeof(ContextManager).GetMethod("GetContextObservable")
                .MakeGenericMethod(ContextType).Invoke(Manager.Context, null);

            //Update the MarkupExtension's _lastValue whenever the ContextObservable changes
            _serialDisposable.Disposable = contextObservable.ObserveOnDispatcher().Subscribe(val =>
            {
                LastValue = val;
                UpdateValue(GetValue());
            });
        }

        #endregion
    }

    /// <summary>
    /// Gets the CurrentContextProviderObservable, and returns the value anytime it changes
    /// </summary>
    public class GetCurrentContextProvider : UpdatableMarkupExtension<object>
    {
        #region Properties and Variables

        //Public

        /// <summary>
        /// The (optional) path to return.
        /// </summary>
        public string Path { get; set; }

        #region Private

        //Clears the LastValue updating subscription whenever the Context changes
        private readonly SerialDisposable _serialDisposable = new SerialDisposable();

        private IProvideContext _lastValue;
        /// <summary>
        /// The last known value of the observable.
        /// </summary>
        private IProvideContext LastValue
        {
            get { return _lastValue; }
            set
            {
                if (_lastValue as INotifyPropertyChanged != null)
                    _lastValue.PropertyChanged -= LastValuePropertyChanged;

                _lastValue = value;

                if (_lastValue as INotifyPropertyChanged != null)
                    _lastValue.PropertyChanged += LastValuePropertyChanged;
            }
        }

        #endregion

        #endregion

        #region Logic

        //Returns the Value after considering the Path
        private object GetValue()
        {
            //Take path into consideration
            return LastValue != null && Path != null ? LastValue.GetProperty<object>(Path) : LastValue;
        }

        //Listen to property changes. If the changed property is the same as Path then UpdateValue
        void LastValuePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (String.IsNullOrEmpty(Path)) return;

            if (Path == e.PropertyName)
                UpdateValue(GetValue());
        }

        protected override object ProvideValueInternal(IServiceProvider serviceProvider)
        {
            SetupContextObservable();
            return GetValue();
        }

        private void SetupContextObservable()
        {
            //Set the LastValue to the CurrentContextProvider
            LastValue = Manager.Context.CurrentContextProvider;

            //Update the MarkupExtension's _lastValue whenever the CurrentContextProvider changes
            _serialDisposable.Disposable = Manager.Context.CurrentContextProviderObservable.ObserveOnDispatcher().Subscribe(val =>
            {
                LastValue = val;
                UpdateValue(GetValue());
            });
        }

        #endregion
    }
}

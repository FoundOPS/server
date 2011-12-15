using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using FoundOps.Common.Composite.Tools;
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

        //Returns the Value after considering the Path and Converter
        private object GetValue()
        {
            //Take path into consideration
            var val = Path != null ? LastValue.GetProperty<object>(Path) : LastValue;

            //Take converter into consideration
            return Converter != null ? Converter.Convert(val, null, null, null) : val;
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
}

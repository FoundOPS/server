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

        private object _lastValue;
        /// <summary>
        /// Gets or sets the last known value.
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

        //Listen to property changes. If the changed property is the same as Path then UpdateValue
        void LastValuePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (String.IsNullOrEmpty(Path)) return;

            if (Path == e.PropertyName)
                UpdateValue(Value);
        }

        private readonly SerialDisposable _serialDisposable = new SerialDisposable();

        private void SetupContextObservable()
        {
            if (ContextType == null) return;

            //LastValue = Manager.Context.GetContext<ContextType>
            LastValue = typeof(ContextManager).GetMethod("GetContext")
                .MakeGenericMethod(ContextType).Invoke(Manager.Context, null);

            //Set contextObservable = Manager.Context.GetContextObservable<ContextType>
            var contextObservable =
                (IObservable<object>)typeof(ContextManager).GetMethod("GetContextObservable")
                .MakeGenericMethod(ContextType).Invoke(Manager.Context, null);

            //Update the MarkupExtension's _lastValue whenever the ContextObservable changes
            _serialDisposable.Disposable = contextObservable.ObserveOnDispatcher().Subscribe(val =>
            {
                LastValue = val;

                UpdateValue(Value);
            });
        }

        protected override object ProvideValueInternal(IServiceProvider serviceProvider)
        {
            return Value;
        }

        private object Value
        {
            get
            {
                if (LastValue == null)
                    return null;

                return Path != null ? LastValue.GetProperty<object>(Path) : LastValue;
            }
        }
    }
}

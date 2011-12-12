using System;
using System.Reactive.Linq;
using System.Windows;
using System.Reflection;
using System.Reactive.Disposables;
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
        #region ContextManager Dependency Property

        /// <summary>
        /// ContextManager
        /// </summary>
        public ContextManager ContextManager
        {
            get { return (ContextManager)GetValue(ContextManagerProperty); }
            set { SetValue(ContextManagerProperty, value); }
        }

        /// <summary>
        /// ContextManager Dependency Property.
        /// </summary>
        public static readonly DependencyProperty ContextManagerProperty =
            DependencyProperty.Register(
                "ContextManager",
                typeof(ContextManager),
                typeof(GetContextObservableValue),
                new PropertyMetadata(new PropertyChangedCallback(ContextManagerChanged)));

        private static void ContextManagerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as GetContextObservableValue;
            if (c != null)
                c.SetupContextObservable((ContextManager)e.NewValue);
        }

        #endregion

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
                SetupContextObservable(ContextManager);
            }
        }

        private object _lastValue;

        private readonly SerialDisposable _serialDisposable = new SerialDisposable();

        private void SetupContextObservable(ContextManager contextManager)
        {
            if (contextManager == null || ContextType == null) return;

            MethodInfo method = typeof(ContextManager).GetMethod("GetContextObservable");
            MethodInfo generic = method.MakeGenericMethod(ContextType);
            var contextObservable = (IObservable<object>)generic.Invoke(contextManager, null);

            //Update the MarkupExtension's value whenever the ContextObservable changes
            _serialDisposable.Disposable = contextObservable.ObserveOnDispatcher().Subscribe(val =>
            {
                _lastValue = val;
                UpdateValue(val);
            });
        }

        protected override object ProvideValueInternal(IServiceProvider serviceProvider)
        {
            //Convert the value if there is a converter
            if (Converter != null)
                return Converter.Convert(_lastValue, null, null, null);

            return _lastValue;
        }
    }
}

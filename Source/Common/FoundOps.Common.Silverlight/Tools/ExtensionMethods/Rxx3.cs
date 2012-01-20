using System;
using System.Windows;
using System.Reactive.Linq;

namespace FoundOps.Common.Silverlight.Tools.ExtensionMethods
{
    public static class Rxx3
    {
        /// <summary>
        /// Binds a DependencyObject to an observable.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="property">The property.</param>
        /// <param name="source">The source.</param>
        public static IDisposable SetOneWayBinding<T>(this DependencyObject obj, DependencyProperty property, IObservable<T> source)
        {
            return source.SubscribeOnDispatcher().Subscribe(value => obj.SetValue(property, value));
        }
    }
}

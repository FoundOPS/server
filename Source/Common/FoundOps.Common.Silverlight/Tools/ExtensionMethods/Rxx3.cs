﻿using System;
using System.Reactive;
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

        /// <summary>
        /// Runs an action on the dispatcher after the delay.
        /// </summary>
        /// <param name="delay">The delayed timespan</param>
        /// <param name="action">The action to run after the delay.</param>
        public static IDisposable RunDelayed(TimeSpan delay, Action action)
        {
            return Observable.Interval(delay).Take(1).ObserveOnDispatcher().Subscribe(_ => action());
        }

        /// <summary>
        /// An observable that pushes whenever the FrameworkElement is loaded to the page.
        /// </summary>
        /// <param name="element">The framework element.</param>
        public static IObservable<EventPattern<RoutedEventArgs>> LoadedObservable(this FrameworkElement element)
        {
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => element.Loaded += h, h => element.Loaded -= h);
        }
    }
}

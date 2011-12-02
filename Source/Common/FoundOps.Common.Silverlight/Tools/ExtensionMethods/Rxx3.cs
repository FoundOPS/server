using System;
using System.Windows;
using System.Reactive.Linq;
using System.ServiceModel.DomainServices.Client;

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
        /// Returns the entityCollection whenever an entity is added or removed.
        /// </summary>
        /// <param name="entityCollection">The entityCollection.</param>
        /// <returns>True whenever the ObservableCollection changed</returns>
        public static IObservable<EntityCollection<T>> FromCollectionChanged<T>(this EntityCollection<T> entityCollection) where T : Entity
        {
            var entityAdded = Observable.FromEventPattern<EntityCollectionChangedEventArgs<T>>(entityCollection, "EntityAdded");
            var entityRemoved = Observable.FromEventPattern<EntityCollectionChangedEventArgs<T>>(entityCollection, "EntityRemoved");
            return entityAdded.Select(_ => entityCollection).Merge(entityRemoved.Select(_ => entityCollection));
        }

        /// <summary>
        /// Returns the entityCollection when you call this method and whenever an entity is added or removed.
        /// </summary>
        /// <param name="entityCollection">The entityCollection.</param>
        /// <returns>True whenever the ObservableCollection changed</returns>
        public static IObservable<EntityCollection<T>> NowAndWhenCollectionChanged<T>(this EntityCollection<T> entityCollection) where T : Entity
        {
            return entityCollection.FromCollectionChanged().Merge(new[] { entityCollection }.ToObservable());
        }
    }
}

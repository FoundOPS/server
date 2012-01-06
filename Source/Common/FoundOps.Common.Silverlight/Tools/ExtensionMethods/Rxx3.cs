using System;
using System.Windows;
using System.Reactive.Linq;
using FoundOps.Common.Tools;
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
        public static IObservable<EntityCollection<T>> FromEntityCollectionChanged<T>(this EntityCollection<T> entityCollection) where T : Entity
        {
            var entityAdded = Observable.FromEventPattern<EntityCollectionChangedEventArgs<T>>(entityCollection, "EntityAdded");
            var entityRemoved = Observable.FromEventPattern<EntityCollectionChangedEventArgs<T>>(entityCollection, "EntityRemoved");
            return entityAdded.Select(_ => entityCollection).Merge(entityRemoved.Select(_ => entityCollection));
        }

        /// <summary>
        /// Creates an Observable of bool whenever a collection changes.
        /// </summary>
        /// <param name="entityCollection">The entity collection.</param>
        public static IObservable<bool> FromEntityCollectionChangedGeneric<T>(this EntityCollection<T> entityCollection) where T : Entity
        {
            return entityCollection.FromEntityCollectionChanged().AsGeneric();
        }

        /// <summary>
        /// Creates an Observable of bool whenever a collection changes.
        /// </summary>
        /// <param name="entityCollection">The entity collection.</param>
        public static IObservable<bool> FromEntityCollectionChangedGenericAndNow<T>(this EntityCollection<T> entityCollection) where T : Entity
        {
            return entityCollection.FromEntityCollectionChanged().AsGeneric().AndNow();
        }

        /// <summary>
        /// Returns the entityCollection when you call this method and whenever an entity is added or removed.
        /// </summary>
        /// <param name="entityCollection">The entityCollection.</param>
        /// <returns>True whenever the ObservableCollection changed</returns>
        public static IObservable<EntityCollection<T>> NowAndWhenEntityCollectionChanged<T>(this EntityCollection<T> entityCollection) where T : Entity
        {
            return entityCollection.FromEntityCollectionChanged().Merge(new[] { entityCollection }.ToObservable());
        }
    }
}

﻿using System;
using System.Linq;
using System.Windows;
using System.Collections;
using System.Reactive.Linq;
using FoundOps.Common.Tools;
using System.Collections.Specialized;
using FoundOps.Common.Silverlight.Tools;

namespace FoundOps.Common.Silverlight.UI.Tools.MarkupExtensions
{
    public class Except : UpdatableMarkupExtension<object>
    {
        #region Collection Dependency Property

        /// <summary>
        /// Collection
        /// </summary>
        public IEnumerable Collection
        {
            get { return (IEnumerable)GetValue(CollectionProperty); }
            set { SetValue(CollectionProperty, value); }
        }

        /// <summary>
        /// Collection Dependency Property.
        /// </summary>
        public static readonly DependencyProperty CollectionProperty =
            DependencyProperty.Register(
                "Collection",
                typeof(IEnumerable),
                typeof(Except),
                new PropertyMetadata(new PropertyChangedCallback(CollectionChanged)));

        private static void CollectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as Except;
            if (c != null)
                c.UpdateValue(c.ExceptHelper(e.NewValue as IEnumerable, c.ExceptCollection));
        }

        #endregion

        #region ExceptCollection Dependency Property

        /// <summary>
        /// ExceptCollection
        /// </summary>
        public IEnumerable ExceptCollection
        {
            get { return (IEnumerable)GetValue(ExceptCollectionProperty); }
            set { SetValue(ExceptCollectionProperty, value); }
        }

        /// <summary>
        /// ExceptCollection Dependency Property.
        /// </summary>
        public static readonly DependencyProperty ExceptCollectionProperty =
            DependencyProperty.Register(
                "ExceptCollection",
                typeof(IEnumerable),
                typeof(Except),
                new PropertyMetadata(new PropertyChangedCallback(ExceptCollectionChanged)));

        private static void ExceptCollectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as Except;
            if (c != null)
                c.UpdateValue(c.ExceptHelper(c.Collection, e.NewValue as IEnumerable));
        }

        #endregion

        protected override object ProvideValueInternal(IServiceProvider serviceProvider)
        {
            return ExceptHelper(Collection, ExceptCollection);
        }

        private IEnumerable ExceptHelper(IEnumerable collection, IEnumerable exceptCollection)
        {
            if (collection == null) return null;

            #region Listen for Collection changes

            //Listen for collection changes and update the value
            if (collection as INotifyCollectionChanged != null)
            {
                var collectionChanged = ((INotifyCollectionChanged)collection).FromCollectionChangedEventGeneric();
                collectionChanged.ObserveOnDispatcher().Subscribe(_ => UpdateValue(ExceptHelper(collection, exceptCollection)));
            }

            #endregion

            //If there is no exceptCollection, only return the collection
            if (exceptCollection == null) return collection;

            #region Listen for ExceptCollection changes

            //Listen for exceptCollection changes and update the value
            if (exceptCollection as INotifyCollectionChanged != null)
            {
                var exceptCollectionChanged = ((INotifyCollectionChanged)exceptCollection).FromCollectionChangedEventGeneric();
                exceptCollectionChanged.ObserveOnDispatcher().Subscribe(_ => UpdateValue(ExceptHelper(collection, exceptCollection)));
            }
            #endregion

            return collection.Cast<object>().Except(exceptCollection.Cast<object>());
        }
    }
}

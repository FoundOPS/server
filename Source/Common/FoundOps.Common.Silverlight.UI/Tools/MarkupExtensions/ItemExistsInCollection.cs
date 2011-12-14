using System;
using System.Linq;
using System.Windows;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace FoundOps.Common.Silverlight.Tools
{
    public class ItemExistsInCollection : UpdatableMarkupExtension<object>
    {
        #region Item Dependency Property

        /// <summary>
        /// Item
        /// </summary>
        public object Item
        {
            get { return (object)GetValue(ItemProperty); }
            set { SetValue(ItemProperty, value); }
        }

        /// <summary>
        /// Item Dependency Property.
        /// </summary>
        public static readonly DependencyProperty ItemProperty =
            DependencyProperty.Register(
                "Item",
                typeof(object),
                typeof(ItemExistsInCollection),
                new PropertyMetadata(new PropertyChangedCallback(ItemChanged)));

        private static void ItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ItemExistsInCollection c = d as ItemExistsInCollection;
            if (c != null)
            {
                if (e.NewValue != null)
                    c.UpdateValue(ItemExistsInCollectionHelper(e.NewValue, c.Collection));
            }
        }

        #endregion

        #region Collection Dependency Property

        /// <summary>
        /// Collection
        /// </summary>
        public IEnumerable<Object> Collection
        {
            get { return (IEnumerable<Object>)GetValue(CollectionProperty); }
            set { SetValue(CollectionProperty, value); }
        }

        /// <summary>
        /// Collection Dependency Property.
        /// </summary>
        public static readonly DependencyProperty CollectionProperty =
            DependencyProperty.Register(
                "Collection",
                typeof(IEnumerable<Object>),
                typeof(ItemExistsInCollection),
                new PropertyMetadata(new PropertyChangedCallback(CollectionChanged)));

        private static void CollectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as ItemExistsInCollection;
            if (c != null)
            {
                Common.Tools.CollectionExtensions.HookupToCollectionChanged((INotifyCollectionChanged)e.OldValue,
                    (INotifyCollectionChanged)e.NewValue, c.ItemExistsInCollectionCollectionChanged);

                if (e.NewValue != null)
                    c.UpdateValue(ItemExistsInCollectionHelper(c.Item, (IEnumerable<object>)e.NewValue));
            }
        }

        private void ItemExistsInCollectionCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateValue(ItemExistsInCollectionHelper(Item, Collection));
        }

        #endregion

        private static bool ItemExistsInCollectionHelper(Object item, IEnumerable<Object> collection)
        {
            if (item == null || collection == null) return false;

            var itemExists = collection.Any(i =>
            {
                if (i is string && item is string)
                    return String.CompareOrdinal((string)i, (string)item) == 0;

                return i == item;
            });

            return itemExists;
        }

        protected override object ProvideValueInternal(IServiceProvider serviceProvider)
        {
            return ItemExistsInCollectionHelper(Item, Collection);
        }
    }
}

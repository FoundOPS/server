using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Interactivity;
using Telerik.Windows.Controls;

namespace FoundOps.Common.Silverlight.UI.Interactivity.Behaviors
{
    public class BindingSelectedItems : Behavior<RadTreeView>
    {
        private RadTreeView Tree
        {
            get
            {
                return AssociatedObject;
            }
        }

        public INotifyCollectionChanged SelectedItems
        {
            get { return (INotifyCollectionChanged)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedItemsProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register("SelectedItems", typeof(INotifyCollectionChanged), typeof(BindingSelectedItems), new PropertyMetadata(OnSelectedItemsPropertyChanged));


        private static void OnSelectedItemsPropertyChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            var collection = args.NewValue as INotifyCollectionChanged;
            if (collection != null)
            {
                collection.CollectionChanged += ((BindingSelectedItems)target).ContextSelectedItemsCollectionChanged;
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            Tree.SelectedItems.CollectionChanged += GridSelectedItemsCollectionChanged;
        }

        void ContextSelectedItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UnsubscribeFromEvents();

            Transfer(SelectedItems as IList, Tree.SelectedItems);

            SubscribeToEvents();
        }

        void GridSelectedItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UnsubscribeFromEvents();

            Transfer(Tree.SelectedItems, SelectedItems as IList);

            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            Tree.SelectedItems.CollectionChanged += GridSelectedItemsCollectionChanged;

            if (SelectedItems != null)
            {
                SelectedItems.CollectionChanged += ContextSelectedItemsCollectionChanged;
            }
        }

        private void UnsubscribeFromEvents()
        {
            Tree.SelectedItems.CollectionChanged -= GridSelectedItemsCollectionChanged;

            if (SelectedItems != null)
            {
                SelectedItems.CollectionChanged -= ContextSelectedItemsCollectionChanged;
            }
        }

        public static void Transfer(IList source, IList target)
        {
            if (source == null || target == null)
                return;

            target.Clear();

            foreach (var o in source)
            {
                target.Add(o);
            }
        }
    }
}

using System;
using System.Windows;
using FoundOps.Common.Silverlight.UI.Tools.ExtensionMethods;

namespace FoundOps.Common.Silverlight.UI.Tools.MarkupExtensions
{
    /// <summary>
    /// Checks if the target framework element's datacontext is equal to the Item
    /// </summary>
    public class DataContextEquals : UpdatableMarkupExtension<object>
    {
        #region Item Dependency Property

        /// <summary>
        /// The Item to check agains the datacontext
        /// </summary>
        public object Item
        {
            get { return GetValue(ItemProperty); }
            set { SetValue(ItemProperty, value); }
        }

        /// <summary>
        /// ItemTwo Dependency Property.
        /// </summary>
        public static readonly DependencyProperty ItemProperty =
            DependencyProperty.Register(
                "Item",
                typeof(object),
                typeof(DataContextEquals),
                new PropertyMetadata(ItemTwoChanged));

        private static void ItemTwoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as DataContextEquals;
            if (c != null)
            {
                c.UpdateValue(c.EqualsDataContextHelper(e.NewValue));
            }
        }

        #endregion

        protected override object ProvideValueInternal(IServiceProvider serviceProvider)
        {
            //Whenever the DataContext changes, update the value
            ((FrameworkElement)TargetObject).RegisterForNotification("DataContext", 
                (_, __) => UpdateValue(EqualsDataContextHelper(Item)));

            return EqualsDataContextHelper(Item);
        }

        private bool EqualsDataContextHelper(object item)
        {
            if (this.TargetObject == null) return false;

            return ((FrameworkElement)TargetObject).DataContext == item;
        }
    }
}

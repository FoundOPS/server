using System;
using System.Windows;
using FoundOps.Common.Silverlight.Tools;

namespace FoundOps.Common.Silverlight.UI.Tools.MarkupExtensions
{
    //Returns if ItemOne and ItemTwo are equal
    public class AreEqual : UpdatableMarkupExtension<object>
    {
        #region ItemOne Dependency Property

        /// <summary>
        /// ItemOne
        /// </summary>
        public object ItemOne
        {
            get { return (object)GetValue(ItemOneProperty); }
            set { SetValue(ItemOneProperty, value); }
        }

        /// <summary>
        /// ItemOne Dependency Property.
        /// </summary>
        public static readonly DependencyProperty ItemOneProperty =
            DependencyProperty.Register(
                "ItemOne",
                typeof(object),
                typeof(AreEqual),
                new PropertyMetadata(ItemOneChanged));

        private static void ItemOneChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as AreEqual;
            if (c != null)
            {
                c.UpdateValue(c.AreEqualHelper(e.NewValue, c.ItemTwo));
            }
        }

        #endregion

        #region ItemTwo Dependency Property

        /// <summary>
        /// ItemTwo
        /// </summary>
        public object ItemTwo
        {
            get { return (object)GetValue(ItemTwoProperty); }
            set { SetValue(ItemTwoProperty, value); }
        }

        /// <summary>
        /// ItemTwo Dependency Property.
        /// </summary>
        public static readonly DependencyProperty ItemTwoProperty =
            DependencyProperty.Register(
                "ItemTwo",
                typeof(object),
                typeof(AreEqual),
                new PropertyMetadata(ItemTwoChanged));

        private static void ItemTwoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as AreEqual;
            if (c != null)
            {
                c.UpdateValue(c.AreEqualHelper(c.ItemOne, e.NewValue));
            }
        }

        #endregion

        protected override object ProvideValueInternal(IServiceProvider serviceProvider)
        {
            return AreEqualHelper(ItemOne, ItemTwo);
        }

        private object AreEqualHelper(object itemOne, object itemTwo)
        {
            return itemOne == itemTwo;
        }
    }
}

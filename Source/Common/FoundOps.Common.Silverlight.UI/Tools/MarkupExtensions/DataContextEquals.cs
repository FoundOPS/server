using System;
using System.Windows;
using System.Windows.Markup;
using FoundOps.Common.Silverlight.Tools;
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
            get { return (object)GetValue(ItemProperty); }
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

        private FrameworkElement _targetFrameworkElement;
        private FrameworkElement TargetFrameworkElement
        {
            get { return _targetFrameworkElement; }
            set
            {
                _targetFrameworkElement = value;
                if (_targetFrameworkElement == null) return;

                TargetFrameworkElement.RegisterForNotification("DataContext", (a, e) => UpdateValue(EqualsDataContextHelper(Item)));
            }
        }

        protected override object ProvideValueInternal(IServiceProvider serviceProvider)
        {
            var ipvt = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));
            TargetFrameworkElement = ipvt.TargetObject as FrameworkElement;

            return EqualsDataContextHelper(Item);
        }

        private bool EqualsDataContextHelper(object item)
        {
            if (TargetFrameworkElement == null) return false;

            return TargetFrameworkElement.DataContext == item;
        }
    }
}

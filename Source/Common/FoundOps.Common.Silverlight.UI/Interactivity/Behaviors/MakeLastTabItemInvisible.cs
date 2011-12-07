using System.Windows;
using Telerik.Windows.Controls;
using System.Windows.Interactivity;

namespace FoundOps.Common.Silverlight.Interactivity.Behaviors
{
    public class MakeLastTabItemInvisible : Behavior<FrameworkElement>
    {
        #region ParentRadTabControl Dependency Property

        /// <summary>
        /// ParentRadTabControl
        /// </summary>
        public RadTabControl ParentRadTabControl
        {
            get { return (RadTabControl)GetValue(ParentRadTabControlProperty); }
            set { SetValue(ParentRadTabControlProperty, value); }
        }

        /// <summary>
        /// ParentRadTabControl Dependency Property.
        /// </summary>
        public static readonly DependencyProperty ParentRadTabControlProperty =
            DependencyProperty.Register(
                "ParentRadTabControl",
                typeof(RadTabControl),
                typeof(MakeLastTabItemInvisible),
                new PropertyMetadata(new PropertyChangedCallback(ParentRadTabControlChanged)));

        private static void ParentRadTabControlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MakeLastTabItemInvisible c = d as MakeLastTabItemInvisible;
            if (c != null)
            {
                if (e.NewValue == null) return;

                var parentTabControl = (RadTabControl)e.NewValue;
                if (parentTabControl.Items.Count <= 0) return;

                var lastTabItem = parentTabControl.ItemContainerGenerator.ContainerFromIndex(parentTabControl.Items.Count - 1) as RadTabItem;

                if (lastTabItem != null) lastTabItem.Visibility = Visibility.Collapsed;
            }
        }

        #endregion
    }
}

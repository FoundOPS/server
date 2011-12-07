using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;

namespace FoundOps.Common.Silverlight.UI.Tools.ExtensionMethods
{
    public static class FrameworkElementExtensions
    {
        /// <summary>
        /// Registers for a notification whenever a DependencyProperty changes.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="dependencyPropertyName">Name of the dependency property.</param>
        /// <param name="callback">The callback.</param>
        public static void RegisterForNotification(this FrameworkElement element, string dependencyPropertyName, PropertyChangedCallback callback)
        {
            //Bind to a depedency property
            var b = new Binding(dependencyPropertyName) { Source = element };
            var prop = DependencyProperty.RegisterAttached(
                "ListenAttached" + dependencyPropertyName,
                typeof(object),
                typeof(UserControl),
                new PropertyMetadata(callback));

            element.SetBinding(prop, b);
        }
    }
}

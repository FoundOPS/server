using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;

namespace FoundOps.Common.Silverlight.UI.Tools.ExtensionMethods
{
    public static class FrameworkElementExtensions
    {
        //Used to prevent multiple listeners from having the same DependencyProperty name
        private static int _uniqueListenerNumber;

        /// <summary>
        /// Registers for a notification whenever a DependencyProperty changes.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="dependencyPropertyName">Name of the dependency property.</param>
        /// <param name="callback">The callback.</param>
        public static void RegisterForNotification(this FrameworkElement element, string dependencyPropertyName,
                                                   PropertyChangedCallback callback)
        {
            //Bind to a dependency property
            var b = new Binding(dependencyPropertyName) {Source = element};

            var prop = DependencyProperty.RegisterAttached(
                "ListenAttached" + dependencyPropertyName + _uniqueListenerNumber,
                typeof (object),
                typeof (UserControl),
                new PropertyMetadata(callback));

            element.SetBinding(prop, b);

            //Increment the _uniqueListenerNumber
            _uniqueListenerNumber++;
        }

        /// <summary>
        /// Gets the dependency property of a type by it's name.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static DependencyProperty GetDependencyProperty(this Type type, string name)
        {
            var fieldInfo = type.GetField(name, BindingFlags.Public | BindingFlags.Static);
            return (fieldInfo != null) ? (DependencyProperty)fieldInfo.GetValue(null) : null;
        }

        /// <summary>
        /// Gets the dependency properties of a type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static IEnumerable<FieldInfo> GetDependencyProperties(this Type type)
        {
            var properties = type.GetFields(BindingFlags.Static | BindingFlags.Public)
                             .Where(f => f.FieldType == typeof(DependencyProperty));
            if (type.BaseType != null)
                properties = properties.Union(GetDependencyProperties(type.BaseType));
            return properties;
        }
    }
}
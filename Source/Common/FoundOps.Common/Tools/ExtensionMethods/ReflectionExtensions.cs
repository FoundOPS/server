using System;
using System.Reflection;

namespace FoundOps.Common.Composite.Tools
{
    public static class ReflectionExtensions
    {
        /// <summary>
        /// Gets the property via Reflection.
        /// Ex. thisCar.GetProperty{string}("Name") would return thisCar.Name
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="obj">The obj this extension method is operating on.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The requested property</returns>
        public static T GetProperty<T>(this object obj, string propertyName)
        {
            var pi = obj.GetType().GetProperty(propertyName);

            return (T) pi.GetValue(obj, null);
        }

        /// <summary>
        /// Sets the property via Reflection.
        /// Ex. thisCar.SetProperty("Name", "Jonathan") would set thisCar.Name = "Jonathan"
        /// </summary>
        /// <param name="obj">The obj this extension method is operating on.</param>
        /// <param name="propertyName">Name of the property to set.</param>
        /// <param name="propertyValue">The value to set.</param>
        public static void SetProperty(this object obj, string propertyName, object propertyValue)
        {
            obj.GetType().InvokeMember(propertyName,
                                       BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty,
                                       Type.DefaultBinder, obj, new[] {propertyValue});
        }
    }
}

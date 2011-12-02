using System.Collections.Specialized;

namespace FoundOps.Common.Tools
{
    public static class CollectionExtensions
    {
        /// <summary>
        /// Hookups the handler to the collection changed event. When the value changes it removes the handler.
        /// </summary>
        /// <param name="oldValue">The old collection.</param>
        /// <param name="newValue">The new collection.</param>
        /// <param name="handler">The handler to hookup.</param>
        public static void HookupToCollectionChanged(INotifyCollectionChanged oldValue, INotifyCollectionChanged newValue,
            NotifyCollectionChangedEventHandler handler)
        {
            if (oldValue != null)
                oldValue.CollectionChanged -= handler;

            if (newValue != null)
                newValue.CollectionChanged += handler;
        }
    }
}

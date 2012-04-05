using FoundOps.Core.Models.CoreEntities.M2M4Ria;
using System.Linq;

namespace FoundOps.SLClient.Data.Extensions
{
    /// <summary>
    /// Extensions for IEntityCollection
    /// </summary>
    public static class EntityCollectionExtensions
    {
        /// <summary>
        /// Clears the specified collection to clear.
        /// </summary>
        /// <param name="collectionToClear">The collection to clear.</param>
        public static void Clear<T>(this IEntityCollection<T> collectionToClear)
        {
            var valuesToRemove = collectionToClear.ToArray();
            foreach (var value in valuesToRemove)
                collectionToClear.Remove(value);
        }
    }
}

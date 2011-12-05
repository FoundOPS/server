using System.Linq;
using FoundOps.Core.Models.CoreEntities.M2M4Ria;

namespace FoundOps.Core.Context.Extensions
{
    public static class EntityCollectionExtensions
    {
        public static void Clear<T>(this IEntityCollection<T> collectionToClear)
        {
            var valuesToRemove = collectionToClear.ToArray();
            foreach (var value in valuesToRemove)
                collectionToClear.Remove(value);
        }
    }
}

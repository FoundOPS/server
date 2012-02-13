using System.Threading.Tasks;
using System.Collections.Generic;
using System.ServiceModel.DomainServices.Client;

namespace FoundOps.Common.Silverlight.Tools.ExtensionMethods
{
    public static class DomainContextExtensions
    {
        public static Task<IEnumerable<T>> LoadAsync<T>(this DomainContext context,
            EntityQuery<T> query) where T : Entity
        {
            var result = new TaskCompletionSource<IEnumerable<T>>();

            context.Load(query, lo => result.TrySetResult(lo.Entities), null);

            return result.Task;
        }

        public static Task<int> CountAsync<T>(this DomainContext context, EntityQuery<T> query) where T : Entity
        {
            var result = new TaskCompletionSource<int>();

            context.Load(query.Take(0).IncludeTotalCount(true), lo => { result.TrySetResult(lo.TotalEntityCount); }, null);

            return result.Task;
        }
    }
}

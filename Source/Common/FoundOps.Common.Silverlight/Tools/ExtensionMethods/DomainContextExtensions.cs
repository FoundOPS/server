using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ServiceModel.DomainServices.Client;

namespace FoundOps.Common.Silverlight.Tools.ExtensionMethods
{
    /// <summary>
    /// Extension methods for the DomainContext.
    /// </summary>
    public static class DomainContextExtensions
    {
        /// <summary>
        /// Loads the query (asynchronous).
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="query">The query.</param>
        /// <param name="cancel">A cancel observable. Push once to cancel this.</param>
        public static Task<IEnumerable<T>> LoadAsync<T>(this DomainContext context, EntityQuery<T> query, IObservable<bool> cancel = null) where T : Entity
        {
            var result = new TaskCompletionSource<IEnumerable<T>>();

            var entityQuery = context.Load(query, lo =>
            {
                if (!lo.IsCanceled)
                    result.TrySetResult(lo.Entities);
                else
                    result.SetCanceled();
            }, null);

            if (cancel != null)
            {
                IDisposable cancelSubscription = null;
                cancelSubscription = cancel.Subscribe(_ =>
                {
                    if (entityQuery.CanCancel)
                        entityQuery.Cancel();
                    cancelSubscription.Dispose();
                });
            }

            return result.Task;
        }

        /// <summary>
        /// Counts the query (asynchronous).
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="query">The query.</param>
        /// <param name="cancel">A cancel observable. Push once to cancel this.</param>
        public static Task<int> CountAsync<T>(this DomainContext context, EntityQuery<T> query, IObservable<bool> cancel = null) where T : Entity
        {
            var result = new TaskCompletionSource<int>();

            query = query.Take(0);
            query.IncludeTotalCount = true;

            var entityQuery = context.Load(query, lo =>
            {
                if (!lo.IsCanceled)
                    result.TrySetResult(lo.TotalEntityCount);
                else
                    result.SetCanceled();
            }, null);

            if (cancel != null)
            {
                IDisposable cancelSubscription = null;
                cancelSubscription = cancel.Subscribe(_ =>
                {
                    if (entityQuery.CanCancel)
                        entityQuery.Cancel();
                    cancelSubscription.Dispose();
                });
            }

            return result.Task;
        }
    }
}
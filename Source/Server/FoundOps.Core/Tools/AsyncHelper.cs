using System;
using System.Threading.Tasks;

namespace FoundOps.Core.Tools
{
    public static class AsyncHelper
    {
        /// <summary>
        /// Converts a syncronous function to run asynchronously as a task.
        /// </summary>
        /// <typeparam name="T">The type to return</typeparam>
        /// <param name="action">The action to convert to an async task.</param>
        public static Task<T> RunAsync<T>(Func<T> action)
        {
            var result = new TaskCompletionSource<T>();
            result.TrySetResult(action());
            return result.Task;
        }
    }
}

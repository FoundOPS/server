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
        /// <param name="function">The function to convert to an async task.</param>
        public static Task<T> RunAsync<T>(Func<T> function)
        {
            var source = new TaskCompletionSource<T>();

            //Create the async callback
            AsyncCallback cb = new AsyncCallback((asyncResult) =>
            {
                try
                {
                    var returnedValue = function.EndInvoke(asyncResult);
                    source.TrySetResult(returnedValue);
                }
                catch (Exception e)
                {
                    source.SetException(e);
                }
            });

            //Call the function asynchrously
            function.BeginInvoke(cb, null);

            return source.Task;
        }

        ///// <summary>
        ///// Converts a syncronous function to run asynchronously as a task. TODO: Replace above when using async ctp.
        ///// </summary>
        ///// <typeparam name="T">The type to return</typeparam>
        ///// <param name="function">The function to convert to an async task.</param>
        //public static async Task<T> RunAsync<T>(Func<T> function)
        //{
        //  return function();
        //}
    }
}

using System;
using System.Windows;
using System.Windows.Navigation;

namespace FoundOps.Common.Silverlight.Loader
{
    /// <summary>
    /// A Utility class that simplifies creation of an INavigationContentLoader.
    /// </summary>
    public abstract class ContentLoaderBase : DependencyObject, INavigationContentLoader
    {
        /// <summary>
        /// Creates an instance of a LoaderBase that will be used to handle loading.
        /// </summary>
        /// <returns>An instance of a LoaderBase.</returns>
        protected abstract LoaderBase CreateLoader();

        private LoaderBase CreateLoaderPrivate()
        {
            LoaderBase loader = CreateLoader();
            loader.ContentLoader = this;
            return loader;
        }

        internal void Complete(IAsyncResult result)
        {
            lock (((ContentLoaderBaseAsyncResult)result).Lock)
            {
                if (Dispatcher.CheckAccess())
                {
                    ((ContentLoaderBaseAsyncResult)result).Callback(result);
                }
                else
                {
                    Dispatcher.BeginInvoke(() => ((ContentLoaderBaseAsyncResult)result).Callback(result));
                }
            }
        }

        #region INavigationContentLoader Members

        /// <summary>
        /// Begins asynchronous loading of the content for the specified target URI.
        /// </summary>
        /// <param name="targetUri">The URI to load content for.</param>
        /// <param name="currentUri">The URI that is currently loaded.</param>
        /// <param name="userCallback">The method to call when the content finishes loading.</param>
        /// <param name="asyncState">An object for storing custom state information.</param>
        /// <returns>An object that stores information about the asynchronous operation.</returns>
        public IAsyncResult BeginLoad(Uri targetUri, Uri currentUri, AsyncCallback userCallback, object asyncState)
        {
            var loader = CreateLoaderPrivate();
            var result = new ContentLoaderBaseAsyncResult(asyncState, loader, userCallback) {BeginLoadCompleted = false};
            loader.Result = result;
            lock (result.Lock)
            {
                loader.Load(targetUri, currentUri);
                result.BeginLoadCompleted = true;
                return result;
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the specified URI can be loaded.
        /// </summary>
        /// <param name="targetUri">The URI to test.</param>
        /// <param name="currentUri">The URI that is currently loaded.</param>
        /// <returns>true if the URI can be loaded; otherwise, false.</returns>
        public virtual bool CanLoad(Uri targetUri, Uri currentUri)
        {
            return true;
        }

        /// <summary>
        /// Attempts to cancel content loading for the specified asynchronous operation.
        /// </summary>
        /// <param name="asyncResult">An object that identifies the asynchronous operation to cancel.</param>
        public void CancelLoad(IAsyncResult asyncResult)
        {
            if(asyncResult==null)
                return;

            ((ContentLoaderBaseAsyncResult)asyncResult).Loader.CancelInternal();
        }

        /// <summary>
        /// Completes the asynchronous content loading operation.
        /// </summary>
        /// <param name="asyncResult">An object that identifies the asynchronous operation.</param>
        /// <returns>An object that represents the result of the asynchronous content loading operation.</returns>
        public LoadResult EndLoad(IAsyncResult asyncResult)
        {
            var result = (ContentLoaderBaseAsyncResult)asyncResult;
            if (result.Error != null)
                throw result.Error;
            return result.Page != null ? new LoadResult(result.Page) : new LoadResult(result.RedirectUri);
        }

        #endregion
    }
}

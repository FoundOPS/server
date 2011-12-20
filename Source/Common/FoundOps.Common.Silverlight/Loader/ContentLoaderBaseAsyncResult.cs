using System;
using System.Threading;

namespace FoundOps.Common.Silverlight.Loader
{
    internal class ContentLoaderBaseAsyncResult : IAsyncResult
    {
        public ContentLoaderBaseAsyncResult(object asyncState, LoaderBase loader, AsyncCallback callback)
        {
            this.AsyncState = asyncState;
            this.Loader = loader;
            this.Lock = new object();
            this.Callback = callback;
            AsyncWaitHandle = new AutoResetEvent(false);
        }

        internal LoaderBase Loader { get; private set; }
        internal AsyncCallback Callback { get; private set; }
        internal Exception Error { get; set; }
        internal object Page { get; set; }
        internal Uri RedirectUri { get; set; }
        internal object Lock { get; private set; }
        internal bool BeginLoadCompleted { get; set; }

        #region IAsyncResult Members

        public object AsyncState
        {
            get;
            private set;
        }

        public System.Threading.WaitHandle AsyncWaitHandle
        {
            get;
            private set;
        }

        public bool CompletedSynchronously
        {
            get;
            private set;
        }

        public bool IsCompleted
        {
            get;
            private set;
        }

        public void Complete()
        {
            this.CompletedSynchronously = !BeginLoadCompleted;
            this.IsCompleted = true;
            (this.AsyncWaitHandle as AutoResetEvent).Set();
        }

        #endregion
    }
}

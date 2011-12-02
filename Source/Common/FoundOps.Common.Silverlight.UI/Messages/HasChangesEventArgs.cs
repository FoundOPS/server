using System;

namespace FoundOps.Common.Silverlight.MVVM.Messages
{
    public class HasChangesEventArgs : EventArgs
    {
        public bool HasChanges { get; set; }
    }
}
namespace FoundOps.Common.Silverlight.MVVM.Messages
{
    public class IsBusyMessage
    {
        public bool IsBusy { get; private set; }

        public IsBusyMessage(bool isBusy)
        {
            IsBusy = isBusy;
        }
    }
}
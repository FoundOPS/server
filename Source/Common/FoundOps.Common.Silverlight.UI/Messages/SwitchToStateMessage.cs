namespace FoundOps.Common.Silverlight.MVVM.Messages
{
    public class SwitchToStateMessage
    {
        public string StateNameToSwitchTo { get; private set; }

        public SwitchToStateMessage(string stateNameToSwitchTo)
        {
            StateNameToSwitchTo = stateNameToSwitchTo;
        }
    }
}

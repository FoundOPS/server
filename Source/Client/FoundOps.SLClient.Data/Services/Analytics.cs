using FoundOps.Common.Silverlight.UI.Tools;

namespace FoundOps.SLClient.Data.Services
{
    /// <summary>
    /// Manages analytics.
    /// </summary>
    public static class Analytics
    {
        /// <summary>
        /// Analytic to track when the layout changes
        /// </summary>
        public static void DispatcherLayoutChanged()
        {
            TrackEventAction.Track("Dispatcher", "LayoutChanged", Manager.Context.OwnerAccount.DisplayName, 1);
        }
    }
}
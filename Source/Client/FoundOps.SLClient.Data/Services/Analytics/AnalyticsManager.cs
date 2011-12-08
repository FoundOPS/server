using System.Windows;
using System.ComponentModel.Composition;
using FoundOps.Common.Silverlight.UI.Tools;

namespace FoundOps.SLClient.Data.Services.Analytics
{
    /// <summary>
    /// Manages analytics.
    /// </summary>
    [Export]
    public class AnalyticsManager
    {
        public readonly ContextManager _contextManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="Analytics"/> class.
        /// </summary>
        /// <param name="contextManager">The context manager.</param>
        [ImportingConstructor]
        public AnalyticsManager(ContextManager contextManager)
        {
            _contextManager = contextManager;
        }

        /// <summary>
        /// Analytic to track when the layout changes
        /// </summary>
        public void DispatcherLayoutChanged()
        {
            TrackEventAction.Track("Dispatcher", "LayoutChanged", _contextManager.OwnerAccount.DisplayName, 1);
        }
    }
}
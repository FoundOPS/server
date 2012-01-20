using FoundOps.SLClient.Data.Tools;
using FoundOps.SLClient.UI.Tools;

namespace FoundOps.SLClient.UI.Controls.Dispatcher
{
    /// <summary>
    /// UI for viewing and editing a route's details.
    /// </summary>
    public partial class RouteDetails
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RouteDetails"/> class.
        /// </summary>
        public RouteDetails()
        {
            InitializeComponent();

            this.DependentWhenVisible(VM.Employees);
            this.DependentWhenVisible(VM.Vehicles);
        }
    }
}

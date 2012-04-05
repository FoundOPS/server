using FoundOps.SLClient.Data.Tools;
using FoundOps.SLClient.UI.Tools;

namespace FoundOps.SLClient.UI.Controls.Vehicles
{
    /// <summary>
    /// UI for displaying Vehicles for the current context.
    /// </summary>
    public partial class VehiclesGrid
    {
        public bool IsMainGrid { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="VehiclesGrid"/> class.
        /// </summary>
        public VehiclesGrid()
        {
            InitializeComponent();

            this.DependentWhenVisible(VM.Vehicles);
        }
    }
}

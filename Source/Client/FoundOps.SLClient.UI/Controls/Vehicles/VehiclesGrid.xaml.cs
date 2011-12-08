using FoundOps.SLClient.Data.Tools;
using FoundOps.SLClient.UI.ViewModels;

namespace FoundOps.SLClient.UI.Controls.Vehicles
{
    public partial class VehiclesGrid
    {
        public bool IsMainGrid { get; set; }

        public VehiclesGrid()
        {
            InitializeComponent();

            this.DependentWhenVisible(VehiclesVM);
        }

        public VehiclesVM VehiclesVM
        {
            get { return (VehiclesVM)this.DataContext; }
        }
    }
}

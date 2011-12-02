using FoundOps.SLClient.UI.ViewModels;

namespace FoundOps.SLClient.UI.Controls.Vehicles
{
    public partial class VehicleLarge
    {
        public VehicleLarge()
        {
            InitializeComponent();
            //Remove the comma that seperates numbers (this is a year)
            YearNumericUpDown.NumberFormatInfo.NumberGroupSeparator = "";
        }

        public VehiclesVM VehiclesVM
        {
            get { return (VehiclesVM)this.DataContext; }
        }
    }
}

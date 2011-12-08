namespace FoundOps.Core.Models.CoreEntities
{
    public partial class SubLocation
    {
        partial void OnLatitudeChanged()
        {
            this.RaisePropertyChanged("TelerikLocation");
        }

        partial void OnLongitudeChanged()
        {
            this.RaisePropertyChanged("TelerikLocation");
        }

        public Telerik.Windows.Controls.Map.Location? TelerikLocation
        {
            get
            {
                if (this.Latitude == null || this.Longitude == null)
                    return null;

                return new Telerik.Windows.Controls.Map.Location(System.Convert.ToDouble(this.Latitude),
                                                                 System.Convert.ToDouble(this.Longitude));
            }
        }
    }
}

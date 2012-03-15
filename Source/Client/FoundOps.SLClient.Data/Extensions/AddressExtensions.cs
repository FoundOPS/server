//Partial class must be part of same namespace
// ReSharper disable CheckNamespace
namespace FoundOps.Common.NET
// ReSharper restore CheckNamespace
{
    public partial class Address
    {
        #region Public Properties

        /// <summary>
        /// Returns a TelerikLocation based off this Latitude and Longitude.
        /// </summary>
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

        #endregion

        #region Logic

        #region Overriden Methods

        partial void OnLatitudeChanged()
        {
            this.RaisePropertyChanged("TelerikLocation");
        }

        partial void OnLongitudeChanged()
        {
            this.RaisePropertyChanged("TelerikLocation");
        }

        #endregion

        #endregion
    }
}

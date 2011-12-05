using System.Linq;

namespace FoundOps.Core.Models.CoreEntities
{
    public partial class VehicleMaintenanceLogEntry
    {
        protected override void OnLoaded(bool isInitialLoad)
        {
            if (isInitialLoad)
            {
                //Whenever the LineItems collection changes, or individual LineItems.Cost changes: update the value

                foreach(var lineItem in this.LineItems)
                {
                    lineItem.PropertyChanged +=
                      (se, ar) =>
                      {
                          if (ar.PropertyName == "Cost")
                              this.RaisePropertyChanged("TotalCost");
                      };
                }

                this.LineItems.EntityAdded += (s, args) =>
                {
                    args.Entity.PropertyChanged +=
                        (se, ar) =>
                        {
                            if (ar.PropertyName == "Cost")
                                this.RaisePropertyChanged("TotalCost");
                        };

                    this.RaisePropertyChanged("TotalCost");
                };

                this.LineItems.EntityRemoved += (s, args) => this.RaisePropertyChanged("TotalCost");

                this.RaisePropertyChanged("TotalCost");
            }

            base.OnLoaded(isInitialLoad);
        }

        ///<summary>
        /// Sums the total cost of the LineItems
        ///</summary>
        public decimal TotalCost
        {
            get
            {
                return LineItems.Sum(li => li.Cost).Value;
            }
        }
    }
}


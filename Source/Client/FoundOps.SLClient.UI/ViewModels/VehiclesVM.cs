using FoundOps.SLClient.Data.Services;
using System.ComponentModel.Composition;
using FoundOps.SLClient.Data.ViewModels;
using FoundOps.Core.Models.CoreEntities;
using MEFedMVVM.ViewModelLocator;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// A view model for all of the Vehicles
    /// </summary>
    [ExportViewModel("VehiclesVM")]
    public class VehiclesVM : CoreEntityCollectionInfiniteAccordionVM<Vehicle>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VehiclesVM"/> class.
        /// </summary>
        /// <param name="dataManager">The data manager.</param>
        [ImportingConstructor]
        public VehiclesVM(DataManager dataManager)
            : base(dataManager)
        {
            this.SetupMainQuery(DataManager.Query.Vehicles, null, "VehicleId");
        }

        #region Logic

        //Implements CoreEntityCollectionVM.OnAddEntity
        protected override void OnAddEntity(Vehicle newEntity)
        {
            //Set the new Vehicles OwnerParty to this OwnerAccount
            newEntity.OwnerParty = ContextManager.OwnerAccount;
        }

        #endregion
    }
}

using System.ComponentModel.Composition;
using FoundOps.SLClient.Data.Services;
using FoundOps.SLClient.Data.ViewModels;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Models.CoreEntities.M2M4Ria;
using MEFedMVVM.ViewModelLocator;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// A view model for all of the SelectedVehicles in Routes
    /// </summary>
    [ExportViewModel("SelectedVehiclesVM")]
    public class SelectedVehiclesVM : CoreSelectedEntitiesCollectionVM<Vehicle>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectedVehiclesVM"/> class.
        /// </summary>
        /// <param name="dataManager">The data manager.</param>
        /// <param name="selectedVehicles">The selected vehicles.</param>
        [ImportingConstructor]
        public SelectedVehiclesVM(DataManager dataManager, IEntityCollection<Vehicle> selectedVehicles)
            : base(dataManager, selectedVehicles, dataManager.GetEntityListObservable<Vehicle>(DataManager.Query.Vehicles))
        {
            //Default this to active
            this.ControlsThatCurrentlyRequireThisVM.Add("This viewmodel always needs data");

            //SetupMainQuery for Vehicles
            this.SetupMainQuery(DataManager.Query.Vehicles);
        }

        // Logic

        protected override void OnAddEntity(Vehicle newVehicle)
        {
            //Setup the OwnerParty
            newVehicle.OwnerParty = ContextManager.OwnerAccount;

            //Add the Vehicle to the BusinessAccount's Vehicles
            ContextManager.OwnerAccount.Vehicles.Add(newVehicle);

            //Add the newEmployee to the SelectedEntities
            this.SelectedEntities.Add(newVehicle);
        }
    }
}

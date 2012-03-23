using FoundOps.Common.Silverlight.UI.Controls.AddEditDelete;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.SLClient.Data.Services;
using FoundOps.SLClient.Data.ViewModels;
using MEFedMVVM.ViewModelLocator;
using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// A view model for all of the Vehicles
    /// </summary>
    [ExportViewModel("VehiclesVM")]
    public class VehiclesVM : InfiniteAccordionVM<Vehicle>, IAddToDeleteFromSource<Vehicle>
    {
        #region Implementation of IAddToDeleteFromSource

        public string MemberPath { get { return "VehicleId"; } }

        /// <summary>
        /// A function to create a new item from a string.
        /// </summary>
        public Func<string, Vehicle> CreateNewItem { get; private set; }

        public Action<string, AutoCompleteBox> ManuallyUpdateSuggestions { get; private set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="VehiclesVM"/> class.
        /// </summary>
        [ImportingConstructor]
        public VehiclesVM()
        {
            SetupTopEntityDataLoading(roleId => Context.GetVehiclesForPartyQuery(roleId));

            #region Implementation of IAddToDeleteFromSource<Employee>

            //Set the new Vehicles OwnerParty to this OwnerAccount
            CreateNewItem = vehicleId =>
            {
                var newVehicle = new Vehicle { VehicleId = vehicleId, OwnerParty = ContextManager.OwnerAccount };

                //Add the entity to the EntitySet so it is tracked by the DomainContext
                Context.Vehicles.Add(newVehicle);
                this.QueryableCollectionView.AddNew(newVehicle);

                return newVehicle;
            };

            ManuallyUpdateSuggestions = (searchText, autoCompleteBox) =>
                SearchSuggestionsHelper(autoCompleteBox, () => Manager.Data.Context.SearchVehiclesForRoleQuery(Manager.Context.RoleId, searchText));

            #endregion
        }

        #region Logic

        protected override Vehicle AddNewEntity(object commandParameter)
        {
            //Reuse the CreateNewItem method
            return CreateNewItem("");
        }

        #endregion
    }
}

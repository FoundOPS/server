﻿using FoundOps.Common.Silverlight.UI.Controls.AddEditDelete;
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
    public class VehiclesVM : InfiniteAccordionVM<Vehicle, Vehicle>, IAddToDeleteFromSource<Vehicle>
    {
        #region Implementation of IAddToDeleteFromSource

        public string MemberPath { get { return "VehicleId"; } }

        /// <summary>
        /// A function to create a new item from a string.
        /// </summary>
        public Func<string, Vehicle> CreateNewItem { get; private set; }

        public Action<AutoCompleteBox> ManuallyUpdateSuggestions { get; private set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="VehiclesVM"/> class.
        /// </summary>
        [ImportingConstructor]
        public VehiclesVM()
        {
            SetupTopEntityDataLoading(roleId => DomainContext.GetVehiclesForPartyQuery(roleId));

            #region Implementation of IAddToDeleteFromSource<Employee>

            //Set the new Vehicles OwnerParty to this OwnerAccount
            CreateNewItem = vehicleId =>
            {
                var newVehicle = new Vehicle { VehicleId = vehicleId, BusinessAccount = ((BusinessAccount)ContextManager.OwnerAccount) };

                //Add the entity to the EntitySet so it is tracked by the DomainContext
                DomainContext.Vehicles.Add(newVehicle);
                this.QueryableCollectionView.AddNew(newVehicle);

                return newVehicle;
            };

            ManuallyUpdateSuggestions = autoCompleteBox =>
                SearchSuggestionsHelper(autoCompleteBox, () => Manager.Data.DomainContext.SearchVehiclesForRoleQuery(Manager.Context.RoleId, autoCompleteBox.SearchText));

            #endregion
        }

        #region Logic

        protected override Vehicle AddNewEntity(object commandParameter)
        {
            Analytics.Track("Add Vehicle");

            //Reuse the CreateNewItem method
            return CreateNewItem("");
        }

        #endregion
    }
}

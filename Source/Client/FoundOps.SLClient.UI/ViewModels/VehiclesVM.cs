using FoundOps.Common.Silverlight.UI.Controls.AddEditDelete;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.SLClient.Data.Services;
using FoundOps.SLClient.Data.ViewModels;
using MEFedMVVM.ViewModelLocator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ServiceModel.DomainServices.Client;
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

        //Want to use the default comparer. So this does not need to be set.
        public IEqualityComparer<object> CustomComparer { get; set; }

        public IEnumerable ExistingItemsSource { get { return Context.Vehicles; } }

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

            ManuallyUpdateSuggestions = UpdateSuggestionsHelper;

            #endregion
        }

        #region IAddToDeleteFromSource

        private LoadOperation<Vehicle> _lastSuggestionQuery;
        /// <summary>
        /// Updates the locations suggestions.
        /// </summary>
        /// <param name="text">The search text</param>
        /// <param name="autoCompleteBox">The autocomplete box.</param>
        private void UpdateSuggestionsHelper(string text, AutoCompleteBox autoCompleteBox)
        {
            if (_lastSuggestionQuery != null && _lastSuggestionQuery.CanCancel)
                _lastSuggestionQuery.Cancel();

            _lastSuggestionQuery = Manager.Data.Context.Load(Manager.Data.Context.SearchVehiclesForRoleQuery(Manager.Context.RoleId, text).Take(10),
                                locationsLoadOperation =>
                                {
                                    if (locationsLoadOperation.IsCanceled || locationsLoadOperation.HasError) return;

                                    autoCompleteBox.ItemsSource = locationsLoadOperation.Entities;
                                    autoCompleteBox.PopulateComplete();
                                }, null);
        }

        #endregion

        #region Logic

        protected override Vehicle AddNewEntity(object commandParameter)
        {
            //Reuse the CreateNewItem method
            return CreateNewItem("");
        }

        #endregion
    }
}

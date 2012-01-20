using System;
using System.Collections;
using System.Collections.Generic;
using FoundOps.SLClient.Data.Services;
using System.ComponentModel.Composition;
using FoundOps.SLClient.Data.ViewModels;
using FoundOps.Core.Models.CoreEntities;
using MEFedMVVM.ViewModelLocator;
using FoundOps.Common.Silverlight.UI.Controls.AddEditDelete;
using Microsoft.Windows.Data.DomainServices;
using ReactiveUI;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// A view model for all of the Vehicles
    /// </summary>
    [ExportViewModel("VehiclesVM")]
    public class VehiclesVM : CoreEntityCollectionInfiniteAccordionVM<Vehicle>, IAddToDeleteFromSource<Vehicle>
    {
        #region Implementation of IAddToDeleteFromSource

        //Want to use the default comparer. So this does not need to be set.
        public IEqualityComparer<object> CustomComparer { get; set; }

        private readonly ObservableAsPropertyHelper<IEnumerable> _loadedVehicles;
        public IEnumerable ExistingItemsSource { get { return _loadedVehicles.Value; } }

        public string MemberPath { get { return "VehicleId"; } }

        /// <summary>
        /// A function to create a new item from a string.
        /// </summary>
        public Func<string, Vehicle> CreateNewItem { get; private set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="VehiclesVM"/> class.
        /// </summary>
        /// <param name="dataManager">The data manager.</param>
        [ImportingConstructor]
        public VehiclesVM(DataManager dataManager)
            : base(dataManager)
        {
            var loadedVehicles = this.SetupMainQuery(DataManager.Query.Vehicles, null, "VehicleId");

            #region Implementation of IAddToDeleteFromSource<Employee>

            //Whenever loadedVehicles changes notify ExistingItemsSource changed
            _loadedVehicles = loadedVehicles.ToProperty(this, x => x.ExistingItemsSource);

            //Set the new Vehicles OwnerParty to this OwnerAccount
            CreateNewItem = vehicleId =>
            {
                var newVehicle = new Vehicle { VehicleId = vehicleId, OwnerParty = ContextManager.OwnerAccount };
                
                //Add the new entity to the EntityList behind the DCV
                ((EntityList<Vehicle>)ExistingItemsSource).Add(newVehicle);
                
                return newVehicle;
            };

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

using System;
using System.ComponentModel.Composition;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.SLClient.Data.ViewModels;
using GalaSoft.MvvmLight.Command;
using MEFedMVVM.ViewModelLocator;

namespace FoundOps.SLClient.UI.ViewModels
{
    ///<summary>
    /// A view model for all of the VehicleMaintenanceLogEntries
    ///</summary>
    [ExportViewModel("VehicleMaintenanceVM")]
    public class VehicleMaintenanceVM : InfiniteAccordionVM<VehicleMaintenanceLogEntry>
    {
        //Public Properties
        public RelayCommand AddLineItemCommand { get; private set; }
        public RelayCommand<VehicleMaintenanceLineItem> DeleteLineItemCommand { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleMaintenanceVM"/> class.
        /// </summary>
        [ImportingConstructor]
        public VehicleMaintenanceVM()
        {
            SetupDataLoading();

            #region Register Commands

            AddLineItemCommand = new RelayCommand(() => this.SelectedEntity.LineItems.Add(new VehicleMaintenanceLineItem()));
            DeleteLineItemCommand = new RelayCommand<VehicleMaintenanceLineItem>((vehicleMaintenanceLineItem) => this.SelectedEntity.LineItems.Remove(vehicleMaintenanceLineItem));

            #endregion
        }

        private void SetupDataLoading()
        {
            SetupContextDataLoading(roleId => Context.GetVehicleMaintenanceLogForPartyQuery(roleId),
                                    new[]
                                        {
                                            new ContextRelationshipFilter
                                                {
                                                    EntityMember = "VehicleId",
                                                    FilterValueGenerator = v => ((Vehicle) v).Id,
                                                    RelatedContextType = typeof (Vehicle)
                                                }
                                        });
        }

        #region Logic

        protected override void OnAddEntity(VehicleMaintenanceLogEntry vehicleMaintenanceLogEntry)
        {
            vehicleMaintenanceLogEntry.Date = DateTime.Now.Date;

            var vehicleContext = ContextManager.GetContext<Vehicle>();

            vehicleMaintenanceLogEntry.Vehicle = vehicleContext;

            if (!IsInDetailsView)
                MoveToDetailsView.Execute(null);
        }

        #endregion
    }
}

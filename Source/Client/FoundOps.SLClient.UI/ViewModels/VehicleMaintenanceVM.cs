using System;
using FoundOps.Core.Models.CoreEntities;
using GalaSoft.MvvmLight.Command;
using MEFedMVVM.ViewModelLocator;
using FoundOps.SLClient.Data.Services;
using FoundOps.SLClient.Data.ViewModels;
using System.ComponentModel.Composition;
using FoundOps.Core.Models.CoreEntities;

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
            SetupMainQuery(DataManager.Query.VehicleMaintenance);

            #region Register Commands

            AddLineItemCommand = new RelayCommand(() => this.SelectedEntity.LineItems.Add(new VehicleMaintenanceLineItem()));
            DeleteLineItemCommand = new RelayCommand<VehicleMaintenanceLineItem>((vehicleMaintenanceLineItem) => this.SelectedEntity.LineItems.Remove(vehicleMaintenanceLineItem));

            #endregion
        }

        #region Logic

        protected override bool EntityIsPartOfView(VehicleMaintenanceLogEntry vehicleMaintenanceLogEntry, bool isNew)
        {
            if (isNew)
                return true;

            var entityIsPartOfView = false;

            //Setup filters

            //Find if there is a client context
            var vehicleContext = ContextManager.GetContext<Vehicle>();

            if (vehicleContext != null)
                entityIsPartOfView = vehicleMaintenanceLogEntry.VehicleId == vehicleContext.Id;

            return entityIsPartOfView;
        }

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

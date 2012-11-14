using System;
using System.Collections.Generic;

namespace FoundOps.Core.Models.CoreEntities.DesignData
{
    public class VehicleMaintenanceDesignData
    {
        public VehicleMaintenanceLogEntry DesignVehicleMaintenance { get; private set; }
        public VehicleMaintenanceLogEntry DesignVehicleMaintenanceTwo { get; private set; }
        public VehicleMaintenanceLogEntry DesignVehicleMaintenanceThree { get; private set; }
        public VehicleMaintenanceLineItem DesignVehicleMaintenanceLineItem { get; private set; }
        public VehicleMaintenanceLineItem VehicleMaintenanceLineItemTwo { get; private set; }
        public VehicleMaintenanceLineItem VehicleMaintenanceLineItemThree { get; private set; }

        public IEnumerable<VehicleMaintenanceLogEntry> DesignVehicleMaintenanceLogEntries { get; private set; }
        public IEnumerable<VehicleMaintenanceLineItem> DesignLineItems { get; private set; }

        public VehicleMaintenanceDesignData()
            : this(new VehiclesDesignData()) //Do not use inside VehiclesDesignData
        {
        }

        public VehicleMaintenanceDesignData(VehiclesDesignData vehiclesDesignData)
        {
            InitializeVehicleMaintenance();

            DesignVehicleMaintenance.Vehicle = vehiclesDesignData.DesignVehicle;
            DesignVehicleMaintenanceTwo.Vehicle = vehiclesDesignData.DesignVehicleTwo;
            DesignVehicleMaintenanceThree.Vehicle = vehiclesDesignData.DesignVehicleThree;
        }

        private void InitializeVehicleMaintenance()
        {
            DesignVehicleMaintenance = new VehicleMaintenanceLogEntry
            {
                Comments = "",
                Date = new DateTime(2009, 06, 09),
                Mileage = 128675,
                ServicedBy = "Linda Splenda",
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow
            };
            AddLineItems(DesignVehicleMaintenance);

            DesignVehicleMaintenanceTwo = new VehicleMaintenanceLogEntry
            {
                Comments = "",
                Date = new DateTime(2010, 06, 12),
                Mileage = 67075,
                ServicedBy = "Jim Smith",
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow
            };
            AddLineItems(DesignVehicleMaintenanceTwo);

            DesignVehicleMaintenanceThree = new VehicleMaintenanceLogEntry
            {
                Comments = "",
                Date = new DateTime(2008, 01, 20),
                Mileage = 18615,
                ServicedBy = "Tim",
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow
            };
            AddLineItems(DesignVehicleMaintenanceThree);

            DesignVehicleMaintenanceLogEntries = new List<VehicleMaintenanceLogEntry> { DesignVehicleMaintenance, DesignVehicleMaintenanceTwo, DesignVehicleMaintenanceThree };
        }

        private void AddLineItems(VehicleMaintenanceLogEntry vehicleMaintenanceLogEntry)
        {
            DesignVehicleMaintenanceLineItem = new VehicleMaintenanceLineItem
            {
                Type = "Changed Tires",
                Cost = (decimal?) 230.52,
                Details = "All 4",
                VehicleMaintenanceLogEntry = vehicleMaintenanceLogEntry,
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow
            };
            VehicleMaintenanceLineItemTwo = new VehicleMaintenanceLineItem
            {
                Type = "Body Repair",
                Cost = (decimal?)112.00,
                Details = "Fixed Rust Spot",
                VehicleMaintenanceLogEntry = vehicleMaintenanceLogEntry,
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow
            };

            VehicleMaintenanceLineItemThree = new VehicleMaintenanceLineItem
            {
                Type = "Replaced Window",
                Cost = (decimal?)90.34,
                Details = "Right Rear",
                VehicleMaintenanceLogEntry = vehicleMaintenanceLogEntry,
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow
            };
            DesignLineItems = new List<VehicleMaintenanceLineItem> { DesignVehicleMaintenanceLineItem, VehicleMaintenanceLineItemTwo, VehicleMaintenanceLineItemThree };
        }
    }
}
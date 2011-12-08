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
            DesignVehicleMaintenanceTwo.Vehicle = vehiclesDesignData.DesignVehicle;
            DesignVehicleMaintenanceThree.Vehicle = vehiclesDesignData.DesignVehicle;
        }

        private void InitializeVehicleMaintenance()
        {
            DesignVehicleMaintenance = new VehicleMaintenanceLogEntry
            {
                Comments = "Oil Changed",
                Date = new DateTime(2009, 06, 09),
                Mileage = 128675,
                ServicedBy = "Linda Splenda"
            };
            AddLineItems(DesignVehicleMaintenance);

            DesignVehicleMaintenanceTwo = new VehicleMaintenanceLogEntry
            {
                Comments = "No comment",
                Date = new DateTime(2010, 06, 12),
                Mileage = 67075,
                ServicedBy = "Jim Smith"
            };
            AddLineItems(DesignVehicleMaintenanceTwo);

            DesignVehicleMaintenanceThree = new VehicleMaintenanceLogEntry
            {
                Comments = "Small amount of comments",
                Date = new DateTime(2008, 01, 20),
                Mileage = 18615,
                ServicedBy = "Another Person"
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
                VehicleMaintenanceLogEntry = vehicleMaintenanceLogEntry
            };
            VehicleMaintenanceLineItemTwo = new VehicleMaintenanceLineItem
            {
                Type = "Body Repair",
                Cost = (decimal?)112.00,
                Details = "Fixed Rust Spot",
                VehicleMaintenanceLogEntry = vehicleMaintenanceLogEntry
            };

            VehicleMaintenanceLineItemThree = new VehicleMaintenanceLineItem
            {
                Type = "Replaced Window",
                Cost = (decimal?)90.34,
                Details = "Right Rear",
                VehicleMaintenanceLogEntry = vehicleMaintenanceLogEntry
            };
            DesignLineItems = new List<VehicleMaintenanceLineItem> { DesignVehicleMaintenanceLineItem, VehicleMaintenanceLineItemTwo, VehicleMaintenanceLineItemThree };
        }
    }
}
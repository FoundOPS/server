using System;
using System.Collections.Generic;
using System.Linq;

namespace FoundOps.Core.Models.CoreEntities.DesignData
{
    public class VehiclesDesignData
    {
        private readonly BusinessAccount _ownerParty;
        public Vehicle DesignVehicle { get; private set; }
        public Vehicle DesignVehicleTwo { get; private set; }
        public Vehicle DesignVehicleThree { get; private set; }

        public IEnumerable<Vehicle> DesignVehicles { get; private set; }

        private readonly VehicleMaintenanceDesignData _vehicleMaintenanceDesignData;


        public VehiclesDesignData():this(null)
        {
        }

        public VehiclesDesignData(BusinessAccount ownerParty)
        {
            _ownerParty = ownerParty;
            InitializeVehicles();
            _vehicleMaintenanceDesignData = new VehicleMaintenanceDesignData(this);
        }

        private void InitializeVehicles()
        {
            var date = DateTime.UtcNow;

            DesignVehicle = new Vehicle
            {
                LicensePlate = "375EMD",
                Make = "Ford",
                Mileage = 12675,
                Model = "Explorer",
                Notes = "",
                VehicleId = "3636",
                VIN = "1M8GDM9AXKP042788",
                Year = 2005,
                BusinessAccount = _ownerParty,
                LastCompassDirection = 30,
                LastLatitude = 40.4589,
                LastLongitude = -86.92,
                LastSource = "Catch Me If You Can",
                LastSpeed = 25.65,
                LastTimeStamp = date,
                CreatedDate = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            };

            DesignVehicleTwo = new Vehicle
            {
                LicensePlate = "998PLK",
                Make = "Toyota",
                Mileage = 100909,
                Model = "Tundra",
                Notes = "",
                VehicleId = "53",
                VIN = "5GZCZ43D13S812715",
                Year = 2008,
                BusinessAccount = _ownerParty,
                LastCompassDirection = -15,
                LastLatitude = 40.4599,
                LastLongitude = -86.94,
                LastSource = "Find Me",
                LastSpeed = 48.82,
                LastTimeStamp = date,
                CreatedDate = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            };

            DesignVehicleThree = new Vehicle
            {
                LicensePlate = "221IJX",
                Make = "Dodge",
                Mileage = 123475,
                Model = "Ram",
                Notes = "",
                VehicleId = "47",
                VIN = "1M8GDM9AXKP042788",
                Year = 2000,
                BusinessAccount = _ownerParty,
                LastCompassDirection = 0,
                LastLatitude = 40.4579,
                LastLongitude = -86.93,
                LastSource = "US Fleet Tracker",
                LastSpeed = 15.2,
                LastTimeStamp = date,
                CreatedDate = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            };

            DesignVehicles = new List<Vehicle> { DesignVehicle, DesignVehicleTwo, DesignVehicleThree };
        }
    }
}

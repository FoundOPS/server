using System;
using System.Collections.Generic;
using FoundOps.Core.Models.CoreEntities;

namespace FoundOps.Core.Models.CoreEntities.DesignData
{
    public class EmployeesDesignData
    {
        public Employee DesignEmployee { get; private set; }
        public Employee DesignEmployeeTwo { get; private set; }
        public Employee DesignEmployeeThree { get; private set; }

        public IEnumerable<Employee> DesignEmployees { get; private set; }

        private readonly EmployeeHistoryDesignData _employeeHistoryDesignData;

        public EmployeesDesignData(BusinessAccount businessAccount)
            : this()
        {
            if (businessAccount == null) return;
            foreach (var employee in DesignEmployees)
                employee.Employer = businessAccount;
        }

        public EmployeesDesignData()
        {
            InitializeEmployees();

            _employeeHistoryDesignData = new EmployeeHistoryDesignData(this);
        }

        private void InitializeEmployees()
        {
            var partyDesignData = new PartyDesignData();

            DesignEmployee = new Employee
            {
                AddressLineOne = "329 W Lutz Ave",
                AddressLineTwo = "Apt 3",
                City = "West Lafayette",
                Comments = "Current residence",
                State = "IN",
                ZipCode = "47906",
                Permissions = "none",
                HireDate = DateTime.UtcNow,
                SSN = "123-45-6789",
                OwnedPerson = partyDesignData.OwnedPerson,
                LastCompassDirection = 100,
                LastLatitude = 87.46325,
                LastLongitude = -86.4582,
                LastSource = "Android",
                LastSpeed = 25.65,
                LastTimeStamp = DateTime.UtcNow
            };

            DesignEmployeeTwo = new Employee
            {
                AddressLineOne = "440 S Grant St",
                AddressLineTwo = "Apt 8",
                City = "West Lafayette",
                Comments = "Current residence",
                State = "IN",
                ZipCode = "47906",
                Permissions = "",
                HireDate = DateTime.UtcNow,
                SSN = "000-11-2222",
                OwnedPerson = partyDesignData.OwnedPersonTwo,
                LastCompassDirection = 32,
                LastLatitude = 86.2225,
                LastLongitude = -89.6589,
                LastSource = "iPhone",
                LastSpeed = 15.2,
                LastTimeStamp = DateTime.UtcNow
            };

            DesignEmployeeThree = new Employee
            {
                AddressLineOne = "300 N Salisbury St",
                AddressLineTwo = "Apt 13",
                City = "West Lafayette",
                Comments = "Current residence",
                State = "IN",
                ZipCode = "47906",
                Permissions = "Level 2",
                HireDate = DateTime.UtcNow,
                SSN = "654-98-0123",
                OwnedPerson = partyDesignData.OwnedPersonThree,
                LastCompassDirection = -11,
                LastLatitude = 84.3654,
                LastLongitude = -85.7894,
                LastSource = "Windows Phone",
                LastSpeed = 48.82,
                LastTimeStamp = DateTime.UtcNow
            };

            DesignEmployees = new List<Employee> { DesignEmployee, DesignEmployeeTwo, DesignEmployeeThree };
        }
    }
}

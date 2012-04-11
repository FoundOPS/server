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
                OwnedPerson = partyDesignData.OwnedPerson
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
                OwnedPerson = partyDesignData.OwnedPersonTwo
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
                OwnedPerson = partyDesignData.OwnedPersonThree
            };

            DesignEmployees = new List<Employee> { DesignEmployee, DesignEmployeeTwo, DesignEmployeeThree };
        }
    }
}

using System;
using System.Collections.Generic;

namespace FoundOps.Core.Models.CoreEntities.DesignData
{
    public class EmployeesDesignData
    {
        public Employee DesignEmployee { get; private set; }
        public Employee DesignEmployeeTwo { get; private set; }
        public Employee DesignEmployeeThree { get; private set; }

        public IEnumerable<Employee> DesignEmployees { get; private set; }

        private readonly EmployeeHistoryDesignData _employeeHistoryDesignData;
        private readonly UserAccountsDesignData _userAccountDesignData;


        public EmployeesDesignData(BusinessAccount businessAccount)
            : this()
        {
            if (businessAccount == null) return;
            foreach (var employee in DesignEmployees)
                employee.Employer = businessAccount;
        }

        public EmployeesDesignData()
        {
            _userAccountDesignData = new UserAccountsDesignData();
            InitializeEmployees();

            _employeeHistoryDesignData = new EmployeeHistoryDesignData(this);
        }

        private void InitializeEmployees()
        {
            var dateTime = DateTime.UtcNow;

            DesignEmployee = new Employee
            {
                FirstName = "Jon",
                LastName = "Perl",
                Gender = Gender.Male,
                AddressLineOne = "329 W Lutz Ave",
                AddressLineTwo = "Apt 3",
                AdminDistrictTwo = "West Lafayette",
                Comments = "Current residence",
                AdminDistrictOne = "IN",
                PostalCode = "47906",
                CountryCode = "US",
                Permissions = "none",
                HireDate = dateTime.Date,
                SSN = "123-45-6789",
                LastCompassDirection = 100,
                LastLatitude = 40.4599,
                LastLongitude = -86.9309,
                LastSource = "android",
                LastSpeed = 25.65,
                LastTimeStamp = dateTime,
                LinkedUserAccountId = _userAccountDesignData.Jon.Id,
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow
            };

            DesignEmployeeTwo = new Employee
            {
                FirstName = "Zach",
                LastName = "Bright",
                Gender = Gender.Male,
                AddressLineOne = "440 S Grant St",
                AddressLineTwo = "Apt 8",
                AdminDistrictTwo = "West Lafayette",
                Comments = "Current residence",
                AdminDistrictOne = "IN",
                PostalCode = "47906",
                CountryCode = "US",
                Permissions = "",
                HireDate = dateTime.Date,
                SSN = "000-11-2222",
                LastCompassDirection = 32,
                LastLatitude = 40.4585,
                LastLongitude = -86.9329,
                LastSource = "iphone",
                LastSpeed = 15.2,
                LastTimeStamp = dateTime,
                LinkedUserAccountId = _userAccountDesignData.Zach.Id,
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow
            };

            DesignEmployeeThree = new Employee
            {
                FirstName = "Oren",
                LastName = "Shatken",
                Gender = Gender.Male,
                AddressLineOne = "300 N Salisbury St",
                AddressLineTwo = "Apt 13",
                AdminDistrictTwo = "West Lafayette",
                Comments = "Current residence",
                AdminDistrictOne = "IN",
                PostalCode = "47906",
                CountryCode = "US",
                Permissions = "Level 2",
                HireDate = dateTime.Date,
                SSN = "654-98-0123",
                LastCompassDirection = -11,
                LastLatitude = 40.4559,
                LastLongitude = -86.9305,
                LastSource = "winphone",
                LastSpeed = 48.82,
                LastTimeStamp = dateTime,
                LinkedUserAccountId = _userAccountDesignData.Oren.Id,
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow
            };

            DesignEmployees = new List<Employee> { DesignEmployee, DesignEmployeeTwo, DesignEmployeeThree };
        }
    }
}

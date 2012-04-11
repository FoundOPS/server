using System;
using System.Collections.Generic;

namespace FoundOps.Core.Models.CoreEntities.DesignData
{
    public class EmployeeHistoryDesignData
    {
        public EmployeeHistoryEntry DesignEmployeeHistory { get; private set; }
        public EmployeeHistoryEntry DesignEmployeeHistoryTwo { get; private set; }
        public EmployeeHistoryEntry DesignEmployeeHistoryThree { get; private set; }

        public IEnumerable<EmployeeHistoryEntry> DesignEmployeeHistoryEnties { get; private set; }

        public EmployeeHistoryDesignData()
            : this(new EmployeesDesignData()) //Do not use inside VehiclesDesignData
        {
        }

        public EmployeeHistoryDesignData(EmployeesDesignData employeesDesignData)
        {
            InitializeEmployees();

            DesignEmployeeHistory.Employee = employeesDesignData.DesignEmployee;
            DesignEmployeeHistoryTwo.Employee = employeesDesignData.DesignEmployeeTwo;
            DesignEmployeeHistoryThree.Employee = employeesDesignData.DesignEmployeeThree;
        }

        private void InitializeEmployees()
        {
            DesignEmployeeHistory = new EmployeeHistoryEntry
            {
                Date = DateTime.UtcNow,
                Type = "Employment Application",
                Summary = "Applied for secretary position",
                Notes = "Meets requirements"
            };

            DesignEmployeeHistoryTwo = new EmployeeHistoryEntry
            {
                Date = DateTime.UtcNow,
                Type = "Performance Review",
                Summary = "Passed",
                Notes = "No noteworthy complaints"
            };

            DesignEmployeeHistoryThree = new EmployeeHistoryEntry
            {
                Date = DateTime.UtcNow,
                Type = "Termination",
                Summary = "Moved away",
                Notes = "Received severence pay"
            };

            DesignEmployeeHistoryEnties = new List<EmployeeHistoryEntry> { DesignEmployeeHistory, DesignEmployeeHistoryTwo, DesignEmployeeHistoryThree };
        }
    }
}

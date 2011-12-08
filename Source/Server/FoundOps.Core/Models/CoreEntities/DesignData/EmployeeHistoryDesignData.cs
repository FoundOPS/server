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
            DesignEmployeeHistoryTwo.Employee = employeesDesignData.DesignEmployee;
            DesignEmployeeHistoryThree.Employee = employeesDesignData.DesignEmployee;
        }

        private void InitializeEmployees()
        {
            DesignEmployeeHistory = new EmployeeHistoryEntry
            {
                Date = DateTime.Now,
                Type = "Employment Application",
                Summary = "Questionable",
                Notes = "Good notes"
            };

            DesignEmployeeHistoryTwo = new EmployeeHistoryEntry
            {
                Date = DateTime.Now,
                Type = "Performance Review",
                Summary = "Not good",
                Notes = "Mediocre notes"
            };

            DesignEmployeeHistoryThree = new EmployeeHistoryEntry
            {
                Date = DateTime.Now,
                Type = "Termination",
                Summary = "The worst",
                Notes = "Bad notes"
            };

            DesignEmployeeHistoryEnties = new List<EmployeeHistoryEntry> { DesignEmployeeHistory, DesignEmployeeHistoryTwo, DesignEmployeeHistoryThree };
        }
    }
}

using System;

namespace FoundOPS.API.Models
{
    public class Employee
    {
        public Guid Id { get; set; }

        public Guid? LinkedUserAccountId { get; set; }

        public string FirstName { get; set; }
        public string MiddleInitial { get; set; }
        public string LastName { get; set; }

        public string DisplayName { get { return FirstName + " " + LastName; }}


        public static Employee ConvertModel(FoundOps.Core.Models.CoreEntities.Employee foundOpsEmployee)
        {
            var employee = new Employee
            {
                Id = foundOpsEmployee.Id,
                LinkedUserAccountId = foundOpsEmployee.LinkedUserAccountId,
                FirstName = foundOpsEmployee.FirstName,
                MiddleInitial = foundOpsEmployee.MiddleInitial,
                LastName = foundOpsEmployee.LastName
            };

            return employee;
        }
    }
}
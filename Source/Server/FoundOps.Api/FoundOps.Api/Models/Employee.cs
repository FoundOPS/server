using System;

namespace FoundOps.Api.Models
{
    public class Employee
    {
        public Guid Id { get; set; }

        public Guid? LinkedUserAccountId { get; set; }

        public string FirstName { get; set; }
        public string MiddleInitial { get; set; }
        public string LastName { get; set; }

        public string DisplayName { get { return FirstName + " " + LastName; }}

        public static Employee ConvertModel(FoundOps.Core.Models.CoreEntities.Employee model)
        {
            var employee = new Employee
            {
                Id = model.Id,
                LinkedUserAccountId = model.LinkedUserAccountId,
                FirstName = model.FirstName,
                MiddleInitial = model.MiddleInitial,
                LastName = model.LastName
            };

            return employee;
        }
    }
}
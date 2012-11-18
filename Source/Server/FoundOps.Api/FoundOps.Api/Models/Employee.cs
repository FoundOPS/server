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

        public DateTime CreatedDate { get; private set; }
        public DateTime? LastModifiedDate { get; set; }
        public Guid? LastModifyingUserId { get; set; }

        public Employee(DateTime currentDate)
        {
            CreatedDate = currentDate;
        }

        public static Employee ConvertModel(Core.Models.CoreEntities.Employee model)
        {
            var employee = new Employee(model.CreatedDate)
            {
                Id = model.Id,
                LinkedUserAccountId = model.LinkedUserAccountId,
                FirstName = model.FirstName,
                MiddleInitial = model.MiddleInitial,
                LastName = model.LastName,
                LastModifiedDate = model.LastModifiedDate,
                LastModifyingUserId = model.LastModifyingUserId
            };

            return employee;
        }
    }
}
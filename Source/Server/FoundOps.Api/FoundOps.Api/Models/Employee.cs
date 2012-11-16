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

        public DateTime CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public Guid? LastModifyingUserId { get; set; }

        public static Employee ConvertModel(Core.Models.CoreEntities.Employee model)
        {
            var employee = new Employee
            {
                Id = model.Id,
                LinkedUserAccountId = model.LinkedUserAccountId,
                FirstName = model.FirstName,
                MiddleInitial = model.MiddleInitial,
                LastName = model.LastName,
                CreatedDate = model.CreatedDate,
                LastModifiedDate = model.LastModifiedDate,
                LastModifyingUserId = model.LastModifyingUserId
            };

            return employee;
        }
    }
}
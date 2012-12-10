using System;

namespace FoundOps.Api.Models
{
    public class Employee : ITrackable
    {
        public Guid Id { get; set; }

        public DateTime CreatedDate { get; private set; }
        public DateTime? LastModified { get; private set; }
        public Guid? LastModifyingUserId { get; private set; }

        public Guid? LinkedUserAccountId { get; set; }

        public string FirstName { get; set; }
        public string MiddleInitial { get; set; }
        public string LastName { get; set; }

        public string DisplayName { get { return FirstName + " " + LastName; }}

        public Employee()
        {
            CreatedDate = DateTime.UtcNow;
        }

        public static Employee ConvertModel(Core.Models.CoreEntities.Employee model)
        {
            var employee = new Employee
            {
                Id = model.Id,
                CreatedDate = model.CreatedDate,
                LinkedUserAccountId = model.LinkedUserAccountId,
                FirstName = model.FirstName,
                MiddleInitial = model.MiddleInitial,
                LastName = model.LastName
            };

            employee.SetLastModified(model.LastModified, model.LastModifyingUserId);

            return employee;
        }

        public void SetLastModified(DateTime? lastModified, Guid? userId)
        {
            LastModified = lastModified;
            LastModifyingUserId = userId;
        }
    }
}
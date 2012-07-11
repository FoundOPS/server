using System;
using System.Linq;
using FoundOps.Core.Models.CoreEntities;

namespace FoundOPS.API.Models
{
    public class UserSettings
    {
        public Guid Id { get; set; }

        public string EmailAddress { get; set; }
        public string ImageUrl { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public Employee Employee { get; set; }

        /// <summary>
        /// Administrator, Mobile, Regular
        /// </summary>
        public string Role { get; set; }

        public static UserSettings ConvertModel(UserAccount userAccount, Role role)
        {
            var foundOpsEmployee = userAccount.LinkedEmployees.FirstOrDefault(e => e.EmployerId == role.OwnerPartyId);

            var userSettings = new UserSettings
                {
                    Id = userAccount.Id,
                    FirstName = userAccount.FirstName,
                    LastName = userAccount.LastName,
                    EmailAddress = userAccount.EmailAddress,
                    Role = role.Name,
                    Employee = Employee.ConvertModel(foundOpsEmployee)
                };

            return userSettings;
        }
    }
}
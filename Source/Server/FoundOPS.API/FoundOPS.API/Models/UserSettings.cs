using System;
using System.Linq;
using FoundOPS.API.Api;
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

        public TimeZoneInfo TimeZoneInfo { get; set; }

        /// <summary>
        /// Administrator, Mobile, Regular
        /// </summary>
        public string Role { get; set; }

        public static UserSettings ConvertModel(UserAccount userAccount, Role role)
        {
            var foundOpsEmployee = userAccount.LinkedEmployees.FirstOrDefault(e => e.EmployerId == role.OwnerBusinessAccountId);

            var userSettings = new UserSettings
                {
                    Id = userAccount.Id,
                    FirstName = userAccount.FirstName,
                    LastName = userAccount.LastName,
                    EmailAddress = userAccount.EmailAddress.Trim(),
                    Role = role.Name
                };

            if(foundOpsEmployee != null)
            {
                userSettings.Employee = Employee.ConvertModel(foundOpsEmployee);
            }

            var controller = new SettingsController();

            var timeZoneinfo = controller.GetTimeZones().FirstOrDefault(tz => tz.TimeZoneId == userAccount.TimeZone);

            userSettings.TimeZoneInfo = timeZoneinfo;

            return userSettings;
        }
    }
}
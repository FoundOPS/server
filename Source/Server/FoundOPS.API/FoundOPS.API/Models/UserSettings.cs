using System;

namespace FoundOPS.API.Models
{
    public class UserSettings
    {
        public Guid Id { get; set; }

        public string EmailAddress { get; set; }
        public string ImageUrl { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        /// <summary>
        /// Administrator, Mobile, Regular
        /// </summary>
        public string Role { get; set; }

        public static UserSettings ConvertModel(FoundOps.Core.Models.CoreEntities.UserAccount userAccount)
        {
            var userSettings = new UserSettings
                {
                    Id = userAccount.Id,
                    FirstName = userAccount.FirstName,
                    LastName = userAccount.LastName,
                    EmailAddress = userAccount.EmailAddress
                };

            return userSettings;
        }
    }
}
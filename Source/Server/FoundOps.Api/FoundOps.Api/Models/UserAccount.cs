using System;

namespace FoundOps.Api.Models
{
    public class UserAccount
    {
        public Guid Id { get; set; }

        public string EmailAddress { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        /// <summary>
        /// The linked employee for the current business account
        /// </summary>
        public Guid? EmployeeId { get; set; }

        /// <summary>
        /// Administrator, Mobile, Regular
        /// </summary>
        public string Role { get; set; }

        public string ImageUrl { get; set; }

        public static UserAccount Convert(FoundOps.Core.Models.CoreEntities.UserAccount model)
        {
            return new UserAccount
            {
                Id = model.Id,
                EmailAddress = model.EmailAddress,
                FirstName = model.FirstName,
                LastName = model.LastName
            };
        }
    }
}
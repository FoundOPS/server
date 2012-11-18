using FoundOps.Core.Models.Azure;
using System;

namespace FoundOps.Api.Models
{
    public class UserAccount : ITrackable
    {
        public Guid Id { get; set; }

        public string EmailAddress { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        //the url for the account's image
        public string ImageUrl { get; set; }

        /// <summary>
        /// The linked employee for the current business account
        /// </summary>
        public Guid? EmployeeId { get; set; }

        /// <summary>
        /// The time zone information
        /// </summary>
        public TimeZone TimeZone { get; set; }

        /// <summary>
        /// Administrator, Mobile, Regular
        /// </summary>
        public string Role { get; set; }

        public DateTime CreatedDate { get; private set; }
        public DateTime? LastModifiedDate { get; set; }
        public Guid? LastModifyingUserId { get; set; }

        public UserAccount(DateTime createdDate)
        {
            CreatedDate = createdDate;
        }

        public static UserAccount Convert(Core.Models.CoreEntities.UserAccount model)
        {
            var userAccount = new UserAccount(model.CreatedDate)
            {
                Id = model.Id,
                EmailAddress = model.EmailAddress,
                FirstName = model.FirstName,
                LastName = model.LastName,
                TimeZone = TimeZone.ConvertModel(model.TimeZoneInfo),
                LastModifiedDate = model.LastModifiedDate,
                LastModifyingUserId = model.LastModifyingUserId
            };
            
            //Load image url
            if (model.PartyImage == null)
                userAccount.ImageUrl = "img/emptyPerson.png";
            else
                userAccount.ImageUrl = model.PartyImage.RawUrl + AzureServerHelpers.GetBlobUrlHelper(model.Id, model.PartyImage.Id);

            return userAccount;
        }
    }
}
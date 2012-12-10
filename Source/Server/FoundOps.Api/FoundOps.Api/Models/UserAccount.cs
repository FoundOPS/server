using FoundOps.Core.Models.Azure;
using System;

namespace FoundOps.Api.Models
{
    public class UserAccount : ITrackable
    {
        public Guid Id { get; set; }

        public DateTime CreatedDate { get; private set; }
        public DateTime? LastModified { get; private set; }
        public Guid? LastModifyingUserId { get; private set; }

        public string EmailAddress { get; set; }

        /// <summary>
        /// The linked employee for the current business account
        /// </summary>
        public Guid? EmployeeId { get; set; }

        public string FirstName { get; set; }

        //the url for the account's image
        public string ImageUrl { get; set; }

        public string LastName { get; set; }

        /// <summary>
        /// Administrator, Mobile, Regular
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// The time zone information
        /// </summary>
        public TimeZone TimeZone { get; set; }

        public UserAccount()
        {
            CreatedDate = DateTime.UtcNow;
        }

        public static UserAccount Convert(Core.Models.CoreEntities.UserAccount model)
        {
            var userAccount = new UserAccount
            {
                Id = model.Id,
                CreatedDate = model.CreatedDate,
                EmailAddress = model.EmailAddress,
                FirstName = model.FirstName,
                LastName = model.LastName,
                TimeZone = TimeZone.ConvertModel(model.TimeZoneInfo)
            };
            
            userAccount.SetLastModified(model.LastModified, model.LastModifyingUserId);

            //Load image url
            if (model.PartyImage == null)
                userAccount.ImageUrl = "img/emptyPerson.png";
            else
                userAccount.ImageUrl = model.PartyImage.RawUrl + AzureServerHelpers.GetBlobUrlHelper(model.Id, model.PartyImage.Id);

            return userAccount;
        }

        public void SetLastModified(DateTime? lastModified, Guid? userId)
        {
            LastModified = lastModified;
            LastModifyingUserId = userId;
        }
    }
}
using System;

namespace FoundOps.Api.Models
{
    public class ContactInfo : ITrackable
    {
        public Guid Id { get; set; }
        /// <summary>
        /// The type of contact info (ex: Phone, email, fax)
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// A label given to the ContactInfo (ex: chef, owner, etc.)
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// The actual contact info (ex: Phone-847-598-5324, email-info@foundops.com, etc.)
        /// </summary>
        public string Data { get; set; }

        public Guid? LocationId { get; set; }
        public Guid? ClientId { get; set; }

        public DateTime CreatedDate { get; private set; }
        public DateTime? LastModifiedDate { get; private set; }
        public Guid? LastModifyingUserId { get; private set; }
        
        public void SetLastModified(DateTime? lastModified, Guid? userId)
        {
            LastModifiedDate = lastModified;
            LastModifyingUserId = LastModifyingUserId;
        }

        public ContactInfo(DateTime createdDate)
        {
            CreatedDate = createdDate;
        }
        
        public static ContactInfo Convert(FoundOps.Core.Models.CoreEntities.ContactInfo contactInfoModel)
        {
            var contactInfo = new ContactInfo(contactInfoModel.CreatedDate)
                {
                    Id = contactInfoModel.Id, 
                    Type = contactInfoModel.Type, 
                    Label = contactInfoModel.Label, 
                    Data = contactInfoModel.Data, 
                    ClientId = contactInfoModel.ClientId, 
                    LocationId = contactInfoModel.LocationId
                };

            contactInfo.SetLastModified(contactInfoModel.LastModifiedDate, contactInfoModel.LastModifyingUserId);

            //TODO Generalize this everywhere
            if (string.IsNullOrEmpty(contactInfo.Type))
                contactInfo.Type = "";
            if (string.IsNullOrEmpty(contactInfo.Label))
                contactInfo.Label = "";
            if (string.IsNullOrEmpty(contactInfo.Data))
                contactInfo.Data = "";

            return contactInfo;
        }

        public static FoundOps.Core.Models.CoreEntities.ContactInfo ConvertBack(Api.Models.ContactInfo contactInfoModel)
        {
            var contactInfo = new FoundOps.Core.Models.CoreEntities.ContactInfo
                {
                    Id = contactInfoModel.Id, 
                    Type = contactInfoModel.Type, 
                    Label = contactInfoModel.Label, 
                    Data = contactInfoModel.Data, 
                    ClientId = contactInfoModel.ClientId,
                    LocationId = contactInfoModel.LocationId,
                    CreatedDate = contactInfoModel.CreatedDate,
                    LastModifiedDate = contactInfoModel.LastModifiedDate,
                    LastModifyingUserId = contactInfoModel.LastModifyingUserId
                };

            //TODO Generalize this everywhere
            if (string.IsNullOrEmpty(contactInfo.Type))
                contactInfo.Type = "";
            if (string.IsNullOrEmpty(contactInfo.Label))
                contactInfo.Label = "";
            if (string.IsNullOrEmpty(contactInfo.Data))
                contactInfo.Data = "";

            return contactInfo;
        }
    }
}

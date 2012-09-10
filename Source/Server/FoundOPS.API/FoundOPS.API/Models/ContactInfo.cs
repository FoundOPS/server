namespace FoundOPS.API.Models
{
    public class ContactInfo
    {
        /// <summary>
        /// The type of contact info (ex: Phone number, email, fax)
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// A label given to the ContactInfo (ex: chef, owner, etc.)
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// The actual contact info (ex: Phone Number-847-598-5324, email-info@foundops.com, etc.)
        /// </summary>
        public string Data { get; set; }

        public static ContactInfo Convert(FoundOps.Core.Models.CoreEntities.ContactInfo contactInfoModel)
        {
            var contactInfo = new ContactInfo { Type = contactInfoModel.Type, Label = contactInfoModel.Label, Data = contactInfoModel.Data };
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

namespace FoundOPS.API.Models
{
    public class ContactInfo
    {
        public string Type { get; set; }
        public string Label { get; set; }
        public string Data { get; set; }

        public static ContactInfo Convert(FoundOps.Core.Models.CoreEntities.ContactInfo contactInfoModel)
        {
            return new ContactInfo { Type = contactInfoModel.Type, Label = contactInfoModel.Label, Data = contactInfoModel.Data };
        }
    }
}

namespace FoundOps.Core.Models.Import
{
    /// <summary>
    /// The type of data to import.
    /// </summary>
    public enum DataCategory
    {
        ClientName,
        SalespersonName,
        ContactInfoEmailAddressLabel,
        ContactInfoEmailAddressData,
        ContactInfoFaxNumberLabel,
        ContactInfoFaxNumberData,
        ContactInfoPhoneNumberLabel,
        ContactInfoPhoneNumberData,
        ContactInfoOtherLabel,
        ContactInfoOtherData,
        ContactInfoWebsiteLabel,
        ContactInfoWebsiteData,
        LocationName,
        LocationLatitude,
        LocationLongitude,
        LocationAddressLineOne,
        LocationAddressLineTwo,
        LocationCity,
        LocationState,
        LocationZipCode,
        ServiceDate
    }
}
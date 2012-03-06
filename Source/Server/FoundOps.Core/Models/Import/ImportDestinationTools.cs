using System.Collections.Generic;

namespace FoundOps.Core.Models.Import
{
    public static class ImportDestinationTools
    {
        public static List<ImportColumnType> ClientsColumnTypes
        {
            get
            {
                return new List<ImportColumnType>
                           {
                               new ImportColumnType("Client Name", DataCategory.ClientName, Multiplicity.Single),
                               //new ImportColumnType("Salesperson", ImportColumnEnum.SalespersonName, Multiplicity.Single),
                               new ImportColumnType("Location", DataCategory.LocationName, Multiplicity.Multiple),
                               new ImportColumnType("Email Address Label", DataCategory.ContactInfoEmailAddressLabel, Multiplicity.Multiple),
                               new ImportColumnType("Email Address", DataCategory.ContactInfoEmailAddressData, Multiplicity.Multiple),
                               new ImportColumnType("Fax Number Label", DataCategory.ContactInfoFaxNumberLabel, Multiplicity.Multiple),
                               new ImportColumnType("Fax Number", DataCategory.ContactInfoFaxNumberData, Multiplicity.Multiple),
                               new ImportColumnType("Phone Number Label", DataCategory.ContactInfoPhoneNumberLabel, Multiplicity.Multiple),
                               new ImportColumnType("Phone Number", DataCategory.ContactInfoPhoneNumberData, Multiplicity.Multiple),
                               new ImportColumnType("Other Label", DataCategory.ContactInfoOtherLabel, Multiplicity.Multiple),
                               new ImportColumnType("Other", DataCategory.ContactInfoOtherData, Multiplicity.Multiple),
                               new ImportColumnType("Website Label", DataCategory.ContactInfoWebsiteLabel, Multiplicity.Multiple),
                               new ImportColumnType("Website", DataCategory.ContactInfoWebsiteData, Multiplicity.Multiple)
                           };
            }
        }

        public static List<ImportColumnType> LocationsColumnTypes
        {
            get
            {
                return new List<ImportColumnType>
                           {
                               new ImportColumnType("Location Name", DataCategory.LocationName, Multiplicity.Single),
                               new ImportColumnType("Latitude", DataCategory.LocationLatitude, Multiplicity.Single),
                               new ImportColumnType("Longitude", DataCategory.LocationLongitude, Multiplicity.Single),
                               new ImportColumnType("Address Line One", DataCategory.LocationAddressLineOne, Multiplicity.Single),
                               new ImportColumnType("Address Line Two", DataCategory.LocationAddressLineTwo, Multiplicity.Single),
                               new ImportColumnType("City", DataCategory.LocationCity, Multiplicity.Single),
                               new ImportColumnType("State", DataCategory.LocationState, Multiplicity.Single),
                               new ImportColumnType("Zip Code", DataCategory.LocationZipCode, Multiplicity.Single),
                               new ImportColumnType("Client Name", DataCategory.ClientName, Multiplicity.Single),
                               new ImportColumnType("Email Address Label", DataCategory.ContactInfoEmailAddressLabel, Multiplicity.Multiple),
                               new ImportColumnType("Email Address", DataCategory.ContactInfoEmailAddressData, Multiplicity.Multiple),
                               new ImportColumnType("Fax Number Label", DataCategory.ContactInfoFaxNumberLabel, Multiplicity.Multiple),
                               new ImportColumnType("Fax Number", DataCategory.ContactInfoFaxNumberData, Multiplicity.Multiple),
                               new ImportColumnType("Phone Number Label", DataCategory.ContactInfoPhoneNumberLabel, Multiplicity.Multiple),
                               new ImportColumnType("Phone Number", DataCategory.ContactInfoPhoneNumberData, Multiplicity.Multiple),
                               new ImportColumnType("Other Label", DataCategory.ContactInfoOtherLabel, Multiplicity.Multiple),
                               new ImportColumnType("Other", DataCategory.ContactInfoOtherData, Multiplicity.Multiple),
                               new ImportColumnType("Website Label", DataCategory.ContactInfoWebsiteLabel, Multiplicity.Multiple),
                               new ImportColumnType("Website", DataCategory.ContactInfoWebsiteData, Multiplicity.Multiple)
                           };
            }
        }

        public static List<ImportColumnType> ServicesColumnTypes
        {
            get
            {
                return new List<ImportColumnType>
                           {
                               new ImportColumnType("Service Date", DataCategory.ServiceDate, Multiplicity.Single),
                               new ImportColumnType("State", DataCategory.LocationState, Multiplicity.Single),
                               new ImportColumnType("State", DataCategory.LocationState, Multiplicity.Single),
                               new ImportColumnType("Client Name", DataCategory.ClientName, Multiplicity.Single)
                           };
            }
        }
    }
}
using System.Collections.Generic;

namespace FoundOps.Core.Models.Import
{
    /// <summary>
    /// Tools for ImportDestinations.
    /// </summary>
    public static class ImportDestinationTools
    {
        /// <summary>
        /// The columns to display when importing Clients.
        /// </summary>
        public static List<ImportColumnType> ClientsColumnTypes
        {
            get
            {
                return new List<ImportColumnType>
                {
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

        /// <summary>
        /// The columns to display when importing Locations.
        /// </summary>
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

        /// <summary>
        /// The columns to display when importing RecurringServices.
        /// </summary>
        public static List<ImportColumnType> RecurringServicesColumnTypes
        {
            get
            {
                return new List<ImportColumnType>
                {
                   new ImportColumnType("Client Name", DataCategory.ClientName, Multiplicity.Single),
                   new ImportColumnType("Location Name", DataCategory.LocationName, Multiplicity.Single)
                };
            }
        }
    }
}
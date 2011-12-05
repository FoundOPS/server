using System;
using System.Collections.Generic;

namespace FoundOps.Framework.Views.Models
{
    public enum ImportColumnEnum
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

    public enum Multiplicity
    {
        Single,
        Multiple
    }

    public class ImportColumnType
    {
        public string DisplayName { get; private set; }
        public ImportColumnEnum Type { get; private set; }
        public Multiplicity Multiplicity { get; private set; }

        public ImportColumnType(string displayName, ImportColumnEnum type, Multiplicity multiplicity)
        {
            DisplayName = displayName;
            Type = type;
            Multiplicity = multiplicity;
        }
    }

    public enum ImportDestination
    {
        Clients,
        Locations,
        Services
    }

    public static class ImportDestinationTools
    {
        public static List<ImportColumnType> ClientsColumnTypes
        {
            get
            {
                return new List<ImportColumnType>
                           {
                               new ImportColumnType("Client Name", ImportColumnEnum.ClientName, Multiplicity.Single),
                               //new ImportColumnType("Salesperson", ImportColumnEnum.SalespersonName, Multiplicity.Single),
                               new ImportColumnType("Location", ImportColumnEnum.LocationName, Multiplicity.Multiple),
                               new ImportColumnType("Email Address Label", ImportColumnEnum.ContactInfoEmailAddressLabel, Multiplicity.Multiple),
                               new ImportColumnType("Email Address", ImportColumnEnum.ContactInfoEmailAddressData, Multiplicity.Multiple),
                               new ImportColumnType("Fax Number Label", ImportColumnEnum.ContactInfoFaxNumberLabel, Multiplicity.Multiple),
                               new ImportColumnType("Fax Number", ImportColumnEnum.ContactInfoFaxNumberData, Multiplicity.Multiple),
                               new ImportColumnType("Phone Number Label", ImportColumnEnum.ContactInfoPhoneNumberLabel, Multiplicity.Multiple),
                               new ImportColumnType("Phone Number", ImportColumnEnum.ContactInfoPhoneNumberData, Multiplicity.Multiple),
                               new ImportColumnType("Other Label", ImportColumnEnum.ContactInfoOtherLabel, Multiplicity.Multiple),
                               new ImportColumnType("Other", ImportColumnEnum.ContactInfoOtherData, Multiplicity.Multiple),
                               new ImportColumnType("Website Label", ImportColumnEnum.ContactInfoWebsiteLabel, Multiplicity.Multiple),
                               new ImportColumnType("Website", ImportColumnEnum.ContactInfoWebsiteData, Multiplicity.Multiple)
                           };
            }
        }

        public static List<ImportColumnType> LocationsColumnTypes
        {
            get
            {
                return new List<ImportColumnType>
                           {
                               new ImportColumnType("Location Name", ImportColumnEnum.LocationName, Multiplicity.Single),
                               new ImportColumnType("Latitude", ImportColumnEnum.LocationLatitude, Multiplicity.Single),
                               new ImportColumnType("Longitude", ImportColumnEnum.LocationLongitude, Multiplicity.Single),
                               new ImportColumnType("Address Line One", ImportColumnEnum.LocationAddressLineOne, Multiplicity.Single),
                               new ImportColumnType("Address Line Two", ImportColumnEnum.LocationAddressLineTwo, Multiplicity.Single),
                               new ImportColumnType("City", ImportColumnEnum.LocationCity, Multiplicity.Single),
                               new ImportColumnType("State", ImportColumnEnum.LocationState, Multiplicity.Single),
                               new ImportColumnType("Zip Code", ImportColumnEnum.LocationZipCode, Multiplicity.Single),
                               new ImportColumnType("Client Name", ImportColumnEnum.ClientName, Multiplicity.Single),
                               new ImportColumnType("Email Address Label", ImportColumnEnum.ContactInfoEmailAddressLabel, Multiplicity.Multiple),
                               new ImportColumnType("Email Address", ImportColumnEnum.ContactInfoEmailAddressData, Multiplicity.Multiple),
                               new ImportColumnType("Fax Number Label", ImportColumnEnum.ContactInfoFaxNumberLabel, Multiplicity.Multiple),
                               new ImportColumnType("Fax Number", ImportColumnEnum.ContactInfoFaxNumberData, Multiplicity.Multiple),
                               new ImportColumnType("Phone Number Label", ImportColumnEnum.ContactInfoPhoneNumberLabel, Multiplicity.Multiple),
                               new ImportColumnType("Phone Number", ImportColumnEnum.ContactInfoPhoneNumberData, Multiplicity.Multiple),
                               new ImportColumnType("Other Label", ImportColumnEnum.ContactInfoOtherLabel, Multiplicity.Multiple),
                               new ImportColumnType("Other", ImportColumnEnum.ContactInfoOtherData, Multiplicity.Multiple),
                               new ImportColumnType("Website Label", ImportColumnEnum.ContactInfoWebsiteLabel, Multiplicity.Multiple),
                               new ImportColumnType("Website", ImportColumnEnum.ContactInfoWebsiteData, Multiplicity.Multiple)
                           };
            }
        }

        public static List<ImportColumnType> ServicesColumnTypes
        {
            get
            {
                return new List<ImportColumnType>
                           {
                               new ImportColumnType("Service Date", ImportColumnEnum.ServiceDate, Multiplicity.Single),
                               new ImportColumnType("State", ImportColumnEnum.LocationState, Multiplicity.Single),
                               new ImportColumnType("State", ImportColumnEnum.LocationState, Multiplicity.Single),
                               new ImportColumnType("Client Name", ImportColumnEnum.ClientName, Multiplicity.Single)
                           };
            }
        }
    }
}
using FoundOps.Core.Models.CoreEntities;
using Kent.Boogaart.KBCsv;
using System;
using System.Collections.Generic;
using System.Data.Objects.DataClasses;
using System.Linq;

namespace FoundOps.Core.Models.Import
{
    /// <summary>
    /// Tools for importing
    /// </summary>
    public static class ImportRowTools
    {
        /// <summary>
        /// Holds the list of PropertyCategory models to be used by the importer.
        /// </summary>
        public static List<object> PropertyCategories = new List<object>();

        /// <summary>
        /// Initializes the <see cref="ImportRowTools"/> class.
        /// </summary>
        static ImportRowTools()
        {
            #region Add Property Categories

            #region Client

            PropertyCategories.Add(new PropertyCategory<Client>(DataCategory.ClientName, (client, name) => ((Business)client.OwnedParty).Name = name));

            #endregion

            #region Contact Info

            Action<ContactInfo, string> setLabel = (contactInfo, label) => contactInfo.Label = label;
            Action<ContactInfo, string> setData = (contactInfo, data) => contactInfo.Data = data;

            PropertyCategories.Add(new PropertyCategory<ContactInfo>(DataCategory.ContactInfoEmailAddressLabel, setLabel));
            PropertyCategories.Add(new PropertyCategory<ContactInfo>(DataCategory.ContactInfoEmailAddressData, setData));

            PropertyCategories.Add(new PropertyCategory<ContactInfo>(DataCategory.ContactInfoFaxNumberLabel, setLabel));
            PropertyCategories.Add(new PropertyCategory<ContactInfo>(DataCategory.ContactInfoFaxNumberData, setData));

            PropertyCategories.Add(new PropertyCategory<ContactInfo>(DataCategory.ContactInfoPhoneNumberLabel, setLabel));
            PropertyCategories.Add(new PropertyCategory<ContactInfo>(DataCategory.ContactInfoPhoneNumberData, setData));

            PropertyCategories.Add(new PropertyCategory<ContactInfo>(DataCategory.ContactInfoOtherLabel, setLabel));
            PropertyCategories.Add(new PropertyCategory<ContactInfo>(DataCategory.ContactInfoOtherData, setData));

            PropertyCategories.Add(new PropertyCategory<ContactInfo>(DataCategory.ContactInfoWebsiteLabel, setLabel));
            PropertyCategories.Add(new PropertyCategory<ContactInfo>(DataCategory.ContactInfoWebsiteData, setData));

            #endregion

            #region Location

            PropertyCategories.Add(new PropertyCategory<Location>(DataCategory.LocationName, (location, val) => location.Name = val));

            PropertyCategories.Add(new PropertyCategory<Location>(DataCategory.LocationAddressLineOne, (location, val) => location.AddressLineOne = val));
            PropertyCategories.Add(new PropertyCategory<Location>(DataCategory.LocationAddressLineTwo, (location, val) => location.AddressLineTwo = val));

            PropertyCategories.Add(new PropertyCategory<Location>(DataCategory.LocationCity, (location, val) => location.City = val));
            PropertyCategories.Add(new PropertyCategory<Location>(DataCategory.LocationState, (location, val) => location.State = val));
            PropertyCategories.Add(new PropertyCategory<Location>(DataCategory.LocationZipCode, (location, val) => location.ZipCode = val));

            PropertyCategories.Add(new PropertyCategory<Location>(DataCategory.LocationLatitude, (location, val) =>
            {
                decimal parsedDecimal;
                if (Decimal.TryParse(val, out parsedDecimal))
                    location.Latitude = parsedDecimal;
            }));

            PropertyCategories.Add(new PropertyCategory<Location>(DataCategory.LocationLongitude, (location, val) =>
            {
                decimal parsedDecimal;
                if (Decimal.TryParse(val, out parsedDecimal))
                    location.Longitude = parsedDecimal;
            }));

            #endregion

            #endregion
        }

        #region Create Entity Methods

        /// <summary>
        /// For 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        private static IEnumerable<ContactInfo> GetContactInfoSet<TEntity>(TEntity entity) where TEntity : EntityObject
        {
            //#region Add contact info to entities

            //var contactInfoSet = new List<ContactInfo>();

            //if (!String.IsNullOrEmpty(contactInfoEmailAddressLabel) || !String.IsNullOrEmpty(contactInfoEmailAddressData))
            //    contactInfoSet.Add(new ContactInfo { Type = "Email Address", Label = contactInfoEmailAddressLabel ?? "", Data = contactInfoEmailAddressData ?? "" });

            //if (!String.IsNullOrEmpty(contactInfoFaxNumberLabel) || !String.IsNullOrEmpty(contactInfoFaxNumberData))
            //    contactInfoSet.Add(new ContactInfo { Type = "Fax Number", Label = contactInfoFaxNumberLabel ?? "", Data = contactInfoFaxNumberData ?? "" });

            //if (!String.IsNullOrEmpty(contactInfoPhoneNumberLabel) || !String.IsNullOrEmpty(contactInfoPhoneNumberData))
            //    contactInfoSet.Add(new ContactInfo { Type = "Phone Number", Label = contactInfoPhoneNumberLabel ?? "", Data = contactInfoPhoneNumberData ?? "" });

            //if (!String.IsNullOrEmpty(contactInfoOtherLabel) || !String.IsNullOrEmpty(contactInfoOtherData))
            //    contactInfoSet.Add(new ContactInfo { Type = "Other", Label = contactInfoOtherLabel ?? "", Data = contactInfoOtherData ?? "" });

            //if (!String.IsNullOrEmpty(contactInfoWebsiteLabel) || !String.IsNullOrEmpty(contactInfoWebsiteData))
            //    contactInfoSet.Add(new ContactInfo { Type = "Website", Label = contactInfoWebsiteLabel ?? "", Data = contactInfoWebsiteData ?? "" });

            //if (newEntity is Location)
            //{
            //    foreach (var contactInfo in contactInfoSet)
            //        ((Location)newEntity).ContactInfoSet.Add(contactInfo);
            //}
            //else if (newEntity is Client)
            //{
            //    foreach (var contactInfo in contactInfoSet)
            //        ((Client)newEntity).OwnedParty.ContactInfoSet.Add(contactInfo);
            //}

            //#endregion
        }

        /// <summary>
        /// Creates a client from a set of categories and values.
        /// </summary>
        /// <param name="currentBusinessAccount">The current business account.</param>
        /// <param name="categoriesValues">The categories/values used to initialize the client.</param>
        /// <returns>A new client.</returns>
        public static Client CreateClient(BusinessAccount currentBusinessAccount, IEnumerable<Tuple<DataCategory, string>> categoriesValues)
        {
            //Need to create an OwnedParty and set the current business account
            var client = new Client { Vendor = currentBusinessAccount, OwnedParty = new Business() };

            SetProperties(client, categoriesValues);

            return client;
        }

        /// <summary>
        /// Creates the location.
        /// </summary>
        /// <param name="currentBusinessAccount">The current business account.</param>
        /// <param name="categoriesValues">The categories/values used to initialize the client.</param>
        /// <param name="clientAssociation">The (optional) client association.</param>
        public static Location CreateLocation(BusinessAccount currentBusinessAccount, IEnumerable<Tuple<DataCategory, string>> categoriesValues, Client clientAssociation)
        {
            var location = new Location { OwnerParty = currentBusinessAccount };

            SetProperties(location, categoriesValues);

            return location;
        }

        /// <summary>
        /// Creates the recurring service.
        /// </summary>
        /// <param name="currentBusinessAccount">The current business account.</param>
        /// <param name="categoriesValues">The categories/values used to initialize the client.</param>
        /// <param name="serviceTemplate">The parent service template.</param>
        /// <param name="clientAssociation">The client association.</param>
        /// <param name="locationAssociation">The location association.</param>
        /// <returns></returns>
        public static RecurringService CreateRecurringService(BusinessAccount currentBusinessAccount,
            IEnumerable<Tuple<DataCategory, string>> categoriesValues, ServiceTemplate serviceTemplate, Client clientAssociation, Location locationAssociation)
        {
            //TODO
            //var recurringService = new RecurringService {Client =  clientAssociation, ServiceTemplate = };

            //SetProperties(location, categoriesValues);

            //return location;

            return null;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Matches the categories with values from a datarecord.
        /// </summary>
        /// <param name="categories">The categories from the header record.</param>
        /// <param name="record">The record to extract values from.</param>
        public static IEnumerable<Tuple<DataCategory, string>> ExtractCategoriesWithValues(DataCategory[] categories, DataRecord record)
        {
            var categoryValues = new List<Tuple<DataCategory, string>>();

            //Go through each value and setup the category value tuple
            int columnIndex = 0;
            foreach (var value in record.Values)
            {
                categoryValues.Add(new Tuple<DataCategory, string>(categories.ElementAt(columnIndex), value));
                columnIndex++;
            }

            return categoryValues;
        }

        /// <summary>
        /// Sets the properties of the entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <param name="entity">The entity to set the properties on.</param>
        /// <param name="categoryValues">The datacategories to set.</param>
        private static void SetProperties<TEntity>(TEntity entity, IEnumerable<Tuple<DataCategory, string>> categoryValues) where TEntity : EntityObject
        {
            var entityPropertyCategories = PropertyCategories.OfType<PropertyCategory<TEntity>>().ToArray();

            foreach (var categoryValue in categoryValues)
            {
                var entityPropertyCategory = entityPropertyCategories.FirstOrDefault(pc => pc.Category == categoryValue.Item1);
                if (entityPropertyCategory != null)
                    entityPropertyCategory.SetProperty(entity, categoryValue.Item2);
            }
        }

        #endregion
    }

    /// <summary>
    /// Represents a property category.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class PropertyCategory<TEntity> where TEntity : EntityObject
    {
        /// <summary>
        /// The DataCategory this property is associated with.
        /// </summary>
        public DataCategory Category { get; private set; }

        /// <summary>
        /// An action that will set the Entity's property value.
        /// </summary>
        public Action<TEntity, string> SetProperty { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyCategory&lt;TEntity&gt;"/> class.
        /// </summary>
        /// <param name="category">The DataCategory this property is associated with.</param>
        /// <param name="setProperty">An action that will set the Entity's property value.</param>
        public PropertyCategory(DataCategory category, Action<TEntity, string> setProperty)
        {
            Category = category;
            SetProperty = setProperty;
        }
    }
}

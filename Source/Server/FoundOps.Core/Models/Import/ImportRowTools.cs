using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Models.CoreEntities.Extensions.Services;
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

            #region Repeats

            PropertyCategories.Add(new PropertyCategory<Repeat>(DataCategory.RepeatFrequency, (repeat, val) =>
            {
                val = val.ToLower();
                if (val == "once" || val == "o")
                    repeat.Frequency = Frequency.Once;
                else if (val == "daily" || val == "d")
                    repeat.Frequency = Frequency.Daily;
                else if (val == "weekly" || val == "w")
                    repeat.Frequency = Frequency.Weekly;
                else if (val == "monthly" || val == "m")
                    repeat.Frequency = Frequency.Monthly;
                else if (val == "yearly" || val == "y")
                    repeat.Frequency = Frequency.Yearly;
            }));

            PropertyCategories.Add(new PropertyCategory<Repeat>(DataCategory.RepeatStartDate, (repeat, val) =>
            {
                var startDate = Convert.ToDateTime(val);
                repeat.StartDate = startDate.Date.ToUniversalTime().Date;
            }));

            PropertyCategories.Add(new PropertyCategory<Repeat>(DataCategory.RepeatEnd, (repeat, val) =>
            {
                if (string.IsNullOrEmpty(val))
                    return;

                //Try to convert the End to a date
                try
                {
                    var endDate = Convert.ToDateTime(val);
                    repeat.EndDate = endDate.Date.ToUniversalTime().Date;
                }
                catch
                {
                    //Try to convert the End to a number
                    var endAfter = Convert.ToInt32(val);
                    repeat.EndAfterTimes = endAfter;
                }
            }));

            PropertyCategories.Add(new PropertyCategory<Repeat>(DataCategory.RepeatEvery, (repeat, val) =>
            {
                if (string.IsNullOrEmpty(val))
                    return;

                var repeatEvery = Convert.ToInt32(val);
                repeat.RepeatEveryTimes = repeatEvery;
            }));

            PropertyCategories.Add(new PropertyCategory<Repeat>(DataCategory.RepeatOn, (repeat, val) =>
            {
                val = val.ToLower();
                val = val.Replace(" ", "");
                if (repeat.Frequency == Frequency.Weekly)
                {
                    var startDayOfWeek = repeat.StartDate.DayOfWeek;

                    //If it is empty assume the Start Date
                    if (string.IsNullOrEmpty(val))
                    {
                        repeat.FrequencyDetailAsWeeklyFrequencyDetail = new[] { startDayOfWeek };
                        return;
                    }

                    var dayStrings = val.Split(',');
                    var daysOfWeek = new List<DayOfWeek>();

                    if (dayStrings.Any(s => s == "s" || s == "su" || s == "sun" || s == "sunday"))
                        daysOfWeek.Add(DayOfWeek.Sunday);

                    if (dayStrings.Any(s => s == "m" || s == "mo" || s == "mon" || s == "monday"))
                        daysOfWeek.Add(DayOfWeek.Monday);

                    if (dayStrings.Any(s => s == "t" || s == "tu" || s == "tues" || s == "tuesday"))
                        daysOfWeek.Add(DayOfWeek.Tuesday);

                    if (dayStrings.Any(s => s == "w" || s == "we" || s == "wed" || s == "wednesday"))
                        daysOfWeek.Add(DayOfWeek.Wednesday);

                    if (dayStrings.Any(s => s == "r" || s == "th" || s == "tr" || s == "thur" || s == "thurs" || s == "thursday"))
                        daysOfWeek.Add(DayOfWeek.Thursday);

                    if (dayStrings.Any(s => s == "f" || s == "fr" || s == "fri" || s == "friday"))
                        daysOfWeek.Add(DayOfWeek.Friday);

                    if (dayStrings.Any(s => s == "s" || s == "sa" || s == "sat" || s == "saturday"))
                        daysOfWeek.Add(DayOfWeek.Saturday);

                    //Make sure the days include the startdate
                    if (!daysOfWeek.Contains(startDayOfWeek))
                        daysOfWeek.Add(startDayOfWeek);

                    repeat.FrequencyDetailAsWeeklyFrequencyDetail = daysOfWeek.OrderBy(e => (int)e).ToArray();

                    return;
                }

                if (repeat.Frequency == Frequency.Monthly)
                {
                    if (string.IsNullOrEmpty(val) || val == "date")
                    {
                        repeat.FrequencyDetailAsMonthlyFrequencyDetail = MonthlyFrequencyDetail.OnDayInMonth;
                    }
                    else if (val == "day")
                    {
                        var detailsAvailable = repeat.AvailableMonthlyFrequencyDetailTypes.ToList();
                        detailsAvailable.Remove(MonthlyFrequencyDetail.OnDayInMonth);
                        repeat.FrequencyDetailAsMonthlyFrequencyDetail = detailsAvailable.First();
                    }
                }
            }));

            #endregion

            #endregion
        }

        #region Create Entity Methods

        /// <summary>
        /// Gets the ContactInfo set from a row's values.
        /// </summary>
        /// <param name="row">The row's categories/values.</param>
        /// <returns>A contact info set.</returns>
        private static IEnumerable<ContactInfo> GetContactInfoSet(Tuple<DataCategory, string>[] row)
        {
            //A row can have up to 5 contact infos on it. Seperate those out.
            var contactInfoSet = new List<ContactInfo>();

            //Email
            var emailCategoryValues = row.Where(r => r.Item1 == DataCategory.ContactInfoEmailAddressLabel || r.Item1 == DataCategory.ContactInfoEmailAddressData).ToArray();
            if (emailCategoryValues.Any(ecv => !string.IsNullOrEmpty(ecv.Item2)))
            {
                var email = new ContactInfo { Type = "Email Address" };
                SetProperties(email, emailCategoryValues);
                contactInfoSet.Add(email);
            }

            //Fax
            var faxCategoryValues = row.Where(r => r.Item1 == DataCategory.ContactInfoFaxNumberLabel || r.Item1 == DataCategory.ContactInfoFaxNumberData).ToArray();
            if (faxCategoryValues.Any(ecv => !string.IsNullOrEmpty(ecv.Item2)))
            {
                var fax = new ContactInfo { Type = "Fax Number" };
                SetProperties(fax, faxCategoryValues);
                contactInfoSet.Add(fax);
            }

            //Phone
            var phoneCategoryValues = row.Where(r => r.Item1 == DataCategory.ContactInfoPhoneNumberLabel || r.Item1 == DataCategory.ContactInfoPhoneNumberData).ToArray();
            if (phoneCategoryValues.Any(ecv => !string.IsNullOrEmpty(ecv.Item2)))
            {
                var phone = new ContactInfo { Type = "Phone Number" };
                SetProperties(phone, phoneCategoryValues);
                contactInfoSet.Add(phone);
            }

            //Other
            var otherCategoryValues = row.Where(r => r.Item1 == DataCategory.ContactInfoOtherLabel || r.Item1 == DataCategory.ContactInfoOtherData).ToArray();
            if (otherCategoryValues.Any(ecv => !string.IsNullOrEmpty(ecv.Item2)))
            {
                var other = new ContactInfo { Type = "Other" };
                SetProperties(other, otherCategoryValues);
                contactInfoSet.Add(other);
            }

            //Website
            var websiteCategoryValues = row.Where(r => r.Item1 == DataCategory.ContactInfoWebsiteLabel || r.Item1 == DataCategory.ContactInfoWebsiteData).ToArray();
            if (websiteCategoryValues.Any(ecv => !string.IsNullOrEmpty(ecv.Item2)))
            {
                var website = new ContactInfo { Type = "Website" };
                SetProperties(website, websiteCategoryValues);
                contactInfoSet.Add(website);
            }

            return contactInfoSet;
        }

        /// <summary>
        /// Creates a client from a set of categories and values.
        /// </summary>
        /// <param name="currentBusinessAccount">The current business account.</param>
        /// <param name="row">The categories/values used to initialize the client.</param>
        /// <returns>A new client.</returns>
        public static Client CreateClient(BusinessAccount currentBusinessAccount, Tuple<DataCategory, string>[] row)
        {
            //Need to create an OwnedParty and set the current business account
            var client = new Client { Vendor = currentBusinessAccount, OwnedParty = new Business() };

            SetProperties(client, row);

            //Add contact info set
            var contactInfoSet = GetContactInfoSet(row);
            foreach (var contactInfo in contactInfoSet)
                client.OwnedParty.ContactInfoSet.Add(contactInfo);

            return client;
        }

        /// <summary>
        /// Creates the location.
        /// It will also set the clientAssociation's DefaultBillingLocation to this new location
        /// </summary>
        /// <param name="currentBusinessAccount">The current business account.</param>
        /// <param name="row">The categories/values used to initialize the client.</param>
        /// <param name="clientAssociation">The (optional) client association.</param>
        public static Location CreateLocation(BusinessAccount currentBusinessAccount, Tuple<DataCategory, string>[] row, Client clientAssociation)
        {
            var location = new Location { OwnerParty = currentBusinessAccount };

            //Set the Location's Party and set the clientAssociation's DefaultBillingLocation to the new location
            //if the clientAssociation is not null
            if (clientAssociation != null)
            {
                location.Party = clientAssociation.OwnedParty;

                //Set this as the default billing location
                clientAssociation.DefaultBillingLocation = location;
            }

            SetProperties(location, row);

            //Add contact info set
            var contactInfoSet = GetContactInfoSet(row);
            foreach (var contactInfo in contactInfoSet)
                location.ContactInfoSet.Add(contactInfo);

            return location;
        }

        /// <summary>
        /// Creates the recurring service.
        /// </summary>
        /// <param name="currentBusinessAccount">The current business account.</param>
        /// <param name="categoriesValues">The categories/values used to initialize the client.</param>
        /// <param name="clientServiceTemplate">The parent service template.</param>
        /// <param name="clientAssociation">The client association.</param>
        /// <param name="locationAssociation">The location association.</param>
        /// <returns></returns>
        public static RecurringService CreateRecurringService(BusinessAccount currentBusinessAccount,
            Tuple<DataCategory, string>[] categoriesValues, ServiceTemplate clientServiceTemplate, Client clientAssociation, Location locationAssociation)
        {
            var recurringService = new RecurringService
            {
                Client = clientAssociation,
                ServiceTemplate = clientServiceTemplate.MakeChild(ServiceTemplateLevel.RecurringServiceDefined)
            };
            recurringService.AddRepeat();

            if (locationAssociation != null)
                recurringService.ServiceTemplate.SetDestination(locationAssociation);

            //Set the repeat properties
            SetProperties(recurringService.Repeat, categoriesValues);

            return recurringService;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Gets the value of a category from a row if it exists.
        /// </summary>
        /// <param name="categoriesValues">The rows categories and values</param>
        /// <param name="category">The category to get the value for.</param>
        public static string GetCategoryValue(this Tuple<DataCategory, string>[] categoriesValues, DataCategory category)
        {
            var categoryValue = categoriesValues.FirstOrDefault(cv => cv.Item1 == category);
            return categoryValue != null ? categoryValue.Item2 : null;
        }

        /// <summary>
        /// Matches the categories with values from a datarecord.
        /// </summary>
        /// <param name="categories">The categories from the header record.</param>
        /// <param name="record">The record to extract values from.</param>
        public static Tuple<DataCategory, string>[] ExtractCategoriesWithValues(DataCategory[] categories, DataRecord record)
        {
            var categoryValues = new List<Tuple<DataCategory, string>>();

            //Go through each value and setup the category value tuple
            int columnIndex = 0;
            foreach (var value in record.Values)
            {
                categoryValues.Add(new Tuple<DataCategory, string>(categories.ElementAt(columnIndex), value));
                columnIndex++;
            }

            return categoryValues.ToArray();
        }

        /// <summary>
        /// Sets the properties of the entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <param name="entity">The entity to set the properties on.</param>
        /// <param name="categoryValues">The datacategories to set.</param>
        private static void SetProperties<TEntity>(TEntity entity, Tuple<DataCategory, string>[] categoryValues) where TEntity : EntityObject
        {
            var entityPropertyCategories = PropertyCategories.OfType<PropertyCategory<TEntity>>()
                .Where(pc => categoryValues.Any(cv => cv.Item1 == pc.Category)).ToArray();

            //Set the properties in order as they are in the PropertyCategories
            foreach (var entityPropertyCategory in entityPropertyCategories)
            {
                var value = categoryValues.First(cv => cv.Item1 == entityPropertyCategory.Category).Item2;
                entityPropertyCategory.SetProperty(entity, value);
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

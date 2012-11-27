using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;
using Dapper;
using FoundOps.Api.Models;
using FoundOps.Api.Tools;
using FoundOps.Common.NET;
using FoundOps.Core.Models;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Tools;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Client = FoundOps.Core.Models.CoreEntities.Client;
using ContactInfo = FoundOps.Api.Models.ContactInfo;
using Location = FoundOps.Core.Models.CoreEntities.Location;
using Region = FoundOps.Api.Models.Region;
using Repeat = FoundOps.Api.Models.Repeat;

namespace FoundOps.Api.Controllers.Rest
{
    public class SuggestionsController : BaseApiController
    {
        private ConcurrentDictionary<Guid, Client> _clients;
        private ConcurrentDictionary<Guid, Location> _locations;
        private ConcurrentDictionary<Guid, FoundOps.Core.Models.CoreEntities.Region> _regions;
        private ConcurrentDictionary<Guid, Core.Models.CoreEntities.ContactInfo> _contactInfoSet;
        private const string Sql = @"SELECT * FROM dbo.Locations WHERE BusinessAccountId = @id
                                     SELECT * FROM dbo.Clients WHERE BusinessAccountId = @id
                                     SELECT * FROM dbo.Regions WHERE BusinessAccountId = @id
                                     SELECT * FROM dbo.ContactInfoSet 
                                        WHERE ClientId IN (SELECT Id FROM dbo.Clients WHERE BusinessAccountId = @id) 
                                        OR LocationId IN (SELECT Id FROM dbo.Locations WHERE BusinessAccountId = @id)";

        /// <summary>
        /// Get suggestions.
        /// In the request specify either RowsWithHeaders or Rows.
        /// </summary>
        /// <param name="roleId">The current role id</param>
        /// <param name="request">
        /// <para>RowWithHeaders (List of sting[]): If not null, call ValidateInput. The first string[] is the headers. The rest are data to be imported.</para>
        /// <para>Rows (ImportRow[]): If not null, call SuggestEntites. The rows to be imported</para>
        /// <para>If both parameters are set or null an error will be thrown.</para></param>
        /// <returns>Entites with suggestions</returns>
        public Suggestions Put(Guid roleId, SuggestionsRequest request)
        {
            //If both parameters are set or null an error will be thrown
            if ((request.RowsWithHeaders == null && request.Rows == null) || (request.RowsWithHeaders != null && request.Rows != null))
                throw Request.BadRequest();

            var businessAccount = CoreEntitiesContainer.Owner(roleId, new[] { RoleType.Administrator }).FirstOrDefault();
            if (businessAccount == null)
                throw Request.BadRequest();

            //load all of the business account's clients, locations, and regions
            using (var conn = new SqlConnection(ServerConstants.SqlConnectionString))
            {
                conn.Open();

                using (var data = conn.QueryMultiple(Sql, new { id = businessAccount.Id }))
                {
                    _locations = new ConcurrentDictionary<Guid, Location>(data.Read<Location>().ToDictionary(l => l.Id, l => l));

                    _clients = new ConcurrentDictionary<Guid, Client>(data.Read<Client>().ToDictionary(l => l.Id, l => l));

                    _regions = new ConcurrentDictionary<Guid, Core.Models.CoreEntities.Region>(data.Read<Core.Models.CoreEntities.Region>().ToDictionary(l => l.Id, l => l));

                    _contactInfoSet = new ConcurrentDictionary<Guid, Core.Models.CoreEntities.ContactInfo>(data.Read<Core.Models.CoreEntities.ContactInfo>().ToDictionary(l => l.Id, l => l));
                }

                conn.Close();
            }

            var itemsProcessed = 0;
            var updater = new Subject<Unit>();
            updater.Subscribe(u =>
                {
                    itemsProcessed += 1;
                    //Update the progress bars value here
                    //Call OnNext at the end of each loop
                    //Set Max on the progress bar to the number of rows if SuggestEntities is called
                    //Set Max on the progress bar to TWICE the number of rows if ValidateThenSuggestEntities is called (goes through both methods)
                });

            //Call the appropriate function and return the Suggestions
            return request.RowsWithHeaders != null ?
                this.ValidateThenSuggestEntities(request.RowsWithHeaders) :
                this.SuggestEntites(request.Rows);
        }

        /// <summary>
        /// Manipulates the input strings to existing or new entities.
        /// Then it passes those entities to SuggestEntities to generate suggestions.
        /// <note>This method is only used for the initial step of the Import. Once entites are generated; use SuggestEntities</note>
        /// </summary>
        /// <param name="rowsWithHeaders">List of string[]. The first string[] is the headers, the rest are data to be imported</param>
        /// <returns>Entites with suggestions</returns>
        public Suggestions ValidateThenSuggestEntities(List<string[]> rowsWithHeaders)
        {
            var headers = rowsWithHeaders[0];
            rowsWithHeaders.RemoveAt(0);
            var rows = rowsWithHeaders;

            #region Assign Column placement

            //Client
            var clientNameCol = Array.IndexOf(headers, "Client Name");

            //Location
            var addressLineOneCol = Array.IndexOf(headers, "Address Line One");
            var addressLineTwoCol = Array.IndexOf(headers, "Address Line Two");
            var cityCol = Array.IndexOf(headers, "AdminDistrictTwo");
            var stateCol = Array.IndexOf(headers, "AdminDistrictOne");
            var zipCodeCol = Array.IndexOf(headers, "PostalCode");
            var countryCodeCol = Array.IndexOf(headers, "Country Code");
            var regionNameCol = Array.IndexOf(headers, "Region Name");
            var latitudeCol = Array.IndexOf(headers, "Latitude");
            var longitudeCol = Array.IndexOf(headers, "Longitude");

            //Repeat
            var frequencyCol = Array.IndexOf(headers, "Frequency");
            var repeatEveryCol = Array.IndexOf(headers, "Repeat Every");
            var startDateCol = Array.IndexOf(headers, "Start Date");
            var endDateCol = Array.IndexOf(headers, "End Date");
            var endAfterCol = Array.IndexOf(headers, "End After Times");
            var frequencyDetailCol = Array.IndexOf(headers, "Frequency Detail");

            //Contact Info
            var phoneNumberValueCols = headers.Where(h => h.Contains("Phone Value")).ToArray();
            var phoneNumberLabelCols = headers.Where(h => h.Contains("Phone Label")).ToArray();

            var emailValueCols = headers.Where(h => h.Contains("Email Value")).ToArray();
            var emailLabelCols = headers.Where(h => h.Contains("Email Label")).ToArray();

            var websiteValueCols = headers.Where(h => h.Contains("Website Value")).ToArray();
            var websiteLabelCols = headers.Where(h => h.Contains("Website Label")).ToArray();

            var otherValueCols = headers.Where(h => h.Contains("Other Value")).ToArray();
            var otherLabelCols = headers.Where(h => h.Contains("Other Label")).ToArray();

            #endregion

            var concurrentDictionary = new ConcurrentDictionary<int, ImportRow>();

            var rowCount = rows.Count();
            Parallel.For((long)0, rowCount, rowIndex =>
            {
                var row = rows[(int)rowIndex];

                var importRow = new ImportRow();

                #region Location

                var importedLocation = new Api.Models.Location();

                string clientName;

                //Checks if at least one location column is passed
                if (new[] { addressLineOneCol, addressLineTwoCol, cityCol, stateCol, countryCodeCol, zipCodeCol, latitudeCol, longitudeCol }.Any(col => col != -1))
                {
                    var latitude = latitudeCol != -1 ? row[latitudeCol] : "";
                    var longitude = longitudeCol != -1 ? row[longitudeCol] : "";
                    var addressLineOne = addressLineOneCol != -1 ? row[addressLineOneCol] : null;
                    var addressLineTwo = addressLineTwoCol != -1 ? row[addressLineTwoCol] : null;
                    var adminDistrictTwo = cityCol != -1 ? row[cityCol] : null;
                    var adminDistrictOne = stateCol != -1 ? row[stateCol] : null;
                    var postalCode = zipCodeCol != -1 ? row[zipCodeCol] : null;
                    var regionName = regionNameCol != -1 ? row[regionNameCol] : null;

                    //If Lat/Lon dont have values, try to Geocode
                    if (latitude == "" && longitude == "")
                    {
                        var address = new Address
                        {
                            AddressLineOne = addressLineOne ?? "",
                            City = adminDistrictTwo ?? "",
                            State = adminDistrictOne ?? "",
                            ZipCode = postalCode ?? ""
                        };

                        //Attempt to Geocode the address
                        var geocodeResult = BingLocationServices.TryGeocode(address).FirstOrDefault(gc => gc != null);

                        latitude = geocodeResult != null ? geocodeResult.Latitude : null;
                        longitude = geocodeResult != null ? geocodeResult.Longitude : null;

                        //If they still do not have values, throw error on the location
                        if (latitude == null && longitude == null)
                            importedLocation.StatusInt = (int)ImportStatus.Error;

                        //Matched the entire address to a location (AddressLineOne, AddressLineTwo, City, State and ZipCode)
                        var matchedLocation = _locations.FirstOrDefault(l => l.Value.AddressLineOne == addressLineOne && l.Value.AddressLineTwo == addressLineTwo
                                                            && l.Value.AdminDistrictTwo == adminDistrictTwo && l.Value.AdminDistrictOne == adminDistrictOne && l.Value.PostalCode == postalCode);

                        if (matchedLocation.Key != Guid.Empty)
                            importedLocation = ConvertLocationSetRegionAndStatus(matchedLocation.Value, regionName, ImportStatus.Linked);
                        else
                        {
                            clientName = clientNameCol != -1 ? row[clientNameCol] : null;

                            //Matched the address entered and client name to a location
                            matchedLocation = _locations.FirstOrDefault(l => clientName != null && l.Value.ClientId != null &&
                                   (addressLineTwo != null && (l.Value.AddressLineOne == addressLineOne && l.Value.AddressLineTwo == addressLineTwo
                                        && _clients.First(c => c.Key == l.Value.ClientId).Value.Name == clientName)));

                            if (matchedLocation.Key != Guid.Empty)
                                importedLocation = ConvertLocationSetRegionAndStatus(matchedLocation.Value, regionName, ImportStatus.Linked);
                        }
                    }

                    //If lat/long exist, try and match based on that
                    if (latitude != "" && longitude != "" && importedLocation.StatusInt != (int)ImportStatus.Error)
                    {
                        var roundedLatitude = Math.Round(Convert.ToDecimal(latitude), 6);
                        var roundedLongitude = Math.Round(Convert.ToDecimal(longitude), 6);

                        //Try and match a location to one in the FoundOPS system
                        var matchedLocation = addressLineTwo != null
                            //Use this statement if Address Line Two is not null
                        ? _locations.FirstOrDefault(l => l.Value.AddressLineTwo == addressLineTwo && (l.Value.Latitude != null && l.Value.Longitude != null)
                            && (decimal.Round(l.Value.Latitude.Value, 6) == roundedLatitude
                            && decimal.Round(l.Value.Longitude.Value, 6) == roundedLongitude))
                            //Use this statement if Address Line Two is null
                        : _locations.FirstOrDefault(l => (l.Value.Latitude != null && l.Value.Longitude != null)
                            && (decimal.Round(l.Value.Latitude.Value, 6) == roundedLatitude
                            && decimal.Round(l.Value.Longitude.Value, 6) == roundedLongitude));

                        //If a match is found, assign Linked status to all cells
                        if (matchedLocation.Key != Guid.Empty)
                            importedLocation = ConvertLocationSetRegionAndStatus(matchedLocation.Value, regionName, ImportStatus.Linked);
                    }

                    //If no linked location is found, it means that the location is new
                    if (importedLocation.StatusInt != (int)ImportStatus.Linked)
                    {
                        importedLocation = new Api.Models.Location
                        {
                            Id = Guid.NewGuid(),
                            AddressLineOne = addressLineOne,
                            AddressLineTwo = addressLineTwo ?? null,
                            AdminDistrictTwo = adminDistrictTwo,
                            AdminDistrictOne = adminDistrictOne,
                            PostalCode = postalCode,
                            CountryCode = "US",
                            ContactInfoSet = new List<ContactInfo>(),
                            Latitude = latitude,
                            Longitude = longitude,
                            StatusInt = (int)ImportStatus.New,
                            Region = SetRegion(regionName)
                        };

                        //Since the location is new, we add it to the list of locations to be searched next time we try and match
                        _locations.GetOrAdd(importedLocation.Id, FoundOps.Api.Models.Location.ConvertBack(importedLocation));
                    }
                    importRow.Location = importedLocation;
                }

                #endregion

                #region Client

                clientName = clientNameCol != -1 ? row[clientNameCol] : null;

                if (clientName != null)
                {
                    var existingClient = _clients.FirstOrDefault(c => c.Value.Name == clientName);

                    Models.Client importedClient;

                    if (existingClient.Key != Guid.Empty)
                    {
                        importedClient = FoundOps.Api.Models.Client.ConvertModel(existingClient.Value);
                        importedClient.StatusInt = (int)ImportStatus.Linked;
                    }
                    else
                    {
                        importedClient = new FoundOps.Api.Models.Client
                        {
                            Id = Guid.NewGuid(),
                            Name = clientName,
                            ContactInfoSet = new List<ContactInfo>(),
                            StatusInt = (int)ImportStatus.New
                        };

                        //Since the client is new, we add it to the list of clients to be searched next time we try and match
                        _clients.GetOrAdd(importedClient.Id, FoundOps.Api.Models.Client.ConvertBack(importedClient));
                    }

                    importRow.Client = importedClient;
                }

                #endregion

                #region Repeat

                if (new[] { startDateCol, endDateCol, endAfterCol, repeatEveryCol, frequencyCol, frequencyDetailCol }.Any(col => col != -1))
                {
                    //Note: If no start date is passed, set it to today
                    var repeat = new Repeat
                    {
                        Id = Guid.NewGuid(),
                        StartDate = startDateCol == -1 || row[startDateCol] == "" ? Convert.ToDateTime(row[startDateCol]) : DateTime.UtcNow.Date,
                        EndDate = endDateCol != -1 && row[endDateCol] != "" ? Convert.ToDateTime(row[endDateCol]) : (DateTime?)null,
                        EndAfterTimes = endAfterCol != -1 && row[endAfterCol] != "" ? Convert.ToInt32(row[endAfterCol]) : (int?)null,
                        RepeatEveryTimes = repeatEveryCol != -1 && row[repeatEveryCol] != "" ? Convert.ToInt32(row[repeatEveryCol]) : (int?)null
                    };

                    #region Frequency

                    var val = frequencyCol != -1 ? row[frequencyCol].ToLower() : "";
                    switch (val)
                    {
                        case "o":
                        case "once":
                            repeat.FrequencyInt = (int)Frequency.Once;
                            break;
                        case "d":
                        case "daily":
                            repeat.FrequencyInt = (int)Frequency.Daily;
                            break;
                        case "w":
                        case "weekly":
                            repeat.FrequencyInt = (int)Frequency.Weekly;
                            break;
                        case "m":
                        case "monthly":
                            repeat.FrequencyInt = (int)Frequency.Monthly;
                            break;
                        case "y":
                        case "yearly":
                            repeat.FrequencyInt = (int)Frequency.Yearly;
                            break;
                        default:
                            repeat.FrequencyInt = null;
                            break;
                    }

                    #endregion

                    #region Frequency Detail

                    val = frequencyDetailCol != -1 ? row[frequencyDetailCol].ToLower() : "";
                    val = val.Replace(" ", "");
                    if (repeat.Frequency == Frequency.Weekly)
                    {
                        var startDayOfWeek = repeat.StartDate.DayOfWeek;

                        //If it is empty assume the Start Date
                        if (string.IsNullOrEmpty(val))
                            repeat.FrequencyDetailAsWeeklyFrequencyDetail = new[] { startDayOfWeek };
                        else
                        {
                            var dayStrings = val.Split(',');
                            var daysOfWeek = new List<DayOfWeek>();

                            if (dayStrings.Any(s => s == "s" || s == "su" || s == "sun" || s == "sunday"))
                                daysOfWeek.Add(DayOfWeek.Sunday);

                            if (dayStrings.Any(s => s == "m" || s == "mo" || s == "mon" || s == "monday"))
                                daysOfWeek.Add(DayOfWeek.Monday);

                            if (dayStrings.Any(s => s == "t" || s == "tu" || s == "tue" || s == "tues" || s == "tuesday"))
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
                        }
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
                            if (detailsAvailable.Count() > 1)
                                detailsAvailable.Remove(MonthlyFrequencyDetail.OnDayInMonth);
                            repeat.FrequencyDetailAsMonthlyFrequencyDetail = detailsAvailable.First();
                        }
                    }

                    #endregion

                    repeat.StatusInt = repeat.EndDate == null || repeat.EndAfterTimes == null ||
                                       repeat.RepeatEveryTimes == null || repeat.FrequencyInt == null
                                           ? (int)ImportStatus.Error
                                           : (int)ImportStatus.New;

                    importRow.Repeat = repeat;
                }

                #endregion

                #region Contact Info

                var phoneValueDictionary = phoneNumberValueCols.ToDictionary(p => Convert.ToInt32(p.Split(' ').ElementAt(2)), p => row[Array.IndexOf(headers, p)]);
                var phoneLabelDictionary = phoneNumberLabelCols.ToDictionary(p => Convert.ToInt32(p.Split(' ').ElementAt(2)), p => row[Array.IndexOf(headers, p)]);

                var emailValueDictionary = emailValueCols.ToDictionary(e => Convert.ToInt32(e.Split(' ').ElementAt(2)), e => row[Array.IndexOf(headers, e)]);
                var emailLabelDictionary = emailLabelCols.ToDictionary(e => Convert.ToInt32(e.Split(' ').ElementAt(2)), e => row[Array.IndexOf(headers, e)]);

                var websiteValueDictionary = websiteValueCols.ToDictionary(w => Convert.ToInt32(w.Split(' ').ElementAt(2)), w => row[Array.IndexOf(headers, w)]);
                var websiteLabelDictionary = websiteLabelCols.ToDictionary(w => Convert.ToInt32(w.Split(' ').ElementAt(2)), w => row[Array.IndexOf(headers, w)]);

                var otherValueDictionary = otherValueCols.ToDictionary(o => Convert.ToInt32(o.Split(' ').ElementAt(2)), o => row[Array.IndexOf(headers, o)]);
                var otherLabelDictionary = otherLabelCols.ToDictionary(o => Convert.ToInt32(o.Split(' ').ElementAt(2)), o => row[Array.IndexOf(headers, o)]);

                //Find which type of contact info is being imported the most
                //This way we only have one loop
                var max = Math.Max(Math.Max(phoneLabelDictionary.Count, emailLabelDictionary.Count), Math.Max(websiteLabelDictionary.Count, otherLabelDictionary.Count));

                var concurrentContactInfoDictionary = new ConcurrentDictionary<Guid, ContactInfo>();

                Parallel.For((long)1, max + 1, contactIndex =>
                {
                    //Phone
                    if (phoneLabelDictionary.Count >= contactIndex)
                        MatchContactInfo(phoneLabelDictionary, phoneValueDictionary, contactIndex, concurrentContactInfoDictionary, "Phone Number");

                    //Email
                    if (emailLabelDictionary.Count >= contactIndex)
                        MatchContactInfo(emailLabelDictionary, emailValueDictionary, contactIndex, concurrentContactInfoDictionary, "Email Address");

                    //Website
                    if (websiteLabelDictionary.Count >= contactIndex)
                        MatchContactInfo(websiteLabelDictionary, websiteValueDictionary, contactIndex, concurrentContactInfoDictionary, "Website");

                    //Other
                    if (otherLabelDictionary.Count >= contactIndex)
                        MatchContactInfo(otherLabelDictionary, otherValueDictionary, contactIndex, concurrentContactInfoDictionary, "Other");
                });

                //Once all the contact info sets are made or matched, add them to the ImportRow
                importRow.ContactInfoSet.AddRange(concurrentContactInfoDictionary.Select(ci => ci.Value));

                #endregion

                concurrentDictionary.GetOrAdd((int)rowIndex, importRow);
            });

            var importRows = concurrentDictionary.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value).ToArray();

            var suggestion = SuggestEntites(importRows);

            return suggestion;
        }

        /// <summary>
        /// Suggests other potential options based on the entities passed
        /// </summary>
        /// <param name="rows">The rows being imported</param>
        /// <returns>Entites with suggestions</returns>
        public Suggestions SuggestEntites(ImportRow[] rows)
        {
            var concurrentDictionary = new ConcurrentDictionary<int, RowSuggestions>();

            var suggestionToReturn = new Suggestions();
            var clients = new List<FoundOps.Api.Models.Client>();
            var locations = new List<FoundOps.Api.Models.Location>();
            var contactInfoSets = new ConcurrentDictionary<Guid, ContactInfo>();

            Parallel.For((long)0, rows.Count(), rowIndex =>
            {
                var row = rows[rowIndex];

                var rowSuggestions = new RowSuggestions();

                #region Location

                if (row.Location != null)
                {
                    //Add the matched/new location as the first suggestion
                    rowSuggestions.LocationSuggestions.Add(row.Location.Id);

                    //Find all the Locations to be suggested by finding all Locations for the Client of the row
                    var locationSuggestions = row.Client != null
                        ? _locations.Where(l => l.Value.ClientId == row.Client.Id || l.Key == row.Location.Id).ToArray()
                        : _locations.Where(l => l.Key == row.Location.Id).ToArray();

                    rowSuggestions.LocationSuggestions.AddRange(locationSuggestions.Select(l => l.Key).Where(k => k != row.Location.Id));

                    //Add all suggested Locations to the list of Locations to be returned
                    locations.AddRange(locationSuggestions.Select(l => l.Value).Select(FoundOps.Api.Models.Location.ConvertModel));

                    //If a new Location was created, add it to the list of location entites
                    if (!_locations.Select(l => l.Key).Contains(row.Location.Id))
                        locations.Add(row.Location);
                }

                #endregion

                #region Client

                if (row.Client != null)
                {
                    //Add the matched/new client as the first suggestion
                    rowSuggestions.ClientSuggestions.Add(row.Client.Id);

                    //Find all the Clients to be suggested by finding all Clients for the Location of the row
                    var clientSuggestions = row.Location != null
                        ? _clients.Where(c => c.Key == row.Location.ClientId || c.Key == row.Client.Id).ToArray()
                        : _clients.Where(c => c.Key == row.Client.Id).ToArray();

                    rowSuggestions.ClientSuggestions.AddRange(clientSuggestions.Select(c => c.Key).Where(k => k != row.Client.Id));

                    //Add all suggested Clients to the list of Clients to be returned
                    clients.AddRange(clientSuggestions.Select(c => c.Value).Select(FoundOps.Api.Models.Client.ConvertModel));

                    //If a new Client was created, add it to the list of client entites
                    if (!_clients.Select(c => c.Key).Contains(row.Client.Id))
                        clients.Add(row.Client);
                }

                #endregion

                //Repeat
                if (row.Repeat != null)
                    rowSuggestions.Repeats.Add(row.Repeat);

                //Contact Info
                if (row.ContactInfoSet.Count != 0)
                {
                    rowSuggestions.ContactInfoSuggestions.AddRange(row.ContactInfoSet.Select(ci => ci.Id));

                    foreach (var contactInfoSet in row.ContactInfoSet)
                        contactInfoSets.GetOrAdd(contactInfoSet.Id, contactInfoSet);
                }
                    //rowSuggestions.ContactInfoSuggestions.AddRange(row.ContactInfoSet);

                //Add this row's suggestions to the list to be returned
                concurrentDictionary.GetOrAdd((int)rowIndex, rowSuggestions);
            });

            suggestionToReturn.RowSuggestions.AddRange(concurrentDictionary.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value));

            //Only add distinct Clients
            var distinctClients = clients.Distinct();
            suggestionToReturn.Clients.AddRange(distinctClients);

            //Only add distinct Locations
            var distinctLocations = locations.Distinct();
            suggestionToReturn.Locations.AddRange(distinctLocations);

            //Only add distinct ContactInfo
            var distinctContactInfo = contactInfoSets.Select(ci => ci.Value).Distinct();
            suggestionToReturn.ContactInfoSet.AddRange(distinctContactInfo);

            return suggestionToReturn;
        }

        #region Helpers

        /// <summary>
        /// Serves three functions. 
        /// 1) Convert the FoundOPS location to the API model.
        /// 2) Set the Region on the location if it exists.
        /// 3) Set the Status of the Location.
        /// </summary>
        /// <param name="matchedLocation">Location to be converted</param>
        /// <param name="regionName">The name of the region the be added to the Locations</param>
        /// <param name="status">The status to be assigned to the Location</param>
        /// <returns>An API location to be imported</returns>
        private Models.Location ConvertLocationSetRegionAndStatus(Location matchedLocation, string regionName, ImportStatus status)
        {
            var location = Api.Models.Location.ConvertModel(matchedLocation);

            location.Region = SetRegion(regionName);

            location.StatusInt = (int)status;

            return location;
        }

        private void MatchContactInfo(Dictionary<int, string> labelDictionary, Dictionary<int, string> valueDictionary, long index, ConcurrentDictionary<Guid, ContactInfo> contactInfoDictionary, string type)
        {
            var label = labelDictionary.First(p => p.Key == index).Value.Trim();
            var data = valueDictionary.First(p => p.Key == index).Value.Trim();

            var existingContact = _contactInfoSet.FirstOrDefault(ci => ci.Value.Label == label && ci.Value.Data == data);

            if (existingContact.Key != Guid.Empty)
                contactInfoDictionary.GetOrAdd(existingContact.Value.Id, ContactInfo.Convert(existingContact.Value));
            else
            {
                var newContactInfo = new ContactInfo
                {
                    Id = Guid.NewGuid(),
                    Data = data,
                    Label = label,
                    Type = type
                };
                //Add the new contact info to the list being imported
                contactInfoDictionary.GetOrAdd(newContactInfo.Id, newContactInfo);

                //Since the contact info is new, we add it to the contact info set to be searched next time we try and match
                _contactInfoSet.GetOrAdd(newContactInfo.Id, ContactInfo.ConvertBack(newContactInfo));
            }
        }

        /// <summary>
        /// Find the correct region(if it exists) for the location
        /// </summary>
        /// <param name="regionName">The name of the region to be set</param>
        /// <returns>The region if it exists</returns>
        private Region SetRegion(string regionName)
        {
            if (regionName != null)
            {
                var region = _regions.Select(r => r.Value).FirstOrDefault(r => r.Name == regionName);

                if (region != null)
                    return Region.Convert(region);
            }

            return null;
        }

        #endregion
    }
}

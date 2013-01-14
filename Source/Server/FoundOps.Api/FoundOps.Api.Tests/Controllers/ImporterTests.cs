using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using FoundOps.Api.Controllers.Rest;
using FoundOps.Api.Models;
using FoundOps.Core.Models.CoreEntities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Client = FoundOps.Core.Models.CoreEntities.Client;
using ContactInfo = FoundOps.Api.Models.ContactInfo;
using Location = FoundOps.Core.Models.CoreEntities.Location;

namespace FoundOps.Api.Tests.Controllers
{
    [TestClass]
    public class ImporterTests
    {
        protected readonly CoreEntitiesContainer CoreEntitiesContainer;
        private static readonly DateTime Today = DateTime.UtcNow.Date;

        private readonly Guid _roleId;
        private readonly Guid _gotGreaseId = new Guid("8528E50D-E2B9-4779-9B29-759DBEA53B61");
        private readonly Guid _foundOpsId = new Guid("5606A728-B99F-4AA1-B0CD-0AB38A649000");

        public ImporterTests()
        {
            CoreEntitiesContainer = new CoreEntitiesContainer();
            CoreEntitiesContainer.ContextOptions.LazyLoadingEnabled = false;
            //Admin role on GotGrease?
            _roleId = CoreEntitiesContainer.Roles.First(r => r.OwnerBusinessAccountId == _gotGreaseId && r.RoleTypeInt == (int)RoleType.Administrator).Id;
        }

        [TestMethod]
        public void ImporterSuggestionsTests()
        {
            #region ValidateInput

            //Importing Client, Location and Repeat for each row
            //Tests all combinations of new/existing entities
            TestValidateAndSuggest(importClients: true, newClient: true, importLocations: true, newLocation: true, importContactInfo: true, newContactInfo: true, importRepeats: true, testValidateInput: true, testSuggestEntites: false);
            TestValidateAndSuggest(importClients: true, newClient: false, importLocations: true, newLocation: false, importContactInfo: true, newContactInfo: true, importRepeats: true, testValidateInput: true, testSuggestEntites: false);
            TestValidateAndSuggest(importClients: true, newClient: false, importLocations: true, newLocation: false, importContactInfo: true, newContactInfo: false, importRepeats: true, testValidateInput: true, testSuggestEntites: false);
            TestValidateAndSuggest(importClients: true, newClient: true, importLocations: true, newLocation: false, importContactInfo: true, newContactInfo: true, importRepeats: true, testValidateInput: true, testSuggestEntites: false);
            TestValidateAndSuggest(importClients: true, newClient: true, importLocations: true, newLocation: false, importContactInfo: true, newContactInfo: false, importRepeats: true, testValidateInput: true, testSuggestEntites: false);
            TestValidateAndSuggest(importClients: true, newClient: false, importLocations: true, newLocation: true, importContactInfo: true, newContactInfo: true, importRepeats: true, testValidateInput: true, testSuggestEntites: false);
            TestValidateAndSuggest(importClients: true, newClient: false, importLocations: true, newLocation: true, importContactInfo: true, newContactInfo: false, importRepeats: true, testValidateInput: true, testSuggestEntites: false);
            
            TestValidateAndSuggest(importClients: true, newClient: false, importLocations: true, newLocation: false, importContactInfo: false, newContactInfo: false, importRepeats: true, testValidateInput: true, testSuggestEntites: false);

            //Importing a Client and Location
            TestValidateAndSuggest(importClients: true, newClient: false, importLocations: true, newLocation: false, importContactInfo: true, newContactInfo: false, importRepeats: false, testValidateInput: true, testSuggestEntites: false);
            TestValidateAndSuggest(importClients: true, newClient: false, importLocations: true, newLocation: false, importContactInfo: false, newContactInfo: false, importRepeats: false, testValidateInput: true, testSuggestEntites: false);

            //Importing a Client and a Repeat
            TestValidateAndSuggest(importClients: true, newClient: false, importLocations: false, newLocation: false, importContactInfo: true, newContactInfo: false, importRepeats: true, testValidateInput: true, testSuggestEntites: false);
            TestValidateAndSuggest(importClients: true, newClient: false, importLocations: false, newLocation: false, importContactInfo: false, newContactInfo: false, importRepeats: true, testValidateInput: true, testSuggestEntites: false);

            //Importing a Location and a Repeat
            TestValidateAndSuggest(importClients: false, newClient: false, importLocations: true, newLocation: false, importContactInfo: true, newContactInfo: false, importRepeats: true, testValidateInput: true, testSuggestEntites: false);
            TestValidateAndSuggest(importClients: false, newClient: false, importLocations: true, newLocation: false, importContactInfo: false, newContactInfo: false, importRepeats: true, testValidateInput: true, testSuggestEntites: false);

            //Importing only a Client
            TestValidateAndSuggest(importClients: true, newClient: false, importLocations: false, newLocation: false, importContactInfo: true, newContactInfo: false, importRepeats: false, testValidateInput: true, testSuggestEntites: false);
            TestValidateAndSuggest(importClients: true, newClient: false, importLocations: false, newLocation: false, importContactInfo: false, newContactInfo: false, importRepeats: false, testValidateInput: true, testSuggestEntites: false);

            //Importing only a Location
            TestValidateAndSuggest(importClients: false, newClient: false, importLocations: true, newLocation: false, importContactInfo: true, newContactInfo: false, importRepeats: false, testValidateInput: true, testSuggestEntites: false);
            TestValidateAndSuggest(importClients: false, newClient: false, importLocations: true, newLocation: false, importContactInfo: false, newContactInfo: false, importRepeats: false, testValidateInput: true, testSuggestEntites: false);

            //Importing only a Repeat
            TestValidateAndSuggest(importClients: false, newClient: false, importLocations: false, newLocation: false, importContactInfo: true, newContactInfo: false, importRepeats: true, testValidateInput: true, testSuggestEntites: false);
            TestValidateAndSuggest(importClients: false, newClient: false, importLocations: false, newLocation: false, importContactInfo: false, newContactInfo: false, importRepeats: true, testValidateInput: true, testSuggestEntites: false);

            #endregion

            #region SuggestEntites

            //Importing Client, Location and Repeat for each row
            //Tests all combinations of new/existing entities
            TestValidateAndSuggest(importClients: true, newClient: true, importLocations: true, newLocation: true, importContactInfo: true, newContactInfo: true, importRepeats: true, testValidateInput: false, testSuggestEntites: true);
            TestValidateAndSuggest(importClients: true, newClient: false, importLocations: true, newLocation: false, importContactInfo: true, newContactInfo: true, importRepeats: true, testValidateInput: false, testSuggestEntites: true);
            TestValidateAndSuggest(importClients: true, newClient: false, importLocations: true, newLocation: false, importContactInfo: true, newContactInfo: false, importRepeats: true, testValidateInput: false, testSuggestEntites: true);
            TestValidateAndSuggest(importClients: true, newClient: true, importLocations: true, newLocation: false, importContactInfo: true, newContactInfo: true, importRepeats: true, testValidateInput: false, testSuggestEntites: true);
            TestValidateAndSuggest(importClients: true, newClient: true, importLocations: true, newLocation: false, importContactInfo: true, newContactInfo: false, importRepeats: true, testValidateInput: false, testSuggestEntites: true);
            TestValidateAndSuggest(importClients: true, newClient: false, importLocations: true, newLocation: true, importContactInfo: true, newContactInfo: true, importRepeats: true, testValidateInput: false, testSuggestEntites: true);
            TestValidateAndSuggest(importClients: true, newClient: false, importLocations: true, newLocation: true, importContactInfo: true, newContactInfo: false, importRepeats: true, testValidateInput: false, testSuggestEntites: true);

            TestValidateAndSuggest(importClients: true, newClient: false, importLocations: true, newLocation: false, importContactInfo: false, newContactInfo: false, importRepeats: true, testValidateInput: false, testSuggestEntites: true);

            //Importing a Client and Location
            TestValidateAndSuggest(importClients: true, newClient: false, importLocations: true, newLocation: false, importContactInfo: true, newContactInfo: false, importRepeats: false, testValidateInput: false, testSuggestEntites: true);
            TestValidateAndSuggest(importClients: true, newClient: false, importLocations: true, newLocation: false, importContactInfo: false, newContactInfo: false, importRepeats: false, testValidateInput: false, testSuggestEntites: true);

            //Importing a Client and a Repeat
            TestValidateAndSuggest(importClients: true, newClient: false, importLocations: false, newLocation: false, importContactInfo: true, newContactInfo: false, importRepeats: true, testValidateInput: false, testSuggestEntites: true);
            TestValidateAndSuggest(importClients: true, newClient: false, importLocations: false, newLocation: false, importContactInfo: false, newContactInfo: false, importRepeats: true, testValidateInput: false, testSuggestEntites: true);

            //Importing a Location and a Repeat
            TestValidateAndSuggest(importClients: false, newClient: false, importLocations: true, newLocation: false, importContactInfo: true, newContactInfo: false, importRepeats: true, testValidateInput: false, testSuggestEntites: true);
            TestValidateAndSuggest(importClients: false, newClient: false, importLocations: true, newLocation: false, importContactInfo: false, newContactInfo: false, importRepeats: true, testValidateInput: false, testSuggestEntites: true);

            //Importing only a Client
            TestValidateAndSuggest(importClients: true, newClient: false, importLocations: false, newLocation: false, importContactInfo: true, newContactInfo: false, importRepeats: false, testValidateInput: false, testSuggestEntites: true);
            TestValidateAndSuggest(importClients: true, newClient: false, importLocations: false, newLocation: false, importContactInfo: false, newContactInfo: false, importRepeats: false, testValidateInput: false, testSuggestEntites: true);

            //Importing only a Location
            TestValidateAndSuggest(importClients: false, newClient: false, importLocations: true, newLocation: false, importContactInfo: true, newContactInfo: false, importRepeats: false, testValidateInput: false, testSuggestEntites: true);
            TestValidateAndSuggest(importClients: false, newClient: false, importLocations: true, newLocation: false, importContactInfo: false, newContactInfo: false, importRepeats: false, testValidateInput: false, testSuggestEntites: true);

            //Importing only a Repeat
            TestValidateAndSuggest(importClients: false, newClient: false, importLocations: false, newLocation: false, importContactInfo: true, newContactInfo: false, importRepeats: true, testValidateInput: false, testSuggestEntites: true);
            TestValidateAndSuggest(importClients: false, newClient: false, importLocations: false, newLocation: false, importContactInfo: false, newContactInfo: false, importRepeats: true, testValidateInput: false, testSuggestEntites: true);

            #endregion
        }

        #region Helpers

        private void TestValidateAndSuggest(bool importClients, bool newClient, bool importLocations, bool newLocation, bool importContactInfo, bool newContactInfo, bool importRepeats, bool testValidateInput, bool testSuggestEntites)
        {
            var controller = new SuggestionsController();

            //If both are false, or both are true => return because this would cause an exception to occur in the controller
            if (testValidateInput == testSuggestEntites)
                return;

            //If testValidateInput is true, make a new SuggestionRequest with RowsWithHeaders only
            //Else, make a new SuggestionRequest with Rows only
            var suggestionRequest = testValidateInput
                ? new SuggestionsRequest { RowsWithHeaders = SetupRowsWithHeaders(importClients, newClient, importLocations, newLocation, importContactInfo, newContactInfo, importRepeats) }
                : new SuggestionsRequest { Rows = SetupRows(importClients, newClient, importLocations, newLocation, importContactInfo, newContactInfo, importRepeats) };

            var suggestions = controller.Put(_roleId, suggestionRequest);

            if (importClients)
            {
                //Test Client output
                var clientSuggestions = suggestions.RowSuggestions.SelectMany(rs => rs.ClientSuggestions).Distinct().ToArray();
                var clients = suggestions.Clients.Select(c => c.Id).ToArray();
                var except = clients.Except(clientSuggestions);
                Assert.AreEqual(0, except.Count());
            }
            if (importLocations)
            {
                //Test Location output
                var locationSuggestions = suggestions.RowSuggestions.SelectMany(rs => rs.LocationSuggestions).Distinct().ToArray();
                var locations = suggestions.Locations.Select(l => l.Id).ToArray();
                var except = locations.Except(locationSuggestions);
                Assert.AreEqual(0, except.Count());
            }
            if (importRepeats)
            {
                //Test Repeat output
                var repeats = suggestions.RowSuggestions.Select(rs => rs.Repeat);
                Assert.AreEqual(suggestions.RowSuggestions.Count(), repeats.Count());
            }
            if (importContactInfo)
            {
                var contactInfoSuggestions = suggestions.RowSuggestions.SelectMany(rs => rs.ContactInfoSuggestions).Distinct().ToArray();
                var contactInfoSets = suggestions.ContactInfoSet.Select(l => l.Id).ToArray();
                var except = contactInfoSets.Except(contactInfoSuggestions);
                Assert.AreEqual(0, except.Count());
            }
        }

        /// <summary>
        /// Setup a fake import using random existing Clients, Locations and Repeats
        /// </summary>
        /// <param name="importClients">Whether or not to import Clients</param>
        /// <param name="newClient">Whether or not to create a new Client</param>
        /// <param name="importLocations">Whether or not to import Locations</param>
        /// <param name="newLocation">Whether or not to create a new Location</param>
        /// <param name="importContactInfo">Whether or not to import ContactInfo</param>
        /// <param name="newContactInfo">Whether or not to create a new ContactInfoSet</param>
        /// <param name="importRepeats">Whether or not to import Repeats</param>
        /// <returns></returns>
        private ImportRow[] SetupRows(bool importClients, bool newClient, bool importLocations, bool newLocation, bool importContactInfo, bool newContactInfo, bool importRepeats)
        {
            var random = new Random();
            var rows = new List<ImportRow>();
            
            var newRow = new ImportRow();

            if (importClients)
            {
                var client = newClient
                    ? new Client { Name = "Test Client" }
                    : CoreEntitiesContainer.Clients.ToArray().ElementAt(random.Next(48));

                newRow.Client = Api.Models.Client.ConvertModel(client);
            }
            if (importLocations)
            {
                var location = newLocation
                ? new Location { AddressLineOne = "2827 Floral Drive", AddressLineTwo = "Linen Closet", AdminDistrictTwo = "Northbrook", AdminDistrictOne = "IL", PostalCode = "60062", CountryCode = "US" }
                : CoreEntitiesContainer.Locations.Where(l => l.BusinessAccountIdIfDepot == null).Include(l => l.Region).ToArray().ElementAt(random.Next(51));

                newRow.Location = Api.Models.Location.ConvertModel(location);
            }
            if (importContactInfo)
            {
                var phoneContactInfo = newContactInfo && (newLocation || newClient)
                ? new Core.Models.CoreEntities.ContactInfo { Id = Guid.NewGuid(), Type = "Phone", Label = "New Phone", Data = "(123) 345-6789" }
                : CoreEntitiesContainer.ContactInfoSet.Where(c => c.Type == "Phone").ToArray().ElementAt(random.Next(70));

                var emailContactInfo = newContactInfo && (newLocation || newClient)
                    ? new Core.Models.CoreEntities.ContactInfo { Id = Guid.NewGuid(), Type = "Email", Label = "New Email", Data = "fake@foundops.com" }
                    : CoreEntitiesContainer.ContactInfoSet.Where(c => c.Type == "Email").ToArray().ElementAt(random.Next(70));

                var websiteContactInfo = newContactInfo && (newLocation || newClient)
                    ? new Core.Models.CoreEntities.ContactInfo { Id = Guid.NewGuid(), Type = "Website", Label = "New Website", Data = "foundops.com" }
                    : CoreEntitiesContainer.ContactInfoSet.Where(c => c.Type == "Website").ToArray().ElementAt(random.Next(70));

                newRow.ContactInfoSet.AddRange(new List<ContactInfo> { ContactInfo.Convert(phoneContactInfo), ContactInfo.Convert(emailContactInfo), ContactInfo.Convert(websiteContactInfo) });
            }
            if (importRepeats)
            {
                var repeat = CoreEntitiesContainer.Repeats.Where(r => r.FrequencyInt == 3).ToArray().ElementAt(random.Next(144));

                newRow.Repeat = Api.Models.Repeat.ConvertModel(repeat);
            }

            rows.Add(newRow);

            return rows.ToArray();
        }

        /// <summary>
        /// Setup a fake import using random existing Clients, Locations and Repeats
        /// </summary>
        /// <param name="importClients">Whether or not to import Clients</param>
        /// <param name="newClient">Whether or not to create a new Client</param>
        /// <param name="importLocations">Whether or not to import Locations</param>
        /// <param name="newLocation">Whether or not to create a new Location</param>
        /// <param name="importContactInfo">Whether or not to import ContactInfo</param>
        /// <param name="newContactInfo">Whether or not to create a new ContactInfoSet</param>
        /// <param name="importRepeats">Whether or not to import Repeats</param>
        /// <returns>A list of string[] to be used in ValidateThenSuggestEntities</returns>
        private List<string[]> SetupRowsWithHeaders(bool importClients, bool newClient, bool importLocations, bool newLocation, bool importContactInfo, bool newContactInfo, bool importRepeats)
        {
            var headers = new List<string>();

            if (importClients)
                headers.Add("Client Name");
            if (importLocations)
            {
                headers.AddRange(new[] { "Address Line One", "Address Line Two", "AdminDistrictTwo", "AdminDistrictOne", "PostalCode", "Country Code" });
            }
            if (importContactInfo)
            {
                headers.AddRange(new[] { "Phone Number 1", "Phone Label 1" });
                headers.AddRange(new[] { "Email Address 1", "Email Label 1" });
                headers.AddRange(new[] { "Website Url 1", "Website Label 1" });
            }
            if (importRepeats)
            {
                headers.AddRange(new string[] { "Frequency", "Repeat Every", "Start Date", "End Date", "End After Times", "Frequency Detail" });
            }

            var rowsWithHeaders = new List<string[]> { headers.ToArray() };

            var random = new Random();

            var newRow = new List<string>();

            if (importClients)
            {
                var client = newClient
                    ? new Client { Name = "Test Client" }
                    : CoreEntitiesContainer.Clients.ToArray().ElementAt(random.Next(48));
                
                newRow.Add(client.Name);
            }
            if (importLocations)
            {
                var location = newLocation
                ? new Location { AddressLineOne = "2827 Floral Drive", AddressLineTwo = "Linen Closet", AdminDistrictTwo = "Northbrook", AdminDistrictOne = "IL", PostalCode = "60062", CountryCode = "US" }
                : CoreEntitiesContainer.Locations.Where(l => l.BusinessAccountIdIfDepot == null).Include(l => l.Region).ToArray().ElementAt(random.Next(51));

                newRow.AddRange(new[] { location.AddressLineOne, location.AddressLineTwo, location.AdminDistrictTwo, location.AdminDistrictOne, location.PostalCode, location.CountryCode });

                if (location.Region != null)
                    newRow.Add(location.Region.Name);
            }
            if (importContactInfo)
            {
                var phoneContactInfo = newContactInfo && (newLocation || newClient)
                ? new Core.Models.CoreEntities.ContactInfo { Id = Guid.NewGuid(), Type = "Phone", Label = "New Phone", Data = "(123) 345-6789" }
                : CoreEntitiesContainer.ContactInfoSet.Where(c => c.Type == "Phone").ToArray().ElementAt(random.Next(70));

                var emailContactInfo = newContactInfo && (newLocation || newClient)
                    ? new Core.Models.CoreEntities.ContactInfo { Id = Guid.NewGuid(), Type = "Email", Label = "New Email", Data = "fake@foundops.com" }
                    : CoreEntitiesContainer.ContactInfoSet.Where(c => c.Type == "Email").ToArray().ElementAt(random.Next(70));

                var websiteContactInfo = newContactInfo && (newLocation || newClient)
                    ? new Core.Models.CoreEntities.ContactInfo { Id = Guid.NewGuid(), Type = "Website", Label = "New Website", Data = "foundops.com" }
                    : CoreEntitiesContainer.ContactInfoSet.Where(c => c.Type == "Website").ToArray().ElementAt(random.Next(70));

                newRow.AddRange(new[] { phoneContactInfo.Data, phoneContactInfo.Label, emailContactInfo.Data, emailContactInfo.Label, websiteContactInfo.Data, websiteContactInfo.Label });
            }
            if (importRepeats)
            {
                var repeat = CoreEntitiesContainer.Repeats.Where(r => r.FrequencyInt == 3).ToArray().ElementAt(random.Next(144));

                newRow.AddRange(new[] { repeat.Frequency.ToString(), repeat.RepeatEveryTimes.ToString(), repeat.StartDate.ToString(), repeat.EndDate.ToString(), repeat.EndAfterTimes.ToString(), repeat.FrequencyDetailAsWeeklyFrequencyDetail.First().ToString() });
            }
            rowsWithHeaders.Add(newRow.ToArray());

            return rowsWithHeaders;
        }

        #endregion
    }
}

using System;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using Dapper;
using FoundOPS.API.Api;
using FoundOPS.API.Models;
using FoundOps.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace API.Tests.Controllers
{
    [TestClass]
    public class ResourceTests
    {
        //Admin role on GotGrease?
        public static readonly Guid RoleId = new Guid("8528E50D-E2B9-4779-9B29-759DBEA53B61");

        [TestMethod]
        public void TestLocations()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost");
            var controller = new LocationsController() { Request = request };

            var getResponseFromId = controller.Get(RoleId, new Guid("C0FA60DA-F736-455B-B9A1-9EB7D8E07769"));

            FoundOps.Core.Models.CoreEntities.Location location;

            using (var conn = new SqlConnection(ServerConstants.SqlConnectionString))
            {
                conn.Open();

                //Find the location with the desired Id
                location = conn.Query<FoundOps.Core.Models.CoreEntities.Location>("SELECT * FROM Locations WHERE Id = @Id", new { Id = new Guid("C0FA60DA-F736-455B-B9A1-9EB7D8E07769") }).FirstOrDefault();

                conn.Close();
            }

            var convertedLocation = Location.ConvertModel(location);

            if (!Tools.AreObjectsEqual(getResponseFromId, convertedLocation))
                throw new Exception("Objects are not equal. Check output window for details");

            var searchText = convertedLocation.AddressLineOne + ' ' + convertedLocation.AdminDistrictTwo + ' ' + convertedLocation.AdminDistrictOne + ' ' + convertedLocation.CountryCode;

            var getResponseFromSearch = controller.Get(RoleId, searchText).FirstOrDefault();

            if (!Tools.AreObjectsEqual(getResponseFromSearch, convertedLocation, new []{"Id", "Name", "AddressLineTwo"}))
                throw new Exception("Objects are not equal. Check output window for details");

            var newLocation = new Location
                {
                    Id = Guid.NewGuid(),
                    Name = "New Location",
                    AddressLineOne = "123 Fake Street",
                    AddressLineTwo = "Apt ???",
                    AdminDistrictTwo = "Nowheresville",
                    AdminDistrictOne = "Atlantis",
                    PostalCode = "012345",
                    CountryCode = "PO"
                };

            var postResponse = controller.Post(new Guid("8528E50D-E2B9-4779-9B29-759DBEA53B61"), newLocation);

            Assert.AreEqual(postResponse.StatusCode, HttpStatusCode.Accepted);

            newLocation.CountryCode = "WK";
            newLocation.AdminDistrictTwo = "I Have Changed";
            newLocation.AddressLineOne = "Not The Same as I Was";
            newLocation.AddressLineTwo = "Different";
            newLocation.PostalCode = "Not Even Postal Code";

            var putResponse = controller.Put(RoleId, newLocation);

            Assert.AreEqual(putResponse.StatusCode, HttpStatusCode.Accepted);
        }
    }
}

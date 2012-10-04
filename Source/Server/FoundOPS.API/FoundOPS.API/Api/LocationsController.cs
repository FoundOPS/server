using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
using System.Web;
using Dapper;
using FoundOps.Common.NET;
using FoundOps.Core.Models;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Tools;
using System;
using System.Linq;
using System.Web.Http;
using Location = FoundOPS.API.Models.Location;

namespace FoundOPS.API.Api
{
    [FoundOps.Core.Tools.Authorize]
    public class LocationsController : ApiController
    {
        private readonly CoreEntitiesContainer _coreEntitiesContainer;
        public LocationsController()
        {
            _coreEntitiesContainer = new CoreEntitiesContainer();
            _coreEntitiesContainer.ContextOptions.LazyLoadingEnabled = false;
        }

        /// <summary>
        /// Gets the Location with the desired Id
        /// </summary>
        /// <param name="roleId">The current roleId</param>
        /// <param name="locationId">The Id of the location to be returned</param>
        /// <returns>A location converted to the API model with the desired Id</returns>
        public Location[] Get(Guid roleId, Guid? locationId, Guid? clientId)
        {
#if !DEBUG
            //Check to be sure the user has correct abilities
            var currentBusinessAccount = _coreEntitiesContainer.Owner(roleId).FirstOrDefault();
            if (currentBusinessAccount == null)
                throw new HttpException(((int)HttpStatusCode.Unauthorized), "User is mobile only");
#endif
            FoundOps.Core.Models.CoreEntities.Location[] locations;

            var sql = "SELECT * FROM Locations WHERE Id = @Id";

            if (clientId != null)
                sql = "SELECT * FROM Locations WHERE ClientId = @Id";

            using (var conn = new SqlConnection(ServerConstants.SqlConnectionString))
            {
                conn.Open();

                //Find the location with the desired Id
                locations = conn.Query<FoundOps.Core.Models.CoreEntities.Location>(sql, new {Id = clientId ?? locationId}).ToArray();

                conn.Close();
            }
            
            //Convert the FoundOPS model to the API model
            return locations.Select(Location.ConvertModel).ToArray();
        }

        /// <summary>
        /// Tries to geocode based off of the search string
        /// </summary>
        /// <param name="roleId">The current roleId</param>
        /// <param name="search">String to be geocoded</param>
        /// <returns>A list of high confidence geocoded locations based on the search string</returns>
        public IEnumerable<Location> GetAllLocations(Guid roleId, string search)
        {

#if !DEBUG
            //Check to be sure the user has correct abilities
            var currentBusinessAccount = _coreEntitiesContainer.Owner(roleId).FirstOrDefault();
            if (currentBusinessAccount == null)
                throw new HttpException(((int)HttpStatusCode.Unauthorized), "User is mobile only");
#endif
            
            //Attempt to geocode, returns only high-confidence results
            var geocodeResult = BingLocationServices.TryGeocode(search);

            //Convert to the API model for locations
            return geocodeResult.Select(Location.ConvertGeocode);       
        }

        /// <summary>
        /// Insert a new location
        /// </summary>
        /// <param name="roleId">The current roleId</param>
        /// <param name="location">The location to be inserted</param>
        /// <returns>Http response to signify whether or not the location was inserted sucessfully</returns>
        public HttpResponseMessage Post(Guid roleId, Location location)
        {
#if !DEBUG
            //Check to be sure the user has admin abilities
            var currentBusinessAccount = _coreEntitiesContainer.Owner(roleId).FirstOrDefault();
            if (currentBusinessAccount == null)
                return Request.CreateResponse(HttpStatusCode.Unauthorized);
#else 
            var currentBusinessAccount = new BusinessAccount {Id = roleId};
#endif

            //Convert the API model back to the FoundOPS model of Locations
            var newLocation = Location.ConvertBack(location);

            //Update the locations business account
            newLocation.BusinessAccountId = currentBusinessAccount.Id;

            //Insert the location
            _coreEntitiesContainer.Locations.AddObject(newLocation);

            //Try to save changes, if an error occurs catch it and return an Http response with an error
            try
            {
                _coreEntitiesContainer.SaveChanges();
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.NotAcceptable, "There was an error while saving. Please check your inputs");
            }

            //If the insert is successful, return Accepted Http response
            return Request.CreateResponse(HttpStatusCode.Accepted);
        }

        /// <summary>
        /// Update a location
        /// </summary>
        /// <param name="roleId">The current roleId</param>
        /// <param name="location">The updated location model to be saved</param>
        /// <returns>Http response to signify whether or not the location was updated sucessfully</returns>
        public HttpResponseMessage Put(Guid roleId, Location location)
        {
#if !DEBUG
            //Check to be sure the user has admin abilities
            var currentBusinessAccount = _coreEntitiesContainer.Owner(roleId).FirstOrDefault();
            if (currentBusinessAccount == null)
                return Request.CreateResponse(HttpStatusCode.Unauthorized);
#endif

            //Find the original location to be updated
            var original = _coreEntitiesContainer.Locations.FirstOrDefault(l => l.Id == location.Id);

            //If no location exists, return Http response with an error
            if (original == null)
                return Request.CreateResponse(HttpStatusCode.NotAcceptable, "You are trying to update an object the does not exist yet!");

            //Update all properties on the original location
            original.AddressLineOne = location.AddressLineOne;
            original.AddressLineTwo = location.AddressLineTwo;
            original.AdminDistrictOne = location.AdminDistrictOne;
            original.AdminDistrictTwo = location.AdminDistrictTwo;
            original.PostalCode = location.PostalCode;
            original.CountryCode= location.CountryCode;

            //Try to save changes, if an error occurs catch it and return an Http response with an error
            try
            {
                _coreEntitiesContainer.SaveChanges();
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.NotAcceptable, "There was an error while saving. Please check your inputs");
            }

            //If the update is successful, return Accepted Http response
            return Request.CreateResponse(HttpStatusCode.Accepted);
        }   
    }
}

using System;
using System.Data.Entity;
using System.Linq;
using FoundOps.Api.Tools;
using FoundOps.Core.Tools;
using Location = FoundOps.Api.Models.Location;
using LocationField = FoundOps.Api.Models.LocationField;

namespace FoundOps.Api.Controllers.Rest
{
    public class LocationFieldsController : BaseApiController
    {
        /// <summary>
        /// Updates an existing LocationField with a new Location
        /// It will add the new Location to the Client passed if it does not already belong to it
        /// It will update the Location's properties if it exists
        /// It will also check for RouteTasks/RouteDestinations that belong to the Service and update their LocationId and ClientId
        /// </summary>
        /// <param name="roleId">The role</param>
        /// <param name="locationField">The LocationField being updated</param>
        /// <param name="serviceId">The service Id</param>
        /// <param name="clientId">The Id of the client that this location belongs to</param>
        public void Put(Guid roleId, LocationField locationField, Guid serviceId, Guid clientId)
        {
            //Makes sure the user is an Administrator or Regular
            var businessAccount = CoreEntitiesContainer.Owner(roleId).FirstOrDefault();
            if (businessAccount == null)
                throw Request.NotAuthorized();

            var existingField = CoreEntitiesContainer.Fields.OfType<FoundOps.Core.Models.CoreEntities.LocationField>().FirstOrDefault(f => f.Id == locationField.Id);

            if (existingField == null)
                throw Request.BadRequest("Field Doesn't Exist");

            var newLocation = Location.ConvertBack(locationField.Location); 

            //If the Location does not exist on the Client passed, add it
            var clientLocation = CoreEntitiesContainer.Locations.FirstOrDefault(l => l.ClientId == clientId && l.Id == locationField.Location.Id);
            if (clientLocation == null)
                newLocation.ClientId = clientId;
            
            //Updating the Location Field
            existingField.LocationId = newLocation.Id;
            existingField.Value = newLocation;

            var routeTasks = CoreEntitiesContainer.RouteTasks.Where(rt => rt.ServiceId == serviceId).Include(rt => rt.RouteDestination).ToArray();
            //If any route tasks exist, update their location and clientId
            //If any route tasks have been put into routes, update the route destination's location and ClientId
            foreach (var routeTask in routeTasks)
            {
                //Update the RouteTask's LocationId and clientId
                routeTask.LocationId = newLocation.Id;
                routeTask.ClientId = clientId;

                //If there is no RouteDestination for the RouteTask, move on to the next one
                if (routeTask.RouteDestination == null) continue;

                //Update the RouteDestination's LocationId and ClientId
                routeTask.RouteDestination.LocationId = newLocation.Id;
                routeTask.RouteDestination.ClientId = clientId;
            }

            //Save all changes
            SaveWithRetry();
        }
    }
}

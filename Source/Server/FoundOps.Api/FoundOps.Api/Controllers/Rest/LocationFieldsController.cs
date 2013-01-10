using System;
using System.Data.Entity;
using System.Linq;
using FoundOps.Api.Tools;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Models.CoreEntities.Extensions.Services;
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
        /// <param name="recurringServiceId">The RecurringService Id</param>
        /// <param name="clientId">The Id of the client that this location belongs to</param>
        /// <param name="occurDate">The date the Service occurs</param>
        public void Put(Guid roleId, LocationField locationField, Guid? serviceId, Guid? recurringServiceId, Guid clientId, DateTime occurDate)
        {
            //Makes sure the user is an Administrator or Regular
            var businessAccount = CoreEntitiesContainer.Owner(roleId).FirstOrDefault();
            if (businessAccount == null)
                throw Request.NotAuthorized();

            var existingService = CoreEntitiesContainer.Services.FirstOrDefault(s => s.Id == serviceId);

            RouteTask[] routeTasks;
            Core.Models.CoreEntities.LocationField destinationField = null;

            //Generate a Service since one does not yet exist
            if (existingService == null)
            {
                if (recurringServiceId == null)
                    throw Request.BadRequest("Service does not exist and no recurringServiceId was passed");

                //Include Client, Client.BusinessAccount and ServiceTemplate w/ fields
                var recurringService = CoreEntitiesContainer.RecurringServices.Where(rs => rs.Id == recurringServiceId)
                    .Include(rs => rs.Client).Include(rs => rs.Client.BusinessAccount).FirstOrDefault();

                if (recurringService == null)
                    throw Request.BadRequest("Service and RecurringService do not exist");

                HardCodedLoaders.LoadServiceTemplateWithDetails(CoreEntitiesContainer, recurringService.Id, null, null, null);

                //Since there wasnt previously a saved Service make one. Otherwise the service will get lost
                existingService = new Service
                {
                    ServiceDate = occurDate,
                    ClientId = recurringService.ClientId,
                    RecurringServiceId = recurringService.Id,
                    ServiceProviderId = recurringService.Client.BusinessAccount.Id,
                    ServiceTemplate = recurringService.ServiceTemplate.MakeChild(ServiceTemplateLevel.ServiceDefined),
                    CreatedDate = DateTime.UtcNow,
                    LastModified = DateTime.UtcNow,
                    LastModifyingUserId = CoreEntitiesContainer.CurrentUserAccount().Id
                };
                existingService.Id = existingService.ServiceTemplate.Id;

                destinationField = existingService.ServiceTemplate.GetDestinationField();
                
                //Load any RouteTasks on the occur date that belong to the RecurringService
                routeTasks = CoreEntitiesContainer.RouteTasks.Where(rt => rt.RecurringServiceId == recurringServiceId && rt.Date == occurDate).ToArray();

                //Add the RouteTasks to the Service
                foreach (var routeTask in routeTasks)
                    existingService.RouteTasks.Add(routeTask);                    
            }
            else
                routeTasks = CoreEntitiesContainer.RouteTasks.Where(rt => rt.ServiceId == serviceId).Include(rt => rt.RouteDestination).ToArray();                

            //Find the existing LocationField (unless it was just created/set above)
            if (destinationField != null)
                destinationField = CoreEntitiesContainer.Fields.OfType<FoundOps.Core.Models.CoreEntities.LocationField>().FirstOrDefault(f => f.ServiceTemplateId == existingService.Id);

            if (destinationField == null)
                throw Request.BadRequest("Field Does Not Exist");

            var newLocation = Location.ConvertBack(locationField.Value);

            //If the Location does not exist on the Client passed, add it
            var clientLocation = CoreEntitiesContainer.Locations.FirstOrDefault(l => l.ClientId == clientId && l.Id == locationField.Value.Id);
            if (clientLocation == null)
                newLocation.ClientId = clientId;

            //Updating the Location Field
            destinationField.Value = newLocation;

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

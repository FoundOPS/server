using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FoundOps.Core.Models.CoreEntities;
using Service = FoundOPS.API.Models.Service;

namespace FoundOPS.API.Controllers
{
    public class ServiceController : ApiController
    {
        private readonly CoreEntitiesContainer _coreEntitiesContainer = new CoreEntitiesContainer();

        #region GET

        /// <summary>
        /// Returns all detail information about the RouteTask based on the routeTaskId
        /// If the RouteTask doesnt have a Service associated with it yet, one with be generated for it based on the RecurringService
        /// TODO +Saves generated services
        /// </summary>
        /// <param name="routeTaskId"></param>
        /// <returns></returns>
        [AcceptVerbs("GET", "POST")]
        public IQueryable<Service> GetTaskDetails(Guid routeTaskId)
        {
#if !DEBUG
            //Load the RouteTasks, their Service and RecurringService
            //Make sure the user has access to the route task
            //by checking it's Route is available to the current user
            var routeTask = (AuthenticationLogic.CurrentUserAccountQueryable(_coreEntitiesContainer).SelectMany(cu => cu.LinkedEmployees)
            .SelectMany(e => e.Routes.Select(r => r.RouteDestinations.Select(rd => rd.RouteTasks.FirstOrDefault(rt => rt.Id == routeTaskId))))
                .First()).FirstOrDefault();

            if (routeTask == null)
                ExceptionHelper.ThrowNotAuthorizedBusinessAccount();
#endif
            var loadedRouteTask = _coreEntitiesContainer.RouteTasks.Where(rt => rt.Id == routeTaskId).Include("Service").Include("ParentRecurringService").FirstOrDefault();

            if (loadedRouteTask == null)
                throw new Exception("Bad Request, no RouteTask exists for that Id");

            var serviceIdToLoad = loadedRouteTask.ServiceId; //routeTasks.Where(rt => rt.ServiceId.HasValue).Select(rt => rt.ServiceId.Value).ToArray();
            var recurringServiceIdToLoad = loadedRouteTask.RecurringServiceId; //routeTasks.Where(rt => !rt.ServiceId.HasValue && rt.RecurringServiceId.HasValue).Select(rt => rt.RecurringServiceId.Value).ToArray();

            var modelServices = new List<FoundOps.Core.Models.CoreEntities.Service>();

            //Add the Existing Service to the model set
            if (serviceIdToLoad != null)
                modelServices.Add(_coreEntitiesContainer.Services.Include(s => s.Client).FirstOrDefault(s => s.Id == serviceIdToLoad));
            else
            {
                var recurringService = _coreEntitiesContainer.RecurringServices.Include(rs => rs.Client).FirstOrDefault(rs => rs.Id == recurringServiceIdToLoad);

                //Add the converted Recurring Service to the model set
                if (recurringService != null)
                {
                    //This will attach the new service to the context
                    var generatedService = GenerateServiceOnDate(loadedRouteTask.Date, recurringService);

                    modelServices.Add(generatedService);

                    var routeTasks = _coreEntitiesContainer.RouteTasks.Where(rt => rt.RecurringServiceId == generatedService.RecurringServiceId && rt.Date == generatedService.ServiceDate).ToArray();

                    foreach (var routeTask in routeTasks)
                        routeTask.Service = generatedService;
                }

                //Save the generated service
                _coreEntitiesContainer.SaveChanges();
            }

            //A service's id and a recurring service's id are the same id's as its service template
            var serviceTemplatesToLoad = serviceIdToLoad.HasValue ? serviceIdToLoad : recurringServiceIdToLoad;

            //Load the service templates with their details
            var templatesWithDetails = from serviceTemplate in _coreEntitiesContainer.ServiceTemplates.Where(st => serviceTemplatesToLoad == st.Id)
                                       from options in serviceTemplate.Fields.OfType<OptionsField>().Select(of => of.Options).DefaultIfEmpty()
                                       from locations in serviceTemplate.Fields.OfType<LocationField>().Select(lf => lf.Value).DefaultIfEmpty()
                                       select new { serviceTemplate, serviceTemplate.OwnerClient, serviceTemplate.Fields, options, locations };

            var apiServices = new List<Service>();

            //Convert the FoundOPS model Service to the API model Service
            apiServices.AddRange(modelServices.Select(Service.ConvertModel));

            return apiServices.AsQueryable();
        }

        /// <summary>
        /// Generates a Service based off this ServiceTemplate for the specified date.
        /// </summary>
        /// <param name="date">The date to generate a service for.</param>
        /// <param name="recurringService"> </param>
        public FoundOps.Core.Models.CoreEntities.Service GenerateServiceOnDate(DateTime date, RecurringService recurringService)
        {
            return new FoundOps.Core.Models.CoreEntities.Service
            {
                ServiceDate = date,
                ClientId = recurringService.ClientId,
                Client = recurringService.Client,
                RecurringServiceParent = recurringService,
                ServiceProviderId = recurringService.Client.BusinessAccount.Id,
                ServiceProvider = recurringService.Client.BusinessAccount,
                ServiceTemplate = recurringService.ServiceTemplate.MakeChild(ServiceTemplateLevel.ServiceDefined)
            };
        }

        #endregion

        #region POST
        /// <summary>
        /// Pushes changes made to a Service from the API model to the FoundOPS model 
        /// </summary>
        /// <param name="service">The API model of a Service</param>
        /// <returns></returns>
        [AcceptVerbs("POST")]
        public HttpResponseMessage UpdateServiceDetails(Service service)
        {
            var foundOpsService = _coreEntitiesContainer.Services.FirstOrDefault(s => s.Id == service.Id);

            //There should always be a service. When GetTaskDetails is called, it inserts any generated services
            if (foundOpsService == null)
                Request.CreateResponse(HttpStatusCode.BadRequest);

            //Load all Field information
            var templatesWithDetails = from serviceTemplate in _coreEntitiesContainer.ServiceTemplates.Where(st => foundOpsService.Id == st.Id)
                                       from options in serviceTemplate.Fields.OfType<OptionsField>().Select(of => of.Options).DefaultIfEmpty()
                                       from locations in serviceTemplate.Fields.OfType<LocationField>().Select(lf => lf.Value).DefaultIfEmpty()
                                       select new { serviceTemplate, serviceTemplate.OwnerClient, serviceTemplate.Fields, options, locations };

            #region Update all Fields

            foreach (var field in service.Fields)
            {
                var foundOpsField = foundOpsService.ServiceTemplate.Fields.FirstOrDefault(f => f.Id == field.Id);

                var dateTimeField = foundOpsField as DateTimeField;
                var numericField = foundOpsField as NumericField;
                var textBoxField = foundOpsField as TextBoxField;
                var locationField = foundOpsField as LocationField;
                var optionsField = foundOpsField as OptionsField;

                //If the field is a DateTimeField, update the value
                if (dateTimeField != null)
                    dateTimeField.Value = ((Models.DateTimeField)field).Value;

                //If the field is a NumericField, update the value
                if (numericField != null)
                    numericField.Value = ((Models.NumericField)field).Value;

                //If the field is a TextBoxField, update the value
                if (textBoxField != null)
                    textBoxField.Value = ((Models.TextBoxField)field).Value;

                //If the field is a OptionsField, update the Options
                if (optionsField != null)
                {
                    var foundOpsOptions = _coreEntitiesContainer.Options.Where(o => o.OptionsFieldId == optionsField.Id).ToArray();

                    if (foundOpsOptions.FirstOrDefault() == null)
                        continue;

                    //Cycle through each option and update the IsChecked value
                    foreach (var option in ((Models.OptionsField)field).Options)
                    {
                        var foundOpsOption = foundOpsOptions.FirstOrDefault(op => op.Id == option.Id);

                        if (foundOpsOption != null)
                            foundOpsOption.IsChecked = option.IsChecked;
                    }
                }

                //If the field is a LocationField, update the value
                if (locationField != null)
                {
                    locationField.LocationFieldTypeInt = ((Models.LocationField)field).LocationFieldTypeInt;
                    locationField.LocationId = ((Models.LocationField)field).LocationId;
                }
            }

            #endregion

            //Save any changes that were made
            _coreEntitiesContainer.SaveChanges();

            //Create the Http response 
            var response = Request.CreateResponse(HttpStatusCode.Accepted);
            response.Headers.Location = new Uri(Request.RequestUri, "/api/service/" + service.Id.ToString());

            //Return the response
            return response;
        }

        #endregion
    }
}
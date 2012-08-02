﻿using System.Data;
using System.Data.SqlClient;
using FoundOps.Common.NET;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Tools;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Service = FoundOPS.API.Models.Service;

namespace FoundOPS.API.Api
{
    [FoundOps.Core.Tools.Authorize]
    public class ServiceController : ApiController
    {
        private readonly CoreEntitiesContainer _coreEntitiesContainer;

        public ServiceController()
        {
            _coreEntitiesContainer = new CoreEntitiesContainer();
            _coreEntitiesContainer.ContextOptions.LazyLoadingEnabled = false;
        }

        #region GET

        [AcceptVerbs("GET", "POST")]
        public IQueryable<Dictionary<string, object>> GetServicesHoldersWithFields(Guid roleId, Guid? clientContext, Guid? recurringServiceContext,
            DateTime startDate, DateTime endDate)
        {
            var currentBusinessAccount = _coreEntitiesContainer.Owner(roleId).FirstOrDefault();

            if (currentBusinessAccount == null)
                throw new Exception("Hopefully this never happens...");

            var connectionString = ConfigWrapper.ConnectionString("CoreConnectionString");

            var command = new SqlCommand("GetServiceHoldersWithFields") { CommandType = CommandType.StoredProcedure };

            var serviceProviderIdContext = command.Parameters.Add("@serviceProviderIdContext", SqlDbType.UniqueIdentifier);
            serviceProviderIdContext.Value = currentBusinessAccount.Id;

            var clientIdContext = command.Parameters.Add("@clientIdContext", SqlDbType.UniqueIdentifier);
            if (clientContext.HasValue)
                clientIdContext.Value = clientContext.Value;
            else
                clientIdContext.Value = DBNull.Value;

            var recurringServiceIdContext = command.Parameters.Add("@recurringServiceIdContext", SqlDbType.UniqueIdentifier);
            if (recurringServiceContext.HasValue)
                recurringServiceIdContext.Value = recurringServiceContext.Value;
            else
                recurringServiceIdContext.Value = DBNull.Value;

            var firstDate = command.Parameters.Add("@firstDate", SqlDbType.Date);
            firstDate.Value = startDate;

            var lastDate = command.Parameters.Add("@lastDate", SqlDbType.Date);
            lastDate.Value = endDate;

            var result = DataReaderTools.GetDynamicSqlData(connectionString, command);

            var list = result.Item2.OrderBy(d => (DateTime)d["OccurDate"]).ToList();

            if (list.Any())
            {
                //Insert the first row to be a dictionary of the column's types
                //TODO: Load the service type & fields. Then for DateTime fields with Date or Time only, change their type to Date and Time
                var columnTypes = result.Item1.ToDictionary(kvp => kvp.Key, kvp => (Object)kvp.Value.ToString());

                list.Insert(0, columnTypes);
            }

            return list.AsQueryable();
        }

        /// <summary>
        /// Gets the service and fields
        /// </summary>
        public HttpResponseMessage GetServiceDetails(Guid? serviceId, DateTime? serviceDate, Guid? recurringServiceId)
        {
            //A service's id and a recurring service's id are the same id's as its service template
            var serviceTemplateIdToLoad = serviceId.HasValue ? serviceId.Value : recurringServiceId.Value;

            //Load the service templates and its details (fields)
            var templatesWithDetails = (from serviceTemplate in _coreEntitiesContainer.ServiceTemplates.Where(st => serviceTemplateIdToLoad == st.Id)
                                        from options in serviceTemplate.Fields.OfType<OptionsField>().Select(of => of.Options).DefaultIfEmpty()
                                        // from locations in serviceTemplate.Fields.OfType<LocationField>().Select(lf => lf.Value).DefaultIfEmpty() //no reason to pass LocationField yet
                                        select new { serviceTemplate, serviceTemplate.OwnerClient, serviceTemplate.Fields, options }).ToArray();//, locations };


            //The set of services to convert then return
            var modelServices = new List<FoundOps.Core.Models.CoreEntities.Service>();

            //This is an ExistingService
            if (serviceId.HasValue)
            {
                var existingService = _coreEntitiesContainer.Services.Include(s => s.Client).First(s => s.Id == serviceId.Value);

                //Check the current user has access to the business account
                var businessAccount = _coreEntitiesContainer.BusinessAccount(existingService.ServiceProviderId).FirstOrDefault();
                if (businessAccount == null)
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);

                //Return the existing service
                modelServices.Add(existingService);
            }
            //The service does not exist yet. Generate it from the recurring service, save it, return it
            else
            {
                var recurringService = _coreEntitiesContainer.RecurringServices.Include(rs => rs.Client).Include(rs => rs.ServiceTemplate)
                    .First(rs => rs.Id == recurringServiceId.Value);

                //Check the current user has access to the business account
                var businessAccount = _coreEntitiesContainer.BusinessAccount(recurringService.Client.BusinessAccountId.Value).FirstOrDefault();
                if (businessAccount == null)
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);

                //Generate the service from the recurring service
                //No need to add it to the object context, because setting the association will automatically add it
                var generatedService = GenerateServiceOnDate(serviceDate.Value, recurringService, businessAccount);

                //Return the generated service
                modelServices.Add(generatedService);
            }

            var apiServices = new List<Service>();

            //Convert the FoundOPS model Service to the API model Service
            apiServices.AddRange(modelServices.Select(Service.ConvertModel));

            return Request.CreateResponse<IQueryable<Service>>(HttpStatusCode.OK, apiServices.AsQueryable());
        }

        /// <summary>
        /// Returns the service and fields for a RouteTask
        /// If the RouteTask doesnt have a Service associated with it yet one with be generated (and saved) for it based on its RecurringService
        /// </summary>
        /// <param name="routeTaskId"></param>
        /// <returns></returns>
        [AcceptVerbs("GET", "POST")]
        public IQueryable<Service> GetTaskDetails(Guid routeTaskId)
        {
            //Make sure the user has access to the route task
            //by checking it's Route is available to the current user
            var routeTaskCheck = (_coreEntitiesContainer.CurrentUserAccount().SelectMany(cu => cu.LinkedEmployees)
            .SelectMany(e => e.Routes.SelectMany(r => r.RouteDestinations.Select(rd => rd.RouteTasks.FirstOrDefault(rt => rt.Id == routeTaskId))))).Any();

            if (!routeTaskCheck)
                ExceptionHelper.ThrowNotAuthorizedBusinessAccount();

            //Load the RouteTask and include its ParentRecurringService as well as its Service if either exist
            var loadedRouteTask = _coreEntitiesContainer.RouteTasks.Where(rt => rt.Id == routeTaskId).Include(rt => rt.Service).Include(rs => rs.ParentRecurringService).FirstOrDefault();

            if (loadedRouteTask == null)
                throw new Exception("Bad Request, no RouteTask exists for that Id");

            //A service's id and a recurring service's id are the same id's as its service template
            var serviceTemplateIdToLoad = loadedRouteTask.ServiceId.HasValue
                                              ? loadedRouteTask.ServiceId.Value
                                              : loadedRouteTask.RecurringServiceId.Value;

            //Load the service templates and its details (fields)
            var templatesWithDetails = (from serviceTemplate in _coreEntitiesContainer.ServiceTemplates.Where(st => serviceTemplateIdToLoad == st.Id)
                                        from options in serviceTemplate.Fields.OfType<OptionsField>().Select(of => of.Options).DefaultIfEmpty()
                                        // from locations in serviceTemplate.Fields.OfType<LocationField>().Select(lf => lf.Value).DefaultIfEmpty() //no reason to pass LocationField yet
                                        select new { serviceTemplate, serviceTemplate.OwnerClient, serviceTemplate.Fields, options }).ToArray();//, locations };

            //The set of services to convert then return
            var modelServices = new List<FoundOps.Core.Models.CoreEntities.Service>();

            //This is an ExistingService
            if (loadedRouteTask.ServiceId.HasValue)
            {
                var existingService = _coreEntitiesContainer.Services.Include(s => s.Client).First(s => s.Id == loadedRouteTask.ServiceId.Value);

                //Return the existing service
                modelServices.Add(existingService);
            }
            //The service does not exist yet. Generate it from the recurring service, save it, return it
            else
            {
                var recurringService = _coreEntitiesContainer.RecurringServices.Include(rs => rs.Client).Include(rs => rs.Client.BusinessAccount).Include(rs => rs.ServiceTemplate)
                    .First(rs => rs.Id == loadedRouteTask.RecurringServiceId.Value);

                //Generate the service from the recurring service
                //No need to add it to the object context, because setting the association will automatically add it
                var generatedService = GenerateServiceOnDate(loadedRouteTask.Date, recurringService, recurringService.Client.BusinessAccount);

                //Return the generated service
                modelServices.Add(generatedService);

                //Find any RouteTasks that have the same RecurringServiceId and Date
                //then update the ServiceId on the RouteTask with the newly created Service's Id
                var routeTasks = _coreEntitiesContainer.RouteTasks.Where(rt => rt.RecurringServiceId == generatedService.RecurringServiceId && rt.Date == generatedService.ServiceDate).ToArray();
                foreach (var routeTask in routeTasks)
                    routeTask.Service = generatedService;

                //Save the generated service
                _coreEntitiesContainer.SaveChanges();
            }

            var apiServices = new List<Service>();

            //Convert the FoundOPS model Service to the API model Service
            apiServices.AddRange(modelServices.Select(Service.ConvertModel));

            return apiServices.AsQueryable();
        }

        /// <summary>
        /// Generates a Service based off this ServiceTemplate for the specified date.
        /// NOTE: Make sure the fields are loaded.
        /// </summary>
        /// <param name="date">The date to generate a service for.</param>
        /// <param name="recurringService"> </param>
        private FoundOps.Core.Models.CoreEntities.Service GenerateServiceOnDate(DateTime date, RecurringService recurringService, BusinessAccount businessAccount)
        {
            return new FoundOps.Core.Models.CoreEntities.Service
            {
                ServiceDate = date,
                ClientId = recurringService.ClientId,
                Client = recurringService.Client,
                RecurringServiceParent = recurringService,
                ServiceProviderId = businessAccount.Id,
                ServiceProvider = businessAccount,
                ServiceTemplate = recurringService.ServiceTemplate.MakeChild(ServiceTemplateLevel.ServiceDefined)
            };
        }

        #endregion

        #region POST

        /// <summary>
        /// Pushes changes made to a Service from the API model to the FoundOPS model
        /// </summary>
        /// <param name="service">The API model of a Service</param>
        [AcceptVerbs("POST")]
        public HttpResponseMessage UpdateServiceDetails(Service service)
        {
            //TODO: Check the current user is allowed to update the service
            //TODO: Generate the service if it does not exist
            var foundOpsService = _coreEntitiesContainer.Services.FirstOrDefault(s => s.Id == service.Id);

            //The Service passed was generated on the Mobile device. Create a new FoundOPS Service and add appropriate field values
            if (foundOpsService == null)
            {
                var recurringService = _coreEntitiesContainer.RecurringServices.FirstOrDefault(rs => rs.Id == service.RecurringServiceId);

                foundOpsService = GenerateServiceOnDate(service.ServiceDate, recurringService);

                //Remove all fields that were generated with the Service Template
                foreach (var field in foundOpsService.ServiceTemplate.Fields)
                    foundOpsService.ServiceTemplate.Fields.Remove(field);

                //Add all fields from the generated Service to the Service Template
                foreach (var field in service.Fields)
                {
                    //Set the Service template for the field to the newly created Service Template
                    field.ServiceTemplateId = foundOpsService.ServiceProviderId;

                    //Add the field to the new Service Template
                    foundOpsService.ServiceTemplate.Fields.Add(Models.Field.ConvertBack(field));
                }
            }
            //The Service passed exists. Load all Field information update Field values
            else 
            {
                var templatesWithDetails = (from serviceTemplate in _coreEntitiesContainer.ServiceTemplates.Where(st => foundOpsService.Id == st.Id)
                                            from options in serviceTemplate.Fields.OfType<OptionsField>().Select(of => of.Options).DefaultIfEmpty()
                                            //from locations in serviceTemplate.Fields.OfType<LocationField>().Select(lf => lf.Value).DefaultIfEmpty() //LocationField not editable yet
                                            select new { serviceTemplate, serviceTemplate.OwnerClient, serviceTemplate.Fields, options }).ToArray();//, locations };
                #region Update all Fields

                foreach (var field in service.Fields)
                {
                    var foundOpsField = foundOpsService.ServiceTemplate.Fields.FirstOrDefault(f => f.Id == field.Id);

                    //this should not happen
                    //TODO figure out some kind of response that triggers a refresh
                    if (foundOpsField == null)
                        continue;

                    var dateTimeField = foundOpsField as DateTimeField;
                    var numericField = foundOpsField as NumericField;
                    var textBoxField = foundOpsField as TextBoxField;
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

                        if (!foundOpsOptions.Any())
                            continue;

                        //Cycle through each option and update the IsChecked value
                        foreach (var apiOption in ((Models.OptionsField)field).Options)
                        {
                            var foundOpsOption = foundOpsOptions.FirstOrDefault(op => op.Id == apiOption.Id);
                            if (foundOpsOption != null)
                                foundOpsOption.IsChecked = apiOption.IsChecked;
                        }
                    }
                }

                #endregion

            }
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
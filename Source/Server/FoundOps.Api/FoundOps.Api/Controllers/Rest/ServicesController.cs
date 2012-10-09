﻿using FoundOps.Api.Tools;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Tools;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using DateTimeField = FoundOps.Core.Models.CoreEntities.DateTimeField;
using LocationField = FoundOps.Core.Models.CoreEntities.LocationField;
using NumericField = FoundOps.Core.Models.CoreEntities.NumericField;
using OptionsField = FoundOps.Core.Models.CoreEntities.OptionsField;
using Service = FoundOps.Api.Models.Service;
using TextBoxField = FoundOps.Core.Models.CoreEntities.TextBoxField;

namespace FoundOps.Api.Controllers.Rest
{
    [Authorize]
    public class ServicesController : BaseApiController
    {
        /// <summary>
        /// Gets the service and fields.
        /// Need to specify one of the following
        /// a) the serviceId -> this will return the existing service and fields
        /// b) the serviceDate and recurringServiceId -> this will generate a service based on the recurringServiceId
        /// c) the serviceTemplateId -> this will generate a service based on the provider level service template
        /// </summary>
        public HttpResponseMessage Get(Guid? serviceId, DateTime? serviceDate, Guid? recurringServiceId, Guid? serviceTemplateId)
        {
            Guid serviceTemplateIdToLoad;
            Core.Models.CoreEntities.Service service = null;
            Guid businessAccountId;

            //for generating a service
            RecurringService recurringService = null;

            //existing service
            if (serviceId.HasValue)
            {
                serviceTemplateIdToLoad = serviceId.Value;
                service = CoreEntitiesContainer.Services.Include(s => s.Client).FirstOrDefault(s => s.Id == serviceId.Value);
                businessAccountId = service.ServiceProviderId;
            }
            //generate it from the recurring service
            else if (recurringServiceId.HasValue)
            {
                serviceTemplateIdToLoad = recurringServiceId.Value;
                recurringService = CoreEntitiesContainer.RecurringServices.Include(rs => rs.Client).Include(rs => rs.ServiceTemplate)
                    .First(rs => rs.Id == recurringServiceId.Value);
                businessAccountId = recurringService.Client.BusinessAccountId.Value;
            }
            //generate it from the service provider service template
            else if(serviceTemplateId.HasValue)
            {
                serviceTemplateIdToLoad = serviceTemplateId.Value;
                var template = CoreEntitiesContainer.ServiceTemplates.First(s => s.Id == serviceTemplateId.Value);
                businessAccountId = template.OwnerServiceProviderId.Value;
            }
            else
            {
                throw ApiExceptions.Create(Request.CreateResponse(HttpStatusCode.BadRequest));
            }

            //check the current user has access to the business account
            var businessAccount = CoreEntitiesContainer.BusinessAccount(businessAccountId, new[] { RoleType.Regular, RoleType.Administrator, RoleType.Mobile }).FirstOrDefault();
            if (businessAccount == null)
                throw Request.NotAuthorized();

            var serviceTemplate = HardCodedLoaders.LoadServiceTemplateWithDetails(CoreEntitiesContainer, serviceTemplateIdToLoad, null, null, null).First();

            //generate the service
            if (!serviceId.HasValue)
            {
                service = new Core.Models.CoreEntities.Service
                {
                    Id = Guid.NewGuid(),
                    ServiceDate = serviceDate.Value.Date,
                    ServiceProviderId = businessAccountId
                };

                if (recurringService != null)
                {
                    service.RecurringServiceId = recurringService.Id;
                    service.Client = recurringService.Client;
                }

                var template = serviceTemplate.MakeChild(ServiceTemplateLevel.ServiceDefined);

                //TODO check if this is necessary
                template.Id = service.Id;
                service.ServiceTemplate = template;
            }

            var apiServices = new List<Service> { Service.ConvertModel(service) };
            return Request.CreateResponse(HttpStatusCode.OK, apiServices.AsQueryable());
        }

        /// <summary>
        /// Pushes changes made to a Service from the API model to the FoundOPS model
        /// </summary>
        /// <param name="service">The API model of a Service</param>
        public HttpResponseMessage Put(Service service)
        {
            var businessAccount = CoreEntitiesContainer.BusinessAccount(service.ServiceProviderId, new[] { RoleType.Regular, RoleType.Administrator, RoleType.Mobile }).FirstOrDefault();
            if (businessAccount == null)
                throw Request.NotAuthorized();

            if (service.ClientId == Guid.Empty)
                throw Request.NotFound("Client");

            var existingService = CoreEntitiesContainer.Services.FirstOrDefault(s => s.Id == service.Id);

            //the service exists. load all field information  and update field values
            if (existingService != null)
            {
                var serviceTemplate = HardCodedLoaders.LoadServiceTemplateWithDetails(CoreEntitiesContainer, existingService.Id, null, null, null).First();

                serviceTemplate.Name = service.Name;

                #region Update all Fields

                foreach (var field in service.Fields)
                {
                    var existingField = existingService.ServiceTemplate.Fields.FirstOrDefault(f => f.Id == field.Id);
                    if (existingField == null)
                        throw Request.NotFound("Service");

                    var dateTimeField = existingField as DateTimeField;
                    var numericField = existingField as NumericField;
                    var textBoxField = existingField as TextBoxField;
                    var locationField = existingField as LocationField;
                    var optionsField = existingField as OptionsField;

                    //If the field is a DateTimeField, update the value
                    if (dateTimeField != null)
                        dateTimeField.Value = ((Models.DateTimeField)field).Value;

                    //If the field is a NumericField, update the value
                    if (numericField != null)
                        numericField.Value = ((Models.NumericField)field).Value;

                    //If the field is a TextBoxField, update the value
                    if (textBoxField != null)
                        textBoxField.Value = ((Models.TextBoxField)field).Value;

                    //If the field is a LocationField, update the value
                    if (locationField != null)
                        locationField.LocationId = ((Models.LocationField)field).LocationId;

                    //If the field is a OptionsField, update the Options
                    if (optionsField != null)
                    {
                        //Cycle through each option and update the IsChecked value
                        foreach (var apiOption in ((Models.OptionsField)field).Options)
                        {
                            var modelOption = optionsField.Options.FirstOrDefault(op => op.Name == apiOption.Name);
                            if (modelOption != null)
                                modelOption.IsChecked = apiOption.IsChecked;
                        }
                    }
                }

                #endregion
            }
            //the service was generated, insert a new Service and set the appropriate field values
            else
            {
                var generatedService = new Core.Models.CoreEntities.Service
                {
                    Id = service.Id,
                    ServiceDate = service.ServiceDate.Date,
                    ServiceProviderId = service.ServiceProviderId,
                    ClientId = service.ClientId,
                    RecurringServiceId = service.RecurringServiceId,
                    ServiceTemplate = new ServiceTemplate { Id = service.Id, Name = service.Name, ServiceTemplateLevel = ServiceTemplateLevel.ServiceDefined, OwnerServiceTemplateId = service.RecurringServiceId }
                };

                //Add all fields from the generated Service to the Service Template
                foreach (var field in service.Fields)
                    generatedService.ServiceTemplate.Fields.Add(Models.Field.ConvertBack(field));

                CoreEntitiesContainer.Services.AddObject(generatedService);
            }
             
            //Save any changes that were made
            SaveWithRetry();

            return Request.CreateResponse(HttpStatusCode.Accepted);
        }

        /// <summary>
        /// Deletes a service
        /// </summary>
        /// <param name="service">The API model of a service</param>
        public HttpResponseMessage Delete(Service service)
        {
            var businessAccount = CoreEntitiesContainer.BusinessAccount(service.ServiceProviderId).FirstOrDefault();
            if (businessAccount == null)
                return Request.CreateResponse(HttpStatusCode.Unauthorized);

            var routeTask = CoreEntitiesContainer.RouteTasks.Include("RouteDestination").Include("RouteDestination.Route").FirstOrDefault(rt => rt.ServiceId == service.Id);
            if (routeTask != null)
            {
                CoreEntitiesContainer.RouteDestinations.DeleteObject(routeTask.RouteDestination);
                CoreEntitiesContainer.RouteTasks.DeleteObject(routeTask);
            }

            var existingService = CoreEntitiesContainer.Services.FirstOrDefault(s => s.Id == service.Id);
            var serviceTemplate = CoreEntitiesContainer.ServiceTemplates.FirstOrDefault(st => st.Id == service.Id);
            if (existingService != null)
            {
                CoreEntitiesContainer.Services.DeleteObject(existingService);
                CoreEntitiesContainer.ServiceTemplates.DeleteObject(serviceTemplate);
            }

            //add an excluded date to the recurring service
            if (service.RecurringServiceId.HasValue)
            {
                var recurringService = CoreEntitiesContainer.RecurringServices.FirstOrDefault(rs => rs.Id == service.RecurringServiceId.Value);
                var excludedDates = recurringService.ExcludedDates.ToList();
                excludedDates.Add(service.ServiceDate);
                recurringService.ExcludedDates = excludedDates;
            }

            SaveWithRetry();
            return Request.CreateResponse(HttpStatusCode.Accepted);
        }
    }
}
using FoundOps.Api.Tools;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Tools;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using LocationField = FoundOps.Core.Models.CoreEntities.LocationField;
using NumericField = FoundOps.Core.Models.CoreEntities.NumericField;
using OptionsField = FoundOps.Core.Models.CoreEntities.OptionsField;
using Service = FoundOps.Api.Models.Service;
using TextBoxField = FoundOps.Core.Models.CoreEntities.TextBoxField;
using SignatureField = FoundOps.Core.Models.CoreEntities.SignatureField;

namespace FoundOps.Api.Controllers.Rest
{
    [Authorize]
    public class ServicesController : BaseApiController
    {
        /// <summary>
        /// Gets the service and fields.
        /// Need to specify one of the following
        /// a) the serviceId -> this will return the existing service and fields
        /// b) recurringServiceId -> this will generate a service based on the recurringServiceId on serviceDate, or the current user's date
        /// c) serviceTemplateId -> this will generate a service based on the provider level service template on serviceDate, or the current user's date
        /// </summary>
        public IQueryable<Service> Get(Guid? serviceId, DateTime? serviceDate, Guid? recurringServiceId, Guid? serviceTemplateId)
        {
            Core.Models.CoreEntities.Service service = null;

            Guid serviceTemplateIdToLoad; //for loading the service template
            Guid businessAccountId; //for loading the service template

            //for generating a service
            RecurringService recurringService = null;

            //try to find the existing service
            if (serviceId.HasValue)
                service = CoreEntitiesContainer.Services.Include(s => s.Client).FirstOrDefault(s => s.Id == serviceId.Value);

            if (service != null)
            {
                serviceTemplateIdToLoad = serviceId.Value;
                businessAccountId = service.ServiceProviderId;
            }
            //otherwise generate it 
            else
            {
                //from the recurring service
                if (recurringServiceId.HasValue)
                {
                    serviceTemplateIdToLoad = recurringServiceId.Value;
                    recurringService = CoreEntitiesContainer.RecurringServices.Include(rs => rs.Client)
                                                            .Include(rs => rs.ServiceTemplate)
                                                            .FirstOrDefault(rs => rs.Id == recurringServiceId.Value);

                    if (recurringService == null)
                        throw Request.BadRequest("The recurring service was not found");

                    if (recurringService.Client == null || !recurringService.Client.BusinessAccountId.HasValue)
                        throw Request.BadRequest("The recurring service does not have a Client with a BusinessAccount associated");

                    businessAccountId = recurringService.Client.BusinessAccountId.Value;
                }
                //or from the service provider service template
                else if (serviceTemplateId.HasValue)
                {
                    serviceTemplateIdToLoad = serviceTemplateId.Value;
                    var template = CoreEntitiesContainer.ServiceTemplates.First(s => s.Id == serviceTemplateId.Value);

                    if (!template.OwnerServiceProviderId.HasValue)
                        throw Request.BadRequest("The template does not have a service provider");

                    businessAccountId = template.OwnerServiceProviderId.Value;
                }
                else
                {
                    throw Request.BadRequest("Not enough parameters were set");
                }
            }

            //check the current user has access to the business account
            var businessAccount = CoreEntitiesContainer.BusinessAccount(businessAccountId, new[] { RoleType.Regular, RoleType.Administrator, RoleType.Mobile }).FirstOrDefault();
            if (businessAccount == null)
                throw Request.NotAuthorized();

            var serviceTemplate = HardCodedLoaders.LoadServiceTemplateWithDetails(CoreEntitiesContainer, serviceTemplateIdToLoad, null, null, null).First();

            //generate the service
            if (service == null)
            {
                //if serviceDate is null: use the current user's date
                if (!serviceDate.HasValue)
                    serviceDate = CoreEntitiesContainer.CurrentUserAccount().Now().Date;

                service = new Core.Models.CoreEntities.Service
                {
                    //Insert a new Guid (so the same entity is tracked and updated) 
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
            return apiServices.AsQueryable();
        }

        /// <summary>
        /// Pushes changes made to a Service from the API model to the FoundOPS model
        /// </summary>
        /// <param name="service">The API model of a Service</param>
        /// <param name="routeTaskId">Optional. If set update this route task to use this service. Only for generated services</param>
        public void Put(Service service, Guid? routeTaskId)
        {
            var businessAccount = CoreEntitiesContainer.BusinessAccount(service.ServiceProviderId, new[] { RoleType.Regular, RoleType.Administrator, RoleType.Mobile }).FirstOrDefault();
            if (businessAccount == null)
                throw Request.NotAuthorized();

            if (service.ClientId == Guid.Empty)
                throw Request.NotFound("Client");

            var existingService = CoreEntitiesContainer.Services.FirstOrDefault(s => s.Id == service.Id);

            //the service exists. load all field information and update field values
            if (existingService != null)
            {
                //update the client id
                existingService.ClientId = service.ClientId;

                var serviceTemplate = HardCodedLoaders.LoadServiceTemplateWithDetails(CoreEntitiesContainer, existingService.Id, null, null, null).First();

                serviceTemplate.Name = service.Name;

                #region Update all Fields

                foreach (var field in service.Fields)
                {
                    var existingField = existingService.ServiceTemplate.Fields.FirstOrDefault(f => f.Id == field.Id);
                    if (existingField == null)
                        throw Request.NotFound("Service");

                    var numericField = existingField as NumericField;
                    var textBoxField = existingField as TextBoxField;
                    var signatureField = existingField as SignatureField;
                    var locationField = existingField as LocationField;
                    var optionsField = existingField as OptionsField;

                    if (numericField != null)
                        numericField.Value = ((Models.NumericField)field).Value;

                    if (textBoxField != null)
                        textBoxField.Value = ((Models.TextBoxField)field).Value;

                    if (signatureField != null)
                        signatureField.Value = ((Models.SignatureField)field).Value;

                    if (locationField != null)
                    {
                        var apiLocationField = (Models.LocationField)field;
                        var oldId = locationField.Id;
                        var newId = apiLocationField.LocationId;
                        //update the location field, any route tasks, and destinations to the new location
                        if (oldId != newId)
                        {
                            locationField.LocationId = newId.Value;

                            var routeTasks = CoreEntitiesContainer.RouteTasks.Where(rt => rt.ServiceId == service.Id).Include(rt => rt.RouteDestination).ToArray();
                            //If route tasks exist, update their locations
                            //If any of the route tasks have been put into routes, update the route destination's locations
                            //Also, update the client Id on the route tasks/destinations - We can do this because if the client changed, there will definitely be a location change as well
                            //Even if the client didn't change, the update will just reset to the same client
                            foreach (var routeTask in routeTasks)
                            {
                                routeTask.LocationId = locationField.LocationId;
                                routeTask.ClientId = service.ClientId;

                                if (routeTask.RouteDestination == null) continue;
                                routeTask.RouteDestination.LocationId = locationField.LocationId;
                                routeTask.RouteDestination.ClientId = service.ClientId;
                            }
                        }
                    }

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

                existingService.LastModified = DateTime.UtcNow;
                existingService.LastModifyingUserId = CoreEntitiesContainer.CurrentUserAccount().Id;

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
                    ServiceTemplate = new ServiceTemplate { Id = service.Id, Name = service.Name, ServiceTemplateLevel = ServiceTemplateLevel.ServiceDefined, OwnerServiceTemplateId = service.RecurringServiceId },
                    CreatedDate = DateTime.UtcNow
                };

                //Add all fields from the generated Service to the Service Template
                foreach (var field in service.Fields)
                    generatedService.ServiceTemplate.Fields.Add(Models.Field.ConvertBack(field));

                CoreEntitiesContainer.Services.AddObject(generatedService);

                if (routeTaskId.HasValue)
                {
                    var routeTask = CoreEntitiesContainer.RouteTasks.FirstOrDefault(rt => rt.Id == routeTaskId.Value);
                    if (routeTask != null)
                    {
                        routeTask.RecurringServiceId = null;
                        routeTask.ServiceId = generatedService.Id;
                    }
                }
            }

            //Save any changes that were made
            SaveWithRetry();
        }

        /// <summary>
        /// Deletes a service
        /// </summary>
        /// <param name="service">The API model of a service</param>
        public void Delete(Service service)
        {
            var businessAccount = CoreEntitiesContainer.BusinessAccount(service.ServiceProviderId).FirstOrDefault();
            if (businessAccount == null)
                throw Request.NotAuthorized();

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
        }
    }
}
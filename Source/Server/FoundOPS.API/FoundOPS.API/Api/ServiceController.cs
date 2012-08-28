using FoundOps.Common.NET;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Tools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Service = FoundOPS.API.Models.Service;
using ServiceType = FoundOPS.API.Models.ServiceType;
using DateTimeField = FoundOps.Core.Models.CoreEntities.DateTimeField;
using LocationField = FoundOps.Core.Models.CoreEntities.LocationField;
using NumericField = FoundOps.Core.Models.CoreEntities.NumericField;
using OptionsField = FoundOps.Core.Models.CoreEntities.OptionsField;
using TextBoxField = FoundOps.Core.Models.CoreEntities.TextBoxField;

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

        /// <summary>
        /// Return the ServiceProvider's service types
        /// </summary>
        [AcceptVerbs("GET", "POST")]
        public IQueryable<ServiceType> GetServiceTypes(Guid roleId)
        {
            var serviceTemplates = _coreEntitiesContainer.Owner(roleId).SelectMany(ba => ba.ServiceTemplates)
                .Where(st => st.LevelInt == (int)ServiceTemplateLevel.ServiceProviderDefined);

            return serviceTemplates.Select(ServiceType.ConvertModel).AsQueryable().OrderBy(st => st.Name);
        }

        /// <summary>
        /// Get service holders with fields 
        /// </summary>
        /// <param name="roleId">The role</param>
        /// <param name="clientContext">The Id of the client to filter by</param>
        /// <param name="recurringServiceContext">The Id of the recurring service to filter by</param>
        /// <param name="startDate">The start date (in the user's time zone)</param>
        /// <param name="endDate">The end date (in the user's time zone)</param>
        /// <param name="serviceType">The service type to filter by</param>
        /// <param name="single">Only return the types</param>
        /// <returns>A queryable of dictionaries that resemble record type javascript objects when serialized</returns>
        [AcceptVerbs("GET", "POST")]
        public HttpResponseMessage GetServicesHoldersWithFields(Guid roleId, string serviceType, Guid? clientContext,
            Guid? recurringServiceContext, DateTime startDate, DateTime endDate, bool single = false)
        {
            var currentBusinessAccount = _coreEntitiesContainer.Owner(roleId).FirstOrDefault();
            if (currentBusinessAccount == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            var command = new SqlCommand("GetServiceHoldersWithFields") { CommandType = CommandType.StoredProcedure };

            var serviceProviderIdContext = command.Parameters.Add("@serviceProviderIdContext", SqlDbType.UniqueIdentifier);
            serviceProviderIdContext.Value = currentBusinessAccount.Id;

            var serviceTypeContext = command.Parameters.Add("@serviceTypeContext", SqlDbType.NVarChar);
            serviceTypeContext.Value = serviceType;

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

            var connectionString = ConfigWrapper.ConnectionString("CoreConnectionString");
            var result = DataReaderTools.GetDynamicSqlData(connectionString, command);

            var list = result.Item2.OrderBy(d => (DateTime)d["OccurDate"]).ToList();

            //Insert the first row to be a dictionary of the column's types

            //load the service template with fields
            var serviceTemplateWithFields = (from serviceTemplate in _coreEntitiesContainer.ServiceTemplates.Where(st => st.Name == serviceType && st.LevelInt == (int)ServiceTemplateLevel.ServiceProviderDefined)
                                             from options in serviceTemplate.Fields.OfType<OptionsField>().Select(of => of.Options).DefaultIfEmpty()
                                             from locations in serviceTemplate.Fields.OfType<LocationField>().Select(lf => lf.Value).DefaultIfEmpty()
                                             select new { serviceTemplate, serviceTemplate.OwnerClient, serviceTemplate.Fields, options, locations }).ToArray()
                                             .Select(a => a.serviceTemplate).First();

            var columnTypes = new Dictionary<string, object>();
            foreach (var key in result.Item1.Keys)
            {
                //certain values are not fields, hardcode those
                if (key == "OccurDate")
                {
                    columnTypes.Add(key, "date");
                    continue;
                }
                if (key == "RecurringServiceId" || key == "ServiceId")
                {
                    columnTypes.Add(key, "guid");
                    continue;
                }

                //find the Field Type
                var field = serviceTemplateWithFields.Fields.FirstOrDefault(f => f.Name == key.Replace("_", " "));
                var type = Models.Field.GetJavascriptFormat(field);

                columnTypes.Add(key, type);
            }

            if (single) //just return the types
            {
                return Request.CreateResponse(HttpStatusCode.OK, new List<Dictionary<string, object>> { columnTypes }.AsQueryable());
            }

            list.Insert(0, columnTypes);

            return Request.CreateResponse(HttpStatusCode.OK, list.AsQueryable());
        }

        /// <summary>
        /// Gets the service and fields.
        /// Need to specify one of the following
        /// a) the serviceId -> this will return the existing service and fields
        /// b) the serviceDate and recurringServiceId -> this will generate a service based on the recurringServiceId
        /// c) the serviceTemplateId -> this will generate a service based on the provider level service template
        /// </summary>
        public HttpResponseMessage GetServiceDetails(Guid? serviceId, DateTime? serviceDate, Guid? recurringServiceId, Guid? serviceTemplateId)
        {
            Guid serviceTemplateIdToLoad;
            FoundOps.Core.Models.CoreEntities.Service service = null;
            Guid businessAccountId;

            //for generating a service
            RecurringService recurringService = null;

            //existing service
            if (serviceId.HasValue)
            {
                serviceTemplateIdToLoad = serviceId.Value;
                service = _coreEntitiesContainer.Services.Include(s => s.Client).FirstOrDefault(s => s.Id == serviceId.Value);
                businessAccountId = service.ServiceProviderId;
            }
            //generate it from the recurring service
            else if (recurringServiceId.HasValue)
            {
                serviceTemplateIdToLoad = recurringServiceId.Value;
                recurringService = _coreEntitiesContainer.RecurringServices.Include(rs => rs.Client).Include(rs => rs.ServiceTemplate)
                    .First(rs => rs.Id == recurringServiceId.Value);
                businessAccountId = recurringService.Client.BusinessAccountId.Value;
            }
            //generate it from the service provider service template
            else
            {
                serviceTemplateIdToLoad = serviceTemplateId.Value;
                businessAccountId = _coreEntitiesContainer.ServiceTemplates.First(s => s.Id == serviceTemplateId.Value).OwnerServiceProviderId.Value;
            }

            //check the current user has access to the business account
            var businessAccount = _coreEntitiesContainer.BusinessAccount(businessAccountId, new[] { RoleType.Regular, RoleType.Administrator, RoleType.Mobile }).FirstOrDefault();
            if (businessAccount == null)
                return Request.CreateResponse(HttpStatusCode.Unauthorized);

            //load the service templates and its details (fields)
            var templateWithDetails = (from serviceTemplate in _coreEntitiesContainer.ServiceTemplates.Where(st => serviceTemplateIdToLoad == st.Id)
                                       from options in serviceTemplate.Fields.OfType<OptionsField>().Select(of => of.Options).DefaultIfEmpty()
                                       from locations in serviceTemplate.Fields.OfType<LocationField>().Select(lf => lf.Value).DefaultIfEmpty() //no reason to pass LocationField yet
                                       select new { serviceTemplate, serviceTemplate.OwnerClient, serviceTemplate.Fields, options, locations })
                                       .ToArray().Select(a => a.serviceTemplate).First();

            //generate the service
            if (!serviceId.HasValue)
            {
                service = new FoundOps.Core.Models.CoreEntities.Service
                {
                    ServiceDate = serviceDate.Value,
                    ServiceProviderId = businessAccountId
                };

                if (recurringService != null)
                {
                    service.RecurringServiceId = recurringService.Id;
                    service.Client = recurringService.Client;
                }

                var template = templateWithDetails.MakeChild(ServiceTemplateLevel.ServiceDefined);
                //TODO check if this is necessary
                template.Id = service.Id;
                service.ServiceTemplate = template;
            }

            var apiServices = new List<Service> { Service.ConvertModel(service) };
            return Request.CreateResponse(HttpStatusCode.OK, apiServices.AsQueryable());
        }

        #endregion

        #region POST

        /// <summary>
        /// Pushes changes made to a Service from the API model to the FoundOPS model
        /// </summary>
        /// <param name="service">The API model of a Service</param>
        [AcceptVerbs("POST")]
        public HttpResponseMessage UpdateService(Service service)
        {
            var businessAccount = _coreEntitiesContainer.BusinessAccount(service.ServiceProviderId, new[] { RoleType.Regular, RoleType.Administrator, RoleType.Mobile }).FirstOrDefault();
            if (businessAccount == null)
                return Request.CreateResponse(HttpStatusCode.Unauthorized);

            if (service.ClientId == Guid.Empty)
                return Request.CreateResponse(HttpStatusCode.BadRequest, "No client selected");

            var existingService = _coreEntitiesContainer.Services.FirstOrDefault(s => s.Id == service.Id);

            //the service exists. load all field information  and update field values
            if (existingService != null)
            {
                (from serviceTemplate in _coreEntitiesContainer.ServiceTemplates.Where(st => existingService.Id == st.Id)
                 from options in serviceTemplate.Fields.OfType<OptionsField>().Select(of => of.Options).DefaultIfEmpty()
                 from locations in serviceTemplate.Fields.OfType<LocationField>().Select(lf => lf.Value).DefaultIfEmpty()
                 select new { serviceTemplate, serviceTemplate.OwnerClient, serviceTemplate.Fields, options, locations }).ToArray();

                #region Update all Fields

                foreach (var field in service.Fields)
                {
                    var existingField = existingService.ServiceTemplate.Fields.FirstOrDefault(f => f.Id == field.Id);
                    if (existingField == null)
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "This service no longer exists.");

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
                        var foundOpsOptions =
                            _coreEntitiesContainer.Options.Where(o => o.OptionsFieldId == optionsField.Id).ToArray();

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
            //the service was generated, insert a new Service and set the appropriate field values
            else
            {
                var generatedService = new FoundOps.Core.Models.CoreEntities.Service
                {
                    Id = service.Id,
                    ServiceDate = service.ServiceDate,
                    ServiceProviderId = service.ServiceProviderId,
                    ClientId = service.ClientId,
                    RecurringServiceId = service.RecurringServiceId,
                    ServiceTemplate = new ServiceTemplate { Id = service.Id, ServiceTemplateLevel = ServiceTemplateLevel.ServiceDefined }
                };

                //Add all fields from the generated Service to the Service Template
                foreach (var field in service.Fields)
                {
                    generatedService.ServiceTemplate.Fields.Add(Models.Field.ConvertBack(field));
                }

                _coreEntitiesContainer.Services.AddObject(generatedService);
            }

            //Save any changes that were made
            _coreEntitiesContainer.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.Accepted);
        }

        [AcceptVerbs("POST")]
        public HttpResponseMessage DeleteService(Service service)
        {
            var businessAccount = _coreEntitiesContainer.BusinessAccount(service.ServiceProviderId).FirstOrDefault();
            if (businessAccount == null)
                return Request.CreateResponse(HttpStatusCode.Unauthorized);

            var existingService = _coreEntitiesContainer.Services.FirstOrDefault(s => s.Id == service.Id);
            if (existingService != null)
            {
                _coreEntitiesContainer.Services.DeleteObject(existingService);
            }

            //add an excluded date to the recurring service
            if (service.RecurringServiceId.HasValue)
            {
                var recurringService = _coreEntitiesContainer.RecurringServices.FirstOrDefault(rs => rs.Id == service.RecurringServiceId.Value);
                var excludedDates = recurringService.ExcludedDates.ToList();
                excludedDates.Add(service.ServiceDate);
                recurringService.ExcludedDates = excludedDates;
            }

            _coreEntitiesContainer.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.Accepted);
        }

        #endregion
    }

}
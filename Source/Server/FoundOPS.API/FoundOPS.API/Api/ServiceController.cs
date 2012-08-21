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
        /// <returns>A queryable of dictionaries that resemble record type javascript objects when serialized</returns>
        [AcceptVerbs("GET", "POST")]
        public HttpResponseMessage GetServicesHoldersWithFields(Guid roleId, string serviceType, Guid? clientContext,
            Guid? recurringServiceContext, DateTime startDate, DateTime endDate)
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

            var list = result.OrderBy(d => (DateTime)d["OccurDate"]).ToList();

            if (list.Any())
            {
                //Insert the first row to be a dictionary of the column's types

                //load the service template with fields
                var serviceTemplateWithFields = (from serviceTemplate in _coreEntitiesContainer.ServiceTemplates.Where(st => st.Name == serviceType && st.LevelInt == (int)ServiceTemplateLevel.ServiceProviderDefined)
                                                 from options in serviceTemplate.Fields.OfType<OptionsField>().Select(of => of.Options).DefaultIfEmpty()
                                                 from locations in serviceTemplate.Fields.OfType<LocationField>().Select(lf => lf.Value).DefaultIfEmpty()
                                                 select new { serviceTemplate, serviceTemplate.OwnerClient, serviceTemplate.Fields, options, locations }).ToArray()
                                                 .Select(a => a.serviceTemplate).First();

                var columnTypes = new Dictionary<string, object>();
                var firstServiceHolder = list.First();
                foreach (var key in firstServiceHolder.Keys)
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
                    var field = serviceTemplateWithFields.Fields.FirstOrDefault(f => f.Name == key);
                    var type = Models.Field.GetJavascriptFormat(field);

                    columnTypes.Add(key, type);
                }

                list.Insert(0, columnTypes);
            }

            return Request.CreateResponse(HttpStatusCode.OK, list.AsQueryable());
        }

        /// <summary>
        /// Gets the service and fields
        /// </summary>
        public HttpResponseMessage GetServiceDetails(Guid? serviceId, DateTime? serviceDate, Guid? recurringServiceId)
        {
            //A service's id and a recurring service's id are the same id's as its service template
            var serviceTemplateIdToLoad = serviceId.HasValue ? serviceId.Value : recurringServiceId.Value;

            //Load the service templates and its details (fields)
            var templatesWithDetails =
                (from serviceTemplate in
                     _coreEntitiesContainer.ServiceTemplates.Where(st => serviceTemplateIdToLoad == st.Id)
                 from options in serviceTemplate.Fields.OfType<OptionsField>().Select(of => of.Options).DefaultIfEmpty()
                 from locations in serviceTemplate.Fields.OfType<LocationField>().Select(lf => lf.Value).DefaultIfEmpty() //no reason to pass LocationField yet
                 select new { serviceTemplate, serviceTemplate.OwnerClient, serviceTemplate.Fields, options, locations }).ToArray();


            //The set of services to convert then return
            var modelServices = new List<FoundOps.Core.Models.CoreEntities.Service>();

            //This is an ExistingService
            if (serviceId.HasValue)
            {
                var existingService =
                    _coreEntitiesContainer.Services.Include(s => s.Client).First(s => s.Id == serviceId.Value);

                //Check the current user has access to the business account
                var businessAccount =
                    _coreEntitiesContainer.BusinessAccount(existingService.ServiceProviderId).FirstOrDefault();
                if (businessAccount == null)
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);

                //Return the existing service
                modelServices.Add(existingService);
            }
            //The service does not exist yet. Generate it from the recurring service, save it, return it
            else
            {
                var recurringService = _coreEntitiesContainer.RecurringServices.Include(rs => rs.Client).Include(
                    rs => rs.ServiceTemplate)
                    .First(rs => rs.Id == recurringServiceId.Value);

                //Check the current user has access to the business account
                var businessAccount =
                    _coreEntitiesContainer.BusinessAccount(recurringService.Client.BusinessAccountId.Value).
                        FirstOrDefault();
                if (businessAccount == null)
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);

                //Generate the service from the recurring service
                //no need to add it to the object context, because setting the association will automatically add it
                var generatedService = GenerateServiceOnDate(serviceDate.Value, recurringService, businessAccount);
                generatedService.ServiceTemplate =
                    recurringService.ServiceTemplate.MakeChild(ServiceTemplateLevel.ServiceDefined);

                //Return the generated service
                modelServices.Add(generatedService);
            }

            var apiServices = new List<Service>();

            //Convert the FoundOPS model Service to the API model Service
            apiServices.AddRange(modelServices.Select(Service.ConvertModel));

            return Request.CreateResponse(HttpStatusCode.OK, apiServices.AsQueryable());
        }

        /// <summary>
        /// Generates a Service based off this ServiceTemplate for the specified date.
        /// NOTE: Make sure the fields are loaded.
        /// </summary>
        /// <param name="date">The date to generate a service for</param>
        /// <param name="recurringService">The recurring service</param>
        /// <param name="businessAccount">The business account</param>
        private FoundOps.Core.Models.CoreEntities.Service GenerateServiceOnDate(DateTime date,
                                                                                RecurringService recurringService,
                                                                                BusinessAccount businessAccount)
        {
            var service = new FoundOps.Core.Models.CoreEntities.Service
                {
                    ServiceDate = date,
                    ClientId = recurringService.ClientId,
                    Client = recurringService.Client,
                    RecurringServiceParent = recurringService,
                    ServiceProviderId = businessAccount.Id,
                    ServiceProvider = businessAccount
                };

            return service;
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
            var existingService = _coreEntitiesContainer.Services.FirstOrDefault(s => s.Id == service.Id);

            //The Service passed was generated on the Mobile device. Create a new FoundOPS Service and set appropriate field values
            if (existingService == null)
            {
                var recurringService =
                    _coreEntitiesContainer.RecurringServices.Include(rs => rs.Client).FirstOrDefault(
                        rs => rs.Id == service.RecurringServiceId);
                //Load recurring service template
                var templatesWithDetails =
                    (from serviceTemplate in
                         _coreEntitiesContainer.ServiceTemplates.Where(st => recurringService.Id == st.Id)
                     from options in
                         serviceTemplate.Fields.OfType<OptionsField>().Select(of => of.Options).DefaultIfEmpty()
                     //from locations in serviceTemplate.Fields.OfType<LocationField>().Select(lf => lf.Value).DefaultIfEmpty() //LocationField not editable yet
                     select new { serviceTemplate, serviceTemplate.OwnerClient, serviceTemplate.Fields, options }).ToArray
                        (); //, locations };

                var businessAccount =
                    _coreEntitiesContainer.BusinessAccount(recurringService.Client.BusinessAccountId.Value).First();

                var generatedService = GenerateServiceOnDate(service.ServiceDate, recurringService, businessAccount);
                generatedService.ServiceTemplate = new ServiceTemplate
                    {
                        Id = service.Id,
                        Name = recurringService.ServiceTemplate.Name,
                        ParentServiceTemplate = recurringService.ServiceTemplate,
                        ServiceTemplateLevel = ServiceTemplateLevel.ServiceDefined
                    };

                //Add all fields from the generated Service to the Service Template
                foreach (var field in service.Fields)
                {
                    //Set the Service template for the field to the newly created Service Template
                    field.ServiceTemplateId = generatedService.ServiceProviderId;

                    //Add the field to the new Service Template
                    generatedService.ServiceTemplate.Fields.Add(Models.Field.ConvertBack(field));
                }
            }
            //The Service passed exists. Load all Field information update Field values
            else
            {
                var templatesWithDetails =
                    (from serviceTemplate in
                         _coreEntitiesContainer.ServiceTemplates.Where(st => existingService.Id == st.Id)
                     from options in
                         serviceTemplate.Fields.OfType<OptionsField>().Select(of => of.Options).DefaultIfEmpty()
                     //from locations in serviceTemplate.Fields.OfType<LocationField>().Select(lf => lf.Value).DefaultIfEmpty() //LocationField not editable yet
                     select new { serviceTemplate, serviceTemplate.OwnerClient, serviceTemplate.Fields, options }).ToArray
                        (); //, locations };

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
            //Save any changes that were made
            _coreEntitiesContainer.SaveChanges();

            //Create the Http response 
            var response = Request.CreateResponse(HttpStatusCode.Accepted);
            response.Headers.Location = new Uri(Request.RequestUri, "/api/service/" + service.Id.ToString());

            //Return the response
            return response;
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
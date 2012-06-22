using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Tools;
using Service = FoundOPS.API.Models.Service;

namespace FoundOPS.API.Controllers
{
    public class ServiceController
    {
        private readonly CoreEntitiesContainer _coreEntitiesContainer = new CoreEntitiesContainer();

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

            var serviceIdToLoad = loadedRouteTask.ServiceId; //routeTasks.Where(rt => rt.ServiceId.HasValue).Select(rt => rt.ServiceId.Value).ToArray();
            var recurringServiceIdToLoad = loadedRouteTask.RecurringServiceId; //routeTasks.Where(rt => !rt.ServiceId.HasValue && rt.RecurringServiceId.HasValue).Select(rt => rt.RecurringServiceId.Value).ToArray();

            var modelServices = new List<FoundOps.Core.Models.CoreEntities.Service>();

            if (serviceIdToLoad != null)
                modelServices.Add(_coreEntitiesContainer.Services.Include(s => s.Client).FirstOrDefault(s => s.Id == serviceIdToLoad));

            //Add the Existing Service to the model set

            var recurringService = _coreEntitiesContainer.RecurringServices.Include(rs => rs.Client).FirstOrDefault(rs => rs.Id == recurringServiceIdToLoad);

            //Add the converted Recurring Service to the model set
            if (recurringService != null)
            {
                var generatedService = GenerateServiceOnDate(loadedRouteTask.Date, recurringService);
                
                modelServices.Add(generatedService);
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
                Generated = true,
                ServiceDate = date,
                ClientId = recurringService.ClientId,
                Client = recurringService.Client,
                RecurringServiceParent = recurringService,
                ServiceProviderId = recurringService.Client.BusinessAccount.Id,
                ServiceProvider = recurringService.Client.BusinessAccount,
                ServiceTemplate = recurringService.ServiceTemplate.MakeChild(ServiceTemplateLevel.ServiceDefined)
            };
        }
    }
}
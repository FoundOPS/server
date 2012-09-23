using System.Data.SqlClient;
using Dapper;
using FoundOps.Common.NET;
using FoundOps.Core.Models;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Models.CoreEntities.Extensions.Services;
using FoundOps.Core.Tools;
using System;
using System.Data;
using System.Data.Entity;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.DomainServices.EntityFramework;
using System.ServiceModel.DomainServices.Server;
using Kent.Boogaart.KBCsv;

namespace FoundOps.Server.Services.CoreDomainService
{
    /// <summary>
    /// Holds the domain service operations for any service entities:
    /// Services and RecurringServices
    /// </summary>
    public partial class CoreDomainService
    {
        #region Service

        /// <summary>
        /// Gets the service details.
        /// It includes the Client and the ServiceTemplate and Fields.
        /// </summary>
        /// <param name="roleId">The current role id.</param>
        /// <param name="serviceId">The service id.</param>
        public Service GetServiceDetailsForRole(Guid roleId, Guid serviceId)
        {
            var businessAccount = ObjectContext.Owner(roleId).First();

            var service = this.ObjectContext.Services.Include(s => s.Client)
                .FirstOrDefault(s => s.Id == serviceId && s.ServiceProviderId == businessAccount.Id);

            //Load the ServiceTemplate for the Service
            GetServiceTemplateDetailsForRole(roleId, serviceId);

            return service;
        }

        /// <summary>
        /// Gets the service details for the current role.
        /// It includes the Client, RecurringServiceParent, the ServiceTemplate and Fields.
        /// </summary>
        /// <param name="roleId">The role id.</param>
        /// <param name="serviceIds">The ids of the Services to load.</param>
        [Query(HasSideEffects = true)] //HasSideEffects so a POST is used and a maximum URI length is not thrown
        public IQueryable<Service> GetServicesDetailsForRole(Guid roleId, IEnumerable<Guid> serviceIds)
        {
            var businessAccount = ObjectContext.Owner(roleId).First();

            var servicesForRole = this.ObjectContext.Services.Where(service => service.ServiceProviderId == businessAccount.Id).Where(s => serviceIds.Contains(s.Id));

            servicesForRole = servicesForRole.Include(s => s.Client).Include(s => s.Client).Include(s => s.RecurringServiceParent)
                .Include(s => s.ServiceTemplate).Include(s => s.ServiceTemplate.Fields);

            //Force Load LocationField's Location
            //TODO: Optimize by IQueryable
            servicesForRole.SelectMany(s => s.ServiceTemplate.Fields).OfType<LocationField>().Select(lf => lf.Value).ToArray();

            return servicesForRole;
        }

        public void InsertService(Service service)
        {
            //If the service being inserted has a RecurringServiceId
            //Find any RouteTasks that have the same RecurringServiceId and Date
            //Update the ServiceId on the RouteTask with the Id of the newly created Service
            if (service.RecurringServiceId != null)
            {
                var routeTasks = this.ObjectContext.RouteTasks.Where(rt => rt.RecurringServiceId == service.RecurringServiceId && rt.Date == service.ServiceDate).ToArray();

                foreach (var routeTask in routeTasks)
                    routeTask.ServiceId = service.Id;
            }

            if ((service.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(service, EntityState.Added);
            }
            else
            {
                this.ObjectContext.Services.AddObject(service);
            }
        }

        public void UpdateService(Service currentService)
        {
            var originalService = ChangeSet.GetOriginal(currentService);

            var user = ObjectContext.CurrentUserAccount().First();

            //If the original date was prior to today, throw an exception
            if (originalService.ServiceDate < user.Now().Date)
                throw new Exception("Cannot change the date of a Service in the past.");

            //Delete any associated route tasks (today/in the future) if the service date changed
            if (originalService.ServiceDate != currentService.ServiceDate)
            {
                var routeTasksToUpdate = this.ObjectContext.RouteTasks.Where(rt =>
                    rt.ServiceId == currentService.Id || (rt.RecurringServiceId == originalService.RecurringServiceId && rt.Date == originalService.ServiceDate)).ToArray();

                //Remove the RouteTask from its RouteDestination, change the date and set its TaskStatus to CreatedDefault
                foreach (var routeTask in routeTasksToUpdate)
                {
                    routeTask.RouteDestination = null;
                    routeTask.RouteDestinationId = null;

                    routeTask.Date = currentService.ServiceDate;

                    routeTask.TaskStatus = this.ObjectContext.TaskStatuses.FirstOrDefault(ts => ts.BusinessAccountId == routeTask.BusinessAccountId && ts.DefaultTypeInt == ((int)StatusDetail.CreatedDefault));
                }
            }

            this.ObjectContext.Services.AttachAsModified(currentService);
        }

        /// <summary>
        /// Goes through each routeTask and removes it from route destinations
        /// Delete any route destinations without remaining tasks.
        /// </summary>
        /// <param name="routeTasksToDelete"></param>
        private void DeleteRouteTasks(RouteTask[] routeTasksToDelete)
        {
            //If no RouteTasks exist, return
            if (!routeTasksToDelete.Any()) return;

            foreach (var routeTask in routeTasksToDelete)
            {
                routeTask.RouteDestinationReference.Load();
                var routeDestination = routeTask.RouteDestination;

                //Remove the RouteTask from the RouteDestination
                if (routeDestination != null)
                {
                    routeDestination.RouteTasks.Load();
                    routeTask.RouteDestination = null;

                    //If the RouteDestination has no remaining tasks, delete it
                    if (routeDestination.RouteTasks.Count == 0)
                        DeleteRouteDestination(routeDestination);
                }

                //Delete the route task
                this.ObjectContext.DetachExistingAndAttach(routeTask);
            }
        }

        /// <summary>
        /// Goes through each Service and deletes it
        /// </summary>
        /// <param name="servicesToDelete"></param>
        private void DeleteServices(IEnumerable<Service> servicesToDelete)
        {
            foreach (var service in servicesToDelete)
                DeleteService(service);
        }

        public void DeleteService(Service service)
        {
            if (service.Generated)
                return;

            if ((service.EntityState == EntityState.Detached))
                this.ObjectContext.Services.Attach(service);

            if (service.ServiceTemplate != null)
                this.DeleteServiceTemplate(service.ServiceTemplate);

            var routeTasksToDelete = this.ObjectContext.RouteTasks.Where(rt =>
                 rt.ServiceId == service.Id || (rt.RecurringServiceId == service.RecurringServiceId && rt.Date == service.ServiceDate)).ToArray();

            DeleteRouteTasks(routeTasksToDelete);

            if ((service.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(service, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.Services.DeleteObject(service);
            }
        }

        #endregion

        #region Service Holders

        /// <summary>
        /// Gets the ServiceHolders based off the current ServiceProvider and optional contexts.
        /// It will return all the services on the seed date.
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="clientContext">(Optional) If this is the only context set, it will generate ServiceHolders only for this client.</param>
        /// <param name="recurringServiceContext">(Optional) If this is the only context set, it will generate ServiceHolders only for this RecurringService.</param>
        /// <param name="seedDate">The date to start generating ServiceHolders from.</param>
        /// <param name="numberOfOccurrences">The (soft) minimum number of occurrences to return. If there are not enough services it will not meet the minimum.</param>
        /// <param name="getPrevious">Return all the services from at least one date before the seed date (if there are any).</param>
        /// <param name="getNext">Return all the services from at least one date after the seed date (if there are any).</param>
        /// <returns></returns>
        [Query]
        public IQueryable<ServiceHolder> GetServiceHolders(Guid roleId, Guid? clientContext, Guid? recurringServiceContext,
            DateTime seedDate, int numberOfOccurrences, bool getPrevious, bool getNext)
        {
            Guid? recurringServiceContextId = null;
            Guid? clientContextId = null;
            Guid? serviceProviderContextId = null;

            if (recurringServiceContext.HasValue)
            {
                //Check the user has access to the RecurringService
                if (!RecurringServicesForServiceProviderOptimized(roleId).Any(rs => rs.Id == recurringServiceContext))
                    return null;

                recurringServiceContextId = recurringServiceContext;
            }
            else if (clientContext.HasValue)
            {
                //Check the user has access to the Client
                if (!GetClientsForRole(roleId).Any(c => c.Id == clientContext))
                    return null;

                clientContextId = clientContext;
            }
            else
            {
                var businessAccount = ObjectContext.Owner(roleId).First();

                //Because there is no other context, assume the context should be the serviceProvider
                //The businessForRole returns null if the user does not have access to the ServiceProvider
                serviceProviderContextId = businessAccount.Id;
            }

            var user = ObjectContext.CurrentUserAccount().FirstOrDefault();

            if (user == null)
                throw new Exception("No User logged in");

            using (var conn = new SqlConnection(ServerConstants.SqlConnectionString))
            {
                conn.Open();

                var parameters = new DynamicParameters();
                parameters.Add("@serviceProviderIdContext", serviceProviderContextId);
                parameters.Add("@clientIdContext", clientContextId);
                parameters.Add("@recurringServiceIdContext", recurringServiceContextId);
                parameters.Add("@seedDate", user.Now().Date);
                parameters.Add("@frontBackMinimum", 50);
                parameters.Add("@getPrevious", 1);
                parameters.Add("@getNext", 1);
                parameters.Add("@serviceTypeContext", null);

                //Calls a stored procedure that will find any Services scheduled for today and create a routetask for them if one doesnt exist
                //Then it will return all RouteTasks that are not in a route joined with their Locations, Location.Regions and Clients
                //Dapper will then map the output table to RouteTasks, RouteTasks.Location, RouteTasks.Location.Region and RouteTasks.Client
                //While that is being mapped we also attach the Client, Location and Region to the objectContext
                var data = conn.QueryMultiple("GetDateRangeForServices", parameters, commandType: CommandType.StoredProcedure);

                var firstDate = data.Read<DateTime>().Single();
                var lastDate = data.Read<DateTime>().Single();

                //Reset parameters
                parameters = new DynamicParameters();
                parameters.Add("@serviceProviderIdContext", serviceProviderContextId);
                parameters.Add("@clientIdContext", clientContextId);
                parameters.Add("@recurringServiceIdContext", recurringServiceContextId);
                parameters.Add("@firstDate", firstDate);
                parameters.Add("@lastDate", lastDate);
                parameters.Add("@serviceTypeContext", null);
                parameters.Add("@withFields", false);

                var serviceHolders = conn.Query<ServiceHolder>("GetServiceHolders", parameters, commandType: CommandType.StoredProcedure);

                conn.Close();

                //Order by OccurDate and Service Name for UI
                //Order by ServiceId and RecurringServiceId to ensure they are always returned in the same order
                return serviceHolders.OrderBy(sh => sh.OccurDate).ThenBy(sh => sh.ServiceName).ThenBy(sh => sh.ServiceId).ThenBy(sh => sh.RecurringServiceId).AsQueryable();
            }
        }

        /// <summary>
        /// An empty method to allow creating ServiceHolders on the client.
        /// </summary>
        public void InsertServiceHolder(ServiceHolder serviceHolder)
        {
            throw new Exception("Cannot save ServiceHolders in the database.");
        }

        #endregion

        #region Recurring Services

        /// <summary>
        /// Gets the RecurringServices for the roleId.
        /// It includes the bare minimum Client.
        /// </summary>
        /// <param name="roleId">The role to determine the business account from.</param>
        private IQueryable<RecurringService> RecurringServicesForServiceProviderOptimized(Guid roleId)
        {
            var businessAccount = ObjectContext.Owner(roleId).First();

            var recurringServices = this.ObjectContext.RecurringServices.Include(rs => rs.Client).Where(v => v.Client.BusinessAccountId == businessAccount.Id);

            return recurringServices.OrderBy(rs => rs.ClientId);
        }

        /// <summary>
        /// Gets the RecurringService details.
        /// It includes the ServiceTemplate and Fields (LocationField and OptionsField).
        /// </summary>
        /// <param name="roleId">The current role id.</param>
        /// <param name="recurringServiceId">The recurring service id.</param>
        public RecurringService GetRecurringServiceDetailsForRole(Guid roleId, Guid recurringServiceId)
        {
            var recurringServices = RecurringServicesForServiceProviderOptimized(roleId).Where(rs => rs.Id == recurringServiceId)
                .Include(c => c.Client).Include(rs => rs.ServiceTemplate).Include(rs => rs.ServiceTemplate.Fields);

            //Force Load LocationField's Location/OptionsField
            recurringServices.SelectMany(s => s.ServiceTemplate.Fields).OfType<LocationField>().Select(lf => lf.Value).ToArray();

            return recurringServices.FirstOrDefault();
        }

        /// <summary>
        /// Gets the RecurringService details.
        /// It includes the ServiceTemplate and Fields (LocationField and OptionsField).
        /// </summary>
        /// <param name="roleId">The current role id.</param>
        /// <param name="recurringServiceIds">The ids of the RecurringServices to load.</param>
        [Query(HasSideEffects = true)] //HasSideEffects so a POST is used and a maximum URI length is not thrown
        public IQueryable<RecurringService> GetRecurringServicesWithDetailsForRole(Guid roleId, IEnumerable<Guid> recurringServiceIds)
        {
            var recurringServices = RecurringServicesForServiceProviderOptimized(roleId)
                .Where(rs => recurringServiceIds.Contains(rs.Id)).Include(c => c.Client).Include(rs => rs.ServiceTemplate).Include(rs => rs.ServiceTemplate.Fields);

            //Force Load LocationField's Location
            //TODO: Optimize by IQueryable
            recurringServices.SelectMany(rs => rs.ServiceTemplate.Fields).OfType<LocationField>().Select(lf => lf.Value).ToArray();

            return recurringServices;
        }

        /// <summary>
        /// Gets the RecurringServices CSV for a role.
        /// </summary>
        /// <param name="roleId">The role id.</param>
        public byte[] GetRecurringServicesCSVForRole(Guid roleId)
        {
            var memoryStream = new MemoryStream();

            var csvWriter = new CsvWriter(memoryStream);

            csvWriter.WriteHeaderRecord("Service Type", "Client", "Location", "Address Line 1", "Frequency",
                                        "Start Date", "End Date", "Repeat Every", "Repeat On");

            var recurringServices = RecurringServicesForServiceProviderOptimized(roleId).Include(rs => rs.Client).Include(rs => rs.Repeat).Include(rs => rs.ServiceTemplate);

            //Force Load LocationField's Location
            //TODO: Optimize by IQueryable
            recurringServices.SelectMany(rs => rs.ServiceTemplate.Fields).OfType<LocationField>().Select(lf => lf.Value).ToArray();

            //join RecurringServices with client names
            var records = from rs in recurringServices
                          orderby rs.Client.Name
                          select new
                          {
                              RecurringService = rs,
                              ServiceType = rs.ServiceTemplate.Name,
                              ClientName = rs.Client.Name
                          };

            foreach (var record in records.ToArray())
            {
                //Get destination information from LocationField
                var destination = record.RecurringService.ServiceTemplate.GetDestination();
                var locationName = "";
                var address = "";
                if (destination != null)
                {
                    locationName = destination.Name;
                    address = destination.AddressLineOne;
                }

                var repeat = record.RecurringService.Repeat;

                //Select proper end date info
                var endDate = "";
                if (repeat.EndDate.HasValue)
                    endDate = repeat.EndDate.Value.ToShortDateString();
                else if (repeat.EndAfterTimes.HasValue)
                    endDate = repeat.EndAfterTimes.Value.ToString();

                #region setup RepeatOn string

                var frequency = repeat.Frequency;
                var repeatOn = "";
                if (frequency == Frequency.Weekly)
                {
                    var weeklyFrequencyDetail = repeat.FrequencyDetailAsWeeklyFrequencyDetail;
                    //Build a weekly frequency string
                    if (weeklyFrequencyDetail.Any(fd => fd == DayOfWeek.Sunday))
                        repeatOn += "Sun";
                    if (weeklyFrequencyDetail.Any(fd => fd == DayOfWeek.Monday))
                    {
                        if (repeatOn.ToCharArray().Any())
                            repeatOn += ",";
                        repeatOn += "Mon";
                    }
                    if (weeklyFrequencyDetail.Any(fd => fd == DayOfWeek.Tuesday))
                    {
                        if (repeatOn.ToCharArray().Any())
                            repeatOn += ",";
                        repeatOn += "Tues";
                    }
                    if (weeklyFrequencyDetail.Any(fd => fd == DayOfWeek.Wednesday))
                    {
                        if (repeatOn.ToCharArray().Any())
                            repeatOn += ",";
                        repeatOn += "Wed";
                    }
                    if (weeklyFrequencyDetail.Any(fd => fd == DayOfWeek.Thursday))
                    {
                        if (repeatOn.ToCharArray().Any())
                            repeatOn += ",";
                        repeatOn += "Thur";
                    }
                    if (weeklyFrequencyDetail.Any(fd => fd == DayOfWeek.Friday))
                    {
                        if (repeatOn.ToCharArray().Any())
                            repeatOn += ",";
                        repeatOn += "Fri";
                    }
                    if (weeklyFrequencyDetail.Any(fd => fd == DayOfWeek.Saturday))
                    {
                        if (repeatOn.ToCharArray().Any())
                            repeatOn += ",";
                        repeatOn += "Sat";
                    }
                }
                else if (frequency == Frequency.Monthly)
                {
                    var monthlyFrequencyDetail = repeat.FrequencyDetailAsMonthlyFrequencyDetail;
                    if (monthlyFrequencyDetail.HasValue)
                        repeatOn = monthlyFrequencyDetail.Value == MonthlyFrequencyDetail.OnDayInMonth ? "Date" : "Day";
                    else
                        repeatOn = "Date";
                }

                #endregion

                //Convert frequency detail to:  day or date
                //"Service Type", "Client", "Location", "Address", "Frequency"
                csvWriter.WriteDataRecord(record.ServiceType, record.ClientName, locationName, address, frequency.ToString(),
                    //"Start Date", "End Date", "Repeat Every", "Repeat On"
                   repeat.StartDate.ToShortDateString(), endDate, repeat.RepeatEveryTimes.ToString(), repeatOn);
            }

            csvWriter.Close();

            var csv = memoryStream.ToArray();
            memoryStream.Dispose();

            return csv;
        }

        public void InsertRecurringService(RecurringService recurringService)
        {
            if ((recurringService.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(recurringService, EntityState.Added);
            }
            else
            {
                this.ObjectContext.RecurringServices.AddObject(recurringService);
            }
        }

        public void UpdateRecurringService(RecurringService currentRecurringService)
        {
            //If there is a newly added excluded date today or in the future. Remove any associated route tasks for that date
            var originalRecurringService = ChangeSet.GetOriginal(currentRecurringService);

            var user = ObjectContext.CurrentUserAccount().First();

            var date = user.Now().Date;

            var newlyAddedFutureDatesToExclude = currentRecurringService.ExcludedDates.Except(originalRecurringService.ExcludedDates).Where(d => d >= date).ToArray();
            if (newlyAddedFutureDatesToExclude.Any())
            {
                var routeTasksToDelete = this.ObjectContext.RouteTasks.Where(rt =>
                    rt.RecurringServiceId == currentRecurringService.Id && newlyAddedFutureDatesToExclude.Contains(rt.Date)).ToArray();

                var servicesToDelete = routeTasksToDelete.Select(rt => rt.Service);

                DeleteRouteTasks(routeTasksToDelete);

                DeleteServices(servicesToDelete);
            }

            this.ObjectContext.RecurringServices.AttachAsModified(currentRecurringService);
        }

        public void DeleteRecurringService(RecurringService recurringService)
        {
            ObjectContext.DeleteRecurringService(recurringService.Id);
        }

        #endregion
    }
}

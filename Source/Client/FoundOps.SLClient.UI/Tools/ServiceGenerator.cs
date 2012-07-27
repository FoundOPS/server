using System;
using System.Linq;
using System.Collections.Generic;
using FoundOps.Common.Composite.Tools;
using FoundOps.Common.Tools.ExtensionMethods;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Models.CoreEntities.Extensions.Services;
using FoundOps.SLClient.Data.Services;

namespace FoundOps.SLClient.UI.Tools
{
    ///<summary>
    /// Generates the proper services based on existing Services and RecurringServices schedules
    ///</summary>
    public static class ServiceGenerator
    {
        /// <summary>
        /// Generate ServiceTuples around the selectedService.
        /// </summary>
        ///<param name="selectedService">The selected service to generate services around. If null, Services will be generated around the current UTC time</param>
        ///<param name="services">The existing/new/modified (not generated) services</param>
        ///<param name="recurringServices">The recurring services</param>
        ///<param name="clientContext">The current client context</param>
        ///<param name="locationContext">The current location context</param>
        ///<param name="recurringServiceContext">The current recurringService context</param>
        ///<param name="size">The size of the sorted set to generate</param>
        public static ServiceGenerationResult GenerateServiceTuples(Service selectedService, IEnumerable<Service> services, IEnumerable<RecurringService> recurringServices,
            Client clientContext, Location locationContext, RecurringService recurringServiceContext, int size)
        {
            var elibileServicesRecurringServices = FilterServicesRecurringServices(services, recurringServices, clientContext, locationContext, recurringServiceContext);

            return SortedExistingAndGeneratedServices(selectedService, elibileServicesRecurringServices.Item1, elibileServicesRecurringServices.Item2, size);
        }

        /// <summary>
        /// The filtered Services and RecurringServices
        /// </summary>
        ///<param name="services">The existing/new/modified (not generated) services</param>
        ///<param name="recurringServices">The recurring services</param>
        ///<param name="clientContext">The current client context</param>
        ///<param name="locationContext">The current location context</param>
        ///<param name="recurringServiceContext">The current recurringService context</param>
        private static Tuple<IEnumerable<Service>, IEnumerable<RecurringService>> FilterServicesRecurringServices(IEnumerable<Service> services, IEnumerable<RecurringService> recurringServices,
            Client clientContext, Location locationContext, RecurringService recurringServiceContext)
        {
            IEnumerable<Service> filteredServices;
            IEnumerable<RecurringService> filteredRecurringServices;

            #region Filter

            if (clientContext != null || locationContext != null || recurringServiceContext != null)
            {
                //Setup all possible services and recurring services by joining the service associations from the different contexts

                IEnumerable<Service> possibleServices = new List<Service>();
                IEnumerable<RecurringService> possibleRecurringServices = new List<RecurringService>();

                if (clientContext != null)
                {
                    possibleServices = clientContext.ServicesToRecieve;
                    possibleRecurringServices = clientContext.RecurringServices;
                }

                if (locationContext != null)
                {
                    possibleServices = possibleServices.Union(locationContext.LocationFieldsWhereValue.Where(
                        lf => lf.OwnerServiceTemplate.ServiceTemplateLevel == ServiceTemplateLevel.ServiceDefined).
                                                                  Select(lf => lf.OwnerServiceTemplate.OwnerService));

                    possibleRecurringServices =
                        possibleRecurringServices.Union(locationContext.LocationFieldsWhereValue.Where(
                            lf =>
                            lf.OwnerServiceTemplate.ServiceTemplateLevel == ServiceTemplateLevel.RecurringServiceDefined)
                                                            .
                                                            Select(lf => lf.OwnerServiceTemplate.OwnerRecurringService));
                }

                if (recurringServiceContext != null)
                {
                    possibleServices = possibleServices.Union(recurringServiceContext.GeneratedServices);
                    possibleRecurringServices = new List<RecurringService> { recurringServiceContext };
                }

                //Filter the possibleServices and possibleRecurringServices by the contexts

                filteredServices = possibleServices;
                filteredRecurringServices = possibleRecurringServices;

                if (clientContext != null)
                {
                    filteredServices = filteredServices.Where(s => s != null && s.ClientId == clientContext.Id);
                    filteredRecurringServices = filteredRecurringServices.Where(rs => rs != null && rs.ClientId == clientContext.Id);
                }

                if (locationContext != null)
                {
                    filteredServices = filteredServices.Where(s =>
                                                                    {
                                                                        if (s == null || s.ServiceTemplate == null)
                                                                            return false;
                                                                        var destination = s.ServiceTemplate.GetDestination();
                                                                        return destination != null && destination.Id == locationContext.Id;
                                                                    });
                    filteredRecurringServices = filteredRecurringServices.Where(rs =>
                                                {
                                                    if (rs == null || rs.ServiceTemplate == null)
                                                        return false;
                                                    var destination = rs.ServiceTemplate.GetDestination();
                                                    return destination != null && destination.Id == locationContext.Id;
                                                });
                }

                if (recurringServiceContext != null)
                {
                    filteredServices = filteredServices.Where(s => s != null && s.RecurringServiceId == recurringServiceContext.Id);
                    filteredRecurringServices =
                        filteredRecurringServices.Where(rs => rs != null && rs.Id == recurringServiceContext.Id);
                }
            }
            else
            {
                filteredServices = services;
                filteredRecurringServices = recurringServices;
            }

            #endregion

            return new Tuple<IEnumerable<Service>, IEnumerable<RecurringService>>(filteredServices,
                                                                                  filteredRecurringServices.Where(rs => rs != null && rs.Repeat != null));
        }

        /// <summary>
        /// A method to generate a SortedSet'ServiceTuple based on existing and generated services. 
        /// It will be the size of pageSize with the selectedService in the middle. If selectedService is null, a service closest to the current UTC time will be in the middle
        /// </summary>
        /// <param name="selectedService">The selected entity to generate services around. Can be null</param>
        /// <param name="existingServices">The existing services to include in the sorted list.</param>
        /// <param name="recurringServices">The recurring services to generate services from.</param>
        /// <param name="pageSize">The page size to generate.</param>
        private static ServiceGenerationResult SortedExistingAndGeneratedServices(Service selectedService, IEnumerable<Service> existingServices, IEnumerable<RecurringService> recurringServices, int pageSize)
        {
            var collection = new SortedSet<ServiceTuple>();

            //Start the date range initially with the selectedService's ServiceDate (or today if there is no selectedService) +- 1 day
            var middleDate = selectedService != null ? selectedService.ServiceDate : Manager.Context.UserAccount.AdjustTimeForUserTimeZone(DateTime.UtcNow).Date;

            var rangeStartDate = middleDate.AddDays(-1);
            var rangeEndDate = middleDate.AddDays(1);

            //Add the initial ServiceTuples to the collection
            AddServiceTuplesToCollection(collection, recurringServices, rangeStartDate, rangeEndDate, CrawlingPosition.StoppedInMiddle);

            Func<int> entitiesBeforeSelectedService = () =>
            {
                var selectedEntityIndex = ServiceIndex(collection, selectedService);
                return selectedEntityIndex < 0 ? 0 : selectedEntityIndex;
            };

            Func<int> entitiesAfterSelectedService = () =>
            {
                int serviceIndex = ServiceIndex(collection, selectedService);
                return serviceIndex < 0 ? 0 : (serviceIndex - collection.Count) * -1;
            };

            //Start two days before the last start date
            DateTime firstHalfStartDate = rangeStartDate.AddDays(-2);
            //End a day after the firstHalfStartDate
            DateTime firstHalfEndDate = firstHalfStartDate.AddDays(1);

            var timeSpanToJump = new TimeSpan(1, 0, 0, 0);

            bool canAddMore = true;


            //Only consider the front half of services
            var frontHalfServicesToAdd = existingServices.ToList();
            frontHalfServicesToAdd.RemoveAll(es => es.ServiceDate > middleDate.Date);

            //Setup the front half of entities (moving backwards from the selectedService)
            while (canAddMore && entitiesBeforeSelectedService() < (pageSize / 2))
            {
                var canGenerateMoreServiceTuplesInFront = AddServiceTuplesToCollection(collection, recurringServices, firstHalfStartDate, firstHalfEndDate, CrawlingPosition.MoveBackward);

                #region Add eligible existing services

                //Add the eligible existing services (after the generated ServiceTuples are added) so that the dates are properly considered

                //The service date is either: a) >= min serviceTuple.ServiceDate, or b) >= the firstHalfStartDate
                var firstServiceTupleDate = collection.Min != null ? new DateTime?(collection.Min.ServiceDate) : null;
                var servicesToAdd = firstServiceTupleDate != null ?
                    frontHalfServicesToAdd.Where(s => s.ServiceDate >= firstServiceTupleDate || s.ServiceDate >= firstHalfStartDate).ToArray() :
                    frontHalfServicesToAdd.Where(s => s.ServiceDate >= firstHalfStartDate).ToArray();

                //Add the existing service to the collection and remove it from the frontHalfServicesToAdd
                foreach (var service in servicesToAdd)
                {
                    collection.Add(new ServiceTuple(service));

                    frontHalfServicesToAdd.Remove(service);
                }

                #endregion

                //canAddMore if there are more service tuples to generate or more services in front of the middleDate
                canAddMore = canGenerateMoreServiceTuplesInFront || frontHalfServicesToAdd.Count() > 0;

                //End a day before the last start date
                firstHalfEndDate = firstHalfStartDate.AddDays(-1);

                //Move start date earlier (doubles every iteration)
                timeSpanToJump = timeSpanToJump.Add(timeSpanToJump);
                firstHalfStartDate = firstHalfStartDate.Subtract(timeSpanToJump);
            }

            //Start a day after the last end date
            DateTime secondHalfStartDate = rangeEndDate.AddDays(1);
            //End a day after the secondHalfStartDate
            DateTime secondHalfEndDate = secondHalfStartDate.AddDays(1);

            timeSpanToJump = new TimeSpan(1, 0, 0, 0);

            canAddMore = true;
            int entitiesToAddOnBack = entitiesBeforeSelectedService() < (pageSize / 2)
                                          ? pageSize - entitiesBeforeSelectedService()
                                          : pageSize / 2;

            //Only consider the back half of services
            var backHalfServicesToAdd = existingServices.ToList();
            backHalfServicesToAdd.RemoveAll(es => es.ServiceDate < middleDate.Date);

            //Setup the back half of entities (moving forwards from the selectedService)
            while (canAddMore && entitiesAfterSelectedService() < entitiesToAddOnBack)
            {
                var canGenerateMoreServiceTuplesInBack =
                    AddServiceTuplesToCollection(collection, recurringServices, secondHalfStartDate, secondHalfEndDate, CrawlingPosition.MoveForward);

                #region Add eligible existing services

                //Add the eligible existing services (after the generated ServiceTuples are added) so that the dates are properly considered

                //The service date is either: a) <= max serviceTuple.ServiceDate, or b) <= the secondHalfEndDate
                var lastServiceTupleDate = collection.Max != null ? new DateTime?(collection.Max.ServiceDate) : null;
                var servicesToAdd = lastServiceTupleDate != null ?
                    backHalfServicesToAdd.Where(s => s.ServiceDate <= lastServiceTupleDate || s.ServiceDate <= secondHalfEndDate).ToArray() :
                    backHalfServicesToAdd.Where(s => s.ServiceDate <= secondHalfEndDate).ToArray();

                //Add the existing service to the collection and remove it from the backHalfServicesToAdd
                foreach (var service in servicesToAdd)
                {
                    collection.Add(new ServiceTuple(service));

                    backHalfServicesToAdd.Remove(service);
                }

                #endregion

                //canAddMore if there are more service tuples to generate or more services in back of the middleDate
                canAddMore = canGenerateMoreServiceTuplesInBack || backHalfServicesToAdd.Count() > 0;

                //Start a day after the last end date
                secondHalfStartDate = secondHalfEndDate.AddDays(1);

                timeSpanToJump = timeSpanToJump.Add(timeSpanToJump);
                //Move the end date later (double every iteration)
                secondHalfEndDate = secondHalfEndDate.Add(timeSpanToJump);
            }

            //Now that there will be no more entities added, convert the sorted collection to a list so that we can easily remove ranges
            var serviceTuplesList = collection.ToList();

            //Keep track of whether or not the current Context can move backward or forward
            //If there are < (pageSize / 2) entities in a direction, the current context cannot move further in that direction
            var canMoveBackward = entitiesBeforeSelectedService() >= (pageSize / 2);
            var canMoveForward = entitiesAfterSelectedService() >= (pageSize / 2);

            //Trim the first half
            int entitiesToRemoveFromFront = entitiesBeforeSelectedService() - (pageSize / 2);

            if (entitiesToRemoveFromFront > 0)
                serviceTuplesList.RemoveRange(0, entitiesToRemoveFromFront);

            //Trim the second half
            int entitiesToRemoveFromBack = entitiesAfterSelectedService() - entitiesToAddOnBack;
            if (entitiesToRemoveFromBack > 0)
                serviceTuplesList.RemoveRange(serviceTuplesList.Count - entitiesToRemoveFromBack, entitiesToRemoveFromBack);

            return new ServiceGenerationResult { CanMoveBackward = canMoveBackward, CanMoveForward = canMoveForward, ServicesTuples = serviceTuplesList };
        }

        /// <summary>
        /// Generates ServiceTuples from recurring services for a start and end date. 
        /// It will probably return service tuples with dates that extend past those (because it uses Repeat.NextOnOrAfterDate).
        /// </summary>
        /// <param name="collection">The collection to add ServiceTuples to</param>
        /// <param name="recurringServices">The recurring services to generate ServiceTuples from</param>
        /// <param name="estimatedStartDate">The estimated start of the generation range</param>
        /// <param name="estimatedEndDate">The estimated end of the generation range</param>
        /// <param name="position">The crawling position sets which direction this should generate service tuples towards</param>
        /// <returns>Whether or not it can continue crawling</returns>
        private static bool AddServiceTuplesToCollection(SortedSet<ServiceTuple> collection, IEnumerable<RecurringService> recurringServices,
            DateTime estimatedStartDate, DateTime estimatedEndDate, CrawlingPosition position)
        {
            //Make a copy of the recurring services, to filter for eligible recurring services
            var eligibleRecurringServices = recurringServices.ToList();

            //Remove any ineligible existing services and recurring services
            if (position == CrawlingPosition.MoveBackward)
            {
                //When moving closer to the beginning of time:
                //if the endDate of the range is before the ServiceDate/StartDate remove the recurring service
                eligibleRecurringServices.RemoveAll(rs => estimatedEndDate < rs.Repeat.StartDate);
            }

            if (position == CrawlingPosition.MoveForward)
            {
                //When moving closer to the end of time:
                //if the startDate of the range is after the ServiceDate/EndDate remove the recurring service
                eligibleRecurringServices.RemoveAll(rs => rs.Repeat.Frequency == Frequency.Once && estimatedStartDate > rs.Repeat.StartDate);
                eligibleRecurringServices.RemoveAll(rs => rs.Repeat.EndDate.HasValue && estimatedStartDate > rs.Repeat.EndDate);
            }

            //Generate service tuples (from the recurring services) that are within the date range (and not excluded dates)
            var generatedServiceTuples = (from rs in eligibleRecurringServices.Where(rs => rs.Repeat != null)
                                          from serviceDate in rs.Repeat.GetOccurrences(estimatedStartDate, estimatedEndDate)
                                          orderby serviceDate
                                          select new ServiceTuple(serviceDate, rs)).ToList();

            //Get rid of excludedDates and already generated dates
            generatedServiceTuples.RemoveAll(
                generatedServiceTuple =>
                generatedServiceTuple.RecurringServiceToGenerateFrom.ExcludedDates.Contains(generatedServiceTuple.ServiceDate) ||
                generatedServiceTuple.RecurringServiceToGenerateFrom.GeneratedServices.Any(s => s.ServiceDate == generatedServiceTuple.ServiceDate));

            //Add all of the generated services to the collection
            foreach (var generatedService in generatedServiceTuples)
                collection.Add(generatedService);

            //Return whether there are any more services to generate
            //This is for keeping track of whether or not the crawler can continue (forward or backward)
            return eligibleRecurringServices.Count != 0;
        }

        private static int ServiceIndex(IEnumerable<ServiceTuple> serviceTuples, Service selectedEntity)
        {
            int middleServiceIndexInt;

            var collectionAsList = serviceTuples.ToList();

            if (selectedEntity == null) //if there is no selectedEntity find the index of the closest ServiceDate on or after today
                middleServiceIndexInt = collectionAsList.FindIndex(s => s.ServiceDate >= Manager.Context.UserAccount.AdjustTimeForUserTimeZone(DateTime.UtcNow).Date);
            else if (selectedEntity.Generated) //Find the corresponding generated service
                middleServiceIndexInt = collectionAsList.FindIndex(s => s.ServiceDate == selectedEntity.ServiceDate && s.RecurringServiceToGenerateFrom != null &&
                    s.RecurringServiceToGenerateFrom.Id == selectedEntity.RecurringServiceId);
            else
                middleServiceIndexInt = collectionAsList.FindIndex(s => s.Service != null && s.Service.Id == selectedEntity.Id);

            //If a corresponding Service to the selectedEntity was not found, find the index of the closest ServiceDate on or after the selectedEntity.ServiceDate
            if (selectedEntity != null && middleServiceIndexInt == -1)
                middleServiceIndexInt = collectionAsList.FindIndex(s => s.ServiceDate >= selectedEntity.ServiceDate);

            return middleServiceIndexInt;
        }
    }

    ///<summary>
    /// A crawling position. Either crawling backward, stopped in the middle, or crawling forward
    ///</summary>
    public enum CrawlingPosition
    {
        ///<summary>
        /// Crawl backwards
        ///</summary>
        MoveBackward,
        ///<summary>
        /// Stay in middle
        ///</summary>
        StoppedInMiddle,
        ///<summary>
        /// Crawl forward
        ///</summary>
        MoveForward
    }

    ///<summary>
    /// The result of a ServiceGeneration request.
    ///</summary>
    public class ServiceGenerationResult
    {
        ///<summary>
        /// Whether or not the set of recurring services can move backwards in time.
        ///</summary>
        public bool CanMoveBackward { get; set; }

        ///<summary>
        /// Whether or not the set of recurring services can move forward in time.
        ///</summary>
        public bool CanMoveForward { get; set; }

        ///<summary>
        /// The resulting services or services to to generate
        ///</summary>
        public List<ServiceTuple> ServicesTuples { get; set; }
    }

    ///<summary>
    /// This class holds either a service, or a DateTime and RecurringService used to generate a service
    ///</summary>
    public class ServiceTuple : IComparable<ServiceTuple>
    {
        ///<param name="existingService">The existing service to store</param>
        public ServiceTuple(Service existingService)
        {
            Service = existingService;
        }

        ///<param name="serviceDateToGenerate">The service date for the service to generate</param>
        ///<param name="recurringServiceToGenerateFrom">The recurring service to generate the service from</param>
        public ServiceTuple(DateTime serviceDateToGenerate, RecurringService recurringServiceToGenerateFrom)
        {
            ServiceDateToGenerate = serviceDateToGenerate;
            RecurringServiceToGenerateFrom = recurringServiceToGenerateFrom;
        }

        ///<summary>
        /// The ServiceDate of this ServiceTuple. Used for sorting.
        ///</summary>
        public DateTime ServiceDate
        {
            get
            {
                if (Service != null)
                {
                    return Service.ServiceDate;
                }

                return ServiceDateToGenerate;
            }
        }

        ///<summary>
        //This date, in conjunction with RecurringServiceToGenerateFrom, is used to generate a Service
        ///</summary>
        public DateTime ServiceDateToGenerate { get; private set; }

        ///<summary>
        //This RecurringService, in conjunction with ServiceDateToGenerate, is used to generate a Service
        ///</summary>
        public RecurringService RecurringServiceToGenerateFrom { get; private set; }

        ///<summary>
        /// The a existing or generated service
        ///</summary>
        public Service Service { get; private set; }

        ///<summary>
        /// Generates a service if there is not one yet (and if the necessary associations are loaded)
        ///</summary>
        public void GenerateService()
        {
            if (Service != null || RecurringServiceToGenerateFrom == null || RecurringServiceToGenerateFrom.Client == null)
                return;

            Service = new Service
                          {
                              ServiceDate = ServiceDateToGenerate,
                              Client = RecurringServiceToGenerateFrom.Client,
                              RecurringServiceParent = RecurringServiceToGenerateFrom,
                              ServiceProviderId = RecurringServiceToGenerateFrom.Client.BusinessAccount.Id,
                              Generated = true
                          };
        }

        //Compare by service dates, then by Id
        public int CompareTo(ServiceTuple other)
        {
            var dateCompare = this.ServiceDate.CompareTo(other.ServiceDate);

            //If the Dates are different, compare those
            if (dateCompare != 0)
                return dateCompare;

            //If the Dates are the same, compare the Ids (because SortedSet will not add duplicates)
            var xId = this.Service != null ? this.Service.Id : this.RecurringServiceToGenerateFrom.Id;
            var yId = other.Service != null ? other.Service.Id : other.RecurringServiceToGenerateFrom.Id;

            return xId.CompareTo(yId);
        }
    }
}
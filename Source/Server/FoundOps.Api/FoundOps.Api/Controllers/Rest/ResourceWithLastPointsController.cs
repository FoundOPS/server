using FoundOps.Api.Models;
using FoundOps.Api.Tools;
using FoundOps.Core.Tools;
using System;
using System.Linq;

namespace FoundOps.Api.Controllers.Rest
{
    public class ResourceWithLastPointsController : BaseApiController
    {
        //GET /api/trackpoint/GetResourcesWithLatestPoints?roleId={Guid}&date=Datetime
        /// <summary>
        /// Gets all resources for a BusinessAccount and their last recorded location on routes today
        /// </summary>
        /// <param name="roleId">Used to find the Business Account</param>
        /// <returns>A list of Resource (employees or vehicles) with their latest tracked point</returns>
        public IQueryable<ResourceWithLastPoint> Get(Guid roleId)
        {
            var currentBusinessAccount = CoreEntitiesContainer.Owner(roleId).First();
            if (currentBusinessAccount == null)
                throw Request.NotAuthorized();

#if DEBUG
            //Setup the design data
            //Makes it seem like each resource being tracked on a Route is moving
            SetupDesignDataForGetResourcesWithLatestPoints(currentBusinessAccount.Id);
#endif

            var currentUserAccount = CoreEntitiesContainer.CurrentUserAccount().First();
            var userToday = currentUserAccount.Now().Date;

            var resourcesWithTrackPoints = CoreEntitiesContainer.GetResourcesWithLatestPoint(currentBusinessAccount.Id, userToday);

            var modelResources = resourcesWithTrackPoints.Select(ResourceWithLastPoint.ConvertToModel);
            return modelResources.AsQueryable();
        }

#if DEBUG

        /// <summary>
        /// Sets up the design data for GetResourcesWithLatestPoints. Makes it seem like the resource is actually moving
        /// </summary>
        /// <param name="currentBusinessAccountId">The business account to update the design data on.</param>
        private void SetupDesignDataForGetResourcesWithLatestPoints(Guid currentBusinessAccountId)
        {
            var user = CoreEntitiesContainer.CurrentUserAccount().First();

            var serviceDate = user.Now().Date;

            var routes = CoreEntitiesContainer.Routes.Where(r => r.Date == serviceDate && r.OwnerBusinessAccountId == currentBusinessAccountId).OrderBy(r => r.Id);
            var numberOfRoutes = routes.Count();

            var count = 1;

            var random = new Random();
            foreach (var route in routes)
            {
                var routeNumber = count % numberOfRoutes;

                #region Adjust Technician Location, Speed and Heading

                foreach (var employee in route.Employees)
                {
                    employee.LastCompassDirection = (employee.LastCompassDirection + 15) % 360;
                    employee.LastTimeStamp = user.Now();
                    employee.LastSpeed = random.Next(30, 50);

                    switch (routeNumber)
                    {
                        //This would be the 4th, 8th, etc
                        case 0:
                            employee.LastLatitude = employee.LastLatitude + .005;
                            employee.LastLongitude = employee.LastLongitude + .007;
                            break;
                        //This would be the 1st, 5th, etc
                        case 1:
                            employee.LastLatitude = employee.LastLatitude - .005;
                            employee.LastLongitude = employee.LastLongitude + .003;
                            break;
                        //This would be the 2nd, 6th, etc
                        case 2:
                            employee.LastLatitude = employee.LastLatitude + .002;
                            employee.LastLongitude = employee.LastLongitude - .005;
                            break;
                        //This would be the 3rd, 7th, etc
                        case 3:
                            employee.LastLatitude = employee.LastLatitude - .006;
                            employee.LastLongitude = employee.LastLongitude - .005;
                            break;
                    }

                    employee.LastSource = random.Next(0, 1) == 0 ? "iPhone" : "Android";
                }

                #endregion

                count++;
            }

            CoreEntitiesContainer.SaveChanges();
        }

#endif
    }
}

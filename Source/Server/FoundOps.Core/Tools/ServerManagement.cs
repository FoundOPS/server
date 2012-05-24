using System.Linq;
using FoundOps.Core.Models.CoreEntities;

namespace FoundOps.Core.Tools
{
#if DEBUG
    public static class ServerManagement
    {
        public static void PerformServerOperations()
        {
            //Perform operations here
            //SetEmptyDestinations();
        }

        /// <summary>
        /// Sets the empty recurring service destinations to the RecurringService's client's first location.
        /// </summary>
        private static void SetEmptyDestinations()
        {
            var coreEntitiesContainer = new CoreEntitiesContainer();
            var recurringServicesWithoutLocations = coreEntitiesContainer.RecurringServices
                .Include("Client")
                .Include("ServiceTemplate.Fields")
                .Where(rs => rs.ServiceTemplate.Fields.OfType<LocationField>().All(lf => lf.LocationId == null && lf.LocationFieldTypeInt == (decimal) LocationFieldType.Destination))
                .ToArray();

            foreach (var recurringService in recurringServicesWithoutLocations)
            {
                var locationField = recurringService.ServiceTemplate.Fields.OfType<LocationField>().First();
                locationField.Value = recurringService.Client.OwnedParty.Locations.FirstOrDefault();
            }

            coreEntitiesContainer.SaveChanges();
        }
    }
#endif
}
using System.Linq;

namespace FoundOps.Core.Models.CoreEntities.Extensions.Services
{
    public static class ServiceTemplateExtensionMethods
    {
        /// <summary>
        /// Gets the destination from the DestinationField
        /// </summary>
        /// <param name="serviceTemplate">The service template.</param>
        /// <returns>
        /// The destination location, or null if there is not one
        /// </returns>
        public static Location GetDestination(this ServiceTemplate serviceTemplate)
        {
            //Find the destination field
            var destinationField = serviceTemplate.Fields.OfType<LocationField>().FirstOrDefault(lf => lf.LocationFieldType == LocationFieldType.Destination);

            return destinationField == null ? null : destinationField.Value;
        }

        /// <summary>
        /// Sets the destination field.
        /// </summary>
        /// <param name="serviceTemplate">The service template.</param>
        /// <param name="location">The location.</param>
        /// <returns>
        /// True if there is a destination field. False if there is not a destination field
        /// </returns>
        public static bool SetDestination(this ServiceTemplate serviceTemplate, Location location)
        {
            if (location == null)
                return false;

            //Find the destination field
            var destinationField = serviceTemplate.Fields.OfType<LocationField>().FirstOrDefault(lf => lf.LocationFieldType == LocationFieldType.Destination);

            if (destinationField == null)
                return false;

            destinationField.Value = location;

            return true;
        }
    }
}
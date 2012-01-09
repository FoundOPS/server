using System.ComponentModel.Composition;
using FoundOps.Core.Models.CoreEntities;
using System.ComponentModel.DataAnnotations;
using RiaServicesContrib.DomainServices.Client.DataValidation;

namespace FoundOps.SLClient.Data.Validation
{
    /// <summary>
    /// Requires a LocationField to have a value if the field is required.
    /// </summary>
    [Export]
    public class LocationFieldValueRequired : ValidationRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocationFieldValueRequired"/> class.
        /// </summary>
        public LocationFieldValueRequired()
            : base(InputOutput<LocationField, Location>(field => field.Value))
        { }

        /// <summary>
        /// Validates the specified location field.
        /// </summary>
        /// <param name="value">The Location field's value</param>
        /// <returns></returns>

        public ValidationResult Validate(Location value)
        {
            if (value != null)
                return ValidationResult.Success;

            return new ValidationResult("The location is required.");
        }
    }
}

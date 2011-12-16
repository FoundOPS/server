using EntityGraph.Validation;
using FoundOps.Core.Models.CoreEntities;
using System.ComponentModel.DataAnnotations;

namespace FoundOps.SLClient.Data.Validation
{
    /// <summary>
    /// Requires a LocationField to have a value if the field is required.
    /// </summary>
    public class LocationFieldValueRequired : ValidationRule<ValidationResult>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocationFieldValueRequired"/> class.
        /// </summary>
        public LocationFieldValueRequired()
            : base(InputOutput<Field, bool>(field => field.Required), InputOutput<LocationField, Location>(field => field.Value))
        {
        }

        /// <summary>
        /// Validates the specified location field.
        /// </summary>
        /// <param name="fieldIsRequired">if set to <c>true</c> [field is required].</param>
        /// <param name="location">The location field's location.</param>
        /// <returns></returns>
        public ValidationResult Validate(bool fieldIsRequired, Location location)
        {
            if (fieldIsRequired || location != null)
                return ValidationResult.Success;

            return new ValidationResult("The location is required.");
        }
    }
}

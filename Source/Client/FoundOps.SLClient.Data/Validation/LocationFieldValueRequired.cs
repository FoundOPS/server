using System;
using System.ComponentModel.DataAnnotations;
using FoundOps.Core.Models.CoreEntities;
using RiaServicesContrib.DataValidation;
using ValidationRule = RiaServicesContrib.DomainServices.Client.DataValidation.ValidationRule;
using ValidationAttribute = RiaServicesContrib.DomainServices.Client.DataValidation.ValidationAttribute;

namespace FoundOps.SLClient.Data.Validation
{
    /// <summary>
    /// Requires a LocationField to have a value if the field is required.
    /// </summary>
    public class LocationFieldValueRequired : ValidationRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocationFieldValueRequired"/> class.
        /// </summary>
        public LocationFieldValueRequired() : base(InputOnly<LocationField, bool>(field => field.Required), InputOutput<LocationField, Location>(field => field.Value))
        { }

        /// <summary>
        /// Validates the specified location field.
        /// </summary>
        /// <param name="required">If the field is required</param>
        /// <param name="value">The Location field's value</param>
        /// <returns></returns>
        public ValidationResult Validate(bool required, Location value)
        {
            if (required || value != null)
                return ValidationResult.Success;

            return new ValidationResult("The location is required.");
        }
    }

    /// <summary>
    /// Requires a LocationField to have a value if the field is required.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class LocationFieldValueRequiredAttribute : ValidationAttribute
    {
        /// <summary>
        /// Creates a new instance of the MandatoryValidator class.
        /// </summary>
        /// <param name="signature"></param>
        /// <returns></returns>
        protected override ValidationRule<ValidationResult> Create(params ValidationRuleDependency[] signature)
        {
           return new LocationFieldValueRequired ();
        }
    }
}

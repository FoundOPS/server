using System;
using System.ComponentModel.DataAnnotations;

namespace FoundOps.Core.Models.CoreEntities.Validation
{
    public class FieldValidators
    {
        private static ValidationResult IsValueValidHelper(decimal? value, decimal minimum, decimal maximum, NumericField numericField)
        {
            if (numericField.Mask == "c")
                return ValidationResult.Success;

            if(maximum < minimum)
                return new ValidationResult(String.Format("The Maximum must be greater than or equal to the minimum ({0})", minimum), new[] { "Maximum", "Minimum" });

            if (value < minimum)
                return new ValidationResult(String.Format("The Value must be greater than or equal to the minimum ({0})", minimum), new[] { "Value", "Minimum" });

            if (value > maximum)
                return new ValidationResult(String.Format("The Value must be less than or equal to the maximum ({0})", maximum), new[] { "Value", "Maximum" });

            return ValidationResult.Success;
        }

        public static ValidationResult IsValueValid(decimal? value, ValidationContext validationContext)
        {
            if (validationContext == null || value == null)
                return ValidationResult.Success;

            var numericField = (NumericField) validationContext.ObjectInstance;

            return IsValueValidHelper((decimal)value, numericField.Minimum, numericField.Maximum, numericField);
        }

        public static ValidationResult IsValueWithinMinimum(decimal? minimum, ValidationContext validationContext)
        {
            if (validationContext == null || minimum == null)
                return ValidationResult.Success;

            var numericField = (NumericField)validationContext.ObjectInstance;

            return IsValueValidHelper(numericField.Value, (decimal)minimum, numericField.Maximum, numericField);
        }

        public static ValidationResult IsValueWithinMaximum(decimal? maximum, ValidationContext validationContext)
        {
            if (validationContext == null || maximum == null)
                return ValidationResult.Success;

            var numericField = (NumericField)validationContext.ObjectInstance;

            return IsValueValidHelper(numericField.Value, numericField.Minimum, (decimal)maximum, numericField);
        }
    }
}
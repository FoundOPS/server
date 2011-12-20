﻿using System;
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

            if (value.HasValue)
            {
                if (value < minimum)
                    return new ValidationResult(String.Format("The Value must be greater than or equal to the minimum ({0})", minimum), new[] { "Value", "Minimum" });

                if (value > maximum)
                    return new ValidationResult(String.Format("The Value must be less than or equal to the maximum ({0})", maximum), new[] { "Value", "Maximum" });
            }

            return ValidationResult.Success;
        }

        public static ValidationResult IsValueValid(decimal value, ValidationContext validationContext)
        {
            if (validationContext == null)
                return ValidationResult.Success;

            var numericField = (NumericField) validationContext.ObjectInstance;

            return IsValueValidHelper((decimal?)value, numericField.Minimum, numericField.Maximum, numericField);
        }

        public static ValidationResult IsValueWithinMinimum(decimal minimum, ValidationContext validationContext)
        {
            if (validationContext == null)
                return ValidationResult.Success;

            var numericField = (NumericField)validationContext.ObjectInstance;

            return IsValueValidHelper(numericField.Value, minimum, numericField.Maximum, numericField);
        }

        public static ValidationResult IsValueWithinMaximum(decimal maximum, ValidationContext validationContext)
        {
            if (validationContext == null)
                return ValidationResult.Success;

            var numericField = (NumericField)validationContext.ObjectInstance;

            return IsValueValidHelper(numericField.Value, numericField.Minimum, maximum, numericField);
        }

        private static ValidationResult IsTimeValueValidHelper(DateTime? value, DateTime earliest, DateTime latest, DateTimeField dateTimeField)
        {
            if (dateTimeField.Value.HasValue)
            {
                if (value < earliest)
                    return new ValidationResult(String.Format("The Value must be after the earliest ({0})", earliest.ToLongTimeString()), new[] { "Value", "Earliest" });

                if (value > latest)
                    return new ValidationResult(String.Format("The Value must be before the latest ({0})", latest.ToLongTimeString()), new[] { "Value", "Latest" });
            }

            return ValidationResult.Success;
        }

        public static ValidationResult IsTimeValueValid(DateTime? value, ValidationContext validationContext)
        {
            if (validationContext == null)
                return ValidationResult.Success;

            var dateTimeField = (DateTimeField)validationContext.ObjectInstance;

            return IsTimeValueValidHelper(value, dateTimeField.Earliest, dateTimeField.Latest, dateTimeField);
        }

        public static ValidationResult IsTimeValueWithinEarliest(DateTime earliest, ValidationContext validationContext)
        {
            if (validationContext == null)
                return ValidationResult.Success;

            var dateTimeField = (DateTimeField)validationContext.ObjectInstance;

            return IsTimeValueValidHelper(dateTimeField.Value, earliest, dateTimeField.Latest, dateTimeField);
        }

        public static ValidationResult IsTimeValueWithinLatest(DateTime latest, ValidationContext validationContext)
        {
            if (validationContext == null)
                return ValidationResult.Success;

            var dateTimeField = (DateTimeField)validationContext.ObjectInstance;

            return IsTimeValueValidHelper(dateTimeField.Value, dateTimeField.Earliest, latest, dateTimeField);
        }
    }
}
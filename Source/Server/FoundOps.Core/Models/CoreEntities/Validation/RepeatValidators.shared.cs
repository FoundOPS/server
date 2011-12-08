using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace FoundOps.Core.Models.CoreEntities.Validation
{
    public static class RepeatValidators
    {
        public static ValidationResult IsCorrectDayCheckedForStartDate(DateTime startDate, ValidationContext validationContext)
        {
            var repeat = (Repeat)validationContext.ObjectInstance;

            return IsCorrectDayCheckedHelper(startDate, repeat.FrequencyInt, repeat.FrequencyDetailInt);
        }

        public static ValidationResult IsCorrectDayCheckedForFrequencyInt(int frequencyInt, ValidationContext validationContext)
        {
            var repeat = (Repeat) validationContext.ObjectInstance;

            return IsCorrectDayCheckedHelper(repeat.StartDate, frequencyInt, repeat.FrequencyDetailInt);
        }

        public static ValidationResult IsCorrectDayCheckedForFrequencyDetailInt(int? frequencyDetailInt, ValidationContext validationContext)
        {
            var repeat = (Repeat)validationContext.ObjectInstance;

            return IsCorrectDayCheckedHelper(repeat.StartDate, repeat.FrequencyInt, frequencyDetailInt);
        }

        private static ValidationResult IsCorrectDayCheckedHelper(DateTime startDate, int frequencyInt, int? frequencyDetailInt)
        {
            var frequency = (Frequency) frequencyInt;

            if (frequency != Frequency.Weekly)
                return ValidationResult.Success;

            var selectedDaysForWeeklyFrequency = Repeat.IntegerToWeeklyFrequencyDetail(frequencyDetailInt);

            if (frequency == Frequency.Weekly)
            {
                if (selectedDaysForWeeklyFrequency.Any(dayOfWeek => dayOfWeek == startDate.DayOfWeek))
                    return ValidationResult.Success;
            }

            return new ValidationResult("You can not have a Start Date that does not have a service occurrence.", new[] { "StartDate", "FrequencyInt", "FrequencyDetailInt" });
        }
    }
}
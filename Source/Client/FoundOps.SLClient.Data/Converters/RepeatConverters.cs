using FoundOps.Common.Composite;
using FoundOps.Core.Models.CoreEntities;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace FoundOps.SLClient.Data.Converters
{
    ///<summary>
    /// Takes a Repeat and displays a string with a detailed description of the repeat
    /// Ex. Reapeat every 2 weeks on Tuesday
    ///</summary>
    public class RepeatDescriptionConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (!values.Any() || values[0] == null) return "";

            var repeat = (Repeat)values[0];
            var dayOfWeekString = "";

            if (repeat.Frequency == Frequency.Once)
                return String.Format("Once on {0}", repeat.StartDate.ToLongDateString());

            if (repeat.Frequency == Frequency.Daily)
            {
                if (repeat.RepeatEveryTimes == 1)
                    return "Daily";
                return "Every " + repeat.RepeatEveryTimes + " days";
            }

            if (repeat.Frequency == Frequency.Weekly)
            {
                var daysOfWeek = repeat.FrequencyDetailAsWeeklyFrequencyDetail;
                var i = 0;

                foreach (var dayOfWeek in daysOfWeek)
                {
                    string day = "";

                    if (dayOfWeek == DayOfWeek.Sunday)
                        day = "Sun";
                    else if (dayOfWeek == DayOfWeek.Monday)
                        day = "Mon";
                    else if (dayOfWeek == DayOfWeek.Tuesday)
                        day = "Tue";
                    else if (dayOfWeek == DayOfWeek.Wednesday)
                        day = "Wed";
                    else if (dayOfWeek == DayOfWeek.Thursday)
                        day = "Thu";
                    else if (dayOfWeek == DayOfWeek.Friday)
                        day = "Fri";
                    else if (dayOfWeek == DayOfWeek.Saturday)
                        day = "Sat";

                    if (i == 0)
                        dayOfWeekString += day;
                    else
                        dayOfWeekString += ", " + day;

                    i++;
                }
                if (repeat.RepeatEveryTimes == 1)
                    return "Weekly on " + dayOfWeekString;
                return "Every " + repeat.RepeatEveryTimes + " weeks on " + dayOfWeekString;
            }

            if (repeat.Frequency == Frequency.Monthly)
            {
                if (repeat.FrequencyDetailAsMonthlyFrequencyDetail != null)
                {
                    var day = repeat.StartDate.DayOfWeek;
                    var frequency = "";
                    var date = repeat.StartDate.Day;
                    var monthlyFrequency = repeat.FrequencyDetailAsMonthlyFrequencyDetail;

                    if (monthlyFrequency == MonthlyFrequencyDetail.OnDayInMonth)
                        frequency = "the " + DateTimeTools.OrdinalSuffix((int)date);
                    else if (monthlyFrequency == MonthlyFrequencyDetail.FirstOfDayOfWeekInMonth)
                        frequency = "the first " + day + " of the month";
                    else if (monthlyFrequency == MonthlyFrequencyDetail.SecondOfDayOfWeekInMonth)
                        frequency = "the second " + day + " of the month";
                    else if (monthlyFrequency == MonthlyFrequencyDetail.ThirdOfDayOfWeekInMonth)
                        frequency = "the third " + day + " of the month";
                    else if (monthlyFrequency == MonthlyFrequencyDetail.LastOfDayOfWeekInMonth)
                        frequency = "the last " + day + " of the month";
                    else if (monthlyFrequency == MonthlyFrequencyDetail.LastOfMonth)
                        frequency = "the last day of the month";

                    if (repeat.RepeatEveryTimes == 1)
                        return "Monthly on " + frequency;

                    return "Every " + repeat.RepeatEveryTimes + " months on " + frequency;
                }
            }

            if (repeat.Frequency == Frequency.Yearly)
            {
                if (repeat.RepeatEveryTimes == 1)
                    return "Yearly";
                return "Every " + repeat.RepeatEveryTimes + " years ";
            }

            return "";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Returns the Frequency unit properly pluralized.
    /// </summary>
    public class RepeatEveryTimesUnitTypeConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Count() != 2 || values[0] == null || values[1] == null) return "";

            var frequency = (Frequency)values[0];
            var repeatEveryTimesIsSingular = ((int)values[1]) == 1;

            switch (frequency)
            {
                case Frequency.Daily:
                    return repeatEveryTimesIsSingular ? "Day" : "Days";
                case Frequency.Weekly:
                    return repeatEveryTimesIsSingular ? "Week" : "Weeks";
                case Frequency.Monthly:
                    return repeatEveryTimesIsSingular ? "Month" : "Months";
                case Frequency.Yearly:
                    return repeatEveryTimesIsSingular ? "Year" : "Years";
                default:
                    return "";
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class EndOfRepeatToIsCheckedConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null || values.Count() != 2)
                return false;

            var endAfterTimes = (int?)values[0];
            var endDate = (DateTime?)values[1];

            var currentControl = (string)parameter;

            if (currentControl == "NeverEnd" && endAfterTimes == null && endDate == null)
                return true;

            if (currentControl == "EndAfterTimes" && endAfterTimes != null)
                return true;

            if (currentControl == "EndDate" && endDate != null)
                return true;

            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class EndAfterTimesToOccurrencesStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "";
            return ((int)value) > 1 ? "Occurrences" : "Occurrence";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

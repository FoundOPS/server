using System;
using System.Globalization;
using System.Windows.Data;
using FoundOps.Common.Composite;
using FoundOps.Core.Models.CoreEntities;
using Repeat = FoundOps.Core.Models.CoreEntities.Repeat;
using System.Linq;

namespace FoundOps.SLClient.UI.Converters
{
    ///<summary>
    /// Takes a Repeat and displays a string with a detailed description of the repeat
    /// Ex. Reapeat every 2 weeks on Tuesday
    ///</summary>
    public class RepeatDescriptionConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Count() < 1 || values[0] == null) return "";

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
                    {
                        day = "Sun";
                    }
                    else if (dayOfWeek == DayOfWeek.Monday)
                    {
                        day = "Mon";
                    }
                    else if (dayOfWeek == DayOfWeek.Tuesday)
                    {
                        day = "Tue";
                    }
                    else if (dayOfWeek == DayOfWeek.Wednesday)
                    {
                        day = "Wed";
                    }
                    else if (dayOfWeek == DayOfWeek.Thursday)
                    {
                        day = "Thu";
                    }
                    else if (dayOfWeek == DayOfWeek.Friday)
                    {
                        day = "Fri";
                    }
                    else if (dayOfWeek == DayOfWeek.Saturday)
                    {
                        day = "Sat";
                    }

                    if (i == 0)
                        dayOfWeekString += day;
                    else
                    {
                        dayOfWeekString += ", " + day;
                    }
                    i++;
                }
                if (repeat.RepeatEveryTimes == 1)
                    return "Weekly on " + dayOfWeekString;
                return "Every " + repeat.RepeatEveryTimes + " weeks on " + dayOfWeekString;
            }

            if (repeat == null)
            {
                return "";
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
                    {
                        frequency = "the " + DateTimeTools.OrdinalSuffix((int)date);
                    }
                    else if (monthlyFrequency == MonthlyFrequencyDetail.FirstOfDayOfWeekInMonth)
                    {
                        frequency = "the first " + day + " of the month";
                    }
                    else if (monthlyFrequency == MonthlyFrequencyDetail.SecondOfDayOfWeekInMonth)
                    {
                        frequency = "the second " + day + " of the month";
                    }
                    else if (monthlyFrequency == MonthlyFrequencyDetail.ThirdOfDayOfWeekInMonth)
                    {
                        frequency = "the third " + day + " of the month";
                    }
                    else if (monthlyFrequency == MonthlyFrequencyDetail.LastOfDayOfWeekInMonth)
                    {
                        frequency = "the last " + day + " of the month";
                    }
                    else if (monthlyFrequency == MonthlyFrequencyDetail.LastOfMonth)
                    {
                        frequency = "the last day of the month";
                    }
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
}

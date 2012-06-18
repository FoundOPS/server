using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FoundOps.Common.Composite
{
    public static class DateTimeTools
    {
        public static DateTimeFormatInfo GetCurrentDateFormat()
        {
            if (CultureInfo.CurrentCulture.Calendar is GregorianCalendar)
            {
                return CultureInfo.CurrentCulture.DateTimeFormat;
            }
            else
            {
                foreach (var cal in CultureInfo.CurrentCulture.OptionalCalendars)
                {
                    if (cal is GregorianCalendar)
                    {
                        // if the default calendar is not Gregorian, return the
                        // first supported GregorianCalendar dtfi
                        DateTimeFormatInfo dtfi = new CultureInfo(CultureInfo.CurrentCulture.Name).DateTimeFormat;
                        dtfi.Calendar = cal;
                        return dtfi;
                    }
                }

                // if there are no GregorianCalendars in the OptionalCalendars
                // list, use the invariant dtfi
                DateTimeFormatInfo dt = new CultureInfo(CultureInfo.InvariantCulture.Name).DateTimeFormat;
                dt.Calendar = new GregorianCalendar();
                return dt;
            }
        }

        public static string[] ShortestDayNamesInOrder(this DateTimeFormatInfo currentDateFormat)
        {
            var shortestDayNames = currentDateFormat.ShortestDayNames;
            var shortestDayNamesInOrder = new List<string>();
            for (var i = (int)currentDateFormat.FirstDayOfWeek; i < 7; i++)
            {
                shortestDayNamesInOrder.Add(shortestDayNames[i]);
            }
            for (var i = 0; i < (int)currentDateFormat.FirstDayOfWeek; i++)
            {
                shortestDayNamesInOrder.Add(shortestDayNames[i]);
            }
            return shortestDayNamesInOrder.ToArray();
        }

        /// <summary>
        /// Returns the RFC822 formatted date time.
        /// </summary>
        /// <param name="dateTime">The date time to formate.</param>
        /// <returns></returns>
        public static string RFC822(this DateTime dateTime)
        {
            var builder = new StringBuilder(dateTime.ToString("ddd, dd MMM yyyy HH:mm:ss zzz", CultureInfo.InvariantCulture));
            builder.Remove(builder.Length - 3, 1);
            return builder.ToString();
        }

        /// <summary>
        /// The day of the week based on its index. This is useful for different calendars.
        /// What is the third day of the week if the first day of the week is Monday. firstDayOfWeek = DayOfWeek.Monday, index = 3.
        /// </summary>
        /// <param name="firstDayOfWeek">The first DayOfWeek.</param>
        /// <param name="index">The index of the day of the week.</param>
        /// <returns></returns>
        public static DayOfWeek DayOfWeek(DayOfWeek firstDayOfWeek, int index)
        {
            var firstDayOfWeekInt = (int)firstDayOfWeek;

            var newSundayIndex = (int)System.DayOfWeek.Sunday + firstDayOfWeekInt;

            if (index < newSundayIndex)
                return (DayOfWeek)(index + firstDayOfWeekInt);

            return (DayOfWeek)(index - newSundayIndex);
        }

        public static DayOfWeek DayOfWeek(this DateTimeFormatInfo currentDateFormat, int index)
        {
            return DayOfWeek(currentDateFormat.FirstDayOfWeek, index);
        }

        public static int IndexInOrder(DayOfWeek firstDayOfWeek, DayOfWeek dayOfWeek)
        {
            var firstDayOfWeekInt = (int)firstDayOfWeek;

            var newSundayIndex = (int)System.DayOfWeek.Sunday + firstDayOfWeekInt;

            //If the dayOfWeek is before the new firstDayOfWeek
            //subtract what the new Sunday is
            if ((int)dayOfWeek < (int)firstDayOfWeek)
            {
                return (int)dayOfWeek - newSundayIndex;
            }
            //If the dayOfWeek is after the new firstDayOfWeek
            //add the firstDayOfWeekInt
            return (int)dayOfWeek + firstDayOfWeekInt;
        }

        public static int IndexInOrder(this DateTimeFormatInfo currentDateFormat, DayOfWeek dayOfWeek)
        {
            return IndexInOrder(currentDateFormat.FirstDayOfWeek, dayOfWeek);
        }

        public static DateTime NextHalfHour(this DateTime dateTime)
        {
            var minute = dateTime.Minute;
            if (minute > 30)
            {
                //if time is 1:31pm make it 2:00pm
                var hourLater = dateTime.AddHours(1);
                return new DateTime(hourLater.Year, hourLater.Month, hourLater.Day, hourLater.Hour, 0, 0);
            }
            else
            {
                return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 30, 0);
            }
        }

        public static DateTime Midnight(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
        }

        /// <summary>
        /// Gets a DateTime representing noon on the current date
        /// </summary>
        /// <param name="current">The current date</param>
        public static DateTime Noon(this DateTime current)
        {
            DateTime noon = new DateTime(current.Year, current.Month, current.Day, 12, 0, 0);
            return noon;
        }
        /// <summary>
        /// Sets the date of the current date
        /// </summary>
        /// <param name="current">The current date</param>
        /// <param name="year">The year</param>
        /// <param name="month">The month</param>
        /// <param name="day">The day</param>
        /// <returns></returns>
        public static DateTime SetDate(this DateTime current, int year, int month, int day)
        {
            DateTime atTime = new DateTime(year, month, day, current.Hour, current.Minute, current.Second, current.Millisecond);
            return atTime;
        }
        /// <summary>
        /// Sets the date of the current date
        /// </summary>
        /// <param name="current">The current date</param>
        /// <param name="dateToSetTo">The day</param>
        /// <returns></returns>
        public static DateTime SetDate(this DateTime current, DateTime dateToSetTo)
        {
            return current.SetDate(dateToSetTo.Year, dateToSetTo.Month, dateToSetTo.Day);
        }

        /// <summary>
        /// Sets the time of the current date with minute precision
        /// </summary>
        /// <param name="current">The current date</param>
        /// <param name="hour">The hour</param>
        /// <param name="minute">The minute</param>
        public static DateTime SetTime(this DateTime current, int hour, int minute)
        {
            return SetTime(current, hour, minute, 0, 0);
        }

        /// <summary>
        /// Sets the time of the current date with second precision
        /// </summary>
        /// <param name="current">The current date</param>
        /// <param name="hour">The hour</param>
        /// <param name="minute">The minute</param>
        /// <param name="second">The second</param>
        /// <returns></returns>
        public static DateTime SetTime(this DateTime current, int hour, int minute, int second)
        {
            return SetTime(current, hour, minute, second, 0);
        }

        /// <summary>
        /// Sets the time of the current date with millisecond precision
        /// </summary>
        /// <param name="current">The current date</param>
        /// <param name="hour">The hour</param>
        /// <param name="minute">The minute</param>
        /// <param name="second">The second</param>
        /// <param name="millisecond">The millisecond</param>
        /// <returns></returns>
        public static DateTime SetTime(this DateTime current, int hour, int minute, int second, int millisecond)
        {
            DateTime atTime = new DateTime(current.Year, current.Month, current.Day, hour, minute, second, millisecond);
            return atTime;
        }

        /// <summary>
        /// Sets the date to the next date with that DayOfWeek. If it is already that DayOfWeek it will return current
        /// </summary>
        /// <param name="current">The current date</param>
        /// <param name="dayOfWeek">The day of week to set to</param>
        /// <returns></returns>
        public static DateTime SetNext(this DateTime current, DayOfWeek dayOfWeek)
        {
            return current.AddDays(current.CountToNext(dayOfWeek));
        }

        public static int CountToNext(this DateTime current, DayOfWeek dayOfWeek)
        {
            System.DayOfWeek startDayOfWeek = current.DayOfWeek;
            if (dayOfWeek >= startDayOfWeek)
                return dayOfWeek - startDayOfWeek;

            return 7 - (dayOfWeek - startDayOfWeek);
        }

        public static int CountDays(this DayOfWeek day, DateTime start, DateTime end)
        {
            TimeSpan ts = end - start;                       // Total duration
            int count = (int)Math.Floor(ts.TotalDays / 7);   // Number of whole weeks
            int remainder = (int)(ts.TotalDays % 7);         // Number of remaining days
            int sinceLastDay = (int)(end.DayOfWeek - day);   // Number of days since last [day]
            if (sinceLastDay < 0) sinceLastDay += 7;         // Adjust for negative days since last [day]

            // If the days in excess of an even week are greater than or equal to the number days since the last [day], then count this one, too.
            if (remainder >= sinceLastDay) count++;

            return count;
        }

        #region TimeSpanExtensions

        public static TimeSpan Earliest(this TimeSpan thisTimeSpan, TimeSpan other)
        {
            return thisTimeSpan.CompareTo(other) <= 0 ? thisTimeSpan : other;
        }

        public static TimeSpan Latest(this TimeSpan thisTimeSpan, TimeSpan other)
        {
            return thisTimeSpan.CompareTo(other) >= 0 ? thisTimeSpan : other;
        }

        #endregion

        #region DayExtensions


        /// <summary>
        /// Gets a DateTime representing the first day in the current month
        /// </summary>
        /// <param name="current">The current date</param>
        /// <returns></returns>
        public static DateTime FirstDayOfWeekInMonth(this DateTime current)
        {
            DateTime first = current.AddDays(1 - current.Day);
            return first;
        }

        /// <summary>
        /// Gets a DateTime representing the first specified day in the current month
        /// </summary>
        /// <param name="current">The current day</param>
        /// <param name="dayOfWeek">The current day of week</param>
        /// <returns></returns>
        public static DateTime FirstDayOfWeekInMonth(this DateTime current, DayOfWeek dayOfWeek)
        {
            DateTime first = current.FirstDayOfWeekInMonth();

            if (first.DayOfWeek != dayOfWeek)
            {
                first = first.Next(dayOfWeek);
            }

            return first;
        }

        /// <summary>
        /// Gets a DateTime representing the last day in the current month
        /// </summary>
        /// <param name="current">The current date</param>
        /// <returns></returns>
        public static DateTime LastDayInMonth(this DateTime current)
        {
            int daysInMonth = DateTime.DaysInMonth(current.Year, current.Month);

            DateTime last = current.FirstDayOfWeekInMonth().AddDays(daysInMonth - 1);
            return last;
        }

        /// <summary>
        /// Gets a DateTime representing the last specified day in the current month
        /// </summary>
        /// <param name="current">The current date</param>
        /// <param name="dayOfWeek">The current day of week</param>
        /// <returns></returns>
        public static DateTime LastDayOfWeekInMonth(this DateTime current, DayOfWeek dayOfWeek)
        {
            DateTime last = current.LastDayInMonth();
            while (dayOfWeek != last.DayOfWeek)
            {
                last = last.AddDays(-1);
            }
            //last = last.AddDays(Math.Abs(dayOfWeek - last.DayOfWeek) * -1);
            return last;
        }

        /// <summary>
        /// Gets a DateTime representing the first date following the current date which falls on the given day of the week
        /// </summary>
        /// <param name="current">The current date</param>
        /// <param name="dayOfWeek">The day of week for the next date to get</param>
        public static DateTime Next(this DateTime current, DayOfWeek dayOfWeek)
        {
            int offsetDays = dayOfWeek - current.DayOfWeek;

            if (offsetDays <= 0)
            {
                offsetDays += 7;
            }

            DateTime result = current.AddDays(offsetDays);
            return result;
        }

        public static bool IsFirstOfMonth(this DateTime current)
        {
            return current.Day == 1;
        }

        public static bool IsLastOfMonth(this DateTime current)
        {
            return current.Day == DateTime.DaysInMonth(current.Year, current.Month);
        }

        /// <summary>
        /// Ex. First Monday of March
        /// </summary>
        public static bool IsFirstOfDayOfWeekInMonth(this DateTime current)
        {
            return current.FirstDayOfWeekInMonth(current.DayOfWeek).Day == current.Day;
        }

        /// <summary>
        /// Ex. Second Monday of March
        /// </summary>
        public static bool IsSecondOfDayOfWeekInMonth(this DateTime current)
        {
            return (current.FirstDayOfWeekInMonth(current.DayOfWeek).Day + 7) == current.Day;
        }

        /// <summary>
        /// Ex. Third Monday of March
        /// </summary>
        public static bool IsThirdOfDayOfWeekInMonth(this DateTime current)
        {
            return (current.FirstDayOfWeekInMonth(current.DayOfWeek).Day + 14) == current.Day;
        }

        /// <summary>
        /// Ex. Last Monday of March
        /// </summary>
        public static bool IsLastOfDayOfWeekInMonth(this DateTime current)
        {
            return current.LastDayOfWeekInMonth(current.DayOfWeek).Day == current.Day;
        }

        #endregion


        /// <summary>
        /// Returns the Ordinal Suffix for a number. In 1st, st is the ordinal suffix
        /// Ex. num=1, returns st
        /// Ex. num=2, returns nd
        /// </summary>
        public static String OrdinalSuffix(int num)
        {
            //can handle negative numbers (-1st, -12th, -21st)

            int last2Digits = Math.Abs(num % 100);
            int lastDigit = last2Digits % 10;

            //the only nonconforming set is numbers ending in <...eleventh, ...twelfth, ...thirteenth>

            return num.ToString() + "thstndrd".Substring((last2Digits > 10 && last2Digits < 14) || lastDigit > 3 ? 0 : lastDigit * 2, 2);
        }
    }
}

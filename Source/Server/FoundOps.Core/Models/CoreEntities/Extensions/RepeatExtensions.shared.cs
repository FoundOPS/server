using System;
using System.Linq;
using System.Collections;
using FoundOps.Common.Composite;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FoundOps.Common.Composite.Entities;

namespace FoundOps.Core.Models.CoreEntities
{
    public partial class Repeat : IEntityDefaultCreation, ICompositeRaiseEntityPropertyChanged
    {
        public delegate void StartDateChangeEventHandler(DateTime oldDate, DateTime newDate);
        public event StartDateChangeEventHandler StartDateChanging;

        #region Implementation of ICompositeRaiseEntityPropertyChanged

#if SILVERLIGHT
        public void CompositeRaiseEntityPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
#else
        public void CompositeRaiseEntityPropertyChanged(string propertyName)
        {
            OnPropertyChanged(propertyName);
        }
#endif
        #endregion

        #region Implementation of IEntityDefaultCreation

#if SILVERLIGHT
        partial void OnCreated()
        {
            ((IEntityDefaultCreation) this).OnCreate();
        }
#else
        public Repeat()
        {
            ((IEntityDefaultCreation)this).OnCreate();
        }
#endif

        partial void OnCreation(); //For Extensions on Silverlight Side

        void IEntityDefaultCreation.OnCreate()
        {
            this.Id = Guid.NewGuid();
            OnCreation();
        }

        #endregion

        partial void OnStartDateChanging(DateTime value)
        {
            if (StartDateChanging != null)
                StartDateChanging(this.StartDate.Date, value.Date);
        }

        partial void OnEndDateChanged()
        {
            if (EndDate != null)
                EndAfterTimes = null; //Cannot have both set
        }

        partial void OnEndAfterTimesChanged()
        {
            if (EndAfterTimes != null)
                EndDate = null; //Cannot have both set
        }

        public Frequency Frequency
        {
            get { return (Frequency)FrequencyInt; }
            set
            {
                FrequencyInt = (int)value;
                CompositeRaiseEntityPropertyChanged("Frequency");
                CompositeRaiseEntityPropertyChanged("FrequencyDetailAsWeeklyFrequencyDetail");
                CompositeRaiseEntityPropertyChanged("FrequencyDetailAsMonthlyFrequencyDetail");
            }
        }

        partial void OnFrequencyIntChanged()
        {
            CompositeRaiseEntityPropertyChanged("AvailableMonthlyFrequencyDetailTypes");
            FixFrequencyDetailInt();
        }

        private void FixFrequencyDetailInt()
        {
            switch (Frequency)
            {
                case Frequency.Monthly:
                    if (FrequencyDetailAsMonthlyFrequencyDetail == null) //Clear FrequencyDetail if it is not a MonthlyFrequencyDetail
                    {
                        if (AvailableMonthlyFrequencyDetailTypes.Count > 0)
                            //If possible set it to the first available MonthlyFrequencyDetailType
                            FrequencyDetailAsMonthlyFrequencyDetail = AvailableMonthlyFrequencyDetailTypes.First();
                        else
                            FrequencyDetailInt = null;
                    }
                    break;
                case Frequency.Weekly:
                    if (FrequencyDetailAsWeeklyFrequencyDetail == null) //Clear FrequencyDetail if it is not a WeeklyFrequencyDetail
                    {
                        FrequencyDetailAsWeeklyFrequencyDetail = new DayOfWeek[] { };
                    }
                    break;
                default: //Clear FrequencyDetail if Frequency is not Monthly or Weekly
                    FrequencyDetailInt = null;
                    break;
            }
        }

        partial void OnFrequencyDetailIntChanged()
        {
            CompositeRaiseEntityPropertyChanged("FrequencyDetailAsWeeklyFrequencyDetail");
            CompositeRaiseEntityPropertyChanged("FrequencyDetailAsMonthlyFrequencyDetail");
        }

        public DayOfWeek[] FrequencyDetailAsWeeklyFrequencyDetail
        {
            get
            {
                if (Frequency != Frequency.Weekly)
                    return new DayOfWeek[] { };

                return IntegerToWeeklyFrequencyDetail(FrequencyDetailInt);
            }
            set
            {
                var daysOfWeekForWeeklyFrequency = value;
                if (value == null || daysOfWeekForWeeklyFrequency.Count() == 0 || Frequency != Frequency.Weekly)
                {
                    FrequencyDetailInt = null;
                    return;
                }

                var frequencyDetailString = "";

                if (daysOfWeekForWeeklyFrequency.Contains(DayOfWeek.Sunday))
                    frequencyDetailString += "1";

                if (daysOfWeekForWeeklyFrequency.Contains(DayOfWeek.Monday))
                    frequencyDetailString += "2";

                if (daysOfWeekForWeeklyFrequency.Contains(DayOfWeek.Tuesday))
                    frequencyDetailString += "3";

                if (daysOfWeekForWeeklyFrequency.Contains(DayOfWeek.Wednesday))
                    frequencyDetailString += "4";

                if (daysOfWeekForWeeklyFrequency.Contains(DayOfWeek.Thursday))
                    frequencyDetailString += "5";

                if (daysOfWeekForWeeklyFrequency.Contains(DayOfWeek.Friday))
                    frequencyDetailString += "6";

                if (daysOfWeekForWeeklyFrequency.Contains(DayOfWeek.Saturday))
                    frequencyDetailString += "7";

                FrequencyDetailInt = Convert.ToInt32(frequencyDetailString);
            }
        }

        public static DayOfWeek[] IntegerToWeeklyFrequencyDetail(int? integer)
        {
            var daysOfWeekForWeeklyFrequency = new ObservableCollection<DayOfWeek>();

            if (integer == null)
                return daysOfWeekForWeeklyFrequency.ToArray();

            var frequencyDetailString = integer.ToString();

            if (frequencyDetailString.Contains("1"))
                daysOfWeekForWeeklyFrequency.Add(DayOfWeek.Sunday);

            if (frequencyDetailString.Contains("2"))
                daysOfWeekForWeeklyFrequency.Add(DayOfWeek.Monday);

            if (frequencyDetailString.Contains("3"))
                daysOfWeekForWeeklyFrequency.Add(DayOfWeek.Tuesday);

            if (frequencyDetailString.Contains("4"))
                daysOfWeekForWeeklyFrequency.Add(DayOfWeek.Wednesday);

            if (frequencyDetailString.Contains("5"))
                daysOfWeekForWeeklyFrequency.Add(DayOfWeek.Thursday);

            if (frequencyDetailString.Contains("6"))
                daysOfWeekForWeeklyFrequency.Add(DayOfWeek.Friday);

            if (frequencyDetailString.Contains("7"))
                daysOfWeekForWeeklyFrequency.Add(DayOfWeek.Saturday);

            return daysOfWeekForWeeklyFrequency.ToArray();
        }

        public MonthlyFrequencyDetail? FrequencyDetailAsMonthlyFrequencyDetail
        {
            get
            {
                if (FrequencyDetailInt == null || Frequency != Frequency.Monthly)
                    return null;

                return (MonthlyFrequencyDetail)FrequencyDetailInt;
            }
            set
            {
                FrequencyDetailInt = value == null ? (int?)null : (int)value;

                //Check the MonthlyFrequencyDetail is valid
                if (FrequencyDetailAsMonthlyFrequencyDetail != null &&
                    !AvailableMonthlyFrequencyDetailTypes.Contains(FrequencyDetailAsMonthlyFrequencyDetail.Value))
                {
                    FixFrequencyDetailInt();
                }
            }
        }

        partial void OnStartDateChanged()
        {
            CompositeRaiseEntityPropertyChanged("AvailableMonthlyFrequencyDetailTypes");

            if (Frequency == Frequency.Monthly && FrequencyDetailAsMonthlyFrequencyDetail == null &&
                AvailableMonthlyFrequencyDetailTypes.Count > 0)
                //If Frequency is Monthly and the FrequencyDetailAsMonthlyFrequencyDetail is null set it to the first available MonthlyFrequencyDetailType
                FrequencyDetailAsMonthlyFrequencyDetail = AvailableMonthlyFrequencyDetailTypes.First();
        }

        public ObservableCollection<MonthlyFrequencyDetail> AvailableMonthlyFrequencyDetailTypes
        {
            get
            {
                var availableFrequencyDetails = new ObservableCollection<MonthlyFrequencyDetail>();

                if (StartDate != null && Frequency == Frequency.Monthly)
                {
                    var startDateTimeValue = StartDate;

                    availableFrequencyDetails.Add(MonthlyFrequencyDetail.OnDayInMonth);

                    if (startDateTimeValue.IsLastOfMonth())
                        availableFrequencyDetails.Add(MonthlyFrequencyDetail.LastOfMonth);

                    if (startDateTimeValue.IsFirstOfDayOfWeekInMonth())
                        availableFrequencyDetails.Add(MonthlyFrequencyDetail.FirstOfDayOfWeekInMonth);

                    if (startDateTimeValue.IsSecondOfDayOfWeekInMonth())
                        availableFrequencyDetails.Add(MonthlyFrequencyDetail.SecondOfDayOfWeekInMonth);

                    if (startDateTimeValue.IsThirdOfDayOfWeekInMonth())
                        availableFrequencyDetails.Add(MonthlyFrequencyDetail.ThirdOfDayOfWeekInMonth);

                    if (startDateTimeValue.IsLastOfDayOfWeekInMonth())
                        availableFrequencyDetails.Add(MonthlyFrequencyDetail.LastOfDayOfWeekInMonth);
                }

                return availableFrequencyDetails;
            }
        }

        /// <summary>
        /// Returns the next scheduled date.
        /// </summary>
        /// <param name="onOrAfterDate">The on or after date.</param>
        /// <returns>The next repeat date on or after the <param name="onOrAfterDate"/> </returns>
        public DateTime? NextRepeatDateOnOrAfterDate(DateTime onOrAfterDate)
        {
            var endDate = EndDate;

            //if (!StartDate.HasValue) // Checks if the start date has a value. If not return null because the schedule never started
            //    return null;

            // If the onOrAfterDate is >= the StartDate we are in luck and can just return the StartDate! :)
            if (StartDate >= onOrAfterDate.Date)
            {
                return StartDate.Date;
            }

            // checks to see if the end date exists and whether you are looking at a date passed it
            if (endDate.HasValue && onOrAfterDate > endDate.Value)
                return null;

            if (Frequency == Frequency.Once)
            {
                // since start date is the only occurence, this checks to see if the day you are looking at is before that date
                if (StartDate >= onOrAfterDate.Date)
                    return StartDate.Date;
                return null;
            }

            #region Daily

            if (Frequency == Frequency.Daily)
            {
                // calculates the actual end date if the user selected the service to end after "x" number of times
                if (EndAfterTimes != null)
                    endDate = StartDate.AddDays(RepeatEveryTimes * ((int)EndAfterTimes - 1));


                var daysRemainingUntilNextScheduledDate = 0;
                // checks whether the current day being viewed is a day on hich service occurs
                // if not then this calculates the number of days until the next service date
                if ((onOrAfterDate.Subtract(StartDate).Days % RepeatEveryTimes) != 0)
                    daysRemainingUntilNextScheduledDate = RepeatEveryTimes - (onOrAfterDate.Subtract(StartDate).Days % RepeatEveryTimes);

                // here the extra days are actually added from above
                var returnedDate = onOrAfterDate.Date.AddDays(daysRemainingUntilNextScheduledDate);

                // this checks to be sure that the date to be returned as the next scheduled date is not after the the end date
                // if it is, null is returned
                if (returnedDate <= endDate || endDate == null)
                    return returnedDate;
                return null;
            }

#endregion

            #region Weekly

            if (Frequency == Frequency.Weekly)
            {
                //If there are no selected days of the week there is never a scheduled date
                if (FrequencyDetailAsWeeklyFrequencyDetail.Count() <= 0)
                    return null;

                var daysLeftInStartWeek = 0;

                // calculates the actual end date if the user selected the service to end after "x" number of times
                if (EndAfterTimes != null)
                {
                    //goes through each day of the week that the service is scheduled for 
                    //checks to see if it is after the DayOfWeek of the StartDate
                    //if so the 1 is added to daysLeftInStartWeek
                    foreach (var dayOfWeek in FrequencyDetailAsWeeklyFrequencyDetail)
                    {
                        if ((int)dayOfWeek >= (int)StartDate.DayOfWeek)
                            daysLeftInStartWeek++;
                    }
                    // claculates the number of weeks that need to be added to the start week in order to get to the end week
                    var numWeeksToAdd = ((EndAfterTimes - 1) - daysLeftInStartWeek) /
                                        FrequencyDetailAsWeeklyFrequencyDetail.Count();
                    // calculates the number of sertvice days that are required in the final week of the service
                    var daysInLastWeek = ((EndAfterTimes - 1) - daysLeftInStartWeek) %
                                         FrequencyDetailAsWeeklyFrequencyDetail.Count();
                    // brings the start date back to sunday so it is easier to work with
                    var sundayOfStartDateWeek = StartDate.AddDays(-1 * (int)StartDate.DayOfWeek);
                    // here the sunday of the end week is found
                    var endWeek = sundayOfStartDateWeek.AddDays(((int)numWeeksToAdd + 1) * (7 * RepeatEveryTimes));

                    // days are added to endWeek as needed to get to the correct number of service days in 
                    // based on the daysInLastWeek calculated above
                    if (EndAfterTimes <= daysLeftInStartWeek) //Checks To be sure that there is a neccessity to repeat past the start week
                        endDate = sundayOfStartDateWeek.AddDays((int)FrequencyDetailAsWeeklyFrequencyDetail[(int)EndAfterTimes - 1]);
                    else
                        endDate = endWeek.AddDays((int)FrequencyDetailAsWeeklyFrequencyDetail[(int)daysInLastWeek]);
                }
                var returnedDate = new DateTime?();
                //brings start date to sunday so it is easier to work with
                var newStartDate = StartDate.AddDays(-1 * ((int)StartDate.DayOfWeek));
                // brings the onOrAfterDate to sunday so it is easier to work with
                var newOnOrAfterDate = onOrAfterDate.AddDays(-1 * ((int)onOrAfterDate.DayOfWeek));
                //calculates the differnce between the current day being viewed and the start date in weeks
                var weekDifference = (newOnOrAfterDate - newStartDate).Days % (7 * RepeatEveryTimes) / 7;

                var serviceWeek = 0;
                // checks if the week being viewed is a week in which a service occurs
                if (weekDifference == 0)
                    serviceWeek = 1;

                if (FrequencyDetailAsWeeklyFrequencyDetail.Count() == 0)
                    return null;
                // finds the next scheduled week


                var nextScheduledWeek = onOrAfterDate.AddDays((RepeatEveryTimes * 7) -
                                                              (onOrAfterDate.Subtract(StartDate).Days %
                                                               (RepeatEveryTimes * 7)));

                //finds the last day in any given week in which a service occurs (i.e. Saturday)
                var lastServiceDayofWeek =
                    (int)FrequencyDetailAsWeeklyFrequencyDetail[FrequencyDetailAsWeeklyFrequencyDetail.Count() - 1];

                var onOrAfterDateNumberRepresentation = (int)onOrAfterDate.DayOfWeek;

                var spanBetweenDays = (onOrAfterDate.Date - StartDate.Date).Days;

                DateTime? nextScheduledDate = null;

                var weekCheck = 0;
                //checks to be sure that the service did not begin less than a week ago and if there will be any off weeks
                if (RepeatEveryTimes == 1)
                    weekCheck = 1;

                if ((lastServiceDayofWeek < onOrAfterDateNumberRepresentation || weekCheck != 1) && serviceWeek == 0)
                {
                    // find the difference between the start week and the next work week
                    var weeksToAdd = RepeatEveryTimes - weekDifference;
                    var daysToAdd = weeksToAdd * 7;

                    // add the difference between the two to the on or after day, then find the first service day of the week
                    var newOnOrAfterDateBeginningOfWeek = newOnOrAfterDate.AddDays(daysToAdd);

                    var newOnOrAfterDateNumberRep = (int)newOnOrAfterDateBeginningOfWeek.DayOfWeek;


                    //goes through each day that a service can be on and checks if that could be the next service day
                    foreach (var dayOfWeek in FrequencyDetailAsWeeklyFrequencyDetail)
                    {
                        if (newOnOrAfterDateNumberRep <= (int)dayOfWeek)
                        {
                            var numDaysUntilNextServiceDay = (int)dayOfWeek - newOnOrAfterDateNumberRep;

                            nextScheduledDate = newOnOrAfterDateBeginningOfWeek.AddDays(numDaysUntilNextServiceDay);

                            returnedDate = nextScheduledDate.Value;
                            if (returnedDate <= endDate || endDate == null)
                                return returnedDate.Value.Date;
                            return null;
                        }
                    }

                }

                else if (lastServiceDayofWeek < onOrAfterDateNumberRepresentation && weekDifference == 0)
                {
                    var daysToAdd = RepeatEveryTimes * 7;

                    var newOnOrAfterDateBeginningOfWeek = newOnOrAfterDate.AddDays(daysToAdd);

                    var newOnOrAfterDateNumberRep = (int)newOnOrAfterDateBeginningOfWeek.DayOfWeek;

                    //goes through each day that a service can be on and checks if that could be the next service day
                    foreach (var dayOfWeek in FrequencyDetailAsWeeklyFrequencyDetail)
                    {
                        if (newOnOrAfterDateNumberRep <= (int)dayOfWeek)
                        {
                            var numDaysUntilNextServiceDay = (int)dayOfWeek - newOnOrAfterDateNumberRep;

                            nextScheduledDate = newOnOrAfterDateBeginningOfWeek.AddDays(numDaysUntilNextServiceDay);

                            returnedDate = nextScheduledDate.Value;

                            if (returnedDate <= endDate || endDate == null)
                                return returnedDate.Value.Date;
                            return null;
                        }
                    }
                }

                //goes through each day that a service can be on and checks if that could be the next service day
                foreach (var dayOfWeek in FrequencyDetailAsWeeklyFrequencyDetail)
                {
                    if (onOrAfterDateNumberRepresentation <= (int)dayOfWeek)
                    {
                        var numDaysUntilNextServiceDay = (int)dayOfWeek - onOrAfterDateNumberRepresentation;

                        nextScheduledDate = onOrAfterDate.AddDays(numDaysUntilNextServiceDay);

                        returnedDate = nextScheduledDate.Value;
                        if (returnedDate <= endDate || endDate == null)
                            return returnedDate.Value.Date;
                        return null;
                    }
                }

                nextScheduledWeek = nextScheduledWeek.AddDays(-1 * (int)nextScheduledWeek.DayOfWeek);
                // not sure if this is actually needed but im too scared to delete it right now...I dont want to break this thing
                foreach (var dayOfWeek in FrequencyDetailAsWeeklyFrequencyDetail)
                {
                    var dayOfWeekInNextScheduledWeek = nextScheduledWeek.AddDays((int)dayOfWeek);

                    if (dayOfWeekInNextScheduledWeek <= endDate &&
                         dayOfWeekInNextScheduledWeek >= StartDate)
                        returnedDate = dayOfWeekInNextScheduledWeek;
                    if (returnedDate <= endDate || endDate == null)
                        return returnedDate;
                    return null;
                }
            }

#endregion

            #region Monthly

            if (Frequency == Frequency.Monthly)
            {
                // calculates the actual end date if the user selected the service to end after "x" number of times
                if (EndAfterTimes != null)
                {
                    var toSubtract = 1 - StartDate.Day;

                    // allows us to start with the first of every month becuase we might be on Jan 31 and need to end sometime in Feb.
                    var startDateAtBegOfMonth = StartDate.AddDays(toSubtract);
                    EndAfterTimes = EndAfterTimes - 1;

                    // calculates the month that the service will end in
                    var endMonth = startDateAtBegOfMonth.AddMonths((int)EndAfterTimes * RepeatEveryTimes);


                    // Checks to be sure that the endMonth has at least the same number of days as the start month of OnDayInMonth case
                    while (FrequencyDetailAsMonthlyFrequencyDetail == MonthlyFrequencyDetail.OnDayInMonth)
                    {
                        if (DateTime.DaysInMonth(endMonth.Year, endMonth.Month) >= StartDate.Day)
                            break;

                        endMonth = endMonth.AddMonths(RepeatEveryTimes);
                    }

                    // switch statement to set the day in the end month correctly
                    switch (FrequencyDetailAsMonthlyFrequencyDetail)
                    {
                        case MonthlyFrequencyDetail.OnDayInMonth:
                            endDate = new DateTime(endMonth.Year, endMonth.Month, StartDate.Day).Date;
                            break;
                        case MonthlyFrequencyDetail.LastOfMonth:
                            endDate = endMonth.LastDayInMonth().Date;
                            break;
                        case MonthlyFrequencyDetail.FirstOfDayOfWeekInMonth:
                            endDate = endMonth.FirstDayOfWeekInMonth(StartDate.DayOfWeek).Date;
                            break;
                        case MonthlyFrequencyDetail.SecondOfDayOfWeekInMonth:
                            endDate = endMonth.FirstDayOfWeekInMonth(StartDate.DayOfWeek).AddDays(7).Date;
                            break;
                        case MonthlyFrequencyDetail.ThirdOfDayOfWeekInMonth:
                            endDate = endMonth.FirstDayOfWeekInMonth(StartDate.DayOfWeek).AddDays(7).AddDays(7).Date;
                            break;
                        case MonthlyFrequencyDetail.LastOfDayOfWeekInMonth:
                            endDate = endMonth.LastDayOfWeekInMonth(StartDate.DayOfWeek).Date;
                            break;
                    }
                }

                // variable to test if the output date of the current month is after the onOrAfterDate given
                DateTime possibleReturnedDateForCurrentMonth;
                // final return date
                var returnedDate = new DateTime();
                // A check to be sure that there is a Frequency Detail selected
                if (FrequencyDetailAsMonthlyFrequencyDetail == null)
                    return null;
                //Checks if the start date is the current date being viewed so everything below can be short circuited
                if (StartDate == onOrAfterDate)
                    return StartDate.Date;
                //finds the first day of the current month being viewed
                var onOrAfterDateBegOfMonth = onOrAfterDate.AddDays((-1 * onOrAfterDate.Day) + 1);

                var differenceInYears = onOrAfterDate.Year - StartDate.Year;
                var differenceInMonths = (onOrAfterDate.Month - StartDate.Month) + (differenceInYears * 12);
                DateTime newOnOrAfterDate;

                if (differenceInMonths % RepeatEveryTimes != 0)
                    newOnOrAfterDate = onOrAfterDateBegOfMonth.AddMonths(RepeatEveryTimes - (differenceInMonths % RepeatEveryTimes));
                else
                {
                    newOnOrAfterDate = onOrAfterDateBegOfMonth.Date;
                }

                if (FrequencyDetailAsMonthlyFrequencyDetail == MonthlyFrequencyDetail.OnDayInMonth)
                {
                    var fixedOnOrAfterDate = CheckDateAvailability(endDate, newOnOrAfterDate);

                    if (fixedOnOrAfterDate == null)
                        return null;

                    newOnOrAfterDate = (DateTime)fixedOnOrAfterDate;

                    //Otherwise return the Start Date's Day in Month for the Next Scheduled Month
                    possibleReturnedDateForCurrentMonth = new DateTime(newOnOrAfterDate.Year, newOnOrAfterDate.Month, StartDate.Day).Date;
                    // checks to be sure the return date found above is on or after the current day being viewed
                    if (possibleReturnedDateForCurrentMonth.Date >= onOrAfterDate.Date)
                        returnedDate = possibleReturnedDateForCurrentMonth;
                    else
                    {
                        // else adds the number of months for RepeatEvery and finds the correct return date
                        newOnOrAfterDate = newOnOrAfterDate.AddMonths(RepeatEveryTimes);

                        //Remains true only if the date that is supposed to be returned is invalid
                        var monthWithNotEnoughDays = true;

                        do
                        {
                            // Makes sure that the return date is a valid DateTime
                            try
                            {
                                monthWithNotEnoughDays = false;
                                returnedDate = new DateTime(newOnOrAfterDate.Year, newOnOrAfterDate.Month, StartDate.Day).Date;
                            }
                            catch (Exception)
                            {
                                // If the month that was supposed to be returned doesnt have enough day in it, skip that occurrance and return the next one
                                newOnOrAfterDate = newOnOrAfterDate.AddMonths(RepeatEveryTimes);

                                //Date returned was invalid so skip this occurrance and move to the next one
                                monthWithNotEnoughDays = true;
                            }
                        } while (monthWithNotEnoughDays);
                        
                        
                    }
                }

                if (FrequencyDetailAsMonthlyFrequencyDetail == MonthlyFrequencyDetail.LastOfMonth)
                {
                    possibleReturnedDateForCurrentMonth = newOnOrAfterDate.LastDayInMonth().Date;
                    // checks to be sure the return date found above is on or after the current day being viewed
                    if (possibleReturnedDateForCurrentMonth.Date >= onOrAfterDate.Date)
                        returnedDate = possibleReturnedDateForCurrentMonth;
                    else
                    {
                        // else adds the number of months for RepeatEvery and finds the correct return date
                        newOnOrAfterDate = newOnOrAfterDate.AddMonths(RepeatEveryTimes);
                        returnedDate = newOnOrAfterDate.LastDayInMonth().Date;
                    }
                }

                if (FrequencyDetailAsMonthlyFrequencyDetail == MonthlyFrequencyDetail.FirstOfDayOfWeekInMonth)
                {
                    possibleReturnedDateForCurrentMonth = newOnOrAfterDate.FirstDayOfWeekInMonth(StartDate.DayOfWeek).Date;
                    // checks to be sure the return date found above is on or after the current day being viewed
                    if (possibleReturnedDateForCurrentMonth.Date >= onOrAfterDate.Date)
                        returnedDate = possibleReturnedDateForCurrentMonth;
                    else
                    {
                        // else adds the number of months for RepeatEvery and finds the correct return date
                        newOnOrAfterDate = newOnOrAfterDate.AddMonths(RepeatEveryTimes);
                        returnedDate = newOnOrAfterDate.FirstDayOfWeekInMonth(StartDate.DayOfWeek).Date;
                    }
                }

                if (FrequencyDetailAsMonthlyFrequencyDetail == MonthlyFrequencyDetail.SecondOfDayOfWeekInMonth)
                {
                    possibleReturnedDateForCurrentMonth = newOnOrAfterDate.FirstDayOfWeekInMonth(StartDate.DayOfWeek).AddDays(7).Date;
                    // checks to be sure the return date found above is on or after the current day being viewed               
                    if (possibleReturnedDateForCurrentMonth.Date >= onOrAfterDate.Date)
                        returnedDate = possibleReturnedDateForCurrentMonth;
                    else
                    {
                        // else adds the number of months for RepeatEvery and finds the correct return date
                        newOnOrAfterDate = newOnOrAfterDate.AddMonths(RepeatEveryTimes);
                        returnedDate = newOnOrAfterDate.FirstDayOfWeekInMonth(StartDate.DayOfWeek).AddDays(7).Date;
                    }
                }

                if (FrequencyDetailAsMonthlyFrequencyDetail == MonthlyFrequencyDetail.ThirdOfDayOfWeekInMonth)
                {
                    possibleReturnedDateForCurrentMonth = newOnOrAfterDate.FirstDayOfWeekInMonth(StartDate.DayOfWeek).AddDays(7).AddDays(7).Date;
                    // checks to be sure the return date found above is on or after the current day being viewed
                    if (possibleReturnedDateForCurrentMonth.Date >= onOrAfterDate.Date)
                        returnedDate = possibleReturnedDateForCurrentMonth;
                    else
                    {
                        // else adds the number of months for RepeatEvery and finds the correct return date
                        newOnOrAfterDate = newOnOrAfterDate.AddMonths(RepeatEveryTimes);
                        returnedDate = newOnOrAfterDate.FirstDayOfWeekInMonth(StartDate.DayOfWeek).AddDays(7).AddDays(7).Date;
                    }
                }

                if (FrequencyDetailAsMonthlyFrequencyDetail == MonthlyFrequencyDetail.LastOfDayOfWeekInMonth)
                {
                    possibleReturnedDateForCurrentMonth = newOnOrAfterDate.LastDayOfWeekInMonth(StartDate.DayOfWeek).Date;
                    // checks to be sure the return date found above is on or after the current day being viewed
                    if (possibleReturnedDateForCurrentMonth.Date >= onOrAfterDate.Date)
                        returnedDate = possibleReturnedDateForCurrentMonth;
                    else
                    {
                        // else adds the number of months for RepeatEvery and finds the correct return date
                        newOnOrAfterDate = newOnOrAfterDate.AddMonths(RepeatEveryTimes);
                        returnedDate = newOnOrAfterDate.LastDayOfWeekInMonth(StartDate.DayOfWeek).Date;
                    }
                }

                //If the current month's last day is less than the Start Date's day return null
                if (returnedDate.Day > DateTime.DaysInMonth(StartDate.Year, StartDate.Month))
                    return null;

                // checks to be sure that the date to be returned is not after the ends date
                if (returnedDate <= endDate || endDate == null)
                    return returnedDate;

                return null;
            }

#endregion

            #region Yearly

            if (Frequency == Frequency.Yearly)
            {
                //checks if the start date is after the day being viewed and returns start date if true
                if (StartDate >= onOrAfterDate)
                    return StartDate.Date;

                // calculates the actual end date if the user selected the service to end after "x" number of times
                if (EndAfterTimes != null)
                    endDate = StartDate.AddYears(RepeatEveryTimes * (int)EndAfterTimes);

                // calculates the difference in years from the current daty being looked at and the start date of the service
                var differenceInYears = onOrAfterDate.Year - StartDate.Year;

                // intializes the variable to the current year 
                var nextScheduledYear = onOrAfterDate.Date;

                // checks whether or not the current year is a year with the service in it
                if (differenceInYears % RepeatEveryTimes != 0 || onOrAfterDate.Date > StartDate.Date)
                    nextScheduledYear = onOrAfterDate.AddYears(RepeatEveryTimes - (differenceInYears % RepeatEveryTimes));

                //uses nextScheduledYear to calculate the date to be returened
                var returnedDate = new DateTime(nextScheduledYear.Year, StartDate.Month, StartDate.Day).Date;

                // checks to be sure that the date to be returned is not after the ends date
                if (returnedDate <= endDate || endDate == null)
                    return returnedDate;

                return null;
            }

            return null;

            #endregion
        }


        private DateTime? CheckDateAvailability(DateTime? endDate, DateTime newOnOrAfterDate)
        {
            var daysInMonth = DateTime.DaysInMonth(newOnOrAfterDate.Year, newOnOrAfterDate.Month);

            if (daysInMonth >= StartDate.Day)
                return newOnOrAfterDate;

            while (endDate != null && newOnOrAfterDate.Date <= ((DateTime)endDate).Date)
            {
                if (daysInMonth < StartDate.Day)
                {
                    newOnOrAfterDate = newOnOrAfterDate.AddMonths(RepeatEveryTimes);
                }
                else
                {
                    return newOnOrAfterDate;
                }

                daysInMonth = DateTime.DaysInMonth(newOnOrAfterDate.Year, newOnOrAfterDate.Month);
            }

            return null;
        }

        /// <summary>
        /// Gets the occurrences.
        /// </summary>
        /// <param name="startRangeInclusive">The start range to iterate from (inclusive).</param>
        /// <param name="endRangeInclusive">The end range to iterate to (inclusive).</param>
        /// <returns></returns>
        public IEnumerable<DateTime> GetOccurrences(DateTime startRangeInclusive, DateTime endRangeInclusive)
        {
            startRangeInclusive = startRangeInclusive.Date;
            endRangeInclusive = endRangeInclusive.Date;

            var currentDateToObserve = new DateTime?(startRangeInclusive);

            do
            {
                var current = this.NextRepeatDateOnOrAfterDate(currentDateToObserve.Value);

                if (!current.HasValue || current.Value == DateTime.MinValue)
                    break;

                currentDateToObserve = current.Value.AddDays(1);

                yield return current.Value;

            } while (currentDateToObserve <= endRangeInclusive);
        }
    }

    public enum Frequency
    {
        Null = 0,
        Once = 1,
        Daily = 2,
        Weekly = 3,
        Monthly = 4,
        Yearly = 5
    }

    public enum MonthlyFrequencyDetail
    {
        /// <summary>
        /// FrequencyDetail is stored as an int.
        /// 1-7 are reserved for WeeklyFrequencyDetail for the days of the week
        /// Ex. 1235 means Sunday, Monday, Tuesday, and Thursday
        /// </summary>

        OnDayInMonth = 8, //Ex. The 3rd of the month. Cannot be greater than 28 days
        LastOfMonth = 10, //Example Febuary 28th
        FirstOfDayOfWeekInMonth = 11, //Ex. First Monday
        SecondOfDayOfWeekInMonth = 12, //Ex. Second Monday
        ThirdOfDayOfWeekInMonth = 13, //Ex. Third Monday
        LastOfDayOfWeekInMonth = 14 //Ex. Last Monday
    }

    /// <summary>
    /// Creates an enumerator which steps through each Repeat date until the end of the Repeat,
    /// until the <see param="endDate"/>, or until the end of time
    /// </summary>
    public class RepeatEnumerator : IEnumerator<DateTime?>, IEnumerable<DateTime?>
    {
        private readonly DateTime? _endDate;
        public DateTime? Current { get; private set; }

        private readonly Repeat _repeat;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepeatEnumerator"/> class.
        /// </summary>
        /// <param name="repeat">The repeat to generate the enumerator from</param>
        public RepeatEnumerator(Repeat repeat)
        {
            if (repeat == null)
                throw new NotSupportedException("Must initialize with a repeat");
            _repeat = repeat;

            Current = repeat.StartDate.Date;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="RepeatEnumerator"/> class.
        /// </summary>
        /// <param name="repeat">The repeat to generate the enumerator from</param>
        /// <param name="endDate">The date to stop enumerating on</param>
        public RepeatEnumerator(Repeat repeat, DateTime endDate)
            : this(repeat)
        {
            _endDate = endDate.Date;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepeatEnumerator"/> class.
        /// </summary>
        /// <param name="repeat">The repeat to generate the enumerator from</param>
        /// <param name="endDate">The date to start enumerating on</param>
        /// <param name="endDate">The date to stop enumerating on</param>
        public RepeatEnumerator(Repeat repeat, DateTime startDate, DateTime? endDate)
            : this(repeat)
        {
            Current = startDate;
            _endDate = endDate;
        }

        #region Implementation of IEnumerator<DateTime>

        public bool MoveNext()
        {
            if (Current == null)
                return false;

            //Near the end of time
            if (Current.Value.Year >= DateTime.MaxValue.Year - 100)
            {
                Current = null;
                return false;
            }

            var dayAfterCurrentDate = Current.Value.AddDays(1);
            var nextDate = _repeat.NextRepeatDateOnOrAfterDate(dayAfterCurrentDate);

            var canMoveNext = nextDate != null && (_endDate == null || nextDate <= _endDate);
            if (canMoveNext)
                Current = nextDate.Value;
            else
                Current = null;

            return canMoveNext;
        }

        public void Reset()
        {
            Current = _repeat.StartDate.Date; ;
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }

        #region Implementation of IDisposable

        public void Dispose()
        {
        }

        #endregion

        #endregion

        #region Implementation of IEnumerable<DateTime?>

        public IEnumerator<DateTime?> GetEnumerator()
        {
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }

        #endregion
    }
}
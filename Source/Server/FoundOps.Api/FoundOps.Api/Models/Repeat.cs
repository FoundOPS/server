using System;
using System.Collections.ObjectModel;
using System.Linq;
using FoundOps.Api.Tools;
using FoundOps.Common.Composite;
using FoundOps.Core.Models.CoreEntities;

namespace FoundOps.Api.Models
{
    public class Repeat : IImportable
    {
        public Guid Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? EndAfterTimes { get; set; }
        public int? RepeatEveryTimes { get; set; }
        public int? FrequencyInt { get; set; }
        public int? FrequencyDetailInt { get; set; }
        public Guid? RecurringServiceId { get; set; }
        public int? StatusInt { get; set; }

        public Frequency Frequency
        {
            get { return (Frequency) FrequencyInt; }
            set { FrequencyInt = (int) value; }
        }

        public DayOfWeek[] FrequencyDetailAsWeeklyFrequencyDetail
        {
            get
            {
                if(Frequency != Frequency.Weekly)
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

        public ObservableCollection<MonthlyFrequencyDetail> AvailableMonthlyFrequencyDetailTypes
        {
            get
            {
                var availableFrequencyDetails = new ObservableCollection<MonthlyFrequencyDetail>();

                if (Frequency == Frequency.Monthly)
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
    }
}
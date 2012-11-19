using System;

namespace FoundOps.Core.Models.CoreEntities.DesignData
{
    public class RepeatDesignData
    {
        public Repeat DesignOnceRepeat { get; private set; }
        public Repeat DesignDailyRepeat { get; private set; }
        public Repeat DesignWeeklyRepeat { get; private set; }
        public Repeat DesignNeverEndingWeeklyRepeat { get; private set; }
        public Repeat DesignMonthlyRepeat { get; private set; }
        public Repeat DesignYearlyRepeat { get; private set; }

        public RepeatDesignData()
        {
            var date = DateTime.UtcNow.Date;

            DesignOnceRepeat = new Repeat
            {
                StartDate = date,
                Frequency = Frequency.Once,
                CreatedDate = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            };

            DesignDailyRepeat = new Repeat
            {
                StartDate = date.AddDays(-5),
                Frequency = Frequency.Daily,
                EndDate = date.Add(new TimeSpan(10, 0, 0, 0)),
                RepeatEveryTimes = 2,
                CreatedDate = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            };

            DesignWeeklyRepeat = new Repeat
            {
                StartDate = date.AddDays(-14),
                Frequency = Frequency.Weekly,
                FrequencyDetailAsWeeklyFrequencyDetail = new[] { date.DayOfWeek },
                RepeatEveryTimes = 2,
                CreatedDate = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            };

            DesignNeverEndingWeeklyRepeat = new Repeat
            {
                StartDate = date.Date.AddDays(-14),
                Frequency = Frequency.Weekly,
                FrequencyDetailAsWeeklyFrequencyDetail = new[] { date.DayOfWeek },
                RepeatEveryTimes = 2,
                CreatedDate = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            };

            DesignMonthlyRepeat = new Repeat
            {
                StartDate = date,
                Frequency = Frequency.Monthly,
                EndDate = date.AddMonths(6),
                RepeatEveryTimes = 1,
                CreatedDate = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            };

            DesignYearlyRepeat = new Repeat
            {
                StartDate = date,
                Frequency = Frequency.Yearly,
                EndAfterTimes = 10,
                RepeatEveryTimes = 2,
                CreatedDate = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            };
        }
    }
}
using System;

namespace FoundOps.Core.Models.CoreEntities.DesignData
{
    public class RepeatDesignData
    {
        private readonly Random _random = new Random((int)DateTime.UtcNow.Ticks);

        public Repeat DesignOnceRepeat { get; private set; }
        public Repeat DesignDailyRepeat { get; private set; }
        public Repeat DesignWeeklyRepeat { get; private set; }
        public Repeat DesignNeverEndingWeeklyRepeat { get; private set; }
        public Repeat DesignMonthlyRepeat { get; private set; }
        public Repeat DesignYearlyRepeat { get; private set; }

        public RepeatDesignData()
        {
            DesignOnceRepeat = new Repeat
            {
                StartDate = DateTime.UtcNow.Date,
                Frequency = Frequency.Once
            };
            DesignDailyRepeat = new Repeat
            {
                StartDate = DateTime.UtcNow.AddDays(-5),
                Frequency = Frequency.Daily,
                EndDate = DateTime.UtcNow.Add(new TimeSpan(10, 0, 0, 0)),
                RepeatEveryTimes = 2
            };
            DesignWeeklyRepeat = new Repeat
            {
                StartDate = DateTime.UtcNow.Date.AddDays(-14),
                Frequency = Frequency.Weekly,
                FrequencyDetailAsWeeklyFrequencyDetail =
                    new[] { DateTime.UtcNow.DayOfWeek},
                RepeatEveryTimes = 2
            };

            DesignNeverEndingWeeklyRepeat = new Repeat
            {
                StartDate = DateTime.UtcNow.Date.AddDays(-14),
                Frequency = Frequency.Weekly,
                FrequencyDetailAsWeeklyFrequencyDetail = new[] { DateTime.UtcNow.DayOfWeek },
                RepeatEveryTimes = 2
            };
            DesignMonthlyRepeat = new Repeat
                                      {
                                          StartDate = DateTime.UtcNow.AddDays(_random.Next(0, 7)),
                                          Frequency = Frequency.Monthly,
                                          EndDate = _random.Next(0, 2) == 0 ? DateTime.UtcNow.AddMonths(6) : new DateTime?(),
                                          RepeatEveryTimes = 2
                                      };
            DesignYearlyRepeat = new Repeat
            {
                StartDate = DateTime.UtcNow,
                Frequency = Frequency.Yearly,
                EndAfterTimes = 10,
                RepeatEveryTimes = 2
            };

        }

        public Repeat RandomDesignRepeat
        {
            get
            {
                switch (_random.Next(0, 5))
                {
                    case 0:
                        return DesignOnceRepeat;
                    case 1:
                        return DesignDailyRepeat;
                    case 2:
                        return DesignWeeklyRepeat;
                    case 3:
                        return DesignMonthlyRepeat;
                    default:
                        return DesignYearlyRepeat;
                }
            }
        }
    }
}
using System;
using FoundOps.Core.Models.CoreEntities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FoundOps.Server.Tests
{
    /// <summary>
    ///This is a test class for RepeatTest and is intended
    ///to contain all RepeatTest Unit Tests
    ///</summary>
    [TestClass()]
    public class RepeatTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        /// <summary>
        ///A test for NextRepeatDateOnOrAfterDate
        ///</summary>
        /// 
        [TestMethod()]
        public void NextRepeatDateOnOrAfterDateOnceTest()
        {
            var target = new Repeat
            {
                Frequency = Frequency.Once,
                StartDate = new DateTime(2011, 5, 3)
            };

            DateTime? expected = new DateTime(2011, 5, 3);
            var onOrAfterDate = new DateTime(2011, 5, 3);

            var actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual); //tests the functionality of requesting to see the service on the only day it exists

            onOrAfterDate = new DateTime(2011, 5, 4);
            expected = null;

            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual); //tests the functionality of requesting to see the service after it has been done

            onOrAfterDate = new DateTime(2011, 5, 1);
            expected = new DateTime(2011, 5, 3);

            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual); //tests the functionality of requesting to see the service before it has been done
        }

        [TestMethod()]
        public void NextRepeatDateOnOrAfterDateDailyTest()
        {
            var target = new Repeat
            {
                Frequency = Frequency.Daily,
                RepeatEveryTimes = 3,
                StartDate = new DateTime(2011, 5, 3),
                EndAfterTimes = 10
            };

            var expected = new DateTime(2011, 5, 3);
            var onOrAfterDate = new DateTime(2011, 5, 2);
            var actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual); //tests the functionality of being before the start date

            expected = new DateTime(2011, 5, 6);
            onOrAfterDate = new DateTime(2011, 5, 5);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual); //tests the functionality of being after a service day

            expected = new DateTime(2011, 5, 9);
            onOrAfterDate = new DateTime(2011, 5, 7);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual); //tests the functionality of being before a service day

            expected = new DateTime(2011, 5, 12);
            onOrAfterDate = new DateTime(2011, 5, 12);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual); //tests the functionality of being on a service day

            expected = new DateTime(2011, 5, 30);
            onOrAfterDate = new DateTime(2011, 5, 30);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual); //tests the functionality of being on the End date

            onOrAfterDate = new DateTime(2011, 5, 31);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(null, actual); //tests the functionality of being after the end date
        }

        [TestMethod()]
        public void NextRepeatDateOnOrAfterDateWeeklyTest()
        {
            var target = new Repeat
            {
                Frequency = Frequency.Weekly,
                RepeatEveryTimes = 2,
                StartDate = new DateTime(2011, 5, 1),
                FrequencyDetailAsWeeklyFrequencyDetail = new[] { DayOfWeek.Sunday, DayOfWeek.Thursday },
                EndAfterTimes = 10
            };
            #region tests Part 1
            // tests having a service on sunday and thursday every other week
            var expected = new DateTime(2011, 5, 1);
            var onOrAfterDate = new DateTime(2011, 5, 1);
            var actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            expected = new DateTime(2011, 5, 5);
            onOrAfterDate = new DateTime(2011, 5, 2);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            expected = new DateTime(2011, 5, 5);
            onOrAfterDate = new DateTime(2011, 5, 3);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            expected = new DateTime(2011, 5, 5);
            onOrAfterDate = new DateTime(2011, 5, 4);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            expected = new DateTime(2011, 5, 5);
            onOrAfterDate = new DateTime(2011, 5, 5);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            expected = new DateTime(2011, 5, 15);
            onOrAfterDate = new DateTime(2011, 5, 6);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            expected = new DateTime(2011, 5, 15);
            onOrAfterDate = new DateTime(2011, 5, 7);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            expected = new DateTime(2011, 5, 15);
            onOrAfterDate = new DateTime(2011, 5, 8);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            expected = new DateTime(2011, 5, 15);
            onOrAfterDate = new DateTime(2011, 5, 9);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            expected = new DateTime(2011, 5, 15);
            onOrAfterDate = new DateTime(2011, 5, 10);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            expected = new DateTime(2011, 5, 15);
            onOrAfterDate = new DateTime(2011, 5, 11);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            expected = new DateTime(2011, 5, 15);
            onOrAfterDate = new DateTime(2011, 5, 12);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            expected = new DateTime(2011, 5, 15);
            onOrAfterDate = new DateTime(2011, 5, 13);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            expected = new DateTime(2011, 5, 15);
            onOrAfterDate = new DateTime(2011, 5, 14);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            expected = new DateTime(2011, 5, 15);
            onOrAfterDate = new DateTime(2011, 5, 15);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            target.EndAfterTimes = 10;
            onOrAfterDate = new DateTime(2011, 7, 1);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(null, actual);

            #endregion

            target.RepeatEveryTimes = 1;

            #region Tests Part 2
            // changed to have service every week, but still on sunday and thursday
            expected = new DateTime(2011, 5, 1);
            onOrAfterDate = new DateTime(2011, 5, 1);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            expected = new DateTime(2011, 5, 5);
            onOrAfterDate = new DateTime(2011, 5, 2);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            expected = new DateTime(2011, 5, 5);
            onOrAfterDate = new DateTime(2011, 5, 3);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            expected = new DateTime(2011, 5, 5);
            onOrAfterDate = new DateTime(2011, 5, 4);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            expected = new DateTime(2011, 5, 5);
            onOrAfterDate = new DateTime(2011, 5, 5);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            expected = new DateTime(2011, 5, 8);
            onOrAfterDate = new DateTime(2011, 5, 6);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            expected = new DateTime(2011, 5, 8);
            onOrAfterDate = new DateTime(2011, 5, 7);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            expected = new DateTime(2011, 5, 8);
            onOrAfterDate = new DateTime(2011, 5, 8);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            expected = new DateTime(2011, 5, 12);
            onOrAfterDate = new DateTime(2011, 5, 9);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            expected = new DateTime(2011, 5, 12);
            onOrAfterDate = new DateTime(2011, 5, 10);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            expected = new DateTime(2011, 5, 12);
            onOrAfterDate = new DateTime(2011, 5, 11);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            expected = new DateTime(2011, 5, 12);
            onOrAfterDate = new DateTime(2011, 5, 12);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            expected = new DateTime(2011, 5, 15);
            onOrAfterDate = new DateTime(2011, 5, 13);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            expected = new DateTime(2011, 5, 15);
            onOrAfterDate = new DateTime(2011, 5, 14);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            expected = new DateTime(2011, 5, 15);
            onOrAfterDate = new DateTime(2011, 5, 15);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            target.EndAfterTimes = 10;
            onOrAfterDate = new DateTime(2011, 6, 3);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(null, actual);

            #endregion

            target.FrequencyDetailAsWeeklyFrequencyDetail = new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Friday };
            target.StartDate = new DateTime(2011, 6, 3);
            target.EndAfterTimes = 100;

            #region Test Part 3

            expected = new DateTime(2011, 6, 6);
            onOrAfterDate = new DateTime(2011, 6, 4);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);


            onOrAfterDate = new DateTime(2011, 6, 7);
            expected = new DateTime(2011, 6, 7);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            #endregion

            target.FrequencyDetailAsWeeklyFrequencyDetail = new[] { DayOfWeek.Tuesday, DayOfWeek.Friday };

            onOrAfterDate = new DateTime(2011, 6, 4);
            expected = new DateTime(2011, 6, 7);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            target.StartDate = new DateTime(2011, 5, 1);

            #region Test Part 4

            // This part tests every other week when there is a service every day of the week
            target.FrequencyDetailAsWeeklyFrequencyDetail = new[] { DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday };
            target.RepeatEveryTimes = 2;

            expected = new DateTime(2011, 5, 1);
            onOrAfterDate = new DateTime(2011, 5, 1);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            expected = new DateTime(2011, 5, 2);
            onOrAfterDate = new DateTime(2011, 5, 2);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            expected = new DateTime(2011, 5, 3);
            onOrAfterDate = new DateTime(2011, 5, 3);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            expected = new DateTime(2011, 5, 4);
            onOrAfterDate = new DateTime(2011, 5, 4);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            expected = new DateTime(2011, 5, 5);
            onOrAfterDate = new DateTime(2011, 5, 5);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            expected = new DateTime(2011, 5, 6);
            onOrAfterDate = new DateTime(2011, 5, 6);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            expected = new DateTime(2011, 5, 7);
            onOrAfterDate = new DateTime(2011, 5, 7);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            expected = new DateTime(2011, 5, 15);
            onOrAfterDate = new DateTime(2011, 5, 8);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            expected = new DateTime(2011, 5, 15);
            onOrAfterDate = new DateTime(2011, 5, 9);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            expected = new DateTime(2011, 5, 15);
            onOrAfterDate = new DateTime(2011, 5, 10);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            expected = new DateTime(2011, 5, 15);
            onOrAfterDate = new DateTime(2011, 5, 11);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            expected = new DateTime(2011, 5, 15);
            onOrAfterDate = new DateTime(2011, 5, 12);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            expected = new DateTime(2011, 5, 15);
            onOrAfterDate = new DateTime(2011, 5, 13);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            expected = new DateTime(2011, 5, 15);
            onOrAfterDate = new DateTime(2011, 5, 14);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            expected = new DateTime(2011, 5, 15);
            onOrAfterDate = new DateTime(2011, 5, 15);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            #endregion

            target.EndAfterTimes = 10;
            onOrAfterDate = new DateTime(2011, 5, 18);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(null, actual);

            target.EndAfterTimes = null;
            target.RepeatEveryTimes = 1;
            target.StartDate = new DateTime(2011, 6, 23);

            onOrAfterDate = new DateTime(2011, 6, 24);
            target.FrequencyDetailAsWeeklyFrequencyDetail = new[] { DayOfWeek.Thursday };
            expected = new DateTime(2011, 6, 30);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void NextRepeatDateOnOrAfterDateMonthlyTest()
        {
            var target = new Repeat
            {
                Frequency = Frequency.Monthly,
                RepeatEveryTimes = 2,
                StartDate = new DateTime(2011, 5, 3)
            };

            #region Part 1
            var expected = new DateTime(2011, 7, 3);
            var onOrAfterDate = new DateTime(2011, 5, 4);
            var actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            target.FrequencyDetailAsMonthlyFrequencyDetail = MonthlyFrequencyDetail.OnDayInMonth;
            Assert.AreEqual(expected, actual);

            target.FrequencyDetailAsMonthlyFrequencyDetail = MonthlyFrequencyDetail.FirstOfDayOfWeekInMonth;
            expected = new DateTime(2011, 7, 5);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            target.StartDate = new DateTime(2011, 5, 10);
            onOrAfterDate = new DateTime(2011, 5, 11);

            target.FrequencyDetailAsMonthlyFrequencyDetail = MonthlyFrequencyDetail.SecondOfDayOfWeekInMonth;
            expected = new DateTime(2011, 7, 12);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            target.StartDate = new DateTime(2011, 5, 17);
            onOrAfterDate = new DateTime(2011, 5, 18);

            target.FrequencyDetailAsMonthlyFrequencyDetail = MonthlyFrequencyDetail.ThirdOfDayOfWeekInMonth;
            expected = new DateTime(2011, 7, 19);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            target.StartDate = new DateTime(2011, 5, 31);
            onOrAfterDate = new DateTime(2011, 6, 1);

            target.FrequencyDetailAsMonthlyFrequencyDetail = MonthlyFrequencyDetail.LastOfDayOfWeekInMonth;
            expected = new DateTime(2011, 7, 26);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            target.StartDate = new DateTime(2011, 5, 3);

            expected = new DateTime(2011, 7, 3);
            onOrAfterDate = new DateTime(2011, 5, 5);

            target.FrequencyDetailAsMonthlyFrequencyDetail = MonthlyFrequencyDetail.OnDayInMonth;
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            target.FrequencyDetailAsMonthlyFrequencyDetail = MonthlyFrequencyDetail.FirstOfDayOfWeekInMonth;
            expected = new DateTime(2011, 7, 5);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            target.StartDate = new DateTime(2011, 5, 10);
            onOrAfterDate = new DateTime(2011, 5, 12);
            target.FrequencyDetailAsMonthlyFrequencyDetail = MonthlyFrequencyDetail.SecondOfDayOfWeekInMonth;
            expected = new DateTime(2011, 7, 12);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            target.StartDate = new DateTime(2011, 5, 17);
            onOrAfterDate = new DateTime(2011, 5, 19);
            target.FrequencyDetailAsMonthlyFrequencyDetail = MonthlyFrequencyDetail.ThirdOfDayOfWeekInMonth;
            expected = new DateTime(2011, 7, 19);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            target.StartDate = new DateTime(2011, 5, 31);
            onOrAfterDate = new DateTime(2011, 6, 2);
            target.FrequencyDetailAsMonthlyFrequencyDetail = MonthlyFrequencyDetail.LastOfDayOfWeekInMonth;
            expected = new DateTime(2011, 7, 26);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            target.StartDate = new DateTime(2011, 5, 3);
            expected = new DateTime(2011, 7, 3);
            onOrAfterDate = new DateTime(2011, 5, 6);
            target.FrequencyDetailAsMonthlyFrequencyDetail = MonthlyFrequencyDetail.OnDayInMonth;
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            target.FrequencyDetailAsMonthlyFrequencyDetail = MonthlyFrequencyDetail.FirstOfDayOfWeekInMonth;
            expected = new DateTime(2011, 7, 5);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            target.StartDate = new DateTime(2011, 5, 10);
            onOrAfterDate = new DateTime(2011, 5, 13);
            target.FrequencyDetailAsMonthlyFrequencyDetail = MonthlyFrequencyDetail.SecondOfDayOfWeekInMonth;
            expected = new DateTime(2011, 7, 12);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            target.StartDate = new DateTime(2011, 5, 17);
            onOrAfterDate = new DateTime(2011, 5, 20);
            target.FrequencyDetailAsMonthlyFrequencyDetail = MonthlyFrequencyDetail.ThirdOfDayOfWeekInMonth;
            expected = new DateTime(2011, 7, 19);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            target.StartDate = new DateTime(2011, 5, 31);
            onOrAfterDate = new DateTime(2011, 6, 3);
            target.FrequencyDetailAsMonthlyFrequencyDetail = MonthlyFrequencyDetail.LastOfDayOfWeekInMonth;
            expected = new DateTime(2011, 7, 26);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            target.StartDate = new DateTime(2011, 5, 3);
            expected = new DateTime(2011, 7, 3);
            onOrAfterDate = new DateTime(2011, 5, 7);
            target.FrequencyDetailAsMonthlyFrequencyDetail = MonthlyFrequencyDetail.OnDayInMonth;
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            target.FrequencyDetailAsMonthlyFrequencyDetail = MonthlyFrequencyDetail.FirstOfDayOfWeekInMonth;
            expected = new DateTime(2011, 7, 5);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            target.StartDate = new DateTime(2011, 5, 10);
            onOrAfterDate = new DateTime(2011, 5, 14);
            target.FrequencyDetailAsMonthlyFrequencyDetail = MonthlyFrequencyDetail.SecondOfDayOfWeekInMonth;
            expected = new DateTime(2011, 7, 12);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            target.StartDate = new DateTime(2011, 5, 17);
            onOrAfterDate = new DateTime(2011, 5, 21);
            target.FrequencyDetailAsMonthlyFrequencyDetail = MonthlyFrequencyDetail.ThirdOfDayOfWeekInMonth;
            expected = new DateTime(2011, 7, 19);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            target.StartDate = new DateTime(2011, 5, 31);
            onOrAfterDate = new DateTime(2011, 6, 4);
            target.FrequencyDetailAsMonthlyFrequencyDetail = MonthlyFrequencyDetail.LastOfDayOfWeekInMonth;
            expected = new DateTime(2011, 7, 26);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            target.StartDate = new DateTime(2011, 5, 3);
            expected = new DateTime(2011, 7, 3);
            onOrAfterDate = new DateTime(2011, 5, 8);
            target.FrequencyDetailAsMonthlyFrequencyDetail = MonthlyFrequencyDetail.OnDayInMonth;
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            target.FrequencyDetailAsMonthlyFrequencyDetail = MonthlyFrequencyDetail.FirstOfDayOfWeekInMonth;
            expected = new DateTime(2011, 7, 5);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            target.StartDate = new DateTime(2011, 5, 10);
            onOrAfterDate = new DateTime(2011, 5, 15);
            target.FrequencyDetailAsMonthlyFrequencyDetail = MonthlyFrequencyDetail.SecondOfDayOfWeekInMonth;
            expected = new DateTime(2011, 7, 12);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            target.StartDate = new DateTime(2011, 5, 17);
            onOrAfterDate = new DateTime(2011, 5, 22);
            target.FrequencyDetailAsMonthlyFrequencyDetail = MonthlyFrequencyDetail.ThirdOfDayOfWeekInMonth;
            expected = new DateTime(2011, 7, 19);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            target.StartDate = new DateTime(2011, 5, 31);
            onOrAfterDate = new DateTime(2011, 6, 5);
            target.FrequencyDetailAsMonthlyFrequencyDetail = MonthlyFrequencyDetail.LastOfDayOfWeekInMonth;
            expected = new DateTime(2011, 7, 26);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            target.StartDate = new DateTime(2011, 5, 3);
            expected = new DateTime(2011, 7, 3);
            onOrAfterDate = new DateTime(2011, 5, 9);
            target.FrequencyDetailAsMonthlyFrequencyDetail = MonthlyFrequencyDetail.OnDayInMonth;
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            target.FrequencyDetailAsMonthlyFrequencyDetail = MonthlyFrequencyDetail.FirstOfDayOfWeekInMonth;
            expected = new DateTime(2011, 7, 5);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            target.StartDate = new DateTime(2011, 5, 10);
            onOrAfterDate = new DateTime(2011, 5, 16);
            target.FrequencyDetailAsMonthlyFrequencyDetail = MonthlyFrequencyDetail.SecondOfDayOfWeekInMonth;
            expected = new DateTime(2011, 7, 12);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            target.StartDate = new DateTime(2011, 5, 17);
            onOrAfterDate = new DateTime(2011, 5, 23);
            target.FrequencyDetailAsMonthlyFrequencyDetail = MonthlyFrequencyDetail.ThirdOfDayOfWeekInMonth;
            expected = new DateTime(2011, 7, 19);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            target.StartDate = new DateTime(2011, 5, 31);
            onOrAfterDate = new DateTime(2011, 6, 6);
            target.FrequencyDetailAsMonthlyFrequencyDetail = MonthlyFrequencyDetail.LastOfDayOfWeekInMonth;
            expected = new DateTime(2011, 7, 26);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            target.StartDate = new DateTime(2011, 5, 3);
            expected = new DateTime(2011, 7, 3);
            onOrAfterDate = new DateTime(2011, 5, 10);
            target.FrequencyDetailAsMonthlyFrequencyDetail = MonthlyFrequencyDetail.OnDayInMonth;
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            target.FrequencyDetailAsMonthlyFrequencyDetail = MonthlyFrequencyDetail.FirstOfDayOfWeekInMonth;
            expected = new DateTime(2011, 7, 5);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            target.StartDate = new DateTime(2011, 5, 10);
            onOrAfterDate = new DateTime(2011, 5, 17);
            target.FrequencyDetailAsMonthlyFrequencyDetail = MonthlyFrequencyDetail.SecondOfDayOfWeekInMonth;
            expected = new DateTime(2011, 7, 12);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            target.StartDate = new DateTime(2011, 5, 17);
            onOrAfterDate = new DateTime(2011, 5, 24);
            target.FrequencyDetailAsMonthlyFrequencyDetail = MonthlyFrequencyDetail.ThirdOfDayOfWeekInMonth;
            expected = new DateTime(2011, 7, 19);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            target.StartDate = new DateTime(2011, 5, 31);
            onOrAfterDate = new DateTime(2011, 6, 7);
            target.FrequencyDetailAsMonthlyFrequencyDetail = MonthlyFrequencyDetail.LastOfDayOfWeekInMonth;
            expected = new DateTime(2011, 7, 26);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);
            #endregion

            target.StartDate = new DateTime(2011, 8, 13);
            onOrAfterDate = new DateTime(2011, 8, 13);
            target.FrequencyDetailAsMonthlyFrequencyDetail = MonthlyFrequencyDetail.SecondOfDayOfWeekInMonth;
            expected = new DateTime(2011, 8, 13);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

            target.StartDate = new DateTime(2011, 7, 31);
            target.EndDate = new DateTime(2012, 1, 25);
            onOrAfterDate = new DateTime(2011, 8, 1);
            target.FrequencyDetailAsMonthlyFrequencyDetail = MonthlyFrequencyDetail.OnDayInMonth;
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(null, actual);

            target.StartDate = new DateTime(2011, 10, 29);
            onOrAfterDate = new DateTime(2012, 12, 30);
            target.EndDate = null;
            target.RepeatEveryTimes = 2;
            target.FrequencyDetailAsMonthlyFrequencyDetail = MonthlyFrequencyDetail.OnDayInMonth;
            expected = new DateTime(2013, 4, 29);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual);

        }

        [TestMethod()]
        public void NextRepeatDateOnOrAfterDateYearlyTest()
        {
            var target = new Repeat
            {
                Frequency = Frequency.Yearly,
                RepeatEveryTimes = 2,
                StartDate = new DateTime(2011, 5, 3),
                EndDate = new DateTime(2020, 10, 20),
                EndAfterTimes = null
            };

            var expected = new DateTime(2013, 5, 3);
            var onOrAfterDate = new DateTime(2011, 5, 4);
            var actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual); //tests the functionality of being in a week with a service day left in it

            expected = new DateTime(2011, 5, 3);
            onOrAfterDate = new DateTime(2011, 5, 3);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual); //tests the functionality of having the start date the same as the onOrAfterDate

            expected = new DateTime(2013, 5, 3);
            onOrAfterDate = new DateTime(2012, 5, 31);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual); //tests the functionality of having the start date the same as the onOrAfterDate

            expected = new DateTime(2011, 5, 3);
            onOrAfterDate = new DateTime(2011, 5, 1);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(expected, actual); //tests the functionality of having the start date before the onOrAfterDate

            target.EndDate = new DateTime(2012, 10, 2);
            onOrAfterDate = new DateTime(2011, 5, 4);
            actual = target.NextRepeatDateOnOrAfterDate(onOrAfterDate);
            Assert.AreEqual(null, actual); //tests the functionality of having the end date before the NextRepeatDate

        }
    }
}
using System;
using System.Globalization;
using System.Windows.Data;
using FoundOps.SLClient.Data.Services;

namespace FoundOps.SLClient.UI.Converters
{
    /// <summary>
    /// This method takes a datetime and sets the date to today's date
    /// </summary>
    public class DateTimeWithTodaysDateConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        //Called when binding from an object property to a control property
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var date = Manager.Context.UserAccount.AdjustTimeForUserTimeZone(DateTime.UtcNow);

                var dateTime = (DateTime)value;
                var currentDateTime = new DateTime(date.Year, date.Month, date.Day,
                                                   dateTime.Hour, dateTime.Minute, dateTime.Second);
                return currentDateTime;
            }
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var dateTime = (DateTime)value;
                return dateTime;
            }
            return true;
        }

        #endregion
    }

    public class CheckIfDateIsBeforeTodayConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dateTime = (DateTime)value;

            var date = Manager.Context.UserAccount.AdjustTimeForUserTimeZone(DateTime.UtcNow);

            return dateTime.ToUniversalTime().Date >= date.Date;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

}

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using FoundOps.Common.Composite;

namespace FoundOps.Common.Silverlight.Converters
{
    public class RelativeDateTimeConverter:IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime dateTime = (DateTime) value;

            if(dateTime.Date==DateTime.Now.Date)
            {
                return dateTime.ToShortTimeString();
            }

            return String.Format("{0} {1}",dateTime.ToShortTimeString(), dateTime.ToLongDateString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class DateTimeOfTodayToTimeSpanConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dateTime = (DateTime)value;
            return dateTime.TimeOfDay;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var timeSpan = (TimeSpan)value;
            return DateTime.Today.SetTime(timeSpan.Hours, timeSpan.Minutes);
        }

        #endregion
    }

    public class DateTimeToLongDateConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        //Called when binding from an object property to a control property
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || (DateTime)value == DateTime.MinValue) return "";
            var dt = (DateTime)value;
            return dt.ToLongDateString();
        }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class DateTimeFormatConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (parameter != null)
            {
                string formatterString = parameter.ToString();

                if (!String.IsNullOrEmpty(formatterString))
                {
                    return String.Format(culture, String.Format("{{0:{0}}}", formatterString), value);
                }
            }

            return (value ?? "").ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }


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
                var dateTime = (DateTime) value;
                var currentDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
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

            if (dateTime.Date < DateTime.Now.Date)
                return false;

            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

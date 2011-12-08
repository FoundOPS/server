using System;
using System.Globalization;
using System.Windows.Data;
using FoundOps.Core.Models.CoreEntities;
using System.Linq;

namespace FoundOps.Framework.Views.Converters
{
    /// <summary>
    /// 
    /// </summary>
    public class RepeatEveryTimesUnitTypeConverter:IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Count() != 2 || values[0] == null || values[1] == null) return "";

            var frequency = (Frequency)values[0];
            var repeatEveryTimesIsSingular = ((int)values[1])==1;

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

    /// <summary>
    /// 
    /// </summary>
    public class EndOfRepeatToIsCheckedConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null || values.Count() != 2)
                return false;

            var endAfterTimes = (int?)values[0];
            var endDate = (DateTime?)values[1];

            var currentControl = (string)parameter;
            if (currentControl == "NeverEnd")
            {
                if (endAfterTimes == null && endDate == null) return true;
            }
            else if (currentControl == "EndAfterTimes")
            {
                if (endAfterTimes != null) return true;
            }
            else if (currentControl == "EndDate")
            {
                if (endDate != null) return true;
            }

            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class EndAfterTimesToOccurrencesStringConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "";
            return ((int) value) > 1 ? "Occurrences" : "Occurrence";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Convert timespan(in minutes) to an int and vise versa
    /// </summary>
    public class EstimatedLengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return 0;
            var minutes = ((TimeSpan)value).Minutes;
            return System.Convert.ToInt32(minutes);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return new TimeSpan();
            return new TimeSpan(0, System.Convert.ToInt32(value), 0);
        }
    }
}

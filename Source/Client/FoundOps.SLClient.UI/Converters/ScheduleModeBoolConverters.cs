using System;
using System.Windows.Data;
using FoundOps.Core.Models.CoreEntities;

namespace FoundOps.SLClient.UI.Converters
{
    ///<summary>
    /// 
    /// The converter parameter should be set to 0 for FixedDate and 1 for Relative
    ///</summary>
    public class ScheduleModeConverter : IValueConverter
    {
        #region Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var mode = (ScheduleMode)value;

            if ((string)parameter == "FixedDate")
                return mode == ScheduleMode.FixedDate;

            return mode == ScheduleMode.Relative;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var selected = (bool)value;

            if (selected)
            {
                var returnValue = ((string)parameter == "FixedDate") ? ScheduleMode.FixedDate : ScheduleMode.Relative;
                return returnValue;
            }

            var reurnValue = ((string)parameter == "FixedDate") ? ScheduleMode.Relative : ScheduleMode.FixedDate;
            return reurnValue;
        }

        #endregion
    }
}

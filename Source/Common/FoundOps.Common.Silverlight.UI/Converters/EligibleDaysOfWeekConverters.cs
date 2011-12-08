using System;
using System.Collections.ObjectModel;
using System.Windows.Data;
using FoundOps.Common.Silverlight.Tools;
using FoundOps.Common.Tools;

namespace FoundOps.Common.Silverlight.Converters
{
    public class EligibleDaysOfWeekXmlConverter: IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var xml = (string)value;
            if (xml == null)
                return new ObservableCollection<DayOfWeek>();
            var returnValue= SerializationTools.Deserialize<ObservableCollection<DayOfWeek>>(xml);
            return returnValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return SerializationTools.Serialize(value);
        }

        #endregion
    }
}

using System;
using System.Windows.Data;
using System.Windows.Media;
using FoundOps.Common.Silverlight.Extensions;

namespace FoundOps.Common.Silverlight.Converters
{
    public class StatusColorConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string)
            {
                string status = value as string;
                if (!String.IsNullOrEmpty(status))
                    return new SolidColorBrush(Colors.Green);
            }
            else if (value is bool)
            {
                bool status = (bool)value;
                if (status)
                    return new SolidColorBrush(Colors.Green);
            }
            return new SolidColorBrush(Colors.Red);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class NumberColorConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int remainder = ((int) value)%8;
            switch(remainder)
            {
                case 0:
                    return new SolidColorBrush(Colors.White);
                case 1:
                    return new SolidColorBrush("FF0098FF".GetColorFromHexString());
                case 2:
                    return new SolidColorBrush(Colors.Orange);
                case 3:
                    return new SolidColorBrush("FF00B700".GetColorFromHexString());
                case 4:
                    return new SolidColorBrush(Colors.Magenta);
                case 5:
                    return new SolidColorBrush(Colors.Yellow);
                case 6:
                    return new SolidColorBrush("FFD37300".GetColorFromHexString());
                default:
                    return new SolidColorBrush(Colors.Cyan);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class StringToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return new SolidColorBrush();

            return new SolidColorBrush(((string) value).GetColorFromHexString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SelectedStatusColorConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            
            if (value is bool)
            {
                bool status = (bool)value;

                if (status)
                    return parameter ?? new SolidColorBrush(Colors.Blue);
            }
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

}

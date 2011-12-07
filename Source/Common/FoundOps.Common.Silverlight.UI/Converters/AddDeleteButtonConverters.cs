using System;
using System.Windows.Data;
using System.Globalization;
using FoundOps.Common.Silverlight.UI.Controls.AddEditDelete;

namespace FoundOps.Common.Silverlight.Converters
{
    public class AddButtonTooltipConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var mode = (AddDeleteMode)value;
            switch (mode)
            {
                case AddDeleteMode.AddItemDelete:
                    return "Add Existing";
                case AddDeleteMode.AddNewExistingDelete:
                    return "Add New or Existing";
                case AddDeleteMode.AddNewItemDeleteCustomTemplate:
                    return "Add New";
                case AddDeleteMode.AddCustomTemplate:
                    return "Add New or Existing";
                case AddDeleteMode.AddDeleteCustomTemplate:
                    return "Add New or Existing";
                default:
                    return "Add New";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class AddDeleteModeConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((int)value <= 1) ? "AddCustomTemplate" : "AddDeleteCustomTemplate";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
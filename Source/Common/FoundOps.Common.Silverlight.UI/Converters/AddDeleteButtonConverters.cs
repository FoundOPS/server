using System;
using System.Windows.Data;
using FoundOps.Common.Silverlight.Controls;
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
}
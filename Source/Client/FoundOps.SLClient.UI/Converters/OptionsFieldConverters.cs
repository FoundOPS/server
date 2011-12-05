using System;
using System.Globalization;
using System.Windows.Data;
using System.Linq;
using FoundOps.Core.Models.CoreEntities;

namespace FoundOps.Framework.Views.Converters
{
    public class ComboBoxSelectedItemConverter : IValueConverter
    {
        private OptionsField _optionsField;

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value as OptionsField == null) return null;

            _optionsField = (OptionsField)value;
            return _optionsField.Options.FirstOrDefault(o => o.IsChecked);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value as Option == null || _optionsField == null) return null;

            var currentOption = (Option)value;

            foreach (var otherOption in _optionsField.Options.Where(o => o != currentOption))
                otherOption.IsChecked = false;

            currentOption.IsChecked = true;

            return _optionsField;
        }

        #endregion
    }
}

using System;
using System.Linq;
using System.Globalization;
using System.Windows.Data;
using FoundOps.Core.Models.CoreEntities;

namespace FoundOps.Core.Context.Converters
{
    public class FieldDefaultValueStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var field = value as Field;

            if (field == null) return "";

            if (field is OptionsField)
            {
                var optionsField = (OptionsField)field;
                if (optionsField.OptionsType == OptionsType.Checkbox && optionsField.Options.FirstOrDefault() != null)
                    return optionsField.Options.First().IsChecked ? "Checked" : "Not Checked";
                if (optionsField.OptionsType == OptionsType.Checklist)
                    return "";
                if (optionsField.OptionsType == OptionsType.Combobox)
                    return "";
            }
            if (field is NumericField)
            {
                return ((NumericField)field).Value;
            }
            if (field is TextBoxField)
            {
                return ((TextBoxField)field).Value;
            }

            if (field is DateTimeField)
            {
                var dateTimeField = (DateTimeField)field;

                if (dateTimeField.DateTimeType == DateTimeType.TimeOnly)
                    return dateTimeField.Value.HasValue ? dateTimeField.Value.Value.ToLongTimeString() : "";
            }

            return "Not setup yet";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

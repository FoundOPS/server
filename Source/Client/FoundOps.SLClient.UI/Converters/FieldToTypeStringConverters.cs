using System;
using System.Windows.Data;
using FoundOps.Core.Models.CoreEntities;

namespace FoundOps.Framework.Views.Converters
{
    public class FieldToTypeStringConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object item, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var field = (Field)item;

            if (field is OptionsField)
            {
                var optionsField = (OptionsField)field;
                if (optionsField.OptionsType == OptionsType.Checkbox)
                    return "Checkbox";
                if (optionsField.OptionsType == OptionsType.Checklist)
                    return "Checklist";
                if (optionsField.OptionsType == OptionsType.Combobox)
                    return "Combobox";
            }
            if (field is NumericField)
            {
                var numericField = (NumericField)field;
                if (numericField.Mask == "c")
                    return "Currency";
                if (numericField.Mask == "g")
                    return "Number";
                if (numericField.Mask == "p")
                    return "Percentage";
            }
            if (field is TextBoxField)
            {
                var textBoxField = (TextBoxField)field;
                if (textBoxField.IsMultiline)
                    return "Textbox Large";
                if (!textBoxField.IsMultiline)
                    return "Textbox Small";
            }

            if (field is DateTimeField)
            {
                var dateTimeField = (DateTimeField)field;

                if (dateTimeField.DateTimeType == DateTimeType.TimeOnly)
                    return "Time";
            }

            if (field is LocationField)
                return "Destination";

            throw new NotImplementedException("Fields not recognized");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

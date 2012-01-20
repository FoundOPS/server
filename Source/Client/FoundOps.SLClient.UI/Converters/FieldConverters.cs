using System;
using System.Linq;
using System.Windows.Data;
using System.Globalization;
using System.ComponentModel;
using FoundOps.Core.Models.CoreEntities;

namespace FoundOps.Framework.Views.Converters
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

    public class OrderByNameConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var viewSource = new CollectionViewSource { Source = value };
            viewSource.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            return viewSource.View;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

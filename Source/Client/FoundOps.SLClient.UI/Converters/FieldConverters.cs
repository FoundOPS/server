using System;
using System.Linq;
using System.Windows.Data;
using System.Globalization;
using System.ComponentModel;
using FoundOps.Core.Models.CoreEntities;

namespace FoundOps.Framework.Views.Converters
{
    public class FieldDefaultValueConverter : IValueConverter
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
                    return dateTimeField.Value.HasValue ? dateTimeField.Value.Value.ToShortTimeString() : "";
            }

            return "Not setup yet";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

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
                var numericField = (NumericField)field;
                if (numericField.Mask == "c")
                    if (numericField.Value != null) return "$" + ((double)numericField.Value).ToString("0.00", CultureInfo.InvariantCulture);
                if (numericField.Mask == "g")
                    if (numericField.Value != null) return ((double)numericField.Value).ToString(CultureInfo.InvariantCulture);
                if (numericField.Mask == "p")
                    if (numericField.Value != null) return ((double)numericField.Value * 100).ToString(CultureInfo.InvariantCulture) + "%";
            }
            if (field is TextBoxField)
            {
                return ((TextBoxField)field).Value;
            }
            if (field is DateTimeField)
            {
                var dateTimeField = (DateTimeField)field;

                if (dateTimeField.DateTimeType == DateTimeType.TimeOnly)
                    return dateTimeField.Value.HasValue ? dateTimeField.Value.Value.ToShortTimeString() : "";
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

            if (field is SignatureField)
            {
                return "Signature";
            }

            if (field is DateTimeField)
            {
                var dateTimeField = (DateTimeField)field;

                if (dateTimeField.DateTimeType == DateTimeType.TimeOnly)
                    return "Time";
            }

            if (field is LocationField)
                return "Destination";

            return "Field not recognized";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    /// <summary>
    /// Takes a field and returns the value if it exists, otherwise returns an empty space to fill in the value.
    /// </summary>
    public class ManifestFieldValueConverter : IValueConverter
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
                var numericField = (NumericField)field;
                if (numericField.Mask == "c")
                    return numericField.Value.HasValue ? "$" + ((double)numericField.Value).ToString("0.00") : "$_______.____";
                if (numericField.Mask == "g")
                    return numericField.Value.HasValue ? ((double)numericField.Value).ToString(CultureInfo.InvariantCulture) : "____________";
                if (numericField.Mask == "p")
                    return numericField.Value.HasValue ? ((double)numericField.Value * 100).ToString(CultureInfo.InvariantCulture) + "%" : "____________%";
            }
            if (field is TextBoxField)
            {
                var textBoxField = (TextBoxField)field;
                if (textBoxField.IsMultiline)
                    return textBoxField.Value != null ? ((TextBoxField)field).Value :
                        "______________________________\r\n______________________________\r\n______________________________";
                if (!textBoxField.IsMultiline)
                    return textBoxField.Value != null ? ((TextBoxField)field).Value : "_________________________";
            }
            if (field is DateTimeField)
            {
                var dateTimeField = (DateTimeField)field;

                if (dateTimeField.DateTimeType == DateTimeType.TimeOnly)
                    return dateTimeField.Value.HasValue ? dateTimeField.Value.Value.ToShortTimeString() : "____:____";
            }

            return "Not setup yet";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Takes a collection and orders the items by name in ascending order.
    /// </summary>
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

    /// <summary>
    /// Takes two values and combines them into one string. 
    /// Ex. "Name" + "Value" becomes "Name: Value"
    /// </summary>
    public class CombineNameValueConverter : IMultiValueConverter
    {
        /// <summary>
        /// Converts the specified values.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var name = values[0] as string;
            var field = values[1] as Field;
            var textBoxField = (TextBoxField)field;
            if (textBoxField != null && !(string.IsNullOrEmpty(textBoxField.Value)))
            {
                return name + ": " + textBoxField.Value;
            }

            if (textBoxField != null && textBoxField.IsMultiline)
                return name + ": ______________________________\r\n______________________________\r\n______________________________";
            return name + ": _________________________";
        }

        /// <summary>
        /// Converts the specified values.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetTypes">The target types.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

using FoundOps.Core.Models.CoreEntities;
using System.Windows;
using Telerik.Windows.Controls;

namespace FoundOps.SLClient.UI.Selectors
{
    public class FieldDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate CheckBoxTemplate { get; set; }
        public DataTemplate CheckListTemplate { get; set; }
        public DataTemplate ComboBoxTemplate { get; set; }
        public DataTemplate CurrencyTemplate { get; set; }
        public DataTemplate LocationTemplate { get; set; }
        public DataTemplate NumberTemplate { get; set; }
        public DataTemplate PercentageTemplate { get; set; }
        public DataTemplate TextLgTemplate { get; set; }
        public DataTemplate TextSmTemplate { get; set; }

        public DataTemplate SignatureTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var field = (Field)item;

            if (field is LocationField)
            {
                return LocationTemplate;
            }

            if (field is NumericField)
            {
                var numericField = (NumericField)field;
                if (numericField.Mask == "c")
                    return CurrencyTemplate;
                if (numericField.Mask == "g")
                    return NumberTemplate;
                if (numericField.Mask == "p")
                    return PercentageTemplate;
            }

            if (field is OptionsField)
            {
                var optionsField = (OptionsField)field;
                if (optionsField.OptionsType == OptionsType.Checkbox)
                    return CheckBoxTemplate;
                if (optionsField.OptionsType == OptionsType.Checklist)
                    return CheckListTemplate;
                if (optionsField.OptionsType == OptionsType.Combobox)
                    return ComboBoxTemplate;
            }

            if (field is TextBoxField)
            {
                var textBoxField = (TextBoxField)field;
                if (textBoxField.IsMultiline)
                    return TextLgTemplate;
                if (!textBoxField.IsMultiline)
                    return TextSmTemplate;
            }

            if (field is SignatureField)
            {
                return SignatureTemplate;
            }

            return new DataTemplate();
        }
    }
}

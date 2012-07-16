using System;
using System.Collections.Generic;
using FoundOps.Core.Models.CoreEntities;

namespace FoundOps.Core.Models.CoreEntities.DesignData
{
    public class FieldsDesignData
    {
        public TextBoxField DesignTextBoxSmallField { get; private set; }
        public TextBoxField DesignTextBoxLargeField { get; private set; }
        public DateTimeField DesignTimeField { get; private set; }
        public NumericField DesignNumberField { get; private set; }
        public NumericField DesignCurrencyField { get; private set; }
        public NumericField DesignPercentageField { get; private set; }
        public OptionsField DesignCheckBoxField { get; private set; }
        public OptionsField DesignComboBoxField { get; private set; }
        public OptionsField DesignCheckListField { get; private set; }
        public LocationField DesignLocationField { get; private set; }

        public IEnumerable<Field> DesignFields { get; private set; }

        public FieldsDesignData()
        {
            InitializeFields();
        }

        private void InitializeFields()
        {
            DesignTextBoxSmallField = new TextBoxField
            {
                Name = "Lock Info",
                Required = false,
                Tooltip = "Helpful Hint",
                IsMultiline = false
            };

            DesignTextBoxLargeField = new TextBoxField
            {
                Name = "Notes",
                Required = false,
                Tooltip = "Helpful Hint",
                IsMultiline = true
            };

            DesignTimeField = new DateTimeField
            {
                Name = "Start/End Times",
                Required = false,
                Tooltip = "Helpful Hint",
                Earliest = DateTime.UtcNow,
                Latest = DateTime.UtcNow.AddHours(2),
                DateTimeType = DateTimeType.TimeOnly
            };

            DesignNumberField = new NumericField
            {
                Name = "Number of Stops",
                Required = false,
                Tooltip = "Helpful Hint",
                Mask = "g",
                Maximum = 8,
                Minimum = 1,
                DecimalPlaces = 3
            };

            DesignCurrencyField = new NumericField
            {
                Name = "Price",
                Required = false,
                Tooltip = "Helpful Hint",
                Mask = "c"
            };

            DesignPercentageField = new NumericField
            {
                Name = "Percentage",
                Required = false,
                Tooltip = "Helpful Hint",
                Mask = "p"
            };

            DesignCheckBoxField = new OptionsField
            {
                Name = "CheckBox Options",
                Required = false,
                Tooltip = "Helpful Hint",
                AllowMultipleSelection = false
            };
            DesignCheckBoxField.Options.Add(new Option { Name = "Collected paperwork" });

            DesignComboBoxField = new OptionsField
            {
                Name = "ComboBox Options",
                Required = false,
                Tooltip = "Helpful Hint",
                AllowMultipleSelection = false
            };
            DesignComboBoxField.Options.Add(new Option { Name = "Yes" });
            DesignComboBoxField.Options.Add(new Option { Name = "No" });
            DesignComboBoxField.Options.Add(new Option { Name = "Maybe" });

            DesignCheckListField = new OptionsField
            {
                Name = "CheckList Options",
                Required = false,
                Tooltip = "Helpful Hint",
                AllowMultipleSelection = true
            };

            DesignCheckListField.Options.Add(new Option { Name = "Yes" });
            DesignCheckListField.Options.Add(new Option { Name = "No" });
            DesignCheckListField.Options.Add(new Option { Name = "Maybe" });

            DesignLocationField = new LocationField
            {
                Name = "Locations",
                Required = false,
                Tooltip = "Helpful Hint"
            };

            DesignFields = new List<Field> { DesignTextBoxSmallField, DesignTextBoxLargeField, DesignTimeField, DesignNumberField, DesignCurrencyField, DesignPercentageField, 
                DesignCheckBoxField, DesignComboBoxField, DesignCheckListField, DesignLocationField};
        }
    }
}

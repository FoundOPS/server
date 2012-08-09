using System;
using System.Collections.Generic;

namespace FoundOps.Core.Models.CoreEntities.DesignData
{
    public class FieldsDesignData
    {
        public IEnumerable<Field> DesignFields { get; private set; }

        public FieldsDesignData()
        {
            InitializeFields();
        }

        private void InitializeFields()
        {
            var designTextBoxSmallField = new TextBoxField
            {
                Id = Guid.NewGuid(),
                Name = "Lock Info",
                Required = false,
                Tooltip = "Helpful Hint",
                IsMultiline = false,
                Value = "Small"
            };

            var designTextBoxLargeField = new TextBoxField
            {
                Id = Guid.NewGuid(),
                Name = "Notes",
                Required = false,
                Tooltip = "Helpful Hint",
                IsMultiline = true,
                Value = "Large"
            };

            var nineAm = DateTime.Now.Date.AddHours(9);
            var sevenPm = DateTime.Now.Date.AddHours(19);

            var designTimeField = new DateTimeField
             {
                 Id = Guid.NewGuid(),
                 Name = "Start and End Times",
                 Required = false,
                 Tooltip = "Helpful Hint",
                 Earliest = nineAm,
                 Latest = sevenPm,
                 DateTimeType = DateTimeType.TimeOnly,
                 Value = DateTime.UtcNow
             };

            var designNumberField = new NumericField
             {
                 Id = Guid.NewGuid(),
                 Name = "Number of Hours",
                 Required = false,
                 Tooltip = "Helpful Hint",
                 Mask = "g",
                 Maximum = 8,
                 Minimum = 1,
                 DecimalPlaces = 3,
                 Value = 1
             };

            var designCurrencyField = new NumericField
             {
                 Id = Guid.NewGuid(),
                 Name = "Price",
                 Required = false,
                 Tooltip = "Helpful Hint",
                 Mask = "c",
                 Maximum = 1000,
                 Minimum = 0,
                 Value = (decimal?)2.54
             };

            var designPercentageField = new NumericField
              {
                  Id = Guid.NewGuid(),
                  Name = "Percentage",
                  Required = false,
                  Tooltip = "Helpful Hint",
                  Maximum = 100,
                  Minimum = 0,
                  Mask = "p",
                  Value = 25
              };

            var designCheckBoxField = new OptionsField
              {
                  Id = Guid.NewGuid(),
                  Name = "CheckBox Options",
                  Required = false,
                  Tooltip = "Helpful Hint",
                  AllowMultipleSelection = false,
                  OptionsType = OptionsType.Checkbox
              };
            designCheckBoxField.Options.Add(new Option { Name = "Collected paperwork", IsChecked = true });

            var designComboBoxField = new OptionsField
             {
                 Id = Guid.NewGuid(),
                 Name = "ComboBox Options",
                 Required = false,
                 Tooltip = "Helpful Hint",
                 AllowMultipleSelection = false,
                 OptionsType = OptionsType.Combobox
             };
            designComboBoxField.Options.Add(new Option { Name = "Yes", IsChecked = false, Index = 0 });
            designComboBoxField.Options.Add(new Option { Name = "No", IsChecked = false, Index = 1 });
            designComboBoxField.Options.Add(new Option { Name = "Maybe", IsChecked = true, Index = 2 });

            var designCheckListField = new OptionsField
              {
                  Id = Guid.NewGuid(),
                  Name = "CheckList Options",
                  Required = false,
                  Tooltip = "Helpful Hint",
                  AllowMultipleSelection = true,
                  OptionsType = OptionsType.Checklist
              };

            var designLocationField = new LocationField
            {
                Id = Guid.NewGuid(),
                Name = "Locations",
                Required = false,
                Tooltip = "Helpful Hint",
                LocationFieldType = LocationFieldType.Destination
            };

            designCheckListField.Options.Add(new Option { Name = "Op 1", IsChecked = true, Index = 0 });
            designCheckListField.Options.Add(new Option { Name = "Op 2", IsChecked = false, Index = 1 });
            designCheckListField.Options.Add(new Option { Name = "Op 3", IsChecked = true, Index = 2 });

            DesignFields = new List<Field> { designTextBoxSmallField, designTextBoxLargeField, designTimeField, designNumberField, designCurrencyField, designPercentageField, 
                designCheckBoxField, designComboBoxField, designCheckListField, designLocationField};
        }
    }
}

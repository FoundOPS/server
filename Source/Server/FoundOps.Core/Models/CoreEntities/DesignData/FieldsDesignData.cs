﻿using System;
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
                Tooltip = "The lock combination",
                IsMultiline = false,
                Value = "124-1515-15155"
            };

            var designTextBoxLargeField = new TextBoxField
            {
                Id = Guid.NewGuid(),
                Name = "Notes",
                Required = false,
                Tooltip = "Any miscellaneous service information",
                IsMultiline = true,
                Value = "Be careful, there is a large scary dog"
            };

            var designSignatureField = new SignatureField()
            {
                Id = Guid.NewGuid(),
                Name = "Notes",
                Required = false,
                Tooltip = "Any miscellaneous service information"
            };

            //var nineAm = DateTime.Now.Date.AddHours(9);
            //var sevenPm = DateTime.Now.Date.AddHours(19);

            //var designTimeField = new DateTimeField
            // {
            //     Id = Guid.NewGuid(),
            //     Name = "Arrival",
            //     Required = false,
            //     Tooltip = "What time did you get to the location?",
            //     Earliest = nineAm,
            //     Latest = sevenPm,
            //     DateTimeType = DateTimeType.TimeOnly,
            //     Value = nineAm.AddHours(1)
            // };

            //var designDateField = new DateTimeField
            //{
            //    Id = Guid.NewGuid(),
            //    Name = "Scheduled",
            //    Required = false,
            //    Tooltip = "What time did you get to the location?",
            //    Earliest = new DateTime(2010, 1, 1),
            //    Latest = new DateTime(2013, 1, 1),
            //    DateTimeType = DateTimeType.DateOnly,
            //    Value = new DateTime(2012, 1, 1)
            //};

            var designNumberField = new NumericField
             {
                 Id = Guid.NewGuid(),
                 Name = "Oil Collected",
                 Required = false,
                 Tooltip = "in gallons",
                 Mask = "g",
                 Maximum = 1000,
                 Minimum = 1,
                 DecimalPlaces = 0,
                 Value = 125
             };

            var designCurrencyField = new NumericField
             {
                 Id = Guid.NewGuid(),
                 Name = "Cash Recieved",
                 Required = false,
                 Tooltip = "How much cash was given on arrival",
                 Mask = "c",
                 Maximum = 1000,
                 Minimum = 0,
                 Value = (decimal?)2.54
             };

            var designPercentageField = new NumericField
              {
                  Id = Guid.NewGuid(),
                  Name = "Percent Full",
                  Required = false,
                  Tooltip = "How full is the barrel?",
                  Maximum = 1,
                  Minimum = 0,
                  Mask = "p",
                  Value = (decimal?) 0.25
              };

            var designCheckBoxField = new OptionsField
              {
                  Id = Guid.NewGuid(),
                  Name = "Paid",
                  Required = false,
                  Tooltip = "Has the client paid?",
                  AllowMultipleSelection = false,
                  OptionsType = OptionsType.Checkbox
              };
            designCheckBoxField.Options.Add(new Option { Name = "Collected paperwork", IsChecked = true });

            var designComboBoxField = new OptionsField
             {
                 Id = Guid.NewGuid(),
                 Name = "Barrel Type",
                 Required = false,
                 Tooltip = "The type of barrel",
                 AllowMultipleSelection = false,
                 OptionsType = OptionsType.Combobox
             };
            designComboBoxField.Options.Add(new Option { Name = "Small", IsChecked = false});
            designComboBoxField.Options.Add(new Option { Name = "Medium", IsChecked = false});
            designComboBoxField.Options.Add(new Option { Name = "Large", IsChecked = true });

            var designCheckListField = new OptionsField
              {
                  Id = Guid.NewGuid(),
                  Name = "Tasks",
                  Required = false,
                  Tooltip = "Helpful Hint",
                  AllowMultipleSelection = true,
                  OptionsType = OptionsType.Checklist
              };


            designCheckListField.Options.Add(new Option { Name = "Empty Barrels", IsChecked = true });
            designCheckListField.Options.Add(new Option { Name = "Assess Oil Quantity", IsChecked = true });
            designCheckListField.Options.Add(new Option { Name = "Get Paperwork Signed", IsChecked = false });

            var designLocationField = new LocationField
            {
                Id = Guid.NewGuid(),
                Name = "Destination",
                Required = false,
                Tooltip = "Where to go",
                LocationFieldType = LocationFieldType.Destination
            };

            //REMOVED TIME FIELDS. They need a redesign to deal w time zones. Many issues to figure out
            //designTimeField, designDateField, 
            DesignFields = new List<Field> { designTextBoxSmallField, designTextBoxLargeField, designSignatureField, designNumberField, designCurrencyField, designPercentageField, 
                designCheckBoxField, designComboBoxField, designCheckListField, designLocationField};
        }
    }
}

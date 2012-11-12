using System;
using System.Collections.Generic;

namespace FoundOps.Core.Models.CoreEntities.DesignData
{
    public enum NumericFieldType
    {
        Currency,
        Numeric,
        Percentage
    }

    public enum TextBoxFieldType
    {
        Small,
        Large
    }

    public class FieldsDesignData
    {
        public IEnumerable<Field> DesignFields { get; private set; }

        public FieldsDesignData()
        {
            DesignFields = new List<Field>
                {
                    NewOptionsField(OptionsType.Checkbox),
                    NewOptionsField(OptionsType.Checklist),
                    NewOptionsField(OptionsType.Combobox),
                    NewLocationField(),
                    NewNumericField(NumericFieldType.Currency),
                    NewNumericField(NumericFieldType.Numeric),
                    NewNumericField(NumericFieldType.Percentage),
                    NewSignatureField(),
                    NewTextBoxField(TextBoxFieldType.Small),
                    NewTextBoxField(TextBoxFieldType.Large)
                };
        }

        public static LocationField NewLocationField(Guid? locationId = null)
        {
            return new LocationField
            {
                Id = Guid.NewGuid(),
                Name = "Destination",
                LocationFieldType = LocationFieldType.Destination,
                LocationId = locationId,
                Required = false,
                Tooltip = "Where to go"
            };
        }

        public static NumericField NewNumericField(NumericFieldType fieldType)
        {
            if (fieldType == NumericFieldType.Currency)
            {
                return new NumericField
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
            }

            if (fieldType == NumericFieldType.Numeric)
            {
                return new NumericField
                {
                    Id = Guid.NewGuid(),
                    DecimalPlaces = 0,
                    Mask = "g", //normal decimal
                    Minimum = 1,
                    Maximum = 1000,
                    Name = "Oil Collected",
                    Tooltip = "in gallons",
                    Value = 125
                };
            }

            return new NumericField
            {
                Id = Guid.NewGuid(),
                Name = "Percent Full",
                Required = false,
                Tooltip = "How full is the barrel?",
                Maximum = 1,
                Minimum = 0,
                Mask = "p",
                Value = (decimal?)0.25
            };
        }

        public static OptionsField NewOptionsField(OptionsType type)
        {
            if (type == OptionsType.Combobox)
            {
                var designComboBoxField = new OptionsField
                {
                    Id = Guid.NewGuid(),
                    Name = "Barrel Size",
                    Tooltip = "The type of barrel",
                    AllowMultipleSelection = false,
                    OptionsType = OptionsType.Combobox,
                    Value = "1,2"
                };
                designComboBoxField.Options.Add(new Option { Name = "Small", IsChecked = false });
                designComboBoxField.Options.Add(new Option { Name = "Medium", IsChecked = false });
                designComboBoxField.Options.Add(new Option { Name = "Large", IsChecked = true });

                return designComboBoxField;
            }

            if (type == OptionsType.Checklist)
            {
                var designCheckListField = new OptionsField
                {
                    Id = Guid.NewGuid(),
                    Name = "Barrel Todo",
                    Tooltip = "Things to do with the barrels",
                    AllowMultipleSelection = true,
                    OptionsType = OptionsType.Checklist,
                    Value = "0,2"
                };

                designCheckListField.Options.Add(new Option { Name = "Empty Barrels", IsChecked = true });
                designCheckListField.Options.Add(new Option { Name = "Assess Oil Quantity", IsChecked = true });
                designCheckListField.Options.Add(new Option { Name = "Lock The Barrel", IsChecked = false });

                return designCheckListField;
            }

            var designCheckBoxField = new OptionsField
            {
                Id = Guid.NewGuid(),
                Name = "Paid",
                Tooltip = "Has the client paid?",
                AllowMultipleSelection = false,
                OptionsType = OptionsType.Checkbox
            };
            designCheckBoxField.Options.Add(new Option { Name = "Collected paperwork", IsChecked = true });

            return designCheckBoxField;
        }

        public static SignatureField NewSignatureField()
        {
            return new SignatureField
            {
                Id = Guid.NewGuid(),
                Name = "Confirmation",
                Tooltip = "Sign Here",
                Signed = DateTime.UtcNow,
                Value = "7UZ32232263353223222333242_3w546647c9b96646475765444_6uZ69647544533210Y33544a67585ba897757988676545444_4G10Z22433223545633322411111000Y11211100000Z121223_8G56676646432Z166878886543300Y136574a487464_6GZ11122223344510000Y224333466642223222120Z2_dyZ75546542Y3656536444Z1435465443_5v0112223431121344337442222223_gHZ3424245334653141200Y142345566645_2D5546489657db46b95976443321Z12322_ey76686686676_4y00000000000"
            };
        }

        public static TextBoxField NewTextBoxField(TextBoxFieldType type)
        {
            if (type == TextBoxFieldType.Small)
            {
                return new TextBoxField
                    {
                        Id = Guid.NewGuid(),
                        Name = "Lock Info",
                        Tooltip = "The lock combination",
                        IsMultiline = false,
                        Value = "124-1515-15155"
                    };
            }

            return new TextBoxField
            {
                Id = Guid.NewGuid(),
                Name = "Notes",
                Tooltip = "Any miscellaneous service information",
                IsMultiline = true,
                Value = "Be careful, there is a large scary dog"
            };
        }
    }
}

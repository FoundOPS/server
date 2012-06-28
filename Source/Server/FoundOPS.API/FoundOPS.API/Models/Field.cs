using System;

namespace FoundOPS.API.Models
{
    public class Field
    {
        public Guid Id { get; set; }

        public string Group { get; set; }

        public string Name { get; set; }

        public bool Required { get; set; }

        public string ToolTip { get; set; }

        public Guid? ParentFieldId { get; set; }

        public Guid? ServiceTemplateId { get; set; }

        /// <summary>
        /// Converts from the FoundOPS model to the API model
        /// </summary>
        /// <param name="fieldModel">Passed as an object, but should always be a Field</param>
        /// <returns>A Field that has been converted to it's API model</returns>
        public static Field ConvertModel(object fieldModel)
        {
            var textBoxField = fieldModel as FoundOps.Core.Models.CoreEntities.TextBoxField;
            var locationField = fieldModel as FoundOps.Core.Models.CoreEntities.LocationField;
            var numericField = fieldModel as FoundOps.Core.Models.CoreEntities.NumericField;
            var optionsField = fieldModel as FoundOps.Core.Models.CoreEntities.OptionsField;
            var dateTimeField = fieldModel as FoundOps.Core.Models.CoreEntities.DateTimeField;

            //If the field is a TextBoxField, convert the field to an API TextBoxField and return
            if (textBoxField != null)
                return TextBoxField.ConvertTextBoxFieldModel(textBoxField);

            //If the field is a LocationField, convert the field to an API LocationField and return
            if (locationField != null)
                return LocationField.ConvertLocationFieldModel(locationField);

            //If the field is a NumericField, convert the field to an API NumericField and return
            if (numericField != null)
                return NumericField.ConvertNumericFieldModel(numericField);

            //If the field is a OptionsField, convert the field to an API OptionsField and it's Options to API Options, then return
            if (optionsField != null)
                return OptionsField.ConvertOptionsFieldModel(optionsField);
            
            //If the field is a DateTimeField, convert the field to an API DateTimeField and return
            if (dateTimeField != null)
                return DateTimeField.ConvertDateTimeFieldModel(dateTimeField);

            throw new Exception("Field does not exist");
        }
    }
}
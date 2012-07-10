using System;
using System.ComponentModel;

namespace FoundOPS.API.Models
{
    public class Field
    {
        public Guid Id { get; set; }

        public string Type { get; set; }

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

            Field field;

            //If the field is a TextBoxField, convert the field to an API TextBoxField and return
            if (textBoxField != null)
                field = TextBoxField.ConvertTextBoxFieldModel(textBoxField);

            //TODO: not using location fields yet
            ////If the field is a LocationField, convert the field to an API LocationField and return
            //else if (locationField != null)
            //    return LocationField.ConvertLocationFieldModel(locationField);

            //If the field is a NumericField, convert the field to an API NumericField and return
            else if (numericField != null)
                field= NumericField.ConvertNumericFieldModel(numericField);

            //If the field is a OptionsField, convert the field to an API OptionsField and it's Options to API Options, then return
            else if (optionsField != null)
                field= OptionsField.ConvertOptionsFieldModel(optionsField);

            //If the field is a DateTimeField, convert the field to an API DateTimeField and return
            else if (dateTimeField != null)
                field= DateTimeField.ConvertDateTimeFieldModel(dateTimeField);

            else
                throw new Exception("Field does not exist");

            //http://stackoverflow.com/questions/9381090/asp-net-web-api-not-serializing-readonly-property
            //Need to manually do this until read only fields are serializable in Web API
            field.Type = field.GetType().Name;

            return field;
        }

        /// <summary>
        /// Converts the API model to the FoundOPS model
        /// </summary>
        /// <param name="field">The field to convert</param>
        /// <returns></returns>
        public static FoundOps.Core.Models.CoreEntities.Field ConvertBack(Field field)
        {
            var textBoxField = field as TextBoxField;
            var locationField = field as LocationField;
            var numericField = field as NumericField;
            var optionsField = field as OptionsField;
            var dateTimeField = field as DateTimeField;

            //If the field is a TextBoxField, convert the field to an API TextBoxField and return
            if (textBoxField != null)
                return TextBoxField.ConvertBackTextBoxField(textBoxField);

            //If the field is a LocationField, convert the field to an API LocationField and return
            if (locationField != null)
                return LocationField.ConvertBackLocationField(locationField);

            //If the field is a NumericField, convert the field to an API NumericField and return
            if (numericField != null)
                return NumericField.ConvertBackNumericField(numericField);

            //If the field is a OptionsField, convert the field to an API OptionsField and it's Options to API Options, then return
            if (optionsField != null)
                return OptionsField.ConvertBackOptionsField(optionsField);

            //If the field is a DateTimeField, convert the field to an API DateTimeField and return
            if (dateTimeField != null)
                return DateTimeField.ConvertBackDateTimeField(dateTimeField);

            throw new Exception("Field does not exist");
        }
    }
}
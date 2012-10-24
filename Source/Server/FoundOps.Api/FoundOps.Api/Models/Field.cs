using System;
using System.ComponentModel;
using FoundOps.Core.Models.CoreEntities;

namespace FoundOps.Api.Models
{
    public abstract class Field
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
            var textBoxField = fieldModel as Core.Models.CoreEntities.TextBoxField;
            var signatureField = fieldModel as Core.Models.CoreEntities.SignatureField;
            var locationField = fieldModel as Core.Models.CoreEntities.LocationField;
            var numericField = fieldModel as Core.Models.CoreEntities.NumericField;
            var optionsField = fieldModel as Core.Models.CoreEntities.OptionsField;
            var dateTimeField = fieldModel as Core.Models.CoreEntities.DateTimeField;

            Field field;

            if (textBoxField != null)
                field = TextBoxField.ConvertModel(textBoxField);
            else if (signatureField != null)
                field = SignatureField.ConvertModel(signatureField);
            else if (locationField != null)
                field = LocationField.ConvertModel(locationField);

            else if (numericField != null)
                field = NumericField.ConvertModel(numericField);

            //If the field is a OptionsField, convert the field to an API OptionsField and it's Options to API Options, then return
            else if (optionsField != null)
                field = OptionsField.ConvertModel(optionsField);

            else if (dateTimeField != null)
                field = DateTimeField.ConvertModel(dateTimeField);

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
        public static Core.Models.CoreEntities.Field ConvertBack(Field field)
        {
            var textBoxField = field as TextBoxField;
            var signatureField = field as SignatureField;
            var locationField = field as LocationField;
            var numericField = field as NumericField;
            var optionsField = field as OptionsField;
            var dateTimeField = field as DateTimeField;

            if (textBoxField != null)
                return TextBoxField.ConvertBack(textBoxField);

            if (signatureField != null)
                return SignatureField.ConvertBack(signatureField);

            if (locationField != null)
                return LocationField.ConvertBack(locationField);

            if (numericField != null)
                return NumericField.ConvertBack(numericField);

            if (optionsField != null)
                return OptionsField.ConvertBack(optionsField);

            if (dateTimeField != null)
                return DateTimeField.ConvertBack(dateTimeField);

            throw new Exception("Field does not exist");
        }

        /// <summary>
        /// Gets the type recognized by the javascript application that the field's value is.
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public static string GetJavascriptFormat(Core.Models.CoreEntities.Field field)
        {
            var numericField = field as Core.Models.CoreEntities.NumericField;
            var dateTimeField = field as Core.Models.CoreEntities.DateTimeField;

            if (numericField != null)
                return "number";

            if (dateTimeField != null)
            {
                switch (dateTimeField.DateTimeType)
                {
                    case DateTimeType.DateOnly:
                        return "date";
                    case DateTimeType.TimeOnly:
                        return "time";
                    case DateTimeType.DateTime:
                        return "dateTime";
                }
            }

            return "string";
        }
    }
}
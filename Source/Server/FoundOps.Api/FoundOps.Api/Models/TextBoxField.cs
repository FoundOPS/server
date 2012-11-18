using System;

namespace FoundOps.Api.Models
{
    public class TextBoxField : Field
    {
        public bool IsMultiLine { get; set; }

        public string Value { get; set; }

        public TextBoxField(DateTime createdDate) : base(createdDate)
        {
        }

        /// <summary>
        /// Converts from the FoundOPS model to the API model
        /// </summary>
        /// <param name="fieldModel">The FoundOPS model of a TextBoxField to be converted</param>
        /// <returns>A TextBoxField that has been converted to it's API model</returns>
        public static TextBoxField ConvertModel(Core.Models.CoreEntities.TextBoxField fieldModel)
        {
            var field = new TextBoxField (fieldModel.CreatedDate)
            {
                Id = fieldModel.Id,
                Name = fieldModel.Name,
                Required = fieldModel.Required,
                ToolTip = fieldModel.Tooltip,
                ParentFieldId = fieldModel.ParentFieldId,
                ServiceTemplateId = fieldModel.ServiceTemplateId,
                IsMultiLine = fieldModel.IsMultiline,
                Value = fieldModel.Value
            };

            field.SetLastModified(fieldModel.LastModifiedDate, fieldModel.LastModifyingUserId);

            return field;
        }

        /// <summary>
        /// Converts the API model back to the FoundOPS model
        /// </summary>
        /// <param name="textBoxField"></param>
        /// <returns></returns>
        public static Core.Models.CoreEntities.TextBoxField ConvertBack(TextBoxField textBoxField)
        {
            var field = new Core.Models.CoreEntities.TextBoxField
            {
                Id = textBoxField.Id,
                Name = textBoxField.Name,
                Required = textBoxField.Required,
                Tooltip = textBoxField.ToolTip,
                ParentFieldId = textBoxField.ParentFieldId,
                ServiceTemplateId = textBoxField.ServiceTemplateId,
                IsMultiline = textBoxField.IsMultiLine,
                Value = textBoxField.Value,
                CreatedDate = textBoxField.CreatedDate,
                LastModifiedDate = textBoxField.LastModifiedDate,
                LastModifyingUserId = textBoxField.LastModifyingUserId
            };

            return field;
        }
    }
}
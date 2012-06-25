namespace FoundOPS.API.Models
{
    public class TextBoxField : Field
    {
        public bool IsMultiLine { get; set; }

        public string Value { get; set; }

        /// <summary>
        /// Converts from the FoundOPS model to the API model
        /// </summary>
        /// <param name="fieldModel">The FoundOPS model of a TextBoxField to be converted</param>
        /// <returns>A TextBoxField that has been converted to it's API model</returns>
        public static TextBoxField ConvertTextBoxFieldModel(FoundOps.Core.Models.CoreEntities.TextBoxField fieldModel)
        {
            var field = new TextBoxField
               {
                   Id = fieldModel.Id,
                   Group = fieldModel.Group,
                   Name = fieldModel.Name,
                   Required = fieldModel.Required,
                   ToolTip = fieldModel.Tooltip,
                   ParentFieldId = fieldModel.ParentFieldId,
                   ServiceTemplateId = fieldModel.ServiceTemplateId,
                   IsMultiLine = fieldModel.IsMultiline,
                   Value = fieldModel.Value
               };

            return field;
        }
    }
}
namespace FoundOPS.API.Models
{
    public class TextBoxField : Field
    {
        public bool IsMultiLine { get; set; }

        public string Value { get; set; }

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
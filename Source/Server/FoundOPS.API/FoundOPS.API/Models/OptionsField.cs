namespace FoundOps.Api.Models
{
    public class OptionsField : Field
    {
        public bool AllowMultipleSelection { get; set; }

        public short TypeInt { get; set; }

        public string OptionsString { get; set; }

        public string Value { get; set; }

        /// <summary>
        /// Converts from the FoundOPS model to the API model
        /// </summary>
        /// <param name="fieldModel">The FoundOPS model of a OptionsField to be converted</param>
        /// <returns>A OptionsField that has been converted to it's API model</returns>
        public static OptionsField ConvertOptionsFieldModel(FoundOps.Core.Models.CoreEntities.OptionsField fieldModel)
        {
            var field = new OptionsField
            {
                Id = fieldModel.Id,
                Name = fieldModel.Name,
                Required = fieldModel.Required,
                ToolTip = fieldModel.Tooltip,
                ParentFieldId = fieldModel.ParentFieldId,
                ServiceTemplateId = fieldModel.ServiceTemplateId,
                AllowMultipleSelection = fieldModel.AllowMultipleSelection,
                TypeInt = fieldModel.TypeInt,
                OptionsString = fieldModel.OptionsString,
                Value = fieldModel.Value
            };

            return field;
        }

        /// <summary>
        /// Converts the API model back to the FoundOPS model
        /// </summary>
        /// <param name="optionsField"></param>
        /// <returns></returns>
        public static FoundOps.Core.Models.CoreEntities.Field ConvertBackOptionsField(OptionsField optionsField)
        {
            var field = new FoundOps.Core.Models.CoreEntities.OptionsField
            {
                Id = optionsField.Id,
                Name = optionsField.Name,
                Required = optionsField.Required,
                Tooltip = optionsField.ToolTip,
                ParentFieldId = optionsField.ParentFieldId,
                ServiceTemplateId = optionsField.ServiceTemplateId,
                AllowMultipleSelection = optionsField.AllowMultipleSelection,
                TypeInt = optionsField.TypeInt,
                OptionsString = optionsField.OptionsString,
                Value = optionsField.Value
            };

            return field;
        }
    }
}
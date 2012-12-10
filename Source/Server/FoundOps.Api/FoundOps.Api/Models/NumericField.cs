namespace FoundOps.Api.Models
{
    public class NumericField : Field
    {
        public string Mask { get; set; }

        public int DecimalPlaces { get; set; }

        public decimal Minimum { get; set; }

        public decimal Maximum { get; set; }

        public decimal? Value { get; set; }
        
        /// <summary>
        /// Converts from the FoundOPS model to the API model
        /// </summary>
        /// <param name="fieldModel">The FoundOPS model of a NumericField to be converted</param>
        /// <returns>A NumericField that has been converted to it's API model</returns>
        public static NumericField ConvertModel(Core.Models.CoreEntities.NumericField fieldModel)
        {
            var field = new NumericField
            {
                Id = fieldModel.Id,
                CreatedDate = fieldModel.CreatedDate,
                Name = fieldModel.Name,
                Required = fieldModel.Required,
                ToolTip = fieldModel.Tooltip,
                ParentFieldId = fieldModel.ParentFieldId,
                ServiceTemplateId = fieldModel.ServiceTemplateId,
                Mask = fieldModel.Mask,
                DecimalPlaces = fieldModel.DecimalPlaces,
                Minimum = fieldModel.Minimum,
                Maximum = fieldModel.Maximum,
                Value = fieldModel.Value
            };

            field.SetLastModified(fieldModel.LastModified, fieldModel.LastModifyingUserId);

            return field;
        }

        /// <summary>
        /// Converts the API model back to the FoundOPS model
        /// </summary>
        /// <param name="numericField"></param>
        /// <returns></returns>
        public static Core.Models.CoreEntities.Field ConvertBack(NumericField numericField)
        {
            var field = new Core.Models.CoreEntities.NumericField
            {
                Id = numericField.Id,
                Name = numericField.Name,
                Required = numericField.Required,
                Tooltip = numericField.ToolTip,
                ParentFieldId = numericField.ParentFieldId,
                ServiceTemplateId = numericField.ServiceTemplateId,
                Mask = numericField.Mask,
                DecimalPlaces = numericField.DecimalPlaces,
                Minimum = numericField.Minimum,
                Maximum = numericField.Maximum,
                Value = numericField.Value,
                CreatedDate = numericField.CreatedDate,
                LastModified = numericField.LastModified,
                LastModifyingUserId = numericField.LastModifyingUserId
            };

            return field;
        }
    }
}
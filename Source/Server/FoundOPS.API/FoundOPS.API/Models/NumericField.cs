namespace FoundOPS.API.Models
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
        public static NumericField ConvertNumericFieldModel(FoundOps.Core.Models.CoreEntities.NumericField fieldModel)
        {
            var field = new NumericField
            {
                Id = fieldModel.Id,
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

            return field;
        }
    }
}
using System;

namespace FoundOps.Api.Models
{
    public class SignatureField : Field
    {
        public string Value { get; set; }
        public DateTime Signed { get; set; }

        /// <summary>
        /// Converts from the FoundOPS model to the API model
        /// </summary>
        /// <param name="fieldModel">The FoundOPS model of a SignatureField to be converted</param>
        /// <returns>A SignatureField that has been converted to it's API model</returns>
        public static SignatureField ConvertModel(Core.Models.CoreEntities.SignatureField fieldModel)
        {
            var field = new SignatureField
            {
                Id = fieldModel.Id,
                Name = fieldModel.Name,
                Required = fieldModel.Required,
                ToolTip = fieldModel.Tooltip,
                ParentFieldId = fieldModel.ParentFieldId,
                ServiceTemplateId = fieldModel.ServiceTemplateId,
                Signed = fieldModel.Signed,
                Value = fieldModel.Value
            };

            return field;
        }

        /// <summary>
        /// Converts the API model back to the FoundOPS model
        /// </summary>
        /// <param name="signatureField"></param>
        /// <returns></returns>
        public static Core.Models.CoreEntities.SignatureField ConvertBack(SignatureField signatureField)
        {
            var field = new Core.Models.CoreEntities.SignatureField
            {
                Id = signatureField.Id,
                Name = signatureField.Name,
                Required = signatureField.Required,
                Tooltip = signatureField.ToolTip,
                ParentFieldId = signatureField.ParentFieldId,
                ServiceTemplateId = signatureField.ServiceTemplateId,
                Signed = signatureField.Signed,
                Value = signatureField.Value
            };

            return field;
        }
    }
}
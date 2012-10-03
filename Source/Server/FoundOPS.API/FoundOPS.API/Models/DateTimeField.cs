using System;

namespace FoundOPS.Api.Models
{
    public class DateTimeField : Field
    {
        public DateTime Earliest { get; set; }

        public DateTime Latest { get; set; }

        public short TypeInt { get; set; }

        public DateTime? Value { get; set; }

        /// <summary>
        /// Converts from the FoundOPS model to the API model
        /// </summary>
        /// <param name="fieldModel">The FoundOPS model of a DateTimeField to be converted</param>
        /// <returns>A DateTimeField that has been converted to it's API model</returns>
        public static DateTimeField ConvertDateTimeFieldModel(FoundOps.Core.Models.CoreEntities.DateTimeField fieldModel)
        {
            var field = new DateTimeField
            {
                Id = fieldModel.Id,
                Name = fieldModel.Name,
                Required = fieldModel.Required,
                ToolTip = fieldModel.Tooltip,
                ParentFieldId = fieldModel.ParentFieldId,
                ServiceTemplateId = fieldModel.ServiceTemplateId,
                Earliest = fieldModel.Earliest,
                Latest = fieldModel.Latest,
                TypeInt = fieldModel.TypeInt,
                Value = fieldModel.Value
            };

            return field;
        }

        /// <summary>
        /// Converts the API model back to the FoundOPS model
        /// </summary>
        /// <param name="dateTimeField"></param>
        /// <returns></returns>
        public static FoundOps.Core.Models.CoreEntities.Field ConvertBackDateTimeField(DateTimeField dateTimeField)
        {
            var field = new FoundOps.Core.Models.CoreEntities.DateTimeField
            {
                Id = dateTimeField.Id,
                Name = dateTimeField.Name,
                Required = dateTimeField.Required,
                Tooltip = dateTimeField.ToolTip,
                ParentFieldId = dateTimeField.ParentFieldId,
                ServiceTemplateId = dateTimeField.ServiceTemplateId,
                Earliest = dateTimeField.Earliest,
                Latest = dateTimeField.Latest,
                TypeInt = dateTimeField.TypeInt,
                Value = dateTimeField.Value
            };

            return field;
        }
    }
}
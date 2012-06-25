using System;

namespace FoundOPS.API.Models
{
    public class DateTimeField : Field
    {
        public DateTime Earliest { get; set; }

        public DateTime Latest { get; set; }

        public int TypeInt { get; set; }

        public DateTime? Value { get; set; }

        public static DateTimeField ConvertDateTimeFieldModel(FoundOps.Core.Models.CoreEntities.DateTimeField fieldModel)
        {
            var field = new DateTimeField
            {
                Id = fieldModel.Id,
                Group = fieldModel.Group,
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
    }
}
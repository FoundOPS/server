using System;

namespace FoundOPS.API.Models
{
    public class LocationField : Field
    {
        public Guid? LocationId { get; set; }

        public short LocationFieldTypeInt { get; set; }

        public static LocationField ConvertLocationFieldModel(FoundOps.Core.Models.CoreEntities.LocationField fieldModel)
        {
            var field = new LocationField
            {
                Id = fieldModel.Id,
                Group = fieldModel.Group,
                Name = fieldModel.Name,
                Required = fieldModel.Required,
                ToolTip = fieldModel.Tooltip,
                ParentFieldId = fieldModel.ParentFieldId,
                ServiceTemplateId = fieldModel.ServiceTemplateId,
                LocationId = fieldModel.LocationId,
                LocationFieldTypeInt = fieldModel.LocationFieldTypeInt
            };

            return field;
        }
    }
}
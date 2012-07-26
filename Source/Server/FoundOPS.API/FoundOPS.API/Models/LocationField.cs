using System;

namespace FoundOPS.API.Models
{
    public class LocationField : Field
    {
        public Guid? LocationId { get; set; }

        public short LocationFieldTypeInt { get; set; }

        /// <summary>
        /// Converts from the FoundOPS model to the API model
        /// </summary>
        /// <param name="fieldModel">The FoundOPS model of a LocationField to be converted</param>
        /// <returns>A LocationField that has been converted to it's API model</returns>
        public static LocationField ConvertLocationFieldModel(FoundOps.Core.Models.CoreEntities.LocationField fieldModel)
        {
            var field = new LocationField
            {
                Id = fieldModel.Id,
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

        /// <summary>
        /// Converts the API model back to the FoundOPS model
        /// </summary>
        /// <param name="locationField"></param>
        /// <returns></returns>
        public static FoundOps.Core.Models.CoreEntities.Field ConvertBackLocationField(LocationField locationField)
        {
            var field = new FoundOps.Core.Models.CoreEntities.LocationField
            {
                Id = locationField.Id,
                Name = locationField.Name,
                Required = locationField.Required,
                Tooltip = locationField.ToolTip,
                ParentFieldId = locationField.ParentFieldId,
                ServiceTemplateId = locationField.ServiceTemplateId,
                LocationId = locationField.LocationId,
                LocationFieldTypeInt = locationField.LocationFieldTypeInt
            };

            return field;
        }
    }
}
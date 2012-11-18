using System;

namespace FoundOps.Api.Models
{
    public class LocationField : Field
    {
        public Guid? LocationId { get; set; }

        public short LocationFieldTypeInt { get; set; }

        public LocationField(DateTime createdDate) : base(createdDate)
        {
        }

        /// <summary>
        /// Converts from the FoundOPS model to the API model
        /// </summary>
        /// <param name="fieldModel">The FoundOPS model of a LocationField to be converted</param>
        /// <returns>A LocationField that has been converted to it's API model</returns>
        public static LocationField ConvertModel(Core.Models.CoreEntities.LocationField fieldModel)
        {
            var field = new LocationField (fieldModel.CreatedDate)
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

            field.SetLastModified(fieldModel.LastModifiedDate, fieldModel.LastModifyingUserId);

            return field;
        }

        /// <summary>
        /// Converts the API model back to the FoundOPS model
        /// </summary>
        /// <param name="locationField"></param>
        /// <returns></returns>
        public static Core.Models.CoreEntities.Field ConvertBack(LocationField locationField)
        {
            var field = new Core.Models.CoreEntities.LocationField
            {
                Id = locationField.Id,
                Name = locationField.Name,
                Required = locationField.Required,
                Tooltip = locationField.ToolTip,
                ParentFieldId = locationField.ParentFieldId,
                ServiceTemplateId = locationField.ServiceTemplateId,
                LocationId = locationField.LocationId,
                LocationFieldTypeInt = locationField.LocationFieldTypeInt,
                CreatedDate = locationField.CreatedDate,
                LastModifiedDate = locationField.LastModifiedDate,
                LastModifyingUserId = locationField.LastModifyingUserId
            };

            return field;
        }
    }
}
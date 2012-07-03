using System.Collections.Generic;

namespace FoundOPS.API.Models
{
    public class OptionsField : Field
    {
        public bool AllowMultipleSelection { get; set; }

        public int TypeInt { get; set; }

        public List<Option> Options { get; set; }

        public OptionsField()
        {
            Options = new List<Option>();
        }

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
                TypeInt = fieldModel.TypeInt
            };

            foreach (var option in fieldModel.Options)
                field.Options.Add(Option.ConvertOptionModel(option));

            return field;
        }
    }
}
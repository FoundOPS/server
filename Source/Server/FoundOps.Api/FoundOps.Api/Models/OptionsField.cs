using System;
using System.Collections.Generic;
using System.Linq;

namespace FoundOps.Api.Models
{
    public class OptionsField : Field
    {
        public bool AllowMultipleSelection { get; set; }

        public short TypeInt { get; set; }

        public List<Option> Options { get; set; }

        public OptionsField(DateTime createdDate) : base(createdDate)
        {
            Options = new List<Option>();
        }

        /// <summary>
        /// Converts from the FoundOPS model to the API model
        /// </summary>
        /// <param name="fieldModel">The FoundOPS model of a OptionsField to be converted</param>
        /// <returns>A OptionsField that has been converted to it's API model</returns>
        public static OptionsField ConvertModel(Core.Models.CoreEntities.OptionsField fieldModel)
        {
            var field = new OptionsField(fieldModel.CreatedDate)
            {
                Id = fieldModel.Id,
                Name = fieldModel.Name,
                Required = fieldModel.Required,
                ToolTip = fieldModel.Tooltip,
                ParentFieldId = fieldModel.ParentFieldId,
                ServiceTemplateId = fieldModel.ServiceTemplateId,
                AllowMultipleSelection = fieldModel.AllowMultipleSelection,
                TypeInt = fieldModel.TypeInt,
                LastModifiedDate = fieldModel.LastModifiedDate,
                LastModifyingUserId = fieldModel.LastModifyingUserId
            };

            foreach (var option in fieldModel.Options.OrderBy(o => o.Name))
                field.Options.Add(Option.ConvertModel(option));

            return field;
        }

        /// <summary>
        /// Converts the API model back to the FoundOPS model
        /// </summary>
        /// <param name="optionsField"></param>
        /// <returns></returns>
        public static Core.Models.CoreEntities.Field ConvertBack(OptionsField optionsField)
        {
            var field = new Core.Models.CoreEntities.OptionsField
            {
                Id = optionsField.Id,
                Name = optionsField.Name,
                Required = optionsField.Required,
                Tooltip = optionsField.ToolTip,
                ParentFieldId = optionsField.ParentFieldId,
                ServiceTemplateId = optionsField.ServiceTemplateId,
                AllowMultipleSelection = optionsField.AllowMultipleSelection,
                TypeInt = optionsField.TypeInt,
                CreatedDate = optionsField.CreatedDate,
                LastModifiedDate = optionsField.LastModifiedDate,
                LastModifyingUserId = optionsField.LastModifyingUserId
            };

            foreach (var option in optionsField.Options)
                field.Options.Add(Option.ConvertBack(option));

            return field;
        }
    }
}
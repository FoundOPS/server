using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoundOPS.API.Models
{
    public class Field
    {
        public Guid Id { get; set; }

        public string Group { get; set; }

        public string Name { get; set; }

        public bool Required { get; set; }

        public string ToolTip { get; set; }

        public Guid? ParentFieldId { get; set; }

        public Guid? ServiceTemplateId { get; set; }

        public static Field ConvertModel(FoundOps.Core.Models.CoreEntities.Field fieldModel)
        {
            var field = new FoundOPS.API.Models.Field
                {
                    Id = fieldModel.Id,
                    Group = fieldModel.Group,
                    Name = fieldModel.Name,
                    Required = fieldModel.Required,
                    ToolTip = fieldModel.Tooltip,
                    ParentFieldId = fieldModel.ParentFieldId,
                    ServiceTemplateId = fieldModel.ServiceTemplateId
                };

            return field;
        }
    }
}
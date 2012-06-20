using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoundOPS.API.Models
{
    public class Field
    {
        public string Group { get; set; }

        public string Name { get; set; }

        public bool Required { get; set; }

        public string ToolTip { get; set; }

        public Guid? ParentFieldId { get; set; }

        public Guid? ServiceTemplateId { get; set; }

    }
}
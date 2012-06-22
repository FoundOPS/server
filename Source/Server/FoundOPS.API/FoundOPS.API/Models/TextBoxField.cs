using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoundOPS.API.Models
{
    public class TextBoxField
    {
        public Guid Id { get; set; }

        public bool IsMultiLine { get; set; }

        public string Value { get; set; }
    }
}
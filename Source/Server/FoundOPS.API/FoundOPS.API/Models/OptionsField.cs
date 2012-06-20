using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoundOPS.API.Models
{
    public class OptionsField
    {
        public Guid Id { get; set; }

        public bool AllowMultipleSelection { get; set; }

        public int TypeInt { get; set; }
    }
}
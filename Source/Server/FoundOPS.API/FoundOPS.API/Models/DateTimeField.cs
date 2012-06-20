using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoundOPS.API.Models
{
    public class DateTimeField
    {
        public DateTime Earliest { get; set; }

        public DateTime Latest { get; set; }

        public int TypeInt { get; set; }

        public DateTime Value { get; set; }
    }
}
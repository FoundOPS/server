using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoundOPS.API.Models
{
    public class LocationField
    {
        public Guid Id { get; set; }

        public Guid LocationId { get; set; }

        public int LocationFieldTypeInt { get; set; }
    }
}
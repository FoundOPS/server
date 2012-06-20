using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoundOPS.API.Models
{
    public class Service 
    {
        public Guid ClientId { get; set; }

        public Guid ServiceproviderId { get; set; }

        public Guid RecurringServiceId { get; set; }

        public DateTime ServiceDate { get; set; }

        public Field[] Fields { get; set; }
    }
}
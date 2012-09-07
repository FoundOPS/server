using System;
using System.Runtime.Serialization;

namespace FoundOps.Core.Models.CoreEntities.ServiceEntites
{
    [DataContract]
    public class ServiceTemplateWithDate
    {
        public Guid? RecurringServiceId { get; set; }

        public Guid? ServiceId { get; set; }
    
        public DateTime OccurDate { get; set; }

        public String ServiceName { get; set; }

        public String ClientName { get; set; }
    }
}

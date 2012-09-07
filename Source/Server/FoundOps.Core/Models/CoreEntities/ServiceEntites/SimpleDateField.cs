using System;
using System.Runtime.Serialization;

namespace FoundOps.Core.Models.CoreEntities.ServiceEntites
{
    [DataContract]
    public class SimpleDateField : ISimpleField
    {
        public Guid Id { get; set; }

        public Guid ServiceTemplateId { get; set; }

        public string Name { get; set; }

        public DateTime Value { get; set; }

        public object ObjectValue { get { return Value; } }
    }
}

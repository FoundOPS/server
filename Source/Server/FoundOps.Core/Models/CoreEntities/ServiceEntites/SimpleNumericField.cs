using System;
using System.Runtime.Serialization;

namespace FoundOps.Core.Models.CoreEntities.ServiceEntites
{
    [DataContract]
    public class SimpleNumericField : ISimpleField
    {
        public Guid Id { get; set; }

        public Guid ServiceTemplateId { get; set; }

        public string Name { get; set; }

        public decimal Value { get; set; }

        public object ObjectValue { get { return Value; } }
    }
}

using System;
using System.Runtime.Serialization;

namespace FoundOps.Core.Models.CoreEntities.ServiceEntites
{
    [DataContract]
    public class SimpleOptionsField : ISimpleField
    {
        public Guid Id { get; set; }

        public Guid ServiceTemplateId { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }
        public string OptionsString { get; set; }

        public object ObjectValue
        {
            get
            {
                return OptionsField.SimpleValue(OptionsString, Value);
            }
        }
    }
}

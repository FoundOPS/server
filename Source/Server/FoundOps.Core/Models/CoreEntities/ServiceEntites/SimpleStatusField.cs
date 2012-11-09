using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoundOps.Core.Models.CoreEntities.ServiceEntites
{
    public class SimpleStatusField : ISimpleField
    {
        public Guid Id { get; private set; }
        public Guid ServiceTemplateId { get; private set; }
        public string Name { get; private set; }
        public object ObjectValue { get { return Value; } }
        public string Value { get; set; }
        public string Color { get; set; }
    }
}

using System;

namespace FoundOps.Common.Composite.Entities
{
    [AttributeUsage(AttributeTargets.Class)]
    public class FieldAttribute : Attribute
    {
        public string PropertyName { get; private set; }
        public string Group { get; private set; }

        public FieldAttribute(string propertyName, string group)
        {
            PropertyName = propertyName;
            Group = group;
        }
    }
}

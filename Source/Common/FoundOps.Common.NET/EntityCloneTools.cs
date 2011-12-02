using System;
using System.Data.Objects.DataClasses;
using System.Reflection;

namespace FoundOps.Common.Composite.Entities
{
    public static class EntityCloneTools
    {
        public static T Clone<T>(this T entity) where T : EntityObject
        {
            var type = entity.GetType();
            var clone = Activator.CreateInstance(type);

            do
            {
                foreach (var property in type.GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.SetProperty))
                {
                    if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(EntityReference<>)) break;
                    if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(EntityCollection<>)) break;
                    if (property.PropertyType.IsSubclassOf(typeof(EntityObject))) break;

                    if (property.CanWrite)
                    {
                        property.SetValue(clone, property.GetValue(entity, null), null);
                    }
                }
                type = type.BaseType;

            } while (type != null);

            ((EntityObject)clone).EntityKey = null;

            return (T)clone;
        }


    }
}

using System;
using System.Data;
using System.Linq;
using System.Data.Objects;
using System.Data.Metadata.Edm;
using System.Data.Objects.DataClasses;

namespace FoundOps.Common.NET
{
    public static class ObjectContextExtensions
    {
        public static EntitySetBase GetEntitySet<TEntity>(this ObjectContext context)
        {
            var container = context.MetadataWorkspace.GetEntityContainer(context.DefaultContainerName, DataSpace.CSpace);
            var baseType = GetBaseType(typeof(TEntity));
            var entitySet = container.BaseEntitySets.FirstOrDefault(item => item.ElementType.Name.Equals(baseType.Name));

            return entitySet;
        }

        private static Type GetBaseType(Type type)
        {
            var baseType = type.BaseType;
            if (baseType != null && baseType != typeof(EntityObject))
                return GetBaseType(type.BaseType);
            return type;
        }

        /// <summary>
        /// Detaches any existing ObjectStateEntry of the same entity, and attaches entity.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="entity">The entity.</param>
        public static void DetachExistingAndAttach<T>(this ObjectContext context, T entity) where T : IEntityWithKey
        {
            if(entity == null)
                return;

            var entitySetName = context.GetEntitySet<T>().Name;
            ObjectStateEntry entry;
            //See if there is an existing ObjectStateEntry of the entity
            if (context.ObjectStateManager.TryGetObjectStateEntry(
                                    context.CreateEntityKey(entitySetName, entity),
                                    out entry))
            {
                var entryEntity = (T)entry.Entity;
                //Detach the existing entry's entity
                if (entry.State != EntityState.Detached)
                    context.Detach(entryEntity);
            }

            context.AttachTo(entitySetName, entity);
        }
    }
}

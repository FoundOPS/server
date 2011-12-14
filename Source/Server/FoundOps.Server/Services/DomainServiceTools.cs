using System.Data.Objects.DataClasses;
using FoundOps.Core.Models.CoreEntities;
using EntityState = System.Data.EntityState;

namespace FoundOps.Server.Services
{
    public static class DomainServiceTools
    {
        /// <summary>
        /// Detaches the Entity.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="entityToCheck">The entity to check.</param>
        /// <returns></returns>
        public static bool PreDelete(this CoreEntitiesContainer container, EntityObject entityToCheck)
        {
            if (entityToCheck == null ||entityToCheck.EntityState == EntityState.Deleted) 
                return false;

            container.Detach(entityToCheck);
            return true;
        }
    }
}
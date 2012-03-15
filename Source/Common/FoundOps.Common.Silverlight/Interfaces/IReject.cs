using System.Linq;
using RiaServicesContrib;
using System.ServiceModel.DomainServices.Client;

namespace FoundOps.Common.Silverlight.Interfaces
{
    /// <summary>
    /// Rejects the changes of this individual entity.
    /// </summary>
    public interface IReject
    {
        void Reject();
    }

    /// <summary>
    /// Extension methods to use with IReject entities.
    /// </summary>
    public static class RejectEntityExtensions
    {
        /// <summary>
        /// Rejects the all changes of an EntityGraph for entities that implement IReject.
        /// </summary>
        /// <typeparam name="TEntityType">The type of the entity type.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <param name="entityGraph">The entity graph.</param>
        public static void RejectChangesExtension<TEntityType>(this TEntityType entity, EntityGraph<TEntityType> entityGraph) where TEntityType : Entity, IReject
        {
            entity.Reject();

            foreach (var element in entityGraph.Where(ele => ele as IReject != null))
                element.Reject();
        }
    }
}

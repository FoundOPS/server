using System.Linq;
using EntityGraph.RIA;
using System.ServiceModel.DomainServices.Client;

namespace FoundOps.Common.Silverlight.Interfaces
{
    public interface IReject
    {
        void Reject();
    }

    public static class RejectEntityExtensions
    {
        public static void RejectChangesExtension<TEntityType>(this TEntityType entity, EntityGraph<TEntityType> entityGraph) where TEntityType : Entity, IReject
        {
            entity.Reject();

            foreach (IReject element in entityGraph.Where(ele => typeof(IReject).IsAssignableFrom(ele.GetType())))
                element.Reject();
        }
    }
}

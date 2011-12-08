using System.Linq;
using System.ServiceModel.DomainServices.Client;
using RiaServicesContrib.DomainServices.Client;

namespace FoundOps.Common.Silverlight.MVVM.Interfaces
{
    public interface IReject
    {
        void Reject();
    }

    public static class RejectEntityExtensions
    {
        public static void RejectChangesExtension<TEntityType>(this TEntityType entity, EntityGraph entityGraph) where TEntityType : Entity, IReject
        {
            entity.Reject();

            foreach (IReject element in entityGraph.Where(ele => typeof(IReject).IsAssignableFrom(ele.GetType())))
                element.Reject();
        }
    }
}

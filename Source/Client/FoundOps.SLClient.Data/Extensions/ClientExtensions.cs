using RiaServicesContrib;
using RiaServicesContrib.DomainServices.Client;
using FoundOps.Common.Silverlight.MVVM.Interfaces;

namespace FoundOps.Core.Models.CoreEntities
{
    public partial class Client : IReject
    {
        public void Reject()
        {
            this.RejectChanges();
        }

        /// <summary>
        /// Gets the entity graph of Client to remove.
        /// </summary>
        public EntityGraph EntityGraphToRemove
        {
            get
            {
                var graphShape =
                    new EntityGraphShape().Edge<Client, ServiceTemplate>(client => client.ServiceTemplates).Edge
                        <ServiceTemplate, Field>(st => st.Fields).Edge<OptionsField, Option>(of => of.Options);

                return new EntityGraph(this, graphShape);
            }
        }
    }
}

using System;
using RiaServicesContrib.DomainServices.Client;

namespace FoundOps.Core.Models.CoreEntities
{
    public partial class RouteTask
    {
        public void RemoveRouteDestination()
        {
            if (this.RouteDestination != null)
                this.RouteDestination.Tasks.Remove(this);
        }

        public EntityGraph EntityGraph
        {
            get
            {
                var entityGraph = new EntityGraph(this, new RiaServicesContrib.EntityGraphShape());

                return entityGraph;
            }
        }

        public EntityGraph EntityGraphWithService
        {
            get
            {
                var entityGraph = new EntityGraph(this,
                                                  new RiaServicesContrib.EntityGraphShape().Edge<RouteTask, Service>(rt => rt.Service));

                return entityGraph;
            }
        }

        /// <summary>
        /// Clones the RouteTask.
        /// </summary>
        /// <param name="withService">if set to <c>true</c> [clones the service and service template as well].</param>
        public RouteTask Clone(bool withService)
        {
            var clonedRouteTask = withService ? (RouteTask)this.EntityGraphWithService.Clone() : (RouteTask)this.EntityGraph.Clone();

            //Update to the Id to a new one
            clonedRouteTask.Id = Guid.NewGuid();

            if (!withService) return clonedRouteTask;

            //clone the service template
            var clonedServiceTemplate = this.Service.ServiceTemplate.MakeSibling();

            //clone the service
            var serviceClone = clonedRouteTask.Service;
            serviceClone.Id = Guid.NewGuid(); //Fix the serviceClone.Id

            //update the serviceClone.Id to the serviceTemplate.Id
            serviceClone.Id = clonedServiceTemplate.Id;

            //set the serviceClone's service template
            serviceClone.ServiceTemplate = clonedServiceTemplate;

            //set the Service property of clonedRouteTask
            clonedRouteTask.Service = serviceClone;

            return clonedRouteTask;
        }
    }
}

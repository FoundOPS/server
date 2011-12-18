using System;
using RiaServicesContrib;
using RiaServicesContrib.DomainServices.Client;

//Partial class must be part of same namespace
// ReSharper disable CheckNamespace
namespace FoundOps.Core.Models.CoreEntities
// ReSharper restore CheckNamespace
{
    public partial class RouteTask
    {
        public void RemoveRouteDestination()
        {
            if (this.RouteDestination != null)
                this.RouteDestination.Tasks.Remove(this);
        }

        public EntityGraphShape EntityGraphWithServiceShape
        {
            get
            {
                return new EntityGraphShape().Edge<RouteTask, Service>(rt => rt.Service);
            }
        }

        /// <summary>
        /// Clones the RouteTask.
        /// </summary>
        /// <param name="withService">if set to <c>true</c> [clones the service and service template as well].</param>
        public RouteTask Clone(bool withService)
        {
            var clonedRouteTask = withService ? this.Clone(EntityGraphWithServiceShape) : this.Clone(new EntityGraphShape());

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

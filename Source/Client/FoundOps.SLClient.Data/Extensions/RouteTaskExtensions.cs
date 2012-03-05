using System;
using FoundOps.Common.Silverlight.Interfaces;
using RiaServicesContrib;
using RiaServicesContrib.DomainServices.Client;

//Partial class must be part of same namespace
// ReSharper disable CheckNamespace
namespace FoundOps.Core.Models.CoreEntities
// ReSharper restore CheckNamespace
{
    public partial class RouteTask : IReject
    {
        /// <summary>
        /// Gets the generated service route task parent which cloned this RouteTask.
        /// </summary>
        public RouteTask GeneratedRouteTaskParent { get; private set; }

        partial void OnCreation()
        {
            InitializeHelper();
        }
        protected override void OnLoaded(bool isInitialLoad)
        {
            if (isInitialLoad)
                InitializeHelper();

            base.OnLoaded(isInitialLoad);
        }

        private void InitializeHelper()
        {
            /* Follow when this routetask is added to a RouteDestination.
             * It is the last place a generated route task can be added to the database.
             * 
             * So if this is a generated service cancel adding itself to the route and add a clone instead.
             *
             * It must add a clone (instead of itself) so it shows up as a new entity.
             * Generated services passed over the wire, even though they are not added to the DB,
             * show up as unmodified/not new entities. */

            //Observable2.FromPropertyChangedPattern(this, x => x.RouteDestination).Where(_ => this.GeneratedOnServer).WhereNotNull().SubscribeOnDispatcher()
            //.Subscribe(routeDestination =>
            //{
            //    //Cancel adding this to a route
            //    this.RouteDestination = null;

            //    //Clone this
            //    var clone = this.Clone(this.Service != null);

            //    //Add the clone to the route destination
            //    routeDestination.RouteTasks.Add(clone);
            //});
        }

        ///<summary>
        /// Remove this RouteTask from it's route destination.
        ///</summary>
        public void RemoveRouteDestination()
        {
            this.RouteDestination = null;
            this.RouteDestinationId = null;
        }

        /// <summary>
        /// Gets the entity graph with service shape.
        /// </summary>
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

            if (this.GeneratedOnServer)
            {
                //Change GeneratedOnServer to false
                clonedRouteTask.GeneratedOnServer = false;
                //Set a link back to this parent
                clonedRouteTask.GeneratedRouteTaskParent = this;
            }

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

        public void Reject()
        {
            this.RejectChanges();
        }
    }
}
